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
    public class ParserWhereTest : ParserTestBase
    {
        [Test]
        public void Test_SingleWhereCondition()
        {
            string qtext = "SENSE foo AT sensors WHERE bar > 12";
            var q = PQLParser.Parse(qtext);

            Assert.AreEqual("bar", q.where_stm().conditionlist().conditiongroup().First().condition().First().sensor().IDENTIFIER().GetText());
            Assert.AreEqual(">", q.where_stm().conditionlist().conditiongroup().First().condition().First().OPERATOR().GetText());
            Assert.AreEqual("12", q.where_stm().conditionlist().conditiongroup().First().condition().First().NUMBER().GetText());
        }

        [Test]
        public void Test_SingleWhereConditionEvent()
        {
            string qtext = "SENSE foo AT sensors WHERE evt.myevent";
            var q = PQLParser.Parse(qtext);

            Assert.AreEqual("myevent", q.where_stm().conditionlist().conditiongroup().First().condition().First().eventname().IDENTIFIER().GetText());            
        }

        [Test]
        public void Test_SingleWhereConditionEventWithOperator()
        {
            string qtext = "SENSE foo AT sensors WHERE evt.myevent > 3";
            var q = PQLParser.Parse(qtext);

            Assert.AreEqual("myevent", q.where_stm().conditionlist().conditiongroup().First().condition().First().eventname().IDENTIFIER().GetText());
            Assert.AreEqual(">", q.where_stm().conditionlist().conditiongroup().First().condition().First().OPERATOR().GetText());
            Assert.AreEqual("3", q.where_stm().conditionlist().conditiongroup().First().condition().First().NUMBER().GetText());
        }

        [Test]
        public void Test_SingleWhereCondition_Reversed()
        {
            string qtext = "SENSE foo AT sensors WHERE 12 < bar";
            var q = PQLParser.Parse(qtext);

            Assert.AreEqual("bar", q.where_stm().conditionlist().conditiongroup().First().condition().First().sensor().IDENTIFIER().GetText());
            Assert.AreEqual("<", q.where_stm().conditionlist().conditiongroup().First().condition().First().OPERATOR().GetText());
            Assert.AreEqual("12", q.where_stm().conditionlist().conditiongroup().First().condition().First().NUMBER().GetText());
        }

        [Test]
        public void Test_WhereConditions_And()
        {
            string qtext = "SENSE foo AT sensors WHERE 12 < bar AND foo <= 5";
            var q = PQLParser.Parse(qtext);

            var conditions = q.where_stm().conditionlist().conditiongroup().First().condition().ToList();

            Assert.AreEqual("bar", conditions[0].sensor().IDENTIFIER().GetText());
            Assert.AreEqual("<", conditions[0].OPERATOR().GetText());
            Assert.AreEqual("12", conditions[0].NUMBER().GetText());

            Assert.AreEqual("AND", q.where_stm().conditionlist().conditiongroup().First().condition_link().First().GetText());

            Assert.AreEqual("foo", conditions[1].sensor().IDENTIFIER().GetText());
            Assert.AreEqual("<=", conditions[1].OPERATOR().GetText());
            Assert.AreEqual("5", conditions[1].NUMBER().GetText());
        }

        [Test]
        public void Test_WhereConditions_Nested()
        {
            string qtext = "SENSE foo AT sensors WHERE (12 < bar AND foo = 5) OR (baz <= 13.6)";
            var q = PQLParser.Parse(qtext);

            // the outer link (i.e. between the condition groups)
            Assert.AreEqual("OR", q.where_stm().conditionlist().condition_link()[0].GetText());

            var conditiongroups = q.where_stm().conditionlist().conditiongroup().ToList();
            Assert.AreEqual(2, conditiongroups.Count);
            Assert.AreEqual("AND", conditiongroups.First().condition_link()[0].GetText());

            var conditionsOne = conditiongroups[0].condition().ToList();
            var conditionsTwo = conditiongroups[1].conditiongroup().First().condition().ToList();

            Assert.AreEqual("bar", conditionsOne[0].sensor().IDENTIFIER().GetText());
            Assert.AreEqual("<", conditionsOne[0].OPERATOR().GetText());
            Assert.AreEqual("12", conditionsOne[0].NUMBER().GetText());

            Assert.AreEqual("foo", conditionsOne[1].sensor().IDENTIFIER().GetText());
            Assert.AreEqual("=", conditionsOne[1].OPERATOR().GetText());
            Assert.AreEqual("5", conditionsOne[1].NUMBER().GetText());

            Assert.AreEqual("baz", conditionsTwo[0].sensor().IDENTIFIER().GetText());
            Assert.AreEqual("<=", conditionsTwo[0].OPERATOR().GetText());
            Assert.AreEqual("13.6", conditionsTwo[0].NUMBER().GetText());
        }

        [Test]
        public void Test_WhereConditions_DeeplyNested()
        {
            string qtext = "SENSE foo AT sensors WHERE (bar < 12) AND (foo = 4 OR (baz > 5 AND baz < 8))";
            var q = PQLParser.Parse(qtext);
        }
    }
}
