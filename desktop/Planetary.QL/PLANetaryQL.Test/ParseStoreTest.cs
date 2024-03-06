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
    public class ParseStoreTest : ParserTestBase
    {
        [Test]
        public void Test_DropStore()
        {
            String qtext = "DROP STORE example";
            var q = PQLParser.Parse(qtext);
            Assert.AreEqual("example", q.store_stm().drop().store().GetText());
        }

        [Test]
        public void Test_CreateStore()
        {
            String qtext = "CREATE STORE example APPLY frugal(2)";
            var q = PQLParser.Parse(qtext);
            Assert.AreEqual("example", q.store_stm().create().store().GetText());
            Assert.AreEqual("2", q.store_stm().create().storefunc().funcparam().GetText());
            Assert.AreEqual("frugal", q.store_stm().create().storefunc().func().GetText());
        }
    }
}
