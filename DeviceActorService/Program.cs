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
using System.Threading;
using Microsoft.ServiceFabric.Actors; 
#endregion

namespace Microsoft.AzureCat.Samples.DeviceActorService
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                // Creating a FabricRuntime connects this host process to the Service Fabric runtime on this node.
                using (FabricRuntime fabricRuntime = FabricRuntime.Create())
                {
                    // This line registers your actor class with the Fabric Runtime.
                    // The contents of your ServiceManifest.xml and ApplicationManifest.xml files
                    // are automatically populated when you build this project.
                    // For more information, see http://aka.ms/servicefabricactorsplatform
                    fabricRuntime.RegisterActor<DeviceActor>();

                    Thread.Sleep(Timeout.Infinite);  // Prevents this host process from terminating to keep the service host process running.
                }
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
