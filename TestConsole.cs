using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Json;
using NUnit.Framework;
using ServiceStack.Text;

namespace SolrCsvSpike
{
    [TestFixture]
    public class TestConsole
    {
        private SolrPerfTester runner;

        [TestFixtureSetUp]
        public void SetUp()
        {
            runner = new SolrPerfTester();
        }
       

        [Test]
        public void Test_50_Tracks()
        {
            
        }

        [Test]
        public void Test_500_Tracks()
        {
            
        }

        [Test]
        public void Test_1000_Tracks()
        {
            
        }

        [Test]
        public void Test_2000_Tracks()
        {
            
        }

       

        [Test]
        public void PerfTest()
        {
            var setSizes = new[] {10};
            
            var results = new List<SetSizePerfResult>();
         
            foreach (var setSize in setSizes)
            {
                results.Add(runner.RunSpikeFor(setSize));
            }

            Display(results);

        }

        private void Display(IEnumerable<SetSizePerfResult> results)
        {
            Console.WriteLine("||     Set Size      ||     Xml      ||     Csv      ||     Json     ||");

            foreach (var r in results)
            {
                Console.WriteLine("||       {0}       ||      {1}      ||     {2}     ||     {3}     ||", r.SetSize, r.XmlTime, r.CsvTime, r.JsonTime);
            }
        }

        //private static void Display(ResponseInfo result)
        //{
        //    Console.WriteLine("Time: " + result.Time);

        //    Console.WriteLine();
        //    Console.WriteLine();
        //    Console.WriteLine("****************************");
        //    Console.WriteLine();
        //    Console.WriteLine();
        //    Console.WriteLine("Response: ");
        //    Console.WriteLine();
        //    Console.Write(result.Body);
        //}

        //[Test]
        //public void RemoteCsv()
        //{
        //    var sizes = new[] {1,2,3,4,5,6,7,8,9,10};

        //    var results = new List<ResponseInfo>();
        //    foreach (var size in sizes)
        //    {
        //        var result = runner.RunRemoteCsvUpdate("/usr/local/tomcat/data/solr/track-aat/blah.csv");

        //        results.Add(result);

        //        SolrPerfTester.ClearSolrAndAllowToProcess();
        //    }

        //    Display(results);

        //    runner.RunRemoteCsvUpdate("/usr/local/tomcat/data/solr/track-aat/blah.csv");

        //private static void Display(List<ResponseInfo> results)
        //{
        //    foreach (var r in results)
        //    {
        //        Console.WriteLine("Time: " + r.Time);
        //    }

        //    Console.WriteLine();
        //    Console.WriteLine();
        //    Console.WriteLine();
        //    Console.WriteLine("Average: " + results.Average(r => r.Time));

        //    Console.WriteLine();
        //    Console.WriteLine();
        //    Console.WriteLine();

        //    foreach (var responseInfo in results)
        //    {
        //        Console.WriteLine(responseInfo.Body);
        //        Console.WriteLine();
        //    }
        //}

        //[Test]
        //public void GetCsv()
        //{
        //    var tracks = runner.GetTracks(100000);
        //    var csv = runner.ConvertToCsv(tracks);

        //    using (var w = new StreamWriter(new FileStream(@"C:\Users\nickt\Desktop\blah.csv.txt", FileMode.Create)))
        //    {
        //        w.Write(csv);
        //    }
        //}
    }
}
