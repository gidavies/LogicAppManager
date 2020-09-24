using Microsoft.Azure.Management.Logic;
using System;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Common.Authentication.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;

namespace Azure.Examples
{
    class LogicAppManager
    {
        static void Main(string[] args)
        {
            // Basic arg validation.
            if (args.Length != 7)
            {
                Console.WriteLine("Usage: LogicAppManager <azureSubscriptionId> <azureTenantId> <azureClientId> <azureClientSecret> <logicAppResourceGroupName> <logicAppName> <logicAppState>");
                Console.WriteLine("where logicAppState is true to enable the Logic App or false to disable it.");
                return;
            }

            var azureSubscriptionId = args[0];
            var azureTenantId = args[1];
            var azureClientId = args[2];
            var azureClientSecret = args[3];
            var logicAppResourceGroupName = args[4];
            var logicAppName = args[5];
            
            bool logicAppState;
            bool test = bool.TryParse(args[6], out logicAppState);
            if (!test)
            {
                Console.WriteLine("Final parameter must be true or false to enable or disable the Logic App.");
            }

            Console.WriteLine($"Setting {logicAppName} enabled state to {logicAppState}");

            var result = ManageLogicApp.WorkflowStateChangeAsync(
                    azureSubscriptionId,
                    azureTenantId,
                    azureClientId,
                    azureClientSecret,
                    logicAppResourceGroupName,
                    logicAppName,
                    logicAppState)
                .GetAwaiter()
                .GetResult();

            Console.WriteLine($"Success: {result}"); 
        }
    }

    public class ManageLogicApp
    {
        public static async Task<bool> WorkflowStateChangeAsync(
            string azureSubscriptionId, 
            string tenantId, 
            string azureClientId, 
            string azureClientSecret, 
            string logicAppResourceGroupName, 
            string logicAppName,
            bool logicAppState
        )
        {
            var logicManagementClient = await GetLogicManagementClient(azureSubscriptionId, tenantId, azureClientId, azureClientSecret);

            if (logicAppState)
                logicManagementClient.Workflows.Enable(logicAppResourceGroupName, logicAppName);
            else
                logicManagementClient.Workflows.Disable(logicAppResourceGroupName, logicAppName);

            return true;
        }

        private static async Task<LogicManagementClient> GetLogicManagementClient(string azureSubscriptionId, string tenantId, string azureClientId, string azureClientSecret)
        {
            var environment = AzureEnvironment.PublicEnvironments[EnvironmentName.AzureCloud];

            var authority = string.Format("{0}{1}", environment.Endpoints[AzureEnvironment.Endpoint.ActiveDirectory], tenantId);

            var authContext = new AuthenticationContext(authority);

            var credential = new ClientCredential(azureClientId, azureClientSecret);

            var authResult = await GetAuthenticationResult(authContext, environment.Endpoints[AzureEnvironment.Endpoint.ActiveDirectoryServiceEndpointResourceId], credential);

            var tokenCloudCredentials = new TokenCloudCredentials(azureSubscriptionId, authResult.AccessToken);

            var tokenCreds = new TokenCredentials(tokenCloudCredentials.Token);

            return new LogicManagementClient(tokenCreds) { SubscriptionId = azureSubscriptionId };
        }

        private static async Task<AuthenticationResult> GetAuthenticationResult(AuthenticationContext authContext, string resource, ClientCredential credentials)
        {
            return await authContext.AcquireTokenAsync(resource, credentials);
        }
    }
}
