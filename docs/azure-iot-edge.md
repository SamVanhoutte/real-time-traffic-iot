# Azure IoT Edge components
## Real time data analytics with Azure IoT
[![Build Status](https://dev.azure.com/SamVanhoutte/real-time-traffic-iot/_apis/build/status/SamVanhoutte.real-time-traffic-iot?branchName=master)](https://dev.azure.com/SamVanhoutte/real-time-traffic-iot/_build/latest?definitionId=1&branchName=master)

The connectivity part of the demo is provided through Azure IoT Edge.  Azure IoT Edge is the component that takes care of connectivity and data analytics on the edge.  

The solution comes with two custom modules for Azure IoT Edge: CameraModule and DisplayModule.
- __Camera Module__: This module is a simulation of the traffic camera that scans cars as they pass by. 
- __Display Module__: This module is a module that displays traffic information or speeding alerts above the highway.  Currently, the module is just writing this information to the output of the console.

# Building the edge modules
Building the edge modules is best done by installing the [Azure IoT Edge plugin for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=vsciot-vscode.azure-iot-edge).  
1. Install the plugin
1. Open the root directory of this repository in Visual Studio Code
1. Edit the module.json if you want to update the version number (`image.tag.version`) and the repository (`image.repository`) for the image you will build.
1. Press <`Ctrl+Shift+P`> to open the command menu and select `Azure IoT Edge: Build IoT Edge Module Image`.
1. In the next step, select the right `module.json` file (two of them should appear as suggestion)
1. Select the right platform you want to build for (amd64 is quite common on a development box)
1. Watch the output window for the evoluation of your build.
1. Push the module to your repository

## Building and configuring the Camera module
The camera module is a simulation module that can be built into a Docker compatible container image, but that can also run as a netcore console application.  

### Configuration of the module
The behavior of the module is defined by injected dependencies that implement the specific interfaces.  This is also the case for the configuration of the module.  The default (current code) behavior is that all configuration is specified in the following code base:
```csharp
    var serviceCollection = new ServiceCollection()
        .AddLogging()
        .AddSingleton<IConfigurationReader, EnvironmentConfigurationReader>()
        .AddSingleton<ITrafficSegmentConfigurator, BlobSegmentConfigurator>()
        .AddSingleton<ITimeSimulationSettings, TimeSimulationSettings>()
        .AddSingleton<IEventGenerator, EventGenerator>();
```

This means that, by default, the configuration values should be coming through the use of Environment Variables.  It is however possible to implement your own IConfigurationReader that could read configuration values from a configuration file, or other location.

In order to run the simulator as a Container Instance or Edge module, it is needed to provide the right configuration values as Environment Variable on the container creation options.

The following configuration values should be specified:
| Configuration key             	| Required by                   	| Description                                                                                                                                                                                                                                	|
|-------------------------------	|-------------------------------	|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------	|
| STORAGE_CONNECTION_STRING     	| BlobSegmentConfigurator       	| Required by the ```BlobSegmentConfigurator``` to load the `segment.configs.json` file from the `traffic-config` container.                                                                                                                           	|
| IOTHUB_OWNER_CONNECTIONSTRING 	| IoTHubTransmitterConfigurator 	| Required by the ```IoTHubTransmitterConfigurator``` to create the simulation device or connect to the existing simulation device on the IoT Hub.  Only used when not running in IoT Edge, but as a standalone console app or Container Instance. 	|
| SEGMENT_ID                    	| All                           	| Required to indicate the ID of the traffic segment.  This will be used to load the right settings from the ```ISegmentConfigurator```.                                                                                                            	|