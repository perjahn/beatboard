using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BeatBoardSource
{
    public class Annotation
    {
        public string annotation { get; set; } = string.Empty;
        public string time { get; set; } = string.Empty;
        public string title { get; set; } = string.Empty;
        public string tags { get; set; } = string.Empty;
        public string text { get; set; } = string.Empty;
    }
}
