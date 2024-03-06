using NUnit.Framework;
using PLANetary.Core.Types;
using PLANetaryQL.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace planetaryql.test
{
    [TestFixture]
    public class TransformTest : ParserTestBase
    {
        private void AssertSensorCondition(string expectedSensor, ConditionOperator expectedOperator, int expectedValue, SensorCondition qc)
        {
            Assert.AreEqual(expectedSensor, qc.Sensor.Name);
            Assert.AreEqual(expectedOperator, qc.Operator);
            Assert.AreEqual(expectedValue, qc.Value);
        }

        private void AssertEventCondition(string expectedEvent, ConditionOperator expectedOperator, int expectedValue, EventCondition qc)
        {
            Assert.AreEqual(expectedEvent, qc.Event.Name);
            Assert.AreEqual(expectedOperator, qc.Operator);
            Assert.AreEqual(expectedValue, qc.Value);
        }

        private void AssertActuator(string expectedName, int[] expectedParameters, ActuatorFunc actor)
        {
            Assert.AreEqual(expectedName, actor.Actuator.Name);
            Assert.AreEqual(expectedName.Length, actor.Parameters.Count);

            for(int i = 0; i < expectedParameters.Length; i++)
            {
                Assert.AreEqual(expectedParameters[i], actor.Parameters[i]);
            }
        }

        [Test]
        public void Test_Transform_Sense()
        {
            string qtext = "SENSE temp, max(humidity), count(id)";
            var qcontext = PQLParser.Parse(qtext);
            var query = PQLParser.Transform(qcontext);

            Assert.AreEqual(3, query.Selections.Count);

            Assert.AreEqual("temp", query.Selections[0].Sensor.Name);
            Assert.AreEqual(SelectionFunction.Single, query.Selections[0].SelFunction);

            Assert.AreEqual("humidity", query.Selections[1].Sensor.Name);
            Assert.AreEqual(SelectionFunction.Max, query.Selections[1].SelFunction);

            Assert.AreEqual("id", query.Selections[2].Sensor.Name);
            Assert.AreEqual(SelectionFunction.Count, query.Selections[2].SelFunction);
        }

        [Test]
        public void Test_Transform_SenseInto()
        {
            string qtext = "SENSE temp INTO store.temp_med";
            var qcontext = PQLParser.Parse(qtext);
            var query = PQLParser.Transform(qcontext);

            Assert.AreEqual(1, query.Selections.Count);
            Assert.AreEqual(0, query.StoreSources.Count);

            Assert.AreEqual("temp", query.Selections[0].Sensor.Name);
            Assert.AreEqual(SelectionFunction.Single, query.Selections[0].SelFunction);

            Assert.AreEqual("temp_med", query.StoreTarget.Store.Name);
        }

        [Test]
        public void Test_Transform_FromStore()
        {
            string qtext = "SENSE store.temp_med(2), store.temp_uni(1)";
            var qcontext = PQLParser.Parse(qtext);
            var query = PQLParser.Transform(qcontext);

            Assert.AreEqual(0, query.Selections.Count);
            Assert.AreEqual(2, query.StoreSources.Count);

            Assert.AreEqual("temp_med", query.StoreSources[0].Store.Name);
            Assert.AreEqual(2.0, query.StoreSources[0].param);
            Assert.AreEqual("temp_uni", query.StoreSources[1].Store.Name);
            Assert.AreEqual(1.0, query.StoreSources[1].param);
        }

        [Test]
        public void Test_Transform_WhereSingle()
        {
            string qtext = "SENSE temp WHERE foo > 5";
            var qcontext = PQLParser.Parse(qtext);
            var query = PQLParser.Transform(qcontext);

            Assert.AreEqual(1, query.Conditions.Count);
            Assert.IsTrue(query.Conditions[0] is SensorCondition);

            AssertSensorCondition("foo", ConditionOperator.OP_GREATER, 5, (SensorCondition)query.Conditions[0]);
        }

        [Test]
        public void Test_Transform_WhereNested()
        {
            string qtext = "SENSE temp WHERE (foo > 5 AND bar = 3) OR (baz <> 4)";

            var qcontext = PQLParser.Parse(qtext);
            var query = PQLParser.Transform(qcontext);

            Assert.AreEqual(2, query.Conditions.Count);
            Assert.AreEqual(BooleanLink.OR, query.Conditions.ConditionLink);

            ConditionGroup groupOne = query.Conditions[0] as ConditionGroup;
            ConditionGroup groupTwo = query.Conditions[1] as ConditionGroup;

            Assert.AreEqual(2, groupOne.Count);
            Assert.AreEqual(1, groupTwo.Count);

            Assert.AreEqual(BooleanLink.AND, groupOne.ConditionLink);

            AssertSensorCondition("foo", ConditionOperator.OP_GREATER, 5, (SensorCondition)groupOne.Conditions[0]);
            AssertSensorCondition("bar", ConditionOperator.OP_EQUAL, 3, (SensorCondition)groupOne.Conditions[1]);

            AssertSensorCondition("baz", ConditionOperator.OP_NOT, 4, (SensorCondition)(groupTwo.Conditions[0]));
        }

        [Test]
        public void Test_Transform_WhereEvent()
        {
            string qtext = "SENSE temp WHERE evt.myevent";
            var qcontext = PQLParser.Parse(qtext);
            var query = PQLParser.Transform(qcontext);

            Assert.AreEqual(1, query.Conditions.Count);
            Assert.IsTrue(query.Conditions[0] is EventCondition);

            AssertEventCondition("myevent", ConditionOperator.OP_GREATER, 0, (EventCondition)query.Conditions[0]);            
        }

        [Test]
        public void Test_Transform_WhereEventWithOperator()
        {
            string qtext = "SENSE temp WHERE evt.myevent = 3";
            var qcontext = PQLParser.Parse(qtext);
            var query = PQLParser.Transform(qcontext);

            Assert.AreEqual(1, query.Conditions.Count);
            Assert.IsTrue(query.Conditions[0] is EventCondition);

            AssertEventCondition("myevent", ConditionOperator.OP_EQUAL, 3, (EventCondition)query.Conditions[0]);
        }

        [Test]        
        public void Test_TransformAct()
        {
            string qtext = "ACT led(64, 128, 255)";
            var qcontext = PQLParser.Parse(qtext);
            var query = PQLParser.Transform(qcontext);

            Assert.AreEqual(1, query.Actuators.Count);

            AssertActuator("led", new int[] { 64, 128, 255 }, query.Actuators[0]);
        }

        [Test]
        public void Test_TransformCreateStore()
        {
            string qtext = "CREATE STORE example APPLY frugal(2)";
            var qcontext = PQLParser.Parse(qtext);
            var query = PQLParser.Transform(qcontext);

            Assert.AreEqual(1, query.Initiations.Count);
            Assert.AreEqual(2, query.Initiations[0].Parameter);
            Assert.AreEqual("frugal", query.Initiations[0].StoreFunction);
            Assert.AreEqual("example", query.Initiations[0].Store.Name);
        }

        [Test]
        public void Test_TransformDropStore()
        {
            string qtext = "DROP STORE example";
            var qcontext = PQLParser.Parse(qtext);
            var query = PQLParser.Transform(qcontext);

            Assert.AreEqual(1, query.Initiations.Count);
            Assert.AreEqual(0, query.Initiations[0].Parameter);
            Assert.AreEqual("Drop", query.Initiations[0].StoreFunction);
            Assert.AreEqual("example", query.Initiations[0].Store.Name);
        }

        [Test]
        public void Test_At_Default()
        {
            string qtext = "SENSE id";
            var qcontext = PQLParser.Parse(qtext);
            var query = PQLParser.Transform(qcontext);

            Assert.AreEqual("sensors", query.VirtualTableName);
        }

        [Test]
        public void Test_At_CustomTableName()
        {
            string qtext = "SENSE id AT MyTable";
            var qcontext = PQLParser.Parse(qtext);
            var query = PQLParser.Transform(qcontext);

            Assert.AreEqual("MyTable", query.VirtualTableName);
        }
    }
}
