using BeatBoardLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace BeatBoard.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            string baseurl = System.IO.File.ReadAllLines("elastic.txt")[0];
            string username = System.IO.File.ReadAllLines("elastic.txt")[1];
            string password = System.IO.File.ReadAllLines("elastic.txt")[2];

            var agents = Agent.GetAgents(baseurl, username, password);

            int count = agents.Count;
        }
    }
}
