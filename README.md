# LogicAppManager
A simple example to show how to use the .NET SDK to manage Azure Logic Apps from a C# .NET Core console app.

# Setup
This example borrows heavily from this blog [Managing Azure Logic Apps using .NET SDK](https://www.serverless360.com/blog/managing-azure-logic-apps-using-net-sdk), 
but simplifies the code to focus only on disabling or enabling Logic Apps. 

Follow the steps in the blog to create the service principal and then authorise it.

# Usage
From the command line run the console app and pass in the following arguments:
- azureSubscriptionId: The subscription id that the Logic App is in
- azureTenantId: The tenant id from the service principal
- azureClientId: The client (or app) id from the service principal
- azureClientSecret: The client secret from the app registration
- logicAppResourceGroupName: The resource group that the Logic App is in
- logicAppName: The name of the Logic App
- logicAppState: True to enable the Logic App, False to disable the Logic App

e.g. dotnet run "GUID" "GUID" "GUID" "GUID" "TheResourceGroup" "LogicAppName" "false"
