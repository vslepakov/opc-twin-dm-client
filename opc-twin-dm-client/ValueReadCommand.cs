using ManyConsole;
using Microsoft.Azure.IIoT.OpcUa.Api.Twin.Models;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace opc_twin_dm_client
{
    public class ValueReadCommand : ConsoleCommand
    {
        public string ServerUrl { get; set; }

        public string NodeId { get; set; }

        public string DeviceId { get; set; }

        public string ModuleName { get; set; }

        public ValueReadCommand()
        {
            // Register the actual command with a simple (optional) description.
            IsCommand("ValueRead", "Read OPC UA Node.");

            // Required options/flags, append '=' to obtain the required value.
            HasRequiredOption("u|url=", "OPC UA Server URL", p => ServerUrl = p);

            // Required options/flags, append '=' to obtain the required value.
            HasRequiredOption("n|nodeId=", "NodeId of the node to start with", p => NodeId = p);

            // Required options/flags, append '=' to obtain the required value.
            HasRequiredOption("d|device=", "IoT Hub DeviceId", p => DeviceId = p);

            // Required options/flags, append '=' to obtain the required value.
            HasRequiredOption("m|module=", "IoT Hub Module Name", p => ModuleName = p);
        }

        public override int Run(string[] remainingArguments)
        {
            return CommandUtil.Execute(RunInternalAsync);
        }

        private async Task RunInternalAsync()
        {
            var twinModuleClient = TwinModuleClientFactory.CreateModueClient(DeviceId, ModuleName);
            var endpoint = TwinModuleClientFactory.CreateEndpoint(ServerUrl);

            var request = new ValueReadRequestApiModel
            {
                NodeId = NodeId
            };

            var result = await twinModuleClient.NodeValueReadAsync(endpoint, request, CancellationToken.None);
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
