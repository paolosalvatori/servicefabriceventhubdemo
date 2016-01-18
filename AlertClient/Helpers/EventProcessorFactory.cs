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
using Microsoft.ServiceBus.Messaging;

#endregion

namespace Microsoft.AzureCat.Samples.AlertClient
{
    public class EventProcessorFactory<T> : IEventProcessorFactory where T : class, IEventProcessor
    {
        #region Private Fields
        private readonly T instance;
        private readonly EventProcessorFactoryConfiguration configuration;
        #endregion

        #region Public Constructors
        public EventProcessorFactory()
        {
            configuration = null;
        }

        public EventProcessorFactory(EventProcessorFactoryConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public EventProcessorFactory(T instance)
        {
            this.instance = instance;
        }
        #endregion

        #region IEventProcessorFactory Methods
        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            return instance ?? Activator.CreateInstance(typeof(T), configuration) as T;
        }
        #endregion
    }
}
