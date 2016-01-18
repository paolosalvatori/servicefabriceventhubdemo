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
using Microsoft.AzureCat.Samples.PayloadEntities;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

#endregion

namespace Microsoft.AzureCat.Samples.AlertClient
{
    public class EventProcessor : IEventProcessor
    {
        #region Private Fields

        private EventProcessorFactoryConfiguration configuration;
        #endregion

        #region Public Constructors
        public EventProcessor(EventProcessorFactoryConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            this.configuration = configuration;
        }
        #endregion

        #region IEventProcessor Methods
        public Task OpenAsync(PartitionContext context)
        {
            try
            {
                // Trace Open Partition
                configuration.WriteToLog($"[EventProcessor].[OpenAsync]:: EventHub=[{context.EventHubPath}] ConsumerGroup=[{context.ConsumerGroupName}] PartitionId=[{context.Lease.PartitionId}]");
            }
            catch (Exception ex)
            {
                // Trace Exception
                configuration.WriteToLog($"[EventProcessor].[OpenAsync]:: Exception=[{ex.Message}]");
            }
            return Task.FromResult<object>(null);
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> events)
        {
            try
            {
                var eventDatas = events as EventData[] ?? events.ToArray();
                if (events == null || !eventDatas.Any())
                {
                    return;
                }
                var eventDataList = events as IList<EventData> ?? eventDatas.ToList();

                // Trace Process Events
                configuration.WriteToLog($"[EventProcessor].[ProcessEventsAsync]:: EventHub=[{context.EventHubPath}] ConsumerGroup=[{context.ConsumerGroupName}] PartitionId=[{context.Lease.PartitionId}] EventCount=[{eventDataList.Count}]");

                // Trace individual events
                foreach (var alert in eventDataList.Select(DeserializeEventData).Where(alert => alert != null))
                {
                    // Trace Payload
                    configuration.WriteToLog($"[Alert] DeviceId=[{alert.DeviceId:000}] " +
                                             $"Name=[{alert.Name}] " +
                                             $"Value=[{alert.Value:000}] " +
                                             $"Timestamp=[{alert.Timestamp}]");

                    // Track event
                    configuration.TrackEvent(alert);
                }

                // Checkpoint
                await context.CheckpointAsync();
            }
            catch (AggregateException ex)
            {
                // Trace Exception
                foreach (var exception in ex.InnerExceptions)
                {
                    configuration.WriteToLog(exception.Message);
                }
            }
            catch (Exception ex)
            {
                // Trace Exception
                configuration.WriteToLog($"[EventProcessor].[ProcessEventsAsync]:: Exception=[{ex.Message}]");
            }
        }

        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            try
            {
                // Trace Open Partition
                configuration.WriteToLog($"[EventProcessor].[CloseAsync]:: EventHub=[{context.EventHubPath}] ConsumerGroup=[{context.ConsumerGroupName}] PartitionId=[{context.Lease.PartitionId}] Reason=[{reason}]");

                if (reason == CloseReason.Shutdown)
                {
                    await context.CheckpointAsync();
                }
            }
            catch (LeaseLostException)
            {
            }
            catch (Exception ex)
            {
                // Trace Exception
                configuration.WriteToLog($"[EventProcessor].[CloseAsync]:: Exception=[{ex.Message}]");
            }
        }
        #endregion

        #region Private Static Methods
        private static Alert DeserializeEventData(EventData eventData)
        {
            return JsonConvert.DeserializeObject<Alert>(Encoding.UTF8.GetString(eventData.GetBytes()));
        }
        #endregion
    }
}
