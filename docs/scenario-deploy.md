# Deployment guidelines
## Real time data analytics with Azure IoT
[![Build Status](https://dev.azure.com/SamVanhoutte/real-time-traffic-iot/_apis/build/status/SamVanhoutte.real-time-traffic-iot?branchName=master)](https://dev.azure.com/SamVanhoutte/real-time-traffic-iot/_build/latest?definitionId=1&branchName=master)

## Deploy core artifacts
In order to deploy the core artifacts, you can use the ARM template file, [located here](../deploy/arm/01-core-artifacts/template.json).  The only (optional) parameter that has to be passed to the deployment is ´solution_name´ (which defaults to 'traffic').  This variable will be used to make all artifacts unique.  (the value should not contain hyphen, but only alphanumeric characters, as it is also used to create a storage account)

Deploying the template through PowerShell can be done, using the following code:
```powershell
Connect-AzAccount

Select-AzSubscription -SubscriptionName <yourSubscriptionName>

New-AzResourceGroup -Name <yourResourceGroup> -Location "West Europe"
New-AzResourceGroupDeployment -Name TrafficDeployment -ResourceGroupName <yourResourceGroup> -TemplateFile .\deploy\arm\01-core-artifacts\template.json -solution_name trafficdemo
```

A successful deployment should result in the following components to be deployed.
![initial architecture schema](./images/scenario-deploy-01.png "Initial architecture schema")

## Deploy Azure Stream Analytics artifacts

To do

 ## Deploy Azure Databricks artifacts

To do