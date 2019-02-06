# Camera simulator docker module
## Real time data analytics with Azure IoT
[![Build Status](https://dev.azure.com/SamVanhoutte/real-time-traffic-iot/_apis/build/status/SamVanhoutte.real-time-traffic-iot?branchName=master)](https://dev.azure.com/SamVanhoutte/real-time-traffic-iot/_build/latest?definitionId=1&branchName=master)

## Behavior
The camera module is a simulation module that can be used as a standalone Docker container instance, but that can also run as an Azure IoT Edge module.  
The camera module is generating simulated events for cars that get detected by a traffic camera.  The following logic is implemented.


The following messages will be transmitted to the configured IoT Hub, or the camera output endpoint on the Edge module:

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
## Pre requisites 

- It is important to create an Azure IoT Hub instance and get a connection string that has manage rights on it.  (see environment variables below)
- A storage account is needed, where a file should be uploaded with the correct segment configuration in it.  [A sample can be found here](./segment-configs.json).  The connection string for that blob account is needed in the configuration (see environment variables below)

## Configuration of the container (environment variables)

The following environment variables should be specified, when creating a container:

| Configuration key             	| Required by                   	| Description                                                                                                                                                                                                                                	|
|-------------------------------	|-------------------------------	|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------	|
| SEGMENT_ID                    	| Always                           	| Required to indicate the ID of the traffic segment.  This will be used to load the right settings from the ```ISegmentConfigurator```.                                                                                                            	|
| EDGE_MODULE_OUTPUT                   	| Edge module	| Used to set the output name of the IoT Edge module transmitter.  This value is only used, when running as an IoT Edge module.  When not provided, this defaults to `camera`.                                                                            |
| STORAGE_CONNECTION_STRING     	| Always       	| Required by the ```BlobSegmentConfigurator``` to load the `segment-configs.json` file from the `traffic-config` container.                                                                                                                           	|
| IOTHUB_OWNER_CONNECTIONSTRING 	| Standalone 	| Required by the ```IoTHubTransmitterConfigurator``` to create the simulation device or connect to the existing simulation device on the IoT Hub.  Only used when not running in IoT Edge, but as a standalone console app or Container Instance. 	|

## Running the container instance 
### As container instance
To run the container in an interactive mode, execute the following docker command:
```docker
docker run --rm -it -e SEGMENT_ID=02 -e STORAGE_CONNECTION_STRING=xxx -e IOTHUB_OWNER_CONNECTIONSTRING=yyy savanh/traffic-camera-simulator:latest
```

### As edge module
Use the module savanh/traffic-camera-simulator:latest and configure the module with the 2 required environment variables (STORAGE_CONNECTION_STRING and SEGMENT_ID) and configure the routes to get the messages from the output.
The following route, will just send all generated messages from the camera module to the connected IoT Hub:

```json
    {
        "routes": {
            "cloud": "FROM /messages/modules/camera/outputs/* INTO $upstream"
        }
    }
```