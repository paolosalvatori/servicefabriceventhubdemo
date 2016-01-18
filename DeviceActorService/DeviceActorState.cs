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
using System.Runtime.Serialization;
using Microsoft.AzureCat.Samples.PayloadEntities;

#endregion

namespace Microsoft.AzureCat.Samples.DeviceActorService
{
    [Serializable]
    [DataContract]
    public class DeviceActorState
    {
        [DataMember]
        public Queue<Payload> Queue { get; set; }

        [DataMember]
        public Device Data { get; set; }
    }
}
