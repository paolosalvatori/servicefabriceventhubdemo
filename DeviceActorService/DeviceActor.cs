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
using System.Fabric;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AzureCat.Samples.DeviceActorService.Interfaces;
using Microsoft.AzureCat.Samples.PayloadEntities;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceFabric.Actors;
using Newtonsoft.Json;

#endregion

namespace Microsoft.AzureCat.Samples.DeviceActorService
{
    [ActorGarbageCollection(IdleTimeoutInSeconds = 300, ScanIntervalInSeconds = 60)]
    public class DeviceActor : StatefulActor<DeviceActorState>, IDeviceActor
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
        private EventHubClient eventHubClient;
        private string serviceBusConnectionString;
        private string eventHubName;
        private int queueLength;
        #endregion

        #region Actor Methods
        protected override Task OnActivateAsync()
        {
            try
            {
                // Initialize State
                if (State == null)
                {
                    State = new DeviceActorState();
                }

                // Initialize Queue
                if (State.Queue == null)
                {
                    State.Queue = new Queue<Payload>();
                }

                // Create default data
                if (State.Data == null)
                {
                    // The device id is a string with the following format: device<number>
                    var deviceIdAsString = Id.ToString();
                    long deviceId;
                    long.TryParse(deviceIdAsString.Substring(6), out deviceId);

                    State.Data = new Device
                    {
                        DeviceId = deviceId,
                        Name = deviceIdAsString,
                        MinThreshold = MinThresholdDefault,
                        MaxThreshold = MaxThresholdDefault,
                        Model = Unknown,
                        Type = Unknown,
                        Manufacturer = Unknown,
                        City = Unknown,
                        Country = Unknown
                    };
                }

                // Read settings from Settings.xml
                ReadSettings();

                // Create EventHubClient
                CreateEventHubClient();
            }
            catch (Exception ex)
            {
                // Trace exception as ETW event
                ActorEventSource.Current.Error(ex);

                // Trace exception using Application Insights
                Program.TelemetryClient.TrackException(ex, new Dictionary<string, string>
                {
                    { "ActorType", "DeviceActor"},
                    { "ActorId", Id.ToString()},
                    { "ServiceName", ActorService.ServiceInitializationParameters.ServiceName.ToString()},
                    { "Partition", ActorService.ServicePartition.PartitionInfo.Id.ToString()},
                    { "Node", FabricRuntime.GetNodeContext().NodeName}
                });
            }
            return Task.FromResult(0);
        }

        protected override Task OnDeactivateAsync()
        {
            return Task.FromResult(true);
        }
        #endregion

        #region IDeviceActor Methods
        public async Task ProcessEventAsync(Payload payload)
        {
            try
            {
                // Enqueue the new payload
                var queue = State.Queue;
                queue.Enqueue(payload);

                // The actor keeps the latest n payloads in a queue, where n is  
                // defined by the QueueLength parameter in the Settings.xml file.
                if (queue.Count > queueLength)
                {
                    queue.Dequeue();
                }

                // Trace ETW event
                ActorEventSource.Current.Message($"Id=[{payload.DeviceId}] Value=[{payload.Value}] Timestamp=[{payload.Timestamp}]");

                // This ETW event is traced to a separate table with respect to the message
                ActorEventSource.Current.Telemetry(State.Data, payload);

                // Track Application Insights event
                Program.TelemetryClient.TrackEvent(new EventTelemetry
                {
                    Name = "Telemetry",
                    Properties =
                            {
                                {"DeviceId", State.Data.DeviceId.ToString(CultureInfo.InvariantCulture)},
                                {"Name", State.Data.Name},
                                {"City", State.Data.City},
                                {"Country", State.Data.Country},
                                {"Manufacturer", State.Data.Manufacturer},
                                {"Model", State.Data.Model},
                                {"Type", State.Data.Type},
                                {"MinThreshold", State.Data.MinThreshold.ToString(CultureInfo.InvariantCulture)},
                                {"MaxThreshold", State.Data.MaxThreshold.ToString(CultureInfo.InvariantCulture)},
                                {"Value", payload.Value.ToString(CultureInfo.InvariantCulture)},
                                {"Status", payload.Status},
                                {"Timestamp", payload.Timestamp.ToString(CultureInfo.InvariantCulture)},
                                {"ActorType", "DeviceActor"},
                                {"ActorId", Id.ToString()},
                                {"ServiceName", ActorService.ServiceInitializationParameters.ServiceName.ToString()},
                                {"Partition", ActorService.ServicePartition.PartitionInfo.Id.ToString()},
                                {"Node", FabricRuntime.GetNodeContext().NodeName}
                            },
                    Metrics =
                            {
                                {"TelemetryValue", payload.Value},
                                {"TelemetryCount", 1}
                            },
                });

                // Real spikes happen when both Spike1 and Spike2 are equal to 1. By the way, you can change the logic
                if (payload.Value < State.Data.MinThreshold || payload.Value > State.Data.MaxThreshold)
                {
                    // Create EventData object with the payload serialized in JSON format 
                    var alert = new Alert
                    {
                        DeviceId = State.Data.DeviceId,
                        Name = State.Data.Name,
                        MinThreshold = State.Data.MinThreshold,
                        MaxThreshold = State.Data.MaxThreshold,
                        Model = State.Data.Model,
                        Type = State.Data.Type,
                        Manufacturer = State.Data.Manufacturer,
                        City = State.Data.City,
                        Country = State.Data.Country,
                        Status = payload.Status,
                        Value = payload.Value,
                        Timestamp = payload.Timestamp
                    };
                    var json = JsonConvert.SerializeObject(alert);
                    using (var eventData = new EventData(Encoding.UTF8.GetBytes(json))
                    {
                        PartitionKey = payload.Name
                    })
                    {
                        // Create custom properties
                        eventData.Properties.Add(DeviceId, payload.DeviceId);
                        eventData.Properties.Add(Value, payload.Value);
                        eventData.Properties.Add(Timestamp, payload.Timestamp);

                        // Send the event to the event hub
                        await eventHubClient.SendAsync(eventData);

                        // Trace ETW event
                        ActorEventSource.Current.Message($"[Alert] Id=[{payload.DeviceId}] Value=[{payload.Value}] Timestamp=[{payload.Timestamp}]");

                        // This ETW event is traced to a separate table
                        ActorEventSource.Current.Alert(State.Data, payload);

                        // Track Application Insights event
                        Program.TelemetryClient.TrackEvent(new EventTelemetry
                        {
                            Name = "Alert",
                            Properties =
                            {
                                {"DeviceId", State.Data.DeviceId.ToString(CultureInfo.InvariantCulture)},
                                {"Name", State.Data.Name},
                                {"City", State.Data.City},
                                {"Country", State.Data.Country},
                                {"Manufacturer", State.Data.Manufacturer},
                                {"Model", State.Data.Model},
                                {"Type", State.Data.Type},
                                {"MinThreshold", State.Data.MinThreshold.ToString(CultureInfo.InvariantCulture)},
                                {"MaxThreshold", State.Data.MaxThreshold.ToString(CultureInfo.InvariantCulture)},
                                {"Value", payload.Value.ToString(CultureInfo.InvariantCulture)},
                                {"Status", payload.Status},
                                {"Timestamp", payload.Timestamp.ToString(CultureInfo.InvariantCulture)},
                                {"ActorType", "DeviceActor"},
                                {"ActorId", Id.ToString()},
                                {"ServiceName", ActorService.ServiceInitializationParameters.ServiceName.ToString()},
                                {"Partition", ActorService.ServicePartition.PartitionInfo.Id.ToString()},
                                {"Node", FabricRuntime.GetNodeContext().NodeName}
                            },
                            Metrics =
                            {
                                {"AlertValue", payload.Value},
                                {"AlertCount", 1}
                            },
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Trace exception as ETW event
                ActorEventSource.Current.Error(ex);

                // Trace exception using Application Insights
                Program.TelemetryClient.TrackException(ex, new Dictionary<string, string>
                {
                    { "ActorType", "DeviceActor"},
                    { "ActorId", Id.ToString()},
                    { "ServiceName", ActorService.ServiceInitializationParameters.ServiceName.ToString()},
                    { "Partition", ActorService.ServicePartition.PartitionInfo.Id.ToString()},
                    { "Node", FabricRuntime.GetNodeContext().NodeName}
                });
            }
        }

        public Task SetData(Device data)
        {
            State.Data = data;

            // Trace ETW event
            ActorEventSource.Current.Metadata(data);

            // Track Application Insights event
            Program.TelemetryClient.TrackEvent(new EventTelemetry
            {
                Name = "Metadata",
                Properties =
                            {
                                {"DeviceId", State.Data.DeviceId.ToString(CultureInfo.InvariantCulture)},
                                {"Name", State.Data.Name},
                                {"City", State.Data.City},
                                {"Country", State.Data.Country},
                                {"Manufacturer", State.Data.Manufacturer},
                                {"Model", State.Data.Model},
                                {"Type", State.Data.Type},
                                {"MinThreshold", State.Data.MinThreshold.ToString(CultureInfo.InvariantCulture)},
                                {"MaxThreshold", State.Data.MaxThreshold.ToString(CultureInfo.InvariantCulture)},
                                {"ActorType", "DeviceActor"},
                                {"ActorId", Id.ToString()},
                                {"ServiceName", ActorService.ServiceInitializationParameters.ServiceName.ToString()},
                                {"Partition", ActorService.ServicePartition.PartitionInfo.Id.ToString()},
                                {"Node", FabricRuntime.GetNodeContext().NodeName}
                            }
            });

            return Task.FromResult(0);
        }

        public Task<Device> GetData()
        {
            return Task.FromResult(State.Data);
        }

        #endregion

        #region Private Methods
        private void ReadSettings()
        {
            // Read settings from the DeviceActorServiceConfig section in the Settings.xml file
            var activationContext = ActorService.ServiceInitializationParameters.CodePackageActivationContext;
            var config = activationContext.GetConfigurationPackageObject(ConfigurationPackage);
            var section = config.Settings.Sections[ConfigurationSection];

            // Read the ServiceBusConnectionString setting from the Settings.xml file
            var parameter = section.Parameters[ServiceBusConnectionStringParameter];
            if (!string.IsNullOrWhiteSpace(parameter?.Value))
            {
                serviceBusConnectionString = parameter.Value;
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
                eventHubName = parameter.Value;
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
                if (!int.TryParse(parameter.Value, out queueLength))
                {
                    queueLength = DefaultQueueLength;
                }
            }
            else
            {
                throw new ArgumentException(string.Format(ParameterCannotBeNullFormat, QueueLengthParameter),
                                            QueueLengthParameter);
            }
        }

        public void CreateEventHubClient()
        {
            if (string.IsNullOrWhiteSpace(serviceBusConnectionString) || string.IsNullOrWhiteSpace(eventHubName))
            {
                return;
            }
            eventHubClient = EventHubClient.CreateFromConnectionString(serviceBusConnectionString, eventHubName);
            ActorEventSource.Current.Message($"Id=[{State.Data.DeviceId}] EventHubClient created");
        }
        #endregion
    }
}
