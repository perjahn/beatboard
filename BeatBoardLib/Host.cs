using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeatBoardLib
{
    public class Host
    {
        public string Name { get; set; } = string.Empty;
        public List<Agent> Agents { get; set; } = new List<Agent>();

        public static async Task<List<Host>> GetHostsAsync(string[] beaturls, string username, string password)
        {
            var tasks = new List<Task<List<Agent>>>();

            foreach (var beaturl in beaturls)
            {
                tasks.Add(Agent.GetAgentsAsync(beaturl, username, password));
            }

            await Task.WhenAll(tasks);

            return tasks.SelectMany(t => t.Result).GroupBy(a => a.Name).Select(a => new Host { Name = a.First().Name, Agents = a.ToList() }).ToList();
        }
    }
}
