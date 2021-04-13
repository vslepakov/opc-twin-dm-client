using System.Collections.Generic;
using ManyConsole;
using System;

namespace opc_twin_dm_client
{
    class Program
    {
        static int Main(string[] args)
        {

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IOT_HUB_CONNECTION_STRING")))
            {
                Console.Error.WriteLine("Please provide IoT Hub Owner or Service Connection String in 'IOT_HUB_CONNECTION_STRING' environment variable");
                return 2;
            }

            var commands = GetCommands();
            return ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
        }

        private static IEnumerable<ConsoleCommand> GetCommands()
        {
            return ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));
        }
    }
}
