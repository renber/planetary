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
    public class ParserRaiseTest : ParserTestBase
    {
        [Test]
        public void Test_RaiseSingleEvent()
        {
            String qtext = "RAISE evt.myevent WHERE battery_level < 20 EVERY 1 H";
            var q = PQLParser.Parse(qtext);

            Assert.AreEqual("myevent", q.raise_stm().eventlist().eventdef().First().eventname().IDENTIFIER().GetText());
        }

        [Test]
        public void Test_RaiseSingleEventWithFor()
        {
            String qtext = "RAISE evt.batlow FOR 10 HOPS WHERE battery_level < 20 EVERY 1 H";
            var q = PQLParser.Parse(qtext);

            Assert.AreEqual("batlow", q.raise_stm().eventlist().eventdef().First().eventname().IDENTIFIER().GetText());
            Assert.AreEqual("10", q.raise_stm().eventlist().eventdef().First().for_stm().time().NUMBER().GetText());
            Assert.AreEqual("HOPS", q.raise_stm().eventlist().eventdef().First().for_stm().time().timeunit().GetText());
        }

        [Test]
        public void Test_RaiseMultipleEvents()
        {
            String qtext = "RAISE evt.foo FOR 2 HOPS, evt.bar FOR 10 HOPS EVERY 1 H";
            var q = PQLParser.Parse(qtext);

            var events = q.raise_stm().eventlist().eventdef().ToList();

            Assert.AreEqual("foo", events[0].eventname().IDENTIFIER().GetText());
            Assert.AreEqual("bar", events[1].eventname().IDENTIFIER().GetText());
        }
    }
}
