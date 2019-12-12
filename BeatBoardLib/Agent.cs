using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BeatBoardLib
{
    public class Agent
    {
        public string Name { get; set; } = string.Empty;
        public DateTime LastDate { get; set; }
        public string Version { get; set; } = string.Empty;
        public string BeatType { get; set; } = string.Empty;

        public static async Task<List<Agent>> GetAgentsAsync(string beaturl, string username, string password)
        {
            var agents = new List<Agent>();

            using (var client = new HttpClient())
            {
                int malformed = 0;

                client.BaseAddress = new Uri(beaturl);

                string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                string fieldname = "agent.name";

                string url = $"{beaturl}/_search";
                string json = "{ \"size\": 0, \"aggs\": { \"names\": { \"terms\": { \"field\": \"" + fieldname + "\", \"size\": 1000 } } } }";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get,
                    Content = new ByteArrayContent(Encoding.UTF8.GetBytes(json))
                };

                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var result = await client.SendAsync(request);
                try
                {
                    result.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException)
                {
                    fieldname = "agent.name.keyword";
                    Console.WriteLine($"Using fieldname: '{fieldname}'");
                    json = "{ \"size\": 0, \"aggs\": { \"names\": { \"terms\": { \"field\": \"" + fieldname + "\", \"size\": 1000 } } } }";

                    request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(url),
                        Method = HttpMethod.Get,
                        Content = new ByteArrayContent(Encoding.UTF8.GetBytes(json))
                    };

                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    result = await client.SendAsync(request);
                    result.EnsureSuccessStatusCode();
                }


                dynamic jsonresult = JObject.Parse(await result.Content.ReadAsStringAsync());

                if (jsonresult["aggregations"] == null)
                {
                    return agents;
                }

                dynamic aggs = (JToken)jsonresult.aggregations;
                dynamic names = (JToken)aggs.names;
                dynamic buckets = (JArray)names.buckets;

                Console.WriteLine($"{beaturl}: Got {buckets.Count} agents.");

                url = $"{beaturl}/_search";
                foreach (var bucket in buckets)
                {
                    if (bucket == null)
                    {
                        continue;
                    }

                    string agentsname = bucket.key.ToString();

                    json = "{ \"query\": { \"term\": { \"" + fieldname + "\": \"" + agentsname + "\" } }, \"sort\": [ { \"@timestamp\": { \"order\": \"desc\", \"mode\": \"max\" } } ], \"size\": 1 }";

                    request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(url),
                        Method = HttpMethod.Get,
                        Content = new ByteArrayContent(Encoding.UTF8.GetBytes(json))
                    };

                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    result = await client.SendAsync(request);
                    result.EnsureSuccessStatusCode();

                    jsonresult = JObject.Parse(await result.Content.ReadAsStringAsync());

                    dynamic source = (JObject)jsonresult["hits"]["hits"][0]["_source"];

                    string datestring = source["@timestamp"].ToString();
                    if (datestring.Contains(","))
                    {
                        malformed++;
                        datestring = datestring.Replace(',', '.');
                    }
                    if (!DateTime.TryParse(datestring, out DateTime date))
                    {
                        Console.WriteLine($"Couldn't parse datestring: '{datestring}'");
                        Console.WriteLine($"_id: '{jsonresult["hits"]["hits"][0]["_id"]}'");
                        continue;
                    }

                    string version = source["agent"]["version"].ToString();

                    agents.Add(new Agent
                    {
                        Name = agentsname,
                        LastDate = date,
                        Version = version,
                        BeatType = LastPart(beaturl, '/')
                    });

                    Console.Write(".");
                }

                Console.WriteLine();
                if (malformed > 0)
                {
                    Console.WriteLine($"Malformed: {malformed}");
                }
            }

            return agents;
        }

        private static string LastPart(string s, char c)
        {
            int index = s.LastIndexOf(c);
            return index < 0 ? s : s.Substring(index + 1);
        }
    }
}
