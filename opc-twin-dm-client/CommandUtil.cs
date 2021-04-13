using System;
using System.Threading.Tasks;

namespace opc_twin_dm_client
{
    public static class CommandUtil
    {
        private const int Success = 0;
        private const int Failure = 2;

        public static int Execute(Func<Task> func)
        {
            try
            {
                func().Wait();
                return Success;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Failed to browse nodes.", ex);
                return Failure;
            }
        }
    }
}
