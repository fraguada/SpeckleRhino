﻿using Newtonsoft.Json;
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

        public System.Drawing.Color ToColor()
        {
            if (!Hex.Contains("#")) Hex = "#" + Hex;
            System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(Hex);
            
            return System.Drawing.Color.FromArgb(color.R, color.G, color.B);
        }

    }
}
