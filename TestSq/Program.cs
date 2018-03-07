using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Deployment.Application.ApplicationDeployment;

namespace TestSq
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"IsNetworkDeployed: {IsNetworkDeployed}");
            if (IsNetworkDeployed)
                Console.WriteLine($"CurrentDeployment: {CurrentDeployment?.CurrentVersion}");
            Console.WriteLine($"Assembly: {Assembly.GetExecutingAssembly().GetName().Version}");
            Console.ReadKey();
        }
    }
}
