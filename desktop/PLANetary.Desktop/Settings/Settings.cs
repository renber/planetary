using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLANetary.Core.Types;
using System.IO;
using System.Xml.Linq;
using System.Reflection;

namespace PLANetary
{
    class Settings
    {

        public static string configFilename = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\data\types.xml";

        /// <summary>
        /// Load all available sensor types from the configuration file
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <returns></returns>
        public static List<Sensor> LoadAvailableSensorsFromXml(String xmlFile)
        {
            List<Sensor> rList = new List<Sensor>();

            if (!File.Exists(xmlFile))
                return rList;

            XDocument doc = XDocument.Load(xmlFile);

            if (!doc.Root.Name.ToString().Equals("planetary", StringComparison.InvariantCultureIgnoreCase))
                return rList;

            var sensors = doc.Root.Elements("sensors").Elements("sensor").Select(s => new Sensor(s.Attribute("id").Value, s.Attributes("name").Any() ? s.Attribute("name").Value : ""));
            rList.AddRange(sensors);

            return rList;
        }

        /// <summary>
        /// Load all available actuator types from the configuration file
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <returns></returns>
        public static List<Actuator> LoadAvailableActuatorsFromXml(String xmlFile)
        {
            List<Actuator> rList = new List<Actuator>();

            if (!File.Exists(xmlFile))
                return rList;

            XDocument doc = XDocument.Load(xmlFile);

            if (!doc.Root.Name.ToString().Equals("planetary", StringComparison.InvariantCultureIgnoreCase))
                return rList;

            var actuators = doc.Root.Elements("actuators").Elements("actuator").Select(s => new Actuator(s.Attribute("id").Value, s.Attributes("name").Any() ? s.Attribute("name").Value : ""));
            rList.AddRange(actuators);

            return rList;
        }

    }
}
