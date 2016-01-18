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

using System.Threading.Tasks;
using Microsoft.AzureCat.Samples.PayloadEntities;
using Microsoft.ServiceFabric.Actors;

#endregion

// ReSharper disable once CheckNamespace
namespace Microsoft.AzureCat.Samples.DeviceActorService.Interfaces
{
    public interface IDeviceActor : IActor
    {
        Task ProcessEventAsync(Payload payload);
        Task SetData(Device data);
        Task<Device> GetData();
    }
}
