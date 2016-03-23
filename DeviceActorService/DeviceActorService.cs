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
using Microsoft.ServiceFabric.Actors.Runtime;

#endregion

namespace Microsoft.AzureCat.Samples.DeviceActorService
{
    
    public class DeviceActorService : ActorService
    {
        #region Private Constants
        //************************************
        // Parameters
        //************************************
        private const string ConfigurationPackage = "Config";
        private const string ConfigurationSection = "DeviceActorServiceConfig";
        private const string ServiceBusConnectionStringParameter = "ServiceBusConnectionString";
        private const string EventHubNameParameter = "EventHubName";
        private const string QueueLengthParameter = "QueueLength";

        //************************************
        // Formats
        //************************************
        private const string ParameterCannotBeNullFormat = "The parameter [{0}] is not defined in the Setting.xml configuration file.";

        //************************************
        // Constants
        //************************************
        private const int DefaultQueueLength = 100;
        #endregion

        #region Public Constructor

        public DeviceActorService(StatefulServiceContext context, 
                                  ActorTypeInformation typeInfo,
                                  Func<ActorBase> actorFactory = null, 
                                  IActorStateProvider stateProvider = null, 
                                  ActorServiceSettings settings = null)
            : base(context, typeInfo, actorFactory, stateProvider, settings)
        {
            // Read settings from the DeviceActorServiceConfig section in the Settings.xml file
            var activationContext = Context.CodePackageActivationContext;
            var config = activationContext.GetConfigurationPackageObject(ConfigurationPackage);
            var section = config.Settings.Sections[ConfigurationSection];

            // Read the ServiceBusConnectionString setting from the Settings.xml file
            var parameter = section.Parameters[ServiceBusConnectionStringParameter];
            if (!string.IsNullOrWhiteSpace(parameter?.Value))
            {
                ServiceBusConnectionString = parameter.Value;
            }
            else
            {
                throw new ArgumentException(
                    string.Format(ParameterCannotBeNullFormat, ServiceBusConnectionStringParameter),
                                  ServiceBusConnectionStringParameter);
            }

            // Read the EventHubName setting from the Settings.xml file
            parameter = section.Parameters[EventHubNameParameter];
            if (!string.IsNullOrWhiteSpace(parameter?.Value))
            {
                EventHubName = parameter.Value;
            }
            else
            {
                throw new ArgumentException(string.Format(ParameterCannotBeNullFormat, EventHubNameParameter),
                                            EventHubNameParameter);
            }

            // Read the QueueLength setting from the Settings.xml file
            parameter = section.Parameters[QueueLengthParameter];
            if (!string.IsNullOrWhiteSpace(parameter?.Value))
            {
                QueueLength = DefaultQueueLength;
                int queueLength;
                if (int.TryParse(parameter.Value, out queueLength))
                {
                    QueueLength = queueLength;
                }
            }
            else
            {
                throw new ArgumentException(string.Format(ParameterCannotBeNullFormat, QueueLengthParameter),
                                            QueueLengthParameter);
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the service bus connection string
        /// </summary>
        public string ServiceBusConnectionString { get; private set; }

        /// <summary>
        /// Gets or sets the event hub name
        /// </summary>
        public string EventHubName { get; private set; }

        /// <summary>
        /// Gets or sets the queue length
        /// </summary>
        public int QueueLength { get; private set; }
        #endregion
    }
}