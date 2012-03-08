using System;
using System.Collections.Generic;
using SolrNet.Attributes;

namespace SolrCsvSpike
{
    public class Track
    {
        public string id { get; set; }

        public string text { get; set; }

        public string edgengramtext { get; set; }

        public string trackShopId { get; set; }

        public string trackTitle { get; set; }

        public string trackFormatIds { get; set; }

        public string trackDuration { get; set; }

        public string trackISRC { get; set; }

        public string trackNumber { get; set; }

        public string trackUrl { get; set; }

        public string trackRank { get; set; }
    }
}