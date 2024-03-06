using NUnit.Framework;
using PLANetaryQL.Parser;
using PLANetaryQL.Parser.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace planetaryql.test
{
    [TestFixture]
    public class ParserSenseThenRaiseThenActTest : ParserTestBase
    {

        [Test]
        public void Test_SenseThenRaise()
        {
            String qtext = "SENSE temp THEN RAISE evt.myevent AT sensors";
            var q = PQLParser.Parse(qtext);

            Assert.AreEqual("temp", q.sense_stm().sensorlist().aggregation()[0].sensor().IDENTIFIER().GetText());
            Assert.AreEqual("myevent", q.raise_stm().eventlist().eventdef().First().eventname().IDENTIFIER().GetText());
        }

        [Test]
        public void Test_SenseThenAct()
        {
            String qtext = "SENSE temp THEN ACT beep(12) AT sensors";
            var q = PQLParser.Parse(qtext);

            Assert.AreEqual("temp", q.sense_stm().sensorlist().aggregation()[0].sensor().IDENTIFIER().GetText());
            Assert.AreEqual("beep", q.act_stm().actorlist().actorfunc().First().func().GetText());
        }

        [Test]
        public void Test_SenseThenRaiseThenAct()
        {
            String qtext = "SENSE temp THEN RAISE evt.myevent THEN ACT beep() AT sensors";
            var q = PQLParser.Parse(qtext);

            Assert.AreEqual("temp", q.sense_stm().sensorlist().aggregation()[0].sensor().IDENTIFIER().GetText());
            Assert.AreEqual("myevent", q.raise_stm().eventlist().eventdef().First().eventname().IDENTIFIER().GetText());
            Assert.AreEqual("beep", q.act_stm().actorlist().actorfunc().First().func().GetText());
        }

        [Test]
        public void Test_SenseAct_MissingThen()
        {
            String qtext = "SENSE temp ACT beep(12) AT sensors";
            ParseException p = Assert.Throws<ParseException>(() => PQLParser.Parse(qtext));
            Assert.IsTrue(p.Errors.Any(x => x.Message.Contains("THEN")));
        }

    }
}
