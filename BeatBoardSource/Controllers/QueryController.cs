using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BeatBoardLib;

namespace BeatBoardSource.Controllers
{
    [Route("query")]
    [ApiController]
    public class QueryController : ControllerBase
    {
        // POST: Query
        [HttpPost]
        public async Task<IEnumerable<Table>> Post(JObject jobject)
        {
            dynamic settings = System.IO.File.Exists("appsettings.Development.json") ?
                JObject.Parse(System.IO.File.ReadAllText("appsettings.Development.json")) :
                JObject.Parse(System.IO.File.ReadAllText("appsettings.json"));

            string username = settings.Username;
            string password = settings.Password;
            string[] beaturls = settings.Beaturls.ToObject<string[]>();

            List<Host> hosts;

            try
            {
                DateTime now = DateTime.UtcNow;
                DateTime old = now.Add(new TimeSpan(0, -5, 0));

                hosts = await Host.GetHostsAsync(beaturls, username, password);
            }
            catch
            {
                return new Table[] { };
            }


            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            var agents = hosts.SelectMany(m => m.Agents).ToArray();

            var name = new[] { "Name" };
            var columns = name
                .Concat(agents
                    .Select(a => a.BeatType)
                    .Distinct()
                    .OrderBy(b => b))
                .Select(b => new Table.Column() { text = b, type = "string" })
                .ToArray();

            var rows = new List<JArray>();

            foreach (var host in hosts)
            {
                JArray row = new JArray();

                row.Add(host.Name);

                foreach (var column in columns.Skip(1))
                {
                    if (host.Agents.Any(a => a.BeatType == column.text))
                    {
                        var agent = host.Agents.Single(a => a.BeatType == column.text);
                        string value = $"{agent.LastDate:yyyy-MM-dd HH:mm:ss}, {agent.Version}";
                        row.Add(value);
                    }
                    else
                    {
                        row.Add(string.Empty);
                    }
                }

                rows.Add(row);
            }

            var tables = new[] { new Table() { columns = columns, rows = rows.ToArray(), type = "table" } };

            return tables;
        }
    }
}
