---
services: service-fabric, event-hubs
platforms: dotnet
author: paolosalvatori
---
# IoT Sample with Service Fabric and Event Hubs #
This demo demonstrates how to build an IoT application for anomaly detection using Service Fabric, Event Hubs,OWIN and ASP.NET Web API.<br/>

# Architecture Design #
The following picture shows the architecture design of the application.
<br/>
<br/>
![alt tag](https://raw.githubusercontent.com/paolosalvatori/servicefabriceventhubdemo/master/Images/VerticalArchitectureDesign.png)
<br/>

# Service Fabric Application #
The Service Fabric application ingest events from the input Event Hub, processes sensor readings and generates an alert whenever a value outside of the tolerance range is received. The application is composed of three services:
	

- **DeviceManagementWebService**: this is a stateless service hosting OWIN and exposing a REST management service. This service can be used to get or set metadata on devices actors.
- **EventProcessorHostService**: this is a stateless service that creates an **EventProcessorHost** listener to receive messages from the event hub. Note: the number of service instances matches the number of event hub partitions. The **ProcessEventsAsync** method of the **EventProcessor** class uses an **ActorProxy** to send individual events to the proper actor, one for each device.
- **DeviceActorService**: a stateful device actor is used to process incoming events at the device level. Each actor processes incoming sensor readings and generates an alert whenever the actual value is outside of the tolerance  range identified by the **MinThreshold** and **MaxThreshold** properties. Alerts are sent to an output Event Hub.

# Message Flow #
1. A client application built using Windows Forms is used to simulate a device management console. When you press the Start button, the client application invokes the OWIN-hosted REST service exposed by the **DeviceManagementWebService** stateless service to initialize the device actors with the following metadata:
	- DeviceId
	- Name
	- MinThreshold
	- MaxThreshold
	- Model
	- Type
	- Manufacturer
	- City
	- Country

![alt tag](https://raw.githubusercontent.com/paolosalvatori/servicefabriceventhubdemo/master/Images/DeviceSimulator.png)

2. The **DeviceManagementWebService** stateless service activates the device actors and set their metadata, which is part of their persistent state.
3. For each device, the client application creates a separate task that simulates a distinct device sending messages to the input Event Hub. Each task acquires uses a different publisher endpoint. In particular, each task uses a SAS policy defined on the input Event Hub to acquire a SAS token to send events to its publisher endpoint.
4. The **EventProcessorHostService** uses an **EventProcessorHost** listener to receive messages from the input Event Hub.
5. The **ProcessEventsAsync** method of the **EventProcessor** class uses an **ActorProxy** to send individual events to the proper device actor.
6. Each actor processes incoming sensor readings and generates an alert whenever the actual value is outside of the tolerance  range identified by the **MinThreshold** and **MaxThreshold** properties. Alerts are sent to an output Event Hub.
7. An **Alert Client** application built using Windows Forms uses **EventProcessorHost** listener to read alerts from the output Event Hub.
8. The **Alert Client** displays incoming events in a **DataGridView**.

![alt tag](https://raw.githubusercontent.com/paolosalvatori/servicefabriceventhubdemo/master/Images/AlertClient.png)

# Application Configuration #
Make sure to replace the following placeholders in the project files below before deploying and testing the application on the local development Service Fabric cluster or before deploying the application to your Service Fabric cluster on Microsoft Azure.

## Placeholders ##
This list contains the placeholders that need to be replaced before deploying and running the application:


- **[Input-Event-Hub-Service-Bus-Namespace]**: contains the name of the **Service Bus** namespace that contains the input **Event Hub**.
- **[Input-Event-Hub-Service-Bus-Key-Name]**: contains the name of the **Shared Access Key** used to connect to the **Service Bus** namespace that contains the input **Event Hub**.
- **[Input-Event-Hub-Service-Bus-Key-Value]**: contains the value of the **Shared Access Key** used to connect to the **Service Bus** namespace that contains the input **Event Hub**.
- **[Input-Event-Hub-Service-Bus-Namespace]**: contains the name of the **Service Bus** namespace that contains the input **Event Hub**.
- **[Input-Event-Hub-Name];[Input-Event-Hub-Name];...**: contains a list of input **Event Hubs** defined in the input **Service Bus**.
- **[Output-Event-Hub-Name];[Output-Event-Hub-Name];...**: contains a list of output **Event Hubs** defined in the output **Service Bus**.
- **[Cluster-Name]**: the name of the **Service Fabric** cluster on Azure.
- **[Cluster-Location]**: the location (e.g. eastus or westeurope) of the **Service Fabric** cluster on Azure. [Cluster-Location]
- **[Input-Event-Storage-Account-Connection-String]**: contains the connection string of the **Storage Account** used by the **EventProcessorHost** to store partition lease information when reading data from the input **Event Hub**.
- **[Input-Event-Consumer-Group-Name]**: contains the name of the **Consumer Group** used by the **EventProcessorHost** to read data from the input **Event Hub**.
- **[Input-Event-Hub-Name]**: contains the name of the input **Event Hub**.
- **[Output-Event-Hub-Service-Bus-Namespace]**: contains the connection string of the **Service Bus** namespace containing the output **Event Hub** used by the application.
- **[Output-Event-Storage-Account-Connection-String]**: contains the connection string of the **Storage Account** used by the **EventProcessorHost** to store partition lease information when reading data from the output **Event Hub**.
- **[Output-Event-Consumer-Group-Name]**: contains the name of the **Consumer Group** used by the **EventProcessorHost** to read data from the output **Event Hub**.
- **[Output-Event-Hub-Name]**: contains the name of the output **Event Hub**.

## Project Files ##

**App.config** file in the **DeviceSimulator** project:

```xml
    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
      <appSettings>
	    <add key="url" value="http://localhost:8088/devicemanagement;http://[Cluster-Name].[Cluster-Location].cloudapp.azure.com:8088/devicemanagement" />
	    <add key="namespace" value="[Input-Event-Hub-Service-Bus-Namespace]" />
	    <add key="keyName" value="[Input-Event-Hub-Service-Bus-Key-Name]" />
	    <add key="keyValue" value="[Input-Event-Hub-Service-Bus-Key-Value]" />
	    <add key="eventHub" value="[Input-Event-Hub-Name];[Input-Event-Hub-Name];..." />
	    <add key="deviceCount" value="100" />
	    <add key="eventInterval" value="2000" />
	    <add key="minValue" value="30" />
	    <add key="maxValue" value="40" />
	    <add key="minOffset" value="20" />
	    <add key="maxOffset" value="30" />
	    <add key="spikePercentage" value="5" />
      </appSettings>
      ...
    </configuration>
```

**App.config** file in the **AlertClient** project:

```xml
    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
      <appSettings>
	    <add key="storageAccountConnectionString" value="[Output-Event-Hub-Storage-Account-Connection-String]" />
	    <add key="serviceBusConnectionString" value="[Output-Event-Hub-Service-Bus-Connection-String]" />
        <add key="consumerGroup" value="[Output-Event-Hub-Consumer-Group-Name]"/>
	    <add key="eventHub" value="[Output-Event-Hub-Name];[Output-Event-Hub-Name];..." />
      </appSettings>
      ...
    </configuration>
```

**ApplicationParameters\Local.xml** file in the **IoTDemo** project:

```xml
    <?xml version="1.0" encoding="utf-8"?>
    <Application xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Name="fabric:/IoTDemo" xmlns="http://schemas.microsoft.com/2011/01/fabric">
       <Parameters>
      <Parameter Name="EventProcessorHostService_InstanceCount" Value="-1" />
      <Parameter Name="EventProcessorHostService_StorageAccountConnectionString" Value="[Input-Event-Hub-Storage-Account-Connection-String]" />
      <Parameter Name="EventProcessorHostService_ServiceBusConnectionString" Value="[Input-Event-Hub-Service-Bus-Connection-String]" />
      <Parameter Name="EventProcessorHostService_ConsumerGroupName" Value="[Input-Event-Hub-Consumer-Group-Name]" />
      <Parameter Name="EventProcessorHostService_EventHubName" Value="[Input-Event-Hub-Name]" />
      <Parameter Name="DeviceManagementWebService_InstanceCount" Value="1" />
      <Parameter Name="DeviceActorService_PartitionCount" Value="1" />
      <Parameter Name="DeviceActorService_ServiceBusConnectionString" Value="[Output-Event-Hub-Service-Bus-Connection-String]" />
      <Parameter Name="DeviceActorService_EventHubName" Value="[Output-Event-Hub-Name]" />
      <Parameter Name="DeviceActorService_QueueLength" Value="100" />
       </Parameters>
    </Application>
```

**ApplicationParameters\Cloud.xml** file in the **IoTDemo** project:

```xml
    <?xml version="1.0" encoding="utf-8"?>
    <Application xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Name="fabric:/IoTDemo" xmlns="http://schemas.microsoft.com/2011/01/fabric">
       <Parameters>
      <Parameter Name="EventProcessorHostService_InstanceCount" Value="-1" />
      <Parameter Name="EventProcessorHostService_StorageAccountConnectionString" Value="[Input-Event-Hub-Storage-Account-Connection-String]" />
      <Parameter Name="EventProcessorHostService_ServiceBusConnectionString" Value="[Input-Event-Hub-Service-Bus-Connection-String]" />
      <Parameter Name="EventProcessorHostService_ConsumerGroupName" Value="[Input-Event-Hub-Consumer-Group-Name]" />
      <Parameter Name="EventProcessorHostService_EventHubName" Value="[Input-Event-Hub-Name]" />
      <Parameter Name="DeviceManagementWebService_InstanceCount" Value="-1" />
      <Parameter Name="DeviceActorService_PartitionCount" Value="5" />
      <Parameter Name="DeviceActorService_ServiceBusConnectionString" Value="[Output-Event-Hub-Service-Bus-Connection-String]" />
      <Parameter Name="DeviceActorService_EventHubName" Value="[Output-Event-Hub-Name]" />
      <Parameter Name="DeviceActorService_QueueLength" Value="100" />
       </Parameters>
    </Application>
```

**ApplicationManifest.xml** file in the **IoTDemo** project:

```xml
    <?xml version="1.0" encoding="utf-8"?>
    <ApplicationManifest xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" ApplicationTypeName="IoTDemoType" ApplicationTypeVersion="1.0.0" xmlns="http://schemas.microsoft.com/2011/01/fabric">
       <Parameters>
      <Parameter Name="DeviceActorService_PartitionCount" DefaultValue="5" />
      <Parameter Name="DeviceActorService_MinReplicaSetSize" DefaultValue="2" />
      <Parameter Name="DeviceActorService_TargetReplicaSetSize" DefaultValue="3" />
      <Parameter Name="DeviceActorService_ServiceBusConnectionString" DefaultValue="[Output-Event-Hub-Service-Bus-Connection-String]" />
      <Parameter Name="DeviceActorService_EventHubName" DefaultValue="[Output-Event-Hub-Name]" />
      <Parameter Name="DeviceActorService_QueueLength" DefaultValue="100" />
      <Parameter Name="EventProcessorHostService_InstanceCount" DefaultValue="-1" />
      <Parameter Name="EventProcessorHostService_StorageAccountConnectionString" DefaultValue="[Input-Event-Hub-Storage-Account-Connection-String]" />
      <Parameter Name="EventProcessorHostService_ServiceBusConnectionString" DefaultValue="[Input-Event-Hub-Service-Bus-Connection-String]" />
      <Parameter Name="EventProcessorHostService_ConsumerGroupName" DefaultValue="[Input-Event-Hub-Consumer-Group-Name]" />
      <Parameter Name="EventProcessorHostService_EventHubName" DefaultValue="[Input-Event-Hub-Name]" />
      <Parameter Name="DeviceManagementWebService_InstanceCount" DefaultValue="-1" />
       </Parameters>
       <ServiceManifestImport>
      <ServiceManifestRef ServiceManifestName="DeviceActorServicePkg" ServiceManifestVersion="1.0.0" />
      <ConfigOverrides>
     <ConfigOverride Name="Config">
    <Settings>
       <Section Name="DeviceActorServiceConfig">
      <Parameter Name="ServiceBusConnectionString" Value="[DeviceActorService_ServiceBusConnectionString]" />
      <Parameter Name="EventHubName" Value="[DeviceActorService_EventHubName]" />
      <Parameter Name="QueueLength" Value="[DeviceActorService_QueueLength]" />
       </Section>
    </Settings>
     </ConfigOverride>
      </ConfigOverrides>
       </ServiceManifestImport>
       <ServiceManifestImport>
      <ServiceManifestRef ServiceManifestName="EventProcessorHostServicePkg" ServiceManifestVersion="1.0.0" />
      <ConfigOverrides>
     <ConfigOverride Name="Config">
    <Settings>
       <Section Name="EventProcessorHostConfig">
      <Parameter Name="StorageAccountConnectionString" Value="[EventProcessorHostService_StorageAccountConnectionString]" />
      <Parameter Name="ServiceBusConnectionString" Value="[EventProcessorHostService_ServiceBusConnectionString]" />
      <Parameter Name="EventHubName" Value="[EventProcessorHostService_EventHubName]" />
      <Parameter Name="ConsumerGroupName" Value="[EventProcessorHostService_ConsumerGroupName]" />
       </Section>
    </Settings>
     </ConfigOverride>
      </ConfigOverrides>
       </ServiceManifestImport>
       <ServiceManifestImport>
      <ServiceManifestRef ServiceManifestName="DeviceManagementWebServicePkg" ServiceManifestVersion="1.0.0" />
      <ConfigOverrides />
       </ServiceManifestImport>
       <DefaultServices>
      <Service Name="DeviceActorService" GeneratedIdRef="de1d8306-f193-45ac-aa15-3efde9ffcb78">
     <StatefulService ServiceTypeName="DeviceActorServiceType" TargetReplicaSetSize="[DeviceActorService_TargetReplicaSetSize]" MinReplicaSetSize="[DeviceActorService_MinReplicaSetSize]">
    <UniformInt64Partition PartitionCount="[DeviceActorService_PartitionCount]" LowKey="-9223372036854775808" HighKey="9223372036854775807" />
     </StatefulService>
      </Service>
      <Service Name="EventProcessorHostService">
     <StatelessService ServiceTypeName="EventProcessorHostServiceType" InstanceCount="[EventProcessorHostService_InstanceCount]">
    <SingletonPartition />
     </StatelessService>
      </Service>
      <Service Name="DeviceManagementWebService">
     <StatelessService ServiceTypeName="DeviceManagementWebServiceType" InstanceCount="[DeviceManagementWebService_InstanceCount]">
    <SingletonPartition />
     </StatelessService>
      </Service>
       </DefaultServices>
    </ApplicationManifest>
```
