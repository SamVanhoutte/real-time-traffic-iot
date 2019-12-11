# Azure IoT Edge components
## Real time data analytics with Azure IoT
[![Build Status](https://dev.azure.com/SamVanhoutte/real-time-traffic-iot/_apis/build/status/SamVanhoutte.real-time-traffic-iot?branchName=master)](https://dev.azure.com/SamVanhoutte/real-time-traffic-iot/_build/latest?definitionId=1&branchName=master)

The connectivity part of the demo is provided through Azure IoT Edge.  Azure IoT Edge is the component that takes care of connectivity and data analytics on the edge.  

The solution comes with two custom modules for Azure IoT Edge: CameraModule and DisplayModule.
- __Camera Module__: This module is a simulation of the traffic camera that scans cars as they pass by. 
- __Display Module__: This module is a module that displays traffic information or speeding alerts above the highway.  Currently, the module is just writing this information to the output of the console.
