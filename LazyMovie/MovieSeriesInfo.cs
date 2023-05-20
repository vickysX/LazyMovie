using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LazyMovie
{
    [Serializable]
    public class MovieSeriesInfo
    {
        [property: JsonPropertyName("type")]
        public string Type { get; set; }

        [property: JsonPropertyName("title")]
        public string Title { get; set; }

        [property: JsonPropertyName("overview")]
        public string Overview { get; set; }

        [property: JsonPropertyName("cast")]
        public List<string> Cast { get; set; } = new List<string>();

        [property: JsonPropertyName("year")]
        public int Year { get; set; }

        [property: JsonPropertyName("directors")]
        public List<string> Directors { get; set; } = new List<string>();

    }
}
