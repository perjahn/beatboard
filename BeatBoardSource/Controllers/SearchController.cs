using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BeatBoardSource.Controllers
{
    [Route("search")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        // POST: Search
        [HttpPost]
        public async Task<IEnumerable<string>> Post(JObject jobject)
        {
            await Task.CompletedTask;

            dynamic settings = System.IO.File.Exists("appsettings.Development.json") ?
                JObject.Parse(System.IO.File.ReadAllText("appsettings.Development.json")) :
                JObject.Parse(System.IO.File.ReadAllText("appsettings.json"));

            string[] beaturls = settings.Beaturls.ToObject<string[]>();
            beaturls = beaturls.Select(u => LastPart(u, '/')).OrderBy(u => u).ToArray();

            return beaturls;
        }

        private static string LastPart(string s, char c)
        {
            int index = s.LastIndexOf(c);
            return index < 0 ? s : s.Substring(index + 1);
        }
    }
}
