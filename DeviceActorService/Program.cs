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
using System.Threading;
using Microsoft.ApplicationInsights;
using Microsoft.ServiceFabric.Actors.Runtime;

#endregion

namespace Microsoft.AzureCat.Samples.DeviceActorService
{
    internal static class Program
    {
        /// <summary>
        /// Application Insights Telemetry Client static field
        /// </summary>
        internal static TelemetryClient TelemetryClient;

        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                TelemetryClient = new TelemetryClient();
            }
            catch (Exception)
            {
                // ignored
            }
            try
            {
                // Create default garbage collection settings for all the actor types
                var actorGarbageCollectionSettings = new ActorGarbageCollectionSettings(300, 60);

                // This line registers your actor class with the Fabric Runtime.
                // The contents of your ServiceManifest.xml and ApplicationManifest.xml files
                // are automatically populated when you build this project.
                // For more information, see http://aka.ms/servicefabricactorsplatform

                ActorRuntime.RegisterActorAsync<DeviceActor>(
                   (context, actorType) => new DeviceActorService(context, actorType, () => new DeviceActor(), null, new ActorServiceSettings
                   {
                       ActorGarbageCollectionSettings = actorGarbageCollectionSettings
                   })).GetAwaiter().GetResult();

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                TelemetryClient.TrackException(e);
                ActorEventSource.Current.ActorHostInitializationFailed(e);
                throw;
            }
            finally
            {
                TelemetryClient.Flush();
            }
        }
    }
}
