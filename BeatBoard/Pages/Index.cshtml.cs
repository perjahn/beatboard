﻿using BeatBoardLib;
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
            var agents = Agent.GetAgents();

            int count = agents.Count;
        }
    }
}
