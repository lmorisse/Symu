#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Repository.Networks.Knowledge.Repository;

#endregion

namespace SymuEngineTests.Repository.Networks.Knowledge.Repository
{
    [TestClass]
    public class KnowledgesTests
    {
        private readonly SymuEngine.Repository.Networks.Knowledge.Repository.Knowledge _knowledge =
            new SymuEngine.Repository.Networks.Knowledge.Repository.Knowledge(1, "1", 1);

        private readonly Knowledges _knowledges = new Knowledges();

        [TestMethod]
        public void ClearTest()
        {
            _knowledges.Add(_knowledge);
            _knowledges.Clear();
            Assert.IsFalse(_knowledges.Any());
        }

        [TestMethod]
        public void AddTest()
        {
            Assert.IsFalse(_knowledges.Any());
            _knowledges.Add(_knowledge);
            Assert.IsTrue(_knowledges.Any());

            _knowledges.Add(_knowledge);
            Assert.AreEqual(1, _knowledges.Count);
        }

        [TestMethod]
        public void ContainsTest()
        {
            Assert.IsFalse(_knowledges.Contains(_knowledge));
            _knowledges.Add(_knowledge);
            Assert.IsTrue(_knowledges.Contains(_knowledge));
        }
    }
}