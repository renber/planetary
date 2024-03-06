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
    public class ParserActTest : ParserTestBase
    {
        [Test]
        public void Test_Act_SingleFuncNoParams()
        {
            String qtext = "ACT beep() AT sensors";
            var q = PQLParser.Parse(qtext);

            Assert.AreEqual("beep", q.act_stm().actorlist().actorfunc().First().func().GetText());
        }

        [Test]
        public void Test_Act_SingleFuncWithParams()
        {
            String qtext = "ACT led(1, 0, 255, 255) AT sensors";
            var q = PQLParser.Parse(qtext);

            Assert.AreEqual("led", q.act_stm().actorlist().actorfunc().First().func().GetText());
            var p = q.act_stm().actorlist().actorfunc().First().funcparam().ToList();
            Assert.AreEqual(4, p.Count);

            Assert.AreEqual("1", p[0].GetText());
            Assert.AreEqual("0", p[1].GetText());
            Assert.AreEqual("255", p[2].GetText());
            Assert.AreEqual("255", p[3].GetText());
        }

        [Test]
        public void Test_Act_MultipleFuncWithParams()
        {
            String qtext = "ACT beep(12), led(1, 0, 255, 255) AT sensors";
            var q = PQLParser.Parse(qtext);

            var actors = q.act_stm().actorlist().actorfunc().ToList();

            Assert.AreEqual("beep", actors[0].func().GetText());
            Assert.AreEqual("12", actors[0].funcparam().First().GetText());

            Assert.AreEqual("led", actors[1].func().GetText());
            var p = actors[1].funcparam().ToList();
            Assert.AreEqual(4, p.Count);
            Assert.AreEqual("1", p[0].GetText());
            Assert.AreEqual("0", p[1].GetText());
            Assert.AreEqual("255", p[2].GetText());
            Assert.AreEqual("255", p[3].GetText());
        }
    }
}
