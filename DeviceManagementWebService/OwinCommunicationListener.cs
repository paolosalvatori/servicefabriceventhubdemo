#region Copyright
//=======================================================================================
// Microsoft Azure Customer Advisory Team  
//
// This sample is supplemental to the technical guidance published on the community
// blog at http://blogs.msdn.com/b/paolos/. 
// 
// Author: Paolo Salvatori
//=======================================================================================
// Copyright © 2015 Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
// EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
// MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. YOU BEAR THE RISK OF USING IT.
//=======================================================================================
#endregion

#region Using Directives

using System;
using System.Fabric;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using System.Linq;

#endregion

namespace Microsoft.AzureCat.Samples.DeviceManagementWebService
{
    public class OwinCommunicationListener : ICommunicationListener
    {
        #region Private Constants
        //************************************
        // Parameters
        //************************************
        private const string ConfigurationPackage = "Config";
        private const string ConfigurationSection = "DeviceManagementWebServiceConfig";
        private const string DeviceActorServiceUriParameter = "DeviceActorServiceUri";

        //***************************
        // Constants
        //***************************
        private const string DeviceId = "id";
        private const string Value = "value";
        private const string Timestamp = "timestamp";

        //************************************
        // Default Values
        //************************************
        private const int MinThresholdDefault = 30;
        private const int MaxThresholdDefault = 50;


        //************************************
        // Formats
        //************************************
        private const string ParameterCannotBeNullFormat = "The parameter [{0}] is not defined in the Setting.xml configuration file.";

        //************************************
        // Constants
        //************************************
        private const int DefaultQueueLength = 100;
        private const string Unknown = "Unknown";
        #endregion

        #region Private Fields
        private readonly IOwinAppBuilder startup;
        private readonly string appRoot;
        private readonly ServiceInitializationParameters serviceInitializationParameters;
        private IDisposable serverHandle;
        private string listeningAddress;
        #endregion

        #region Public Constructor
        public OwinCommunicationListener(string appRoot, IOwinAppBuilder startup, ServiceInitializationParameters serviceInitializationParameters)
        {
            this.startup = startup;
            this.appRoot = appRoot;
            this.serviceInitializationParameters = serviceInitializationParameters;
        }
        #endregion

        #region Public Static Properties
        public static string DeviceActorServiceUri { get; private set; }
        #endregion

        #region ICommunicationListener Methods
        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Read settings from the DeviceActorServiceConfig section in the Settings.xml file
                var activationContext = serviceInitializationParameters.CodePackageActivationContext;
                var config = activationContext.GetConfigurationPackageObject(ConfigurationPackage);
                var section = config.Settings.Sections[ConfigurationSection];

                // Check if a parameter called DeviceActorServiceUri exists in the DeviceActorServiceConfig config section
                if (section.Parameters.Any(p => string.Compare(p.Name,
                                                               DeviceActorServiceUriParameter,
                                                               StringComparison.InvariantCultureIgnoreCase) == 0))
                {
                    var parameter = section.Parameters[DeviceActorServiceUriParameter];
                    DeviceActorServiceUri = !string.IsNullOrWhiteSpace(parameter?.Value) ?
                                            parameter.Value :
                                            // By default, the current service assumes that if no URI is explicitly defined for the actor service
                                            // in the Setting.xml file, the latter is hosted in the same Service Fabric application.
                                            $"fabric:/{serviceInitializationParameters.ServiceName.Segments[1]}DeviceActorService";
                }
                else
                {
                    // By default, the current service assumes that if no URI is explicitly defined for the actor service
                    // in the Setting.xml file, the latter is hosted in the same Service Fabric application.
                    DeviceActorServiceUri = $"fabric:/{serviceInitializationParameters.ServiceName.Segments[1]}DeviceActorService";
                }

                var serviceEndpoint = serviceInitializationParameters.CodePackageActivationContext.GetEndpoint("ServiceEndpoint");
                var port = serviceEndpoint.Port;

                listeningAddress = String.Format(
                    CultureInfo.InvariantCulture,
                    "http://+:{0}/{1}",
                    port,
                    String.IsNullOrWhiteSpace(appRoot)
                        ? String.Empty
                        : appRoot.TrimEnd('/') + '/');

                serverHandle = WebApp.Start(listeningAddress, appBuilder => startup.Configuration(appBuilder));
                string publishAddress = listeningAddress.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);

                ServiceEventSource.Current.Message("Listening on {0}", publishAddress);

                return Task.FromResult(publishAddress);
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.Message(ex.Message);
                throw;
            }
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            ServiceEventSource.Current.Message("Close");

            StopWebServer();

            return Task.FromResult(true);
        }

        public void Abort()
        {
            ServiceEventSource.Current.Message("Abort");

            StopWebServer();
        } 
        #endregion

        #region Private Methods
        private void StopWebServer()
        {
            if (serverHandle == null)
            {
                return;
            }
            try
            {
                serverHandle.Dispose();
            }
            catch (ObjectDisposedException)
            {
                // no-op
            }
        } 
        #endregion
    }
}