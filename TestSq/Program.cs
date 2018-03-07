using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Squirrel;
using static System.Deployment.Application.ApplicationDeployment;
using System.Configuration;

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
            Updater.Update();
            Console.ReadKey();
        }
    }

    public static class Updater
    {
        private static readonly string _updatePath = ConfigurationManager.AppSettings["UpdatePath"];
        private static readonly string _packageId = ConfigurationManager.AppSettings["PackageID"];
        //private const string PACKAGE_ID = "";
        //private static readonly string _path = Path.Combine(Directory.GetCurrentDirectory(), _updatePath);

        public static async Task<UpdateInfo> Check()
        {
            
            Console.WriteLine($"Check update: {_updatePath}");
            
            using (var mgr = new UpdateManager(_updatePath, _packageId)) {
                return await mgr.CheckForUpdate(false, Progress).ConfigureAwait(false);
            }
        }

        private static void Progress(int i)
        {
            Console.Write($"Progress: {i}%");
            Console.Write("\r".PadLeft(Console.WindowWidth - Console.CursorLeft - 1));
        }

        public static async void Update()
        {

            using (var mgr = new UpdateManager(_updatePath, _packageId))
            {
                var updates = await mgr.CheckForUpdate().ConfigureAwait(false);
                Console.WriteLine($"UpdatesAvailable: {updates.ReleasesToApply.Any()}");
                Console.WriteLine($"Latest: {updates.ReleasesToApply.LastOrDefault()?.Version}");
                if (updates.ReleasesToApply.Any())
                {
                    var lastVersion = updates.ReleasesToApply.OrderBy(x => x.Version).Last();
                    await mgr.DownloadReleases(new[] { lastVersion }).ConfigureAwait(false);
                    await mgr.ApplyReleases(updates).ConfigureAwait(false);
                    await mgr.UpdateApp(Progress).ConfigureAwait(false);
                    //mgr.RemoveShortcutsForExecutable(_packageId, ShortcutLocation.Desktop);
                    var exePath = Assembly.GetEntryAssembly().Location;
                    var appName = Path.GetFileName(exePath);
                    mgr.CreateShortcutsForExecutable(appName, ShortcutLocation.Desktop, false);

                    Console.WriteLine("The application has been updated - please close and restart.");
                }
                else
                {
                    Console.WriteLine("No Updates are available at this time.");
                }
            }
        }
    }
}
