using Microsoft.Azure.IIoT.Diagnostics;
using Microsoft.Azure.IIoT.Http.Default;
using Microsoft.Azure.IIoT.Hub.Client;
using Microsoft.Azure.IIoT.Module.Default;
using Microsoft.Azure.IIoT.OpcUa.Api.Core.Models;
using Microsoft.Azure.IIoT.OpcUa.Api.Twin;
using Microsoft.Azure.IIoT.OpcUa.Api.Twin.Clients;
using Microsoft.Azure.IIoT.Serializers.NewtonSoft;
using Microsoft.Azure.IIoT.Utils;
using Serilog.Events;
using System;

namespace opc_twin_dm_client
{
    public static class TwinModuleClientFactory
    {
        public static ITwinModuleApi CreateModueClient(string deviceId, string moduleName)
        {
            var logger = ConsoleLogger.Create(LogEventLevel.Error);
            var iotHubConfig = ConnectionStringEx.ToIoTHubConfig(Environment.GetEnvironmentVariable("IOT_HUB_CONNECTION_STRING"));

            var httpClient = new HttpClient(logger);
            var iotHubServiceClient = new IoTHubServiceHttpClient(httpClient, iotHubConfig, new NewtonSoftJsonSerializer(), logger);
            var methodClient = new ChunkMethodClient(new IoTHubTwinMethodClient(iotHubServiceClient, logger), new NewtonSoftJsonSerializer(), logger);
            return new TwinModuleClient(methodClient, deviceId, moduleName);
        }

        public static EndpointApiModel CreateEndpoint(string url)
        {
            return new EndpointApiModel
            {
                Url = url
            };
        }
    }
}
