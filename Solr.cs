using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using SolrNet;

namespace SolrCsvSpike
{
    internal static class Solr
    {
        public static void Reset()
        {
            var deleteAllXml = new XDocument();
            var root = new XElement("delete");
           
            deleteAllXml.Add(root);

            var query = new XElement("query");
            query.SetValue("*:*");
            root.Add(query);

            var request = GetUpdateRequest();
            SetRequestBody(deleteAllXml, request);

            GetResponseAndDuration(request);
        }

        public static HttpWebRequest GetXmlRequest(XDocument xml)
        {
            var request = GetUpdateRequest();
            SetRequestBody(xml, request);
            return request;
        }

        public static HttpWebRequest GetCsvRequest(string csv)
        {
            var request = GetUpdateRequest("text/plain", @"http://systest-solr-slave00.nix.sys.7d:8080/solr/track-aat/update/csv?commit=true");

            SetRequestBody(csv, request);

            return request;
        }

        public static HttpWebRequest GetJsonRequest(string json)
        {
            var request = GetUpdateRequest("application/json", "http://systest-solr-slave00.nix.sys.7d:8080/solr/track-aat/update/json?commit=true");

            SetRequestBody(json, request);

            return request;
        }

        private static void SetRequestBody(string csv, HttpWebRequest request, bool useGzip = false)
        {
            var requestStream = request.GetRequestStream();
            using (var writer = new StreamWriter(requestStream))
            {
                writer.Write(csv);
            }
        }

        private static HttpWebRequest GetUpdateRequest(string contentType = "text/xml", string url = null, string method = "POST")
        {
            url = url ?? @"http://systest-solr-slave00.nix.sys.7d:8080/solr/track-aat/update?commit=true";

            var request = (HttpWebRequest) WebRequest.Create(url);
            request.ContentType = contentType;
            request.Method = method;
            
            return request;
        }

        private static void SetRequestBody(XDocument xml, HttpWebRequest request)
        {
            var requestStream = request.GetRequestStream();
            using (var writer = XmlWriter.Create(requestStream))
            {
                xml.WriteTo(writer);
            }
        }
     
        private static string GetResponseBody(WebResponse response)
        {
            using (var r = new StreamReader(response.GetResponseStream()))
            {
                return r.ReadToEnd();
            }
        }

        public static ResponseInfo GetResponseAndDuration(WebRequest request)
        {
            try
            {
                var st = new Stopwatch();
                st.Start();

                using (var response = request.GetResponse())
                {
                    st.Stop();

                    var time = st.ElapsedMilliseconds;
                    var body = GetResponseBody(response);

                    var responseInfo = new ResponseInfo(time, body);

                    return responseInfo;
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(GetResponseBody(ex.Response));
                throw;
            }
        }

        public static WebRequest GetCsvRemoteUpdateRequest(string fileName)
        {
            var r = GetUpdateRequest(url: @"http://systest-solr-slave00.nix.sys.7d:8080/solr/track-aat/update/csv?stream.file=" + fileName, method: "GET");

            return r;
        }
    }
}