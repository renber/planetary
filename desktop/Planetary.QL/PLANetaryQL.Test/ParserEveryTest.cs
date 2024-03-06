using NUnit.Framework;
using PLANetaryQL.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace planetaryql.test
{
    [TestFixture]
    public class ParserEveryTest : ParserTestBase
    {

        [Test]
        public void Test_EveryUnlimited()
        {
            String qtext = "SENSE temp EVERY 5 MIN";
            var q = PQLParser.Parse(qtext);

            Assert.AreEqual("5", q.every_stm().time().NUMBER().GetText());
            Assert.AreEqual("MIN", q.every_stm().time().timeunit().GetText());
        }

        [Test]
        public void Test_EveryWithFor()
        {
            String qtext = "SENSE temp EVERY 5 MIN FOR 1 H";
            var q = PQLParser.Parse(qtext);

            Assert.AreEqual("5", q.every_stm().time().NUMBER().GetText());
            Assert.AreEqual("MIN", q.every_stm().time().timeunit().GetText());

            Assert.AreEqual("1", q.every_stm().for_stm().time().NUMBER().GetText());
            Assert.AreEqual("H", q.every_stm().for_stm().time().timeunit().GetText());
        }

    }
}
