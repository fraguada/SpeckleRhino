using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeckleRhino
{
    public class SpeckleColor
    {
        [JsonProperty("a")]
        public float Alpha { get; set; }

        [JsonProperty("hex")]
        public string Hex { get; set; }
        public SpeckleColor() { }
    }
}
