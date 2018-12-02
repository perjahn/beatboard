using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BeatBoardSource.Controllers
{
    [Route("")]
    [ApiController]
    public class RootController : ControllerBase
    {
        // GET: Root
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return Enumerable.Empty<string>();
        }
    }
}
