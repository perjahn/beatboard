using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BeatBoardSource
{
    public class Table
    {
        public class Column
        {
            public string text { get; set; }
            public string type { get; set; }
        }

        public Column[] columns { get; set; }
        public JArray[] rows { get; set; }
        public string type { get; set; }
    }
}
