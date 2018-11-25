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
            string[] baseurls = System.IO.File.ReadAllLines("elastic.txt")[0].Split(',');
            string username = System.IO.File.ReadAllLines("elastic.txt")[1];
            string password = System.IO.File.ReadAllLines("elastic.txt")[2];

            var hosts = Host.GetHostsAsync(baseurls, username, password).Result;

            int count = hosts.Count;
        }
    }
}
