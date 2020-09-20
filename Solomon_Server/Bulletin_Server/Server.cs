using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Solomon_Server
{
    internal class Server
    {
        static void Main(string[] args)
        {
            var server = new WebServiceHost(typeof(Services.SolomonService));
            server.AddServiceEndpoint(typeof(IService), new WebHttpBinding(), "");
            server.Open();
            Console.Title = "Solomon Server";
            Console.WriteLine("Bulletin Server Start");
            Console.WriteLine("If you want to exit this application, please push enter key.");
            Console.ReadLine();
            Console.WriteLine("Bulletin Server Stop");
        }
    }
}
