using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FerryLegacy.Tests
{
    public class GoldenMaster
    {
        private static void GenerateFile(string filename)
        {
            var fs = new FileStream(filename, FileMode.Create);
            var sw = new StreamWriter(fs);
            Console.SetOut(sw);

            Program.MainWithTestData();

            sw.Close();
        }

        [Test]
        public void save_test_output()
        {
            GenerateFile(@"C:\code\legacy-ferry-booking-system\TestResult.txt");

            var goldenmaster = File.ReadAllLines(@"C:\code\legacy-ferry-booking-system\GoldenMaster.txt");
            var testresult = File.ReadAllLines(@"C:\code\legacy-ferry-booking-system\TestResult.txt");

            Assert.That(goldenmaster, Is.EqualTo(testresult));
        }
    }
}
