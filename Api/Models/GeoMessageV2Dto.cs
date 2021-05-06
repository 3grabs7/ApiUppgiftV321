﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public class GeoMessageV2Dto
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
