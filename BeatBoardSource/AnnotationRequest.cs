using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BeatBoardSource
{
    public class Range
    {
        public string from { get; set; } = string.Empty;
        public string to { get; set; } = string.Empty;
    }

    public class AnnotationInfo
    {
        public string name { get; set; } = string.Empty;
        public string datasource { get; set; } = string.Empty;
        public string iconColor { get; set; } = string.Empty;
        public string enable { get; set; } = string.Empty;
        public string query { get; set; } = string.Empty;
    }

    public class AnnotationRequest
    {
        public Range range { get; set; } = new Range();
        public Range rangeRaw { get; set; } = new Range();
        public AnnotationInfo annotation { get; set; } = new AnnotationInfo();
    }
}
