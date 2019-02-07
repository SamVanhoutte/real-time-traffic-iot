New-AzResourceGroup -Name traffic-newdeploy -Location "West Europe"
New-AzResourceGroupDeployment -Name ExampleDeployment -ResourceGroupName traffic-newdeploy -Mode Complete -TemplateFile .\deploy\arm\01-iot-hub\template.json -solution_name sabambo
