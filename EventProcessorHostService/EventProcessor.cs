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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AzureCat.Samples.DeviceActorService.Interfaces;
using Microsoft.AzureCat.Samples.PayloadEntities;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceFabric.Actors;
using Newtonsoft.Json;

#endregion

namespace Microsoft.AzureCat.Samples.EventProcessorHostService
{
    public class EventProcessor : IEventProcessor
    {
        #region Private Constants
        private const string DeviceActorServiceUriCannotBeNull = "DeviceActorServiceUri setting cannot be null";
        #endregion

        #region Private Fields
        private readonly Uri serviceUri;
        #endregion

        #region Private Static Fields
        private static readonly Dictionary<long, IDeviceActor> actorProxyDictionary = new Dictionary<long, IDeviceActor>();
        #endregion

        #region Public Constructors
        public EventProcessor(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                throw new ArgumentNullException(DeviceActorServiceUriCannotBeNull);
            }
            serviceUri = new Uri(parameter);
        }
        #endregion

        #region IEventProcessor Methods
        public Task OpenAsync(PartitionContext context)
        {
           return Task.FromResult<object>(null);
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> events)
        {
            try
            {
                if (events == null)
                {
                    return;
                }
                var eventDataList = events as IList<EventData> ?? events.ToList();

                // Trace individual events
                foreach (var payload in eventDataList.Select(DeserializeEventData))
                {
                    // Invoke Actor
                    if (payload == null)
                    {
                        continue;
                    }

                    // Invoke Device Actor
                    var proxy = GetActorProxy(payload.DeviceId);
                    if (proxy != null)
                    {
                        await proxy.ProcessEventAsync(payload);
                    }
                }

                // Checkpoint
                await context.CheckpointAsync();
            }
            catch (LeaseLostException ex)
            {
                // Trace Exception as message
                ServiceEventSource.Current.Message(ex.Message);
            }
            catch (AggregateException ex)
            {
                // Trace Exception
                foreach (var exception in ex.InnerExceptions)
                {
                    ServiceEventSource.Current.Message(exception.Message);
                }
            }
            catch (Exception ex)
            {
                // Trace Exception
                ServiceEventSource.Current.Message(ex.Message);
            }
        }

        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            try
            {

                if (reason == CloseReason.Shutdown)
                {
                    await context.CheckpointAsync();
                }
            }
            catch (Exception ex)
            {
                // Trace Exception
                ServiceEventSource.Current.Message(ex.Message);
            }
        }
        #endregion

        #region Private Static Methods
        private static Payload DeserializeEventData(EventData eventData)
        {
            return JsonConvert.DeserializeObject<Payload>(Encoding.UTF8.GetString(eventData.GetBytes()));
        }

        private IDeviceActor GetActorProxy(long deviceId)
        {
            lock (actorProxyDictionary)
            {
                if (actorProxyDictionary.ContainsKey(deviceId))
                {
                    return actorProxyDictionary[deviceId];
                }
                actorProxyDictionary[deviceId] = ActorProxy.Create<IDeviceActor>(new ActorId(deviceId), serviceUri);
                return actorProxyDictionary[deviceId];
            }
        }
        #endregion
    }
}
