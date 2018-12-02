using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BeatBoardSource
{
    public class Range
    {
        public string from { get; set; }
        public string to { get; set; }
    }

    public class AnnotationInfo
    {
        public string name { get; set; }
        public string datasource { get; set; }
        public string iconColor { get; set; }
        public string enable { get; set; }
        public string query { get; set; }
    }

    public class AnnotationRequest
    {
        public Range range { get; set; }
        public Range rangeRaw { get; set; }
        public AnnotationInfo annotation { get; set; }
    }
}
