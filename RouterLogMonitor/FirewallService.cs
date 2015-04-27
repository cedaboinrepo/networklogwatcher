using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace RouterLogMonitor
{
    public partial class FirewallService : ServiceBase
    {
        System.Timers.Timer timer;

        public FirewallService(string[] args)
        {
            this.ServiceName = "RouterLogMonitor";
        }

        internal void TestStartupAndStop(string[] args)
        {
            this.OnStart(args);
            Console.ReadLine();
            this.OnStop();
        }

        protected override void OnStart(string[] args)
        {
            eventLog = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("ReadRouterLog"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "ReadRouterLog", "ReadRouterLog event created");
            }

            eventLog.Source = "ReadRouterLog";
            eventLog.Log = string.Empty;

            eventLog.WriteEntry("RouterLogMonitor OnStart");

            // Create a timer with a two second interval.
            timer = new System.Timers.Timer(120000);
            // Hook up the Elapsed event for the timer. 
            timer.Elapsed += OnTimedEvent;
            timer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            CreateFirewallRouterLogs();
        }

        protected override void OnStop()
        {
            timer.Close();
            timer.Dispose();
            eventLog.WriteEntry("RouterLogMonitor OnStop");
        }

        protected override void OnContinue()
        {
            eventLog.WriteEntry("RouterLogMonitor OnContinue.");
        }

        private void CreateFirewallRouterLogs()
        {
            var folder = @"c:\FirewallLogs";
            FileOperations.CreateDirectory(folder);

            Router.Login();

            var log = Router.GetFirewallLogs();
            var location = GeoLocation.GetIpGeoLocation(log);
            var locationInfo = GeoLocation.GetIpGeoLocationInfo(log);

            var fileName = string.Format("{0}\\xfinity_FLOG_{1}.{2}", folder, DateTime.Now.ToShortDateString().Replace("/", "-"), "txt");

            if (location != null && !string.IsNullOrEmpty(location.latitude) && !string.IsNullOrEmpty(location.longitude))
            {

                var logLine = log.Date + "|" + log.Source + "|" + log.Destination + "|" + location.country_name + "|" + location.city
                    + "|" + location.latitude + "|" + location.longitude;

                FileOperations.WriteToFile(fileName, logLine);

            }
            else if (locationInfo != null && !string.IsNullOrEmpty(locationInfo.loc))
            {
                var coords = locationInfo.loc.Split(',');
                var lat = coords[0];
                var lon = coords[1];

                var logLineInfo = log.Date + "|" + log.Source + "|" + log.Destination + "|" + locationInfo.country + "|" + locationInfo.city
                    + "|" + lat + "|" + lon;

                FileOperations.WriteToFile(fileName, logLineInfo);
            }
        }
    }
}
