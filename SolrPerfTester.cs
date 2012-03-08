using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Json;

namespace SolrCsvSpike
{
    public class SolrPerfTester
    {
        public IEnumerable<SetSizePerfResult> RunTestFor(int[] setSizes)
        {
            var results = new List<SetSizePerfResult>();

            foreach (var setSize in setSizes)
            {
                results.Add(RunSpikeFor(setSize));
            }
            return results;
        }

        // TODO - this code looks a bit scripty - which is what we want for demonstration purposes
        public SetSizePerfResult RunSpikeFor(int setSize)
        {
            var tracks = GetTracks(setSize).ToList();

            var xml = SolrConverter.ConvertToXml(tracks);
            var csv = SolrConverter.ConvertToCsv(tracks);
            var json = SolrConverter.ConvertToJson(tracks); 

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

        public static void ClearSolrAndAllowToProcess()
        {
            Solr.Reset();

            Thread.Sleep(2000);
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