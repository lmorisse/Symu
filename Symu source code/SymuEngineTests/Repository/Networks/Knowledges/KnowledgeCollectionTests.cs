#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Repository.Networks.Knowledges;

#endregion

namespace SymuEngineTests.Repository.Networks.Knowledges
{
    [TestClass]
    public class KnowledgeCollectionTests
    {
        private readonly Knowledge _knowledge =
            new Knowledge(1, "1", 1);

        private readonly KnowledgeCollection _knowledges = new KnowledgeCollection();

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