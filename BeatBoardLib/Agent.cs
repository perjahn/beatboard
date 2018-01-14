using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace BeatBoardLib
{
    public class Agent
    {
        public string name { get; set; }
        public DateTime lastdate { get; set; }
        public string version { get; set; }

        public static List<Agent> GetAgents()
        {
            List<Agent> agents = new List<Agent>();

            using (HttpClient client = new HttpClient())
            {
                string baseurl = System.IO.File.ReadAllLines("elastic.txt")[0];
                string username = System.IO.File.ReadAllLines("elastic.txt")[1];
                string password = System.IO.File.ReadAllLines("elastic.txt")[2];

                client.BaseAddress = new Uri(baseurl);

                string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);


                string url = $"{baseurl}/metricbeat-*/_search";
                string json = "{ \"size\": 0, \"aggs\": { \"names\": { \"terms\": { \"field\": \"beat.name\", \"size\": 1000 } } } }";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get,
                    Content = new ByteArrayContent(Encoding.UTF8.GetBytes(json))
                };


                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var result = client.SendAsync(request).Result;
                result.EnsureSuccessStatusCode();

                JObject jsonresult = JObject.Parse(result.Content.ReadAsStringAsync().Result);

                JArray buckets = (JArray)jsonresult["aggregations"]["names"]["buckets"];

                Console.WriteLine($"Got {buckets.Count} agents.");

                url = $"{baseurl}/metricbeat-*/_search";
                foreach (var bucket in buckets)
                {
                    string agentsname = bucket["key"].ToString();

                    json = "{ \"query\": { \"term\": { \"beat.name\": \"" + agentsname + "\" } }, \"sort\": [ { \"@timestamp\": { \"order\": \"desc\", \"mode\": \"max\" } } ], \"size\": 1 }";

                    request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(url),
                        Method = HttpMethod.Get,
                        Content = new ByteArrayContent(Encoding.UTF8.GetBytes(json))
                    };

                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    result = client.SendAsync(request).Result;
                    result.EnsureSuccessStatusCode();

                    jsonresult = JObject.Parse(result.Content.ReadAsStringAsync().Result);

                    JObject source = (JObject)jsonresult["hits"]["hits"][0]["_source"];

                    agents.Add(new Agent
                    {
                        name = agentsname,
                        lastdate = source["@timestamp"].ToObject<DateTime>(),
                        version = source["beat"]["version"].ToString()
                    });

                    Console.Write(".");
                }

                Console.WriteLine();
            }

            return agents;
        }
    }
}
