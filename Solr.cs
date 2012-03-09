using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
            var request = GetUpdateRequest("text/plain", SolrUrls.UpdateCsvUrl);

            SetRequestBody(csv, request);

            return request;
        }

        public static HttpWebRequest GetJsonRequest(string json)
        {
            var request = GetUpdateRequest("application/json", SolrUrls.UpdateJsonUrl);

            SetRequestBody(json, request);

            return request;
        }

        private static void SetRequestBody(string csv, HttpWebRequest request)
        {
            var requestStream = request.GetRequestStream();
            using (var writer = new StreamWriter(requestStream))
            {
                writer.Write(csv);
            }
        }

        private static HttpWebRequest GetUpdateRequest(string contentType = "text/xml", string url = null, string method = "POST")
        {
            url = url ?? SolrUrls.UpdateXmlUrl;

            var request = (HttpWebRequest) WebRequest.Create(url);
            request.ContentType = contentType;
            request.Method = method;
            request.Timeout = 1000 * 320;
            
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

                    var status = ((HttpWebResponse) response).StatusCode;
                    if (status != HttpStatusCode.OK)
                    {
                        throw new Exception("Bad respnose. Status: " + status);
                    }

                    var responseInfo = new ResponseInfo(time, GetQTimeFrom(body), body);

                    return responseInfo;
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine("Error: " +ex.Message);
                throw;
            }
        }

        private static double GetQTimeFrom(string body)
        {
            var xml = XDocument.Parse(body);

            var ints = xml.Descendants("int");

            var qtTElement = ints.SingleOrDefault(a => a.Attributes()
                                                        .Any(x => x.Value == "QTime"));

            if (qtTElement == null) return -1;

            return double.Parse(qtTElement.Value);
        }

        public static WebRequest GetCsvRemoteUpdateRequest(string fileName)
        {
            var r = GetUpdateRequest(url: SolrUrls.UpdateRemoteCsvUrl + fileName, method: "GET");

            return r;
        }
    }
}