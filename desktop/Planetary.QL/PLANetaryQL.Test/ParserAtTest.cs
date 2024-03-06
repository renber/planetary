using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace planetaryql.test
{

    [TestFixture]
    public class ParserAtTest : ParserTestBase
    {
        [Test]
        public void Test_At()
        {
            String qtext = "SENSE id AT MyTable";
            var q = PQLParser.Parse(qtext);

            Assert.AreEqual("MyTable", q.at_stm().table().IDENTIFIER().GetText());
        }
    }
}
