using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatBoardLib
{
    public class Host
    {
        public List<Agent> Agents { get; set; }

        public static async Task<List<Host>> GetHostsAsync(string[] beaturls, string username, string password)
        {
            var tasks = new List<Task<List<Agent>>>();

            foreach (var beaturl in beaturls)
            {
                tasks.Add(Agent.GetAgentsAsync(beaturl, username, password));
            }

            await Task.WhenAll(tasks);

            return tasks.SelectMany(t => t.Result).GroupBy(a => a.Name).Select(a => new Host { Agents = a.ToList() }).ToList();
        }
    }
}
