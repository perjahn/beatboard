using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BeatBoardSource.Controllers
{
    [Route("annotations")]
    [ApiController]
    public class AnnotationsController : ControllerBase
    {
        // POST: Annotations
        [HttpPost]
        public IEnumerable<Annotation> Post(JObject jobject)
        {
            //string s = jobject.ToString();

            var a1 = new Annotation
            {
                annotation = "eee",
                time = "2018-11-25T15:10:10Z",
                title = "t1",
                tags = "aaa",
                text = "ccc"
            };
            /*
            var a2 = new Annotation
            {
                annotation = "www",
                time = "2018-11-25T15:20:10Z",
                title = "t2",
                tags = "bbb",
                text = "ddd"
            };
            */

            return new Annotation[] { a1 };
        }
    }
}
