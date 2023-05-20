using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LazyMovie
{
    [Serializable]
    public class Result
    {
        [property: JsonPropertyName("result")]
        public List<MovieSeriesInfo> Results { get; set; }
    }
}
