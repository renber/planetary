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
    public class ParserGroupByTest : ParserTestBase
    {
        [Test]
        public void Test_GroupBy_Single()
        {
            String qtext = "SENSE temp, room GROUP BY room";
            var q = PQLParser.Parse(qtext);

            Assert.AreEqual("room", q.groupby_stm().sensor().First().GetText());
        }

        [Test]
        public void Test_GroupBy_Multiple()
        {
            String qtext = "SENSE temp, room GROUP BY room, temp";
            var q = PQLParser.Parse(qtext);

            var groupBySensors = q.groupby_stm().sensor().ToList();

            Assert.AreEqual("room", groupBySensors[0].GetText());
            Assert.AreEqual("temp", groupBySensors[1].GetText());
        }
    }
}
