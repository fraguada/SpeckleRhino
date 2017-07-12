using Newtonsoft.Json;
using System;

namespace SpeckleRhino
{
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
