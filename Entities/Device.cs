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

using Newtonsoft.Json;

#endregion

namespace Microsoft.AzureCat.Samples.PayloadEntities
{
    public class Device
    {
        /// <summary>
        /// Gets or sets the device id.
        /// </summary>
        [JsonProperty(PropertyName = "deviceId", Order = 1)]
        public long DeviceId { get; set; }

        /// <summary>
        /// Gets or sets the device name.
        /// </summary>
        [JsonProperty(PropertyName = "name", Order = 2)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the device min threshold.
        /// </summary>
        [JsonProperty(PropertyName = "minThreshold", Order = 3)]
        public int MinThreshold { get; set; }

        /// <summary>
        /// Gets or sets the device max threshold.
        /// </summary>
        [JsonProperty(PropertyName = "maxThreshold", Order = 4)]
        public int MaxThreshold { get; set; }

        /// <summary>
        /// Gets or sets the device model.
        /// </summary>
        [JsonProperty(PropertyName = "model", Order = 5)]
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the device type.
        /// </summary>
        [JsonProperty(PropertyName = "type", Order = 6)]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the device manufacturer.
        /// </summary>
        [JsonProperty(PropertyName = "manufacturer", Order = 7)]
        public string Manufacturer { get; set; }

        /// <summary>
        /// Gets or sets the device dity.
        /// </summary>
        [JsonProperty(PropertyName = "city", Order = 8)]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the device country.
        /// </summary>
        [JsonProperty(PropertyName = "country", Order = 9)]
        public string Country { get; set; }
    }
}
