using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using ServiceStack.Text;

namespace SolrCsvSpike
{
    public class SolrConverter
    {
        public static XDocument ConvertToXml(IEnumerable<Track> tracks)
        {
            var x = new XDocument();
            var root = new XElement("add");
            x.Add(root);

            foreach (var t in tracks)
            {
                root.Add(BuildDocFor(t));
            }

            var c = new XElement("commit");
            root.Add(c);

            return x;
        }

        private static XElement BuildDocFor(Track t)
        {
            var doc = new XElement("doc");

            AddField(doc, "id", t.id);
            AddField(doc, "text", t.text);
            AddField(doc, "edgengramtext", t.edgengramtext);
            AddField(doc, "trackShopId", t.trackShopId);
            AddField(doc, "trackTitle", t.trackTitle);
            AddField(doc, "trackFormatIds", t.trackFormatIds);
            AddField(doc, "trackDuration", t.trackDuration);
            AddField(doc, "trackISRC", t.trackISRC);
            AddField(doc, "trackNumber", t.trackNumber);
            AddField(doc, "trackUrl", t.trackUrl);
            AddField(doc, "trackRank", t.trackRank);

            return doc;
        }

        private static void AddField(XElement doc, string name, string value)
        {
            var x = new XElement("field");
            x.SetAttributeValue("name", name);
            x.SetValue(value);
            doc.Add(x);
        }

        public static string ConvertToCsv(IEnumerable<Track> tracks)
        {
            var builder = new StringBuilder();
            builder.AppendLine("id,text,edgengramtext,trackShopId,trackTitle,trackFormatIds,trackDuration,trackISRC,trackNumber,trackUrl,trackRank");

            foreach (var t in tracks)
            {
                builder.AppendLine(string.Join(",", t.id, t.text, t.edgengramtext, t.trackShopId, t.trackTitle,
                                               t.trackFormatIds, t.trackDuration, t.trackISRC, t.trackNumber,
                                               t.trackUrl, t.trackRank));
            }

            return builder.ToString();
        }

        public static string ConvertToJson(IEnumerable<Track> tracks)
        {
            return tracks.ToJson().Replace("\"__type\":\"SolrCsvSpike.Track, SolrCsvSpike\",", "");
        }
    }
}