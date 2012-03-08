using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Json;
using ServiceStack.Text;

namespace SolrCsvSpike
{
    public class SolrPerfTester
    {
        // TODO - this code looks a bit scripty - which is what we want for demonstration purposes
        public SetSizePerfResult RunSpikeFor(int setSize)
        {
            var tracks = GetTracks(setSize).ToList();

            var xml = ConvertToXml(tracks);
            var csv = ConvertToCsv(tracks);
            var json = tracks.ToJson().Replace("\"__type\":\"SolrCsvSpike.Track, SolrCsvSpike\",", "");

            var xmlRequest = Solr.GetXmlRequest(xml);
            var csvRequest = Solr.GetCsvRequest(csv);
            var jsonRequest = Solr.GetJsonRequest(json);

            ClearSolrAndAllowToProcess();

            var xmlTime = Solr.GetResponseAndDuration(xmlRequest).Time;

            ClearSolrAndAllowToProcess();
            
            var csvTime = Solr.GetResponseAndDuration(csvRequest).Time;

            ClearSolrAndAllowToProcess();

            var jsonTime = Solr.GetResponseAndDuration(jsonRequest).Time;

            ClearSolrAndAllowToProcess();
        
            return new SetSizePerfResult {SetSize = setSize, XmlTime = xmlTime, CsvTime = csvTime, JsonTime = jsonTime};
        }

        public IEnumerable<SetSizePerfResult> RunTestFor(int[] setSizes)
        {
            var results = new List<SetSizePerfResult>();

            foreach (var setSize in setSizes)
            {
                results.Add(RunSpikeFor(setSize));
            }
            return results;
        }

        public static void ClearSolrAndAllowToProcess()
        {
            Solr.Reset();

            Thread.Sleep(500);
        }

        public IEnumerable<Track> GetTracks(int setSize)
        {
            for (int i = 0; i < setSize; i++)
            {
                var t = @"
                            Shootout is at high-noon. Everybody knows Xml is going to lose...
                            But who will win... Csv or Json?
                         ";

                yield return new Track
                                 {
                                     id = (1000 + i).ToString(),
                                     edgengramtext = "BlahBlah dkjfl dkjdfd kdjf",
                                     text = t,
                                     trackDuration = "3.15",
                                     trackFormatIds = "1",
                                     trackISRC = (2000 + i).ToString(),
                                     trackNumber = "1",
                                     trackRank = "1",
                                     trackShopId = "34",
                                     trackTitle = "Track " + i,
                                     trackUrl = "http://track.track.com"
                                 };
            }
        }

        private static XDocument ConvertToXml(IEnumerable<Track> tracks)
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

        public string ConvertToCsv(IEnumerable<Track> tracks)
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

        public ResponseInfo RunRemoteCsvUpdate(string fileName)
        {
            var request = Solr.GetCsvRemoteUpdateRequest(fileName);

            return Solr.GetResponseAndDuration(request);
        }
    }

    public class ResponseInfo
    {
        public long Time { get; set; }

        public string Body { get; set; }

        public ResponseInfo(long time, string body)
        {
            Time = time;
            Body = body;
        }
    }

    public class SetSizePerfResult
    {
        public int SetSize { get; set; }

        public double CsvTime { get; set; }

        public double XmlTime { get; set; }

        public double JsonTime { get; set; }
    }
}