using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PLANetary.Core.Types;
using PLANetary.Communication.Connection;

namespace PLANetaryTests
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für PlanetPacketTests
    /// </summary>
    [TestClass]
    public class PlanetPacketTests
    {
        [TestMethod]
        public void TestCreateQueryPacket()
        {            
            Query query = new Query();
            query.QueryId = 42;
            query.Periodic = true;
            byte periodInSec = 10;
            query.PeriodInMS = periodInSec * 1000;

            ConditionGroup g = new ConditionGroup();
            g.ConditionLink = BooleanLink.AND;
            g.Conditions.Add(new SensorCondition(new Sensor("id", ""), ConditionOperator.OP_LESS_OR_EQUAL, 3));
            query.Conditions.Conditions.Add(g);

            query.Selections.Add(new ValueSelection(new Sensor("temp", "Temperature"), SelectionFunction.Avg));
            query.Selections.Add(new ValueSelection(new Sensor("id", ""), SelectionFunction.GroupBy));
            query.Selections.Add(new ValueSelection(new Sensor("randno", "Random number"), SelectionFunction.Sum));

            query.Actuators.Add(new ActuatorFunc(new Actuator("lamp", "Lamp"), 5));

            /*byte[] packet = PlanetarySerialConnection.CreateQueryPacket(query);
            byte[] expected = new byte[] { 1, 42, (byte)(query.Conditions.Count << 4 + (byte)query.Conditions.ConditionLink), (byte)((query.Selections.Count << 4) + query.Actuators.Count), periodInSec, (1 << 4) + (int)BooleanLink.AND , 0, 4, 0, 3, 1, 4, 0, 6, 2, 1, 13, 5 };

            Assert.IsTrue(expected.SequenceEqual(packet));*/
        }
    }
}
