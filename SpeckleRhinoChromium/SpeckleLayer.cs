using Newtonsoft.Json;
using System;

namespace SpeckleRhino
{
    public class SpeckleLayer
    {
        [JsonProperty("guid")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("properties")]
        public object Properties { get; set; }

        [JsonProperty("objectCount")]
        public int ObjectCount { get; set; }

        [JsonProperty("orderIndex")]
        public int OrderIndex { get; set; }

        [JsonProperty("startIndex")]
        public int StartIndex { get; set; }

        [JsonProperty("topology")]
        public string Topology { get; set; }

        public SpeckleLayer() { }

    }
}
