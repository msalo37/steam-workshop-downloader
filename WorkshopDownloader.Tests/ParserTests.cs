using NUnit.Framework;
using System.Threading.Tasks;
using WorkshopDownloader.Core.Parsers;

namespace WorkshopDownloader.Tests.Parsers
{
    public class ParserTests
    {
        private BaseParser collectionParser, addonParser;

        [SetUp]
        public void Setup()
        {
            collectionParser = new CollectionParser();
            addonParser = new AddonInfoParser();
        }

        [Test]
        public async Task AddonParserWork()
        {
            const string expected = "HugsLib";
            const ulong addonId = 818773962;
            
            string[] addonInfo = await addonParser.RequestInfo(addonId);

            if (addonInfo != null)
                Assert.AreEqual(expected, addonInfo[0]);
            else
                Assert.Fail("Returned data is null!");
        }

        [Test]
        public async Task CollectionParserWork()
        {
            const ulong collectionId = 1884025115;

            string[] addons = await collectionParser.RequestInfo(collectionId);

            if (addons != null)
                Assert.IsTrue(addons.Length > 0);
            else
                Assert.Fail("Returned data is null!");
        }


    }
}