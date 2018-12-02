using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BeatBoardSource
{
    public class Metric
    {
        public string target { get; set; }
        public long[][] datapoints { get; set; }
    }
}
