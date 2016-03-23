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

#region Using Directices

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AzureCat.Samples.DeviceActorService.Interfaces;
using Microsoft.AzureCat.Samples.PayloadEntities;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

#endregion

namespace Microsoft.AzureCat.Samples.DeviceManagementWebService
{
    public class DeviceController : ApiController
    {
        #region Private Static Fields
        private static readonly Dictionary<long, IDeviceActor> actorProxyDictionary = new Dictionary<long, IDeviceActor>();
        #endregion

        #region Public Methods
        [HttpGet]
        public async Task<Device> GetDevice(long id)
        {
            try
            {
                var proxy = GetActorProxy(id);
                if (proxy != null)
                {
                    return await proxy.GetData();
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions?.Count > 0)
                {
                    foreach (var exception in ex.InnerExceptions)
                    {
                        ServiceEventSource.Current.Message(exception.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.Message(ex.Message);
            }
            return null;
        }

        [HttpPost]
        [Route("api/devices/get")]
        public async Task<IEnumerable<Device>> GetDevices(IEnumerable<long> ids)
        {
            try
            {
                var enumerable = ids as IList<long> ?? ids.ToList();
                if (ids == null || !enumerable.Any())
                {
                    return null;
                }
                var deviceList = new List<Device>();
                foreach (var id in enumerable)
                {
                    var proxy = GetActorProxy(id);
                    if (proxy != null)
                    {
                        deviceList.Add(await proxy.GetData());
                    }
                }
                return deviceList;
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions?.Count > 0)
                {
                    foreach (var exception in ex.InnerExceptions)
                    {
                        ServiceEventSource.Current.Message(exception.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.Message(ex.Message);
            }
            return null;
        }

        [HttpPost]
        public async Task SetDevice(Device device)
        {
            try
            {
                var proxy = GetActorProxy(device.DeviceId);
                if (proxy != null)
                {
                    await proxy.SetData(device);
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions?.Count > 0)
                {
                    foreach (var exception in ex.InnerExceptions)
                    {
                        ServiceEventSource.Current.Message(exception.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.Message(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/devices/set")]
        public async Task SetDevices(IEnumerable<Device> devices)
        {
            try
            {
                var enumerable = devices as IList<Device> ?? devices.ToList();
                if (devices == null || !enumerable.Any())
                {
                    return;
                }
                foreach (var device in enumerable)
                {
                    var proxy = GetActorProxy(device.DeviceId);
                    if (proxy != null)
                    {
                        await proxy.SetData(device);
                    }
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions?.Count > 0)
                {
                    foreach (var exception in ex.InnerExceptions)
                    {
                        ServiceEventSource.Current.Message(exception.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.Message(ex.Message);
            }
        }
        #endregion

        #region Private Static Methods
        private IDeviceActor GetActorProxy(long deviceId)
        {
            lock (actorProxyDictionary)
            {
                if (actorProxyDictionary.ContainsKey(deviceId))
                {
                    return actorProxyDictionary[deviceId];
                }
                actorProxyDictionary[deviceId] = ActorProxy.Create<IDeviceActor>(new ActorId($"device{deviceId}"),
                                                                                 new Uri(OwinCommunicationListener.DeviceActorServiceUri));
                return actorProxyDictionary[deviceId];
            }
        }
        #endregion
    }
}
