# Azure IoT Edge components
## Real time data analytics with Azure IoT
[![Build Status](https://dev.azure.com/SamVanhoutte/real-time-traffic-iot/_apis/build/status/SamVanhoutte.real-time-traffic-iot?branchName=master)](https://dev.azure.com/SamVanhoutte/real-time-traffic-iot/_build/latest?definitionId=1&branchName=master)

The connectivity part of the demo is provided through Azure IoT Edge.  Azure IoT Edge is the component that takes care of connectivity and data analytics on the edge.  

The solution comes with two custom modules for Azure IoT Edge: CameraModule and DisplayModule.
- __Camera Module__: This module is a simulation of the traffic camera that scans cars as they pass by. 
- __Display Module__: This module is a module that displays traffic information or speeding alerts above the highway.  Currently, the module is just writing this information to the output of the console.

# Using the edge modules
## Building the modules
Building the edge modules is best done by installing the [Azure IoT Edge plugin for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=vsciot-vscode.azure-iot-edge).  
1. Install the plugin
1. Open the root directory of this repository in Visual Studio Code
1. Edit the module.json if you want to update the version number (`image.tag.version`) and the repository (`image.repository`) for the image you will build.
1. Press <`Ctrl+Shift+P`> to open the command menu and select `Azure IoT Edge: Build IoT Edge Module Image`.
1. In the next step, select the right `module.json` file (two of them should appear as suggestion)
1. Select the right platform you want to build for (amd64 is quite common on a development box)
1. Watch the output window for the evoluation of your build.
1. Push the module to your repository

## The Camera module
The camera module is a simulation module that can be built into a Docker compatible container image, but that can also run as a netcore console application.  
The camera module is generating simulated events for cars that get detected by a traffic camera.  The following logic is implemented.
1. The simulated clock is configured, using the TimeSimulationAcceleration settings.  This allows to generate events in a faster pace than the actual time.  
1. The traffic segment configuration is loaded from the injected ```ITrafficSegmentConfigurator```.  The default one is the ```BlobSegmentConfigurator```.  This configurator, loads the configuration from a storage account, using the connection string, provided in the `STORAGE_CONNECTION_STRING` configuration variable.  Using this connection string, the `segment-configs.json` file from the `traffic-config` container is loaded and the right segment configuration is loaded, using the configuration variable defined in the `SEGMENT_ID` configuration variable.  A sample configuration file can be found [here](segment-configs.json) and the description of the fields can be found below this section.
1. After the segment configuration is loaded, two ```IEventTransmitter``` instances are created (based on the injected ```ICameraTransmitterConfigurator```).  These instances (defaulting to IoT Hub transmitter when running standalone, or to IoT Edge transmitter, when running as an Edge module) will be used to transmit messages from the simulation module.  The IoT Hub event transmitter will create or get the device on the IoT Hub, using the right connection string credentials.
1.  Once all of the above logic is successful, the simulation will start generating simulated events and will transmit them to the configured endpoints.  The following is an example of a transmitted telemetry message.

    ```json
    [
        {
            "trajectid":"01",
            "cameraid":"Camera2",
            "eventtime":"2019-01-16T05:16:21.5200000Z",
            "lane":2,
            "country":"BE",
            "licenseplate":"1-UVF-558",
            "make":"BMW",
            "color":"DarkGray"
        }
    ]
    ```
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

This means that, by default, the configuration values should be coming through the use of Environment Variables.  It is however possible to implement your own ```IConfigurationReader``` that could read configuration values from a configuration file, or other location.

In order to run the simulator as a Container Instance or Edge module, it is needed to provide the right configuration values as Environment Variable on the container creation options.

The following configuration values should be specified:

| Configuration key             	| Required by                   	| Description                                                                                                                                                                                                                                	|
|-------------------------------	|-------------------------------	|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------	|
| SEGMENT_ID                    	| All                           	| Required to indicate the ID of the traffic segment.  This will be used to load the right settings from the ```ISegmentConfigurator```.                                                                                                            	|
| EDGE_MODULE_OUTPUT                   	| IoTEdgeModuleTransmitterConfiguration	| Used to set the output name of the IoT Edge module transmitter.  This value is only used, when running as an IoT Edge module.  When not provided, this defaults to camera.                                                                            |
| STORAGE_CONNECTION_STRING     	| BlobSegmentConfigurator       	| Required by the ```BlobSegmentConfigurator``` to load the `segment-configs.json` file from the `traffic-config` container.                                                                                                                           	|
| IOTHUB_OWNER_CONNECTIONSTRING 	| IoTHubTransmitterConfigurator 	| Required by the ```IoTHubTransmitterConfigurator``` to create the simulation device or connect to the existing simulation device on the IoT Hub.  Only used when not running in IoT Edge, but as a standalone console app or Container Instance. 	|


### Segment configuration

| Json value           	| Default when not provided 	| Description                                                                                                                                                                                                                     	|
|----------------------	|---------------------------	|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------	|
| segmentId            	| __required__                	| The id of the segment that will be used to match with the specific simulator instance.                                                                                                                                          	|
| description          	| __empty__                   	| The description or name of the segment. (example: E17 Highway Ghent)                                                                                                                                                            	|
| lat                  	| __empty__                   	| Latitude of the segment                                                                                                                                                                                                         	|
| lon                  	| __empty__                   	| Longitude of the segment                                                                                                                                                                                                        	|
| numberOfLanes        	| 3                         	| The number of lanes on the traffic segment                                                                                                                                                                                      	|
| speedLimit           	| 120                       	| The maximum speed limit on the segment (expressed in Km / h)                                                                                                                                                                    	|
| rushHours            	| __empty__                   	| A configuration of the rush hour periods.  (example: 7:00-8:00,8:15-10:00,true) A comma seperated list of time periods, optionally with a boolean as last value that indicates if rushhour is also applied during weekend days. 	|
| maxSpeed             	| 180                       	| The fastest speed a simulated car on this segment can have.                                                                                                                                                                     	|
| minSpeed             	| 20                        	| The slowest speed a simulated car on this segment should have.                                                                                                                                                                  	|
| averageCarsPerMinute 	| 30                        	| The average cars per minute that will drive through the segment.  (this will vary throughout the simulation)                                                                                                                    	|
| speedingPercentage   	| 2                         	| The percentage of simulated cars that should be speeding on the segment.                                                                                                                                                        	|
| cameraDistance       	| 2000                      	| The distance (in meter) between the two cameras.  This will be used for measuring speed.                                                                                                                                        	|


## The display module
The display module is a very straight forward and simple module that just accepts incoming messages (on the camera input endpoint).  
The messages the module expects are deserialized and a message is displayed, using the console out (only using the `licenseplate` property).