# Real time data analytics with Azure IoT
[![Build Status](https://dev.azure.com/SamVanhoutte/real-time-traffic-iot/_apis/build/status/SamVanhoutte.real-time-traffic-iot?branchName=master)](https://dev.azure.com/SamVanhoutte/real-time-traffic-iot/_build/latest?definitionId=1&branchName=master)

This project contains a fully working solution on Azure IoT that ingests data from (simulated) traffic camera and applies real time intelligence in a PaaS setup.  The following technologies are involved:
- Azure IoT Edge
- Azure IoT Hub
- Azure Databricks
- Azure Stream Analytics
- Azure Time Series Insights
- Azure Functions
- Azure Event Grid
- Azure Logic Apps

# Installation

This will project will be supplied with all relevant arm templates to deploy everything to your Microsoft Azure subscription.

# Documentation

- [Automated deployment of the scenario](./docs/scenario-deploy.md)
- [Using the public camera simulator container image](./docs/docker-camera-simulator.md)
- [Building and configurating the Azure IoT Edge components](./docs/azure-iot-edge.md)

# Powerpoint presentation
The latest powerpoint presentation can be found on my slideshare account:

<iframe src="//www.slideshare.net/slideshow/embed_code/key/hucoCTPtnQuLJD" width="595" height="485" frameborder="0" marginwidth="0" marginheight="0" scrolling="no" style="border:1px solid #CCC; border-width:1px; margin-bottom:5px; max-width: 100%;" allowfullscreen> </iframe> <div style="margin-bottom:5px"> <strong> <a href="//www.slideshare.net/samvanhoutte/real-time-analytics-in-azure-iot" title="Real time analytics in Azure IoT " target="_blank">Real time analytics in Azure IoT </a> </strong> from <strong><a href="https://www.slideshare.net/samvanhoutte" target="_blank">Sam Vanhoutte</a></strong> </div>

# License Information
This is licensed under The MIT License (MIT). Which means that you can use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the web application. But you always need to state that Codit is the original author of this web application.
