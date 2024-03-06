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
    public class ParserSenseTest : ParserTestBase
    {

        [Test]
        public void Test_WithAtTable()
        {
            String qtext = "SENSE temp AT my_tablename";

            var q = PQLParser.Parse(qtext);
            Assert.AreEqual("temp", q.sense_stm().sensorlist().aggregation().First().GetText());
            
            Assert.AreEqual("my_tablename", q.at_stm().table().GetText());
        }

        [Test]
        public void Test_SenseSingleSensor()
        {
            String qtext = "SENSE temp";

            var q = PQLParser.Parse(qtext);
            Assert.AreEqual("SENSE", q.sense_stm().SENSE().GetText());
            Assert.AreEqual("temp", q.sense_stm().sensorlist().aggregation().First().GetText());

        }

        [Test]
        public void Test_MultipleSensor()
        {
            String qtext = "SENSE temp, humidity, brightness";

            var q = PQLParser.Parse(qtext);
            Assert.AreEqual("SENSE", q.sense_stm().SENSE().GetText());
            Assert.AreEqual(3, q.sense_stm().sensorlist().aggregation().Count());

            var sensorList = q.sense_stm().sensorlist().aggregation().ToList();
            Assert.AreEqual("temp", sensorList[0].GetText());
            Assert.AreEqual("humidity", sensorList[1].GetText());
            Assert.AreEqual("brightness", sensorList[2].GetText());

        }

    }
}
