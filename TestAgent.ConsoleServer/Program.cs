using System;
using System.Threading;
using TestAgent.Services.FileService;
using TestAgent.Services.TestService;

namespace TestAgent.ConsoleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileServiceHost = new FileServiceHost(9123, @"C:\Uploads");

            fileServiceHost.LaunchService();

            Console.WriteLine("File Server Address: {0}", fileServiceHost.ServiceAddress);
            Console.WriteLine("File Server State: {0}", fileServiceHost.State);

            var testServiceHost = new TestServiceHost(9123, @"C:\Uploads");

            testServiceHost.LaunchService();

            Console.WriteLine("Test Server Address: {0}", testServiceHost.ServiceAddress);
            Console.WriteLine("Test Server State: {0}", testServiceHost.State);

            while (true)
            {
                Thread.Sleep(5000);
            }
        }
    }
}
