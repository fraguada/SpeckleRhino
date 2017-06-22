using Newtonsoft.Json;
using System;

namespace SpeckleRhino
{
    public class SpeckleLayerMaterial
    {
        [JsonProperty("guid")]
        public Guid Id { get; set; }

        [JsonProperty("color")]
        public SpeckleColor Color { get; set; }

        [JsonProperty("linewidth")]
        public float Linewidth { get; set; }

        [JsonProperty("pointsize")]
        public int PointSize { get; set; }

        [JsonProperty("shininess")]
        public float Shininess { get; set; }

        [JsonProperty("streamId")]
        public string StreamId { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        public SpeckleLayerMaterial() { }

    }
}
