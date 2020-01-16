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
            public string text { get; set; } = string.Empty;
            public string type { get; set; } = string.Empty;
        }

        public Column[] columns { get; set; } = new Column[0];
        public JArray[] rows { get; set; } = new JArray[0];
        public string type { get; set; } = string.Empty;
    }
}
