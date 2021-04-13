using ManyConsole;
using Microsoft.Azure.IIoT.Diagnostics;
using Microsoft.Azure.IIoT.Http.Default;
using Microsoft.Azure.IIoT.Hub.Client;
using Microsoft.Azure.IIoT.Module.Default;
using Microsoft.Azure.IIoT.OpcUa.Api.Core.Models;
using Microsoft.Azure.IIoT.OpcUa.Api.Twin.Clients;
using Microsoft.Azure.IIoT.OpcUa.Api.Twin.Models;
using Microsoft.Azure.IIoT.Serializers.NewtonSoft;
using Microsoft.Azure.IIoT.Utils;
using Newtonsoft.Json;
using Serilog.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace opc_twin_dm_client
{
    public class NodeBrowseCommand : ConsoleCommand
    {
        public string ServerUrl { get; set; }

        public string NodeId { get; set; }

        public bool IncludeSubtypes { get; set; }

        public string DeviceId { get; set; }

        public string ModuleName { get; set; }

        public NodeBrowseCommand()
        {
            // Register the actual command with a simple (optional) description.
            IsCommand("NodeBrowse", "Browsing OPC UA Address Space.");

            // Required options/flags, append '=' to obtain the required value.
            HasRequiredOption("u|url=", "OPC UA Server URL", p => ServerUrl = p);

            // Required options/flags, append '=' to obtain the required value.
            HasRequiredOption("n|nodeId=", "NodeId of the node to start with", p => NodeId = p);

            // Required options/flags, append '=' to obtain the required value.
            HasRequiredOption("d|device=", "IoT Hub DeviceId", p => DeviceId = p);

            // Required options/flags, append '=' to obtain the required value.
            HasRequiredOption("m|module=", "IoT Hub Module Name", p => ModuleName = p);

            // Optional options/flags, append ':' to obtain an optional value, or null if not specified.
            HasOption("s|subTypes:", "Include Subtypes.",
                t => IncludeSubtypes = t == null ? true : Convert.ToBoolean(t));
        }

        public override int Run(string[] remainingArguments)
        {
             return CommandUtil.Execute(RunInternalAsync);
        }

        private async Task RunInternalAsync()
        {
            var twinModuleClient = TwinModuleClientFactory.CreateModueClient(DeviceId, ModuleName);
            var endpoint = TwinModuleClientFactory.CreateEndpoint(ServerUrl);

            var request = new BrowseRequestApiModel
            {
                NodeId = NodeId
            };

            var result = await twinModuleClient.NodeBrowseFirstAsync(endpoint, request, CancellationToken.None);
            while (result.ContinuationToken != null)
            {
                try
                {
                    var next = await twinModuleClient.NodeBrowseNextAsync(endpoint,
                        new BrowseNextRequestApiModel
                        {
                            ContinuationToken = result.ContinuationToken,
                            Header = request.Header,
                            ReadVariableValues = request.ReadVariableValues,
                            TargetNodesOnly = request.TargetNodesOnly
                        }, CancellationToken.None);

                    result.References.AddRange(next.References);
                    result.ContinuationToken = next.ContinuationToken;
                }
                catch (Exception)
                {
                    await Try.Async(() => twinModuleClient.NodeBrowseNextAsync(endpoint,
                        new BrowseNextRequestApiModel
                        {
                            ContinuationToken = result.ContinuationToken,
                            Abort = true
                        }, CancellationToken.None));
                    throw;
                }
            }

            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
