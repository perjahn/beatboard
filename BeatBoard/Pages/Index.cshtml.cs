using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatBoardLib;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;

namespace BeatBoard.Pages
{
    public class IndexModel : PageModel
    {
        public string[] Columns { get; private set; }
        public string[,] Values { get; private set; }
        public bool[,] Old { get; private set; }

        public async Task OnGet()
        {
            dynamic settings = System.IO.File.Exists("appsettings.Development.json") ?
                JObject.Parse(System.IO.File.ReadAllText("appsettings.Development.json")) :
                JObject.Parse(System.IO.File.ReadAllText("appsettings.json"));

            string username = settings.Username;
            string password = settings.Password;
            string[] beaturls = settings.Beaturls.ToObject<string[]>();

            try
            {
                await ListBeatsAsync(beaturls, username, password, new TimeSpan(0, 5, 0));
            }
            catch
            {
                Values = new string[1, 1];
                Values[0, 0] = "Tough luck";
                Old = new bool[1, 1];
                Old[0, 0] = false;
            }
        }

        async Task ListBeatsAsync(string[] beaturls, string username, string password, TimeSpan duration)
        {
            DateTime now = DateTime.UtcNow;
            DateTime old = now.Add(-duration);

            var hosts = await Host.GetHostsAsync(beaturls, username, password);

            GenerateTableValues(hosts, old);
        }

        void GenerateTableValues(List<Host> hosts, DateTime old)
        {
            var agents = hosts.SelectMany(m => m.Agents).ToArray();

            var name = new[] { "Name" };
            Columns = name.Concat(agents.Select(a => a.BeatType).Distinct().OrderBy(b => b)).ToArray();

            var sortedHosts = hosts.OrderBy(h => h.Name).ToArray();

            Values = new string[sortedHosts.Length, Columns.Length];
            Old = new bool[sortedHosts.Length, Columns.Length];

            for (int hostIndex = 0; hostIndex < sortedHosts.Length; hostIndex++)
            {
                var host = sortedHosts[hostIndex];

                var value = host.Name;
                Values[hostIndex, 0] = value;

                for (int column = 1; column < Columns.Length; column++)
                {
                    if (host.Agents.Any(a => a.BeatType == Columns[column]))
                    {
                        var agent = host.Agents.Single(a => a.BeatType == Columns[column]);
                        value = $"{agent.LastDate:yyyy-MM-dd HH:mm:ss}, {agent.Version}";

                        Values[hostIndex, column] = value;
                        Old[hostIndex, column] = agent.LastDate < old;
                    }
                    else
                    {
                        Values[hostIndex, column] = string.Empty;
                        Old[hostIndex, column] = false;
                    }
                }
            }
        }
    }
}
