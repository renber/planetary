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
    public class ParseStoreSense : ParserTestBase
    {
        [Test]
        public void Test_IntoStore()
        {
            String qtext = "SENSE temp INTO store.temp_med";
            var q = PQLParser.Parse(qtext);
            Assert.AreEqual("temp_med", q.into_store().storename().IDENTIFIER().GetText());
        }

        [Test]
        public void Test_FromStore()
        {
            String qtext = "SENSE store.temp_med(2)";
            var q = PQLParser.Parse(qtext);
            var p = q.sense_stm().sensorlist().aggregation().ToList();
            Assert.AreEqual("temp_med", p[0].storename().IDENTIFIER().GetText());
            Assert.AreEqual("2", p[0].funcparam().GetText());
        }

        [Test]
        public void Test_MultipleFromStore()
        {
            String qtext = "SENSE store.temp_med(2), store.second(1)";
            var q = PQLParser.Parse(qtext);
            var p = q.sense_stm().sensorlist().aggregation().ToList();
            Assert.AreEqual("temp_med", p.First().storename().IDENTIFIER().GetText());
            Assert.AreEqual("2", p.First().funcparam().GetText());
            Assert.AreEqual("second", p[1].storename().IDENTIFIER().GetText());
            Assert.AreEqual("1", p[1].funcparam().GetText());
        }

        [Test]
        public void Test_Piping()
        {
            String qtext = "SENSE store.temp_med(2), store.second(1) INTO store.temp_avg";
            var q = PQLParser.Parse(qtext);
            var p = q.sense_stm().sensorlist().aggregation().ToList();
            Assert.AreEqual("temp_med", p.First().storename().IDENTIFIER().GetText());
            Assert.AreEqual("2", p.First().funcparam().GetText());
            Assert.AreEqual("second", p[1].storename().IDENTIFIER().GetText());
            Assert.AreEqual("1", p[1].funcparam().GetText());
            Assert.AreEqual("temp_avg", q.into_store().storename().IDENTIFIER().GetText());
        }
    }
}
