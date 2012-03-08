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
        private SolrPerfTester _runner;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _runner = new SolrPerfTester();
        }

        [Test]
        public void Test_50_Tracks()
        {
            var setSizes = Enumerable.Repeat(50, 10).ToArray();

            var results = _runner.RunTestFor(setSizes);

            Display(results);
        }

        [Test]
        public void Test_500_Tracks()
        {
            var setSizes = Enumerable.Repeat(500, 10).ToArray();

            var results = _runner.RunTestFor(setSizes);

            Display(results);
        }

        [Test]
        public void Test_1000_Tracks()
        {
            var setSizes = Enumerable.Repeat(1000, 10).ToArray();

            var results = _runner.RunTestFor(setSizes);

            Display(results);
        }

        [Test]
        public void Test_2000_Tracks()
        {
            var setSizes = Enumerable.Repeat(2000, 10).ToArray();

            var results = _runner.RunTestFor(setSizes);

            Display(results);
        }

        private void Display(IEnumerable<SetSizePerfResult> results)
        {
            Console.WriteLine("||     Set Size      ||     Xml      ||     Csv      ||     Json     ||");

            foreach (var r in results)
            {
                Console.WriteLine("||       {0}       ||      {1}      ||     {2}     ||     {3}     ||", r.SetSize, r.XmlTime, r.CsvTime, r.JsonTime);
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("*************************************************");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Averages: Xml = {0}   Csv = {1}   Json = {2}", 
                results.Average(r => r.XmlTime), results.Average(r => r.CsvTime), results.Average(r => r.JsonTime));

            Console.WriteLine();
            Console.WriteLine("Medians: Xml = {0} Csv = {1} Json = {2}", 
                GetMedian(results.Select(r => r.XmlTime)), GetMedian(results.Select(r => r.CsvTime)), GetMedian(results.Select(r => r.JsonTime)));

            Console.WriteLine();
            Console.WriteLine("*************************************************");
        }

        private double GetMedian(IEnumerable<double> numbers)
        {
            int numberCount = numbers.Count();
            int halfIndex = numbers.Count() / 2;
            var sortedNumbers = numbers.OrderBy(n => n);
            
            if ((numberCount % 2) == 0)
            {
                return ((sortedNumbers.ElementAt(halfIndex) +
                    sortedNumbers.ElementAt((halfIndex - 1))) / 2);
            }
            else
            {
                return sortedNumbers.ElementAt(halfIndex);
            } 
        }
    }
}
