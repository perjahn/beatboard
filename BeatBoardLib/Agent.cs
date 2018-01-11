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
        public List<string> GetAgentsAsync()
        {
            List<string> agents = new List<string>();

            using (HttpClient client = new HttpClient())
            {
                string baseurl = System.IO.File.ReadAllLines("elastic.txt")[0];
                string username = System.IO.File.ReadAllLines("elastic.txt")[1];
                string password = System.IO.File.ReadAllLines("elastic.txt")[2];

                client.BaseAddress = new Uri(baseurl);

                string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);


                string url = $"{baseurl}/metricbeat-*/_search";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get,
                };

                string json = "{ \"size\": \"0\", \"aggs\": { \"names\": { \"terms\": { \"field\": \"beat.name\", \"size\": \"1000\" } } } }";
                request.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(json));

                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var result = client.SendAsync(request).Result;
                result.EnsureSuccessStatusCode();

                JObject buckets = JObject.Parse(result.Content.ReadAsStringAsync().Result);


                foreach (var bucket in buckets["aggregations"]["names"]["buckets"])
                {
                    agents.Add(bucket["key"].ToString());
                }

            }

            return agents;
        }
    }
}
