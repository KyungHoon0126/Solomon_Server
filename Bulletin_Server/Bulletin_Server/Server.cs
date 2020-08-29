using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Bulletin_Server
{
    internal class Server
    {
        static void Main(string[] args)
        {
            var server = new WebServiceHost(typeof(Service.MealService, tyepof(Service.BulletinService));
            server.AddServiceEndpoint(typeof(IService), new WebHttpBinding(), "");
            server.Open();
            Console.WriteLine("Bulletin Server Start");
            Console.WriteLine("If you want to exit this application, please push enter key.");
            Console.ReadLine();
            Console.WriteLine("Bulletin Server Stop");
        }
    }
}
