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

    public class SpeckleLayerData
    {
        [JsonProperty("guid")]
        public Guid Id { get; set; }

        [JsonProperty("streamId")]
        public string StreamId { get; set; }

        [JsonProperty("visibility")]
        public bool Visible { get; set; }

        [JsonProperty("color")]
        public SpeckleColor Color { get; set; }

        [JsonProperty("opacity")]
        public float Opacity { get; set; }

        public SpeckleLayerData() { }

    }
}
