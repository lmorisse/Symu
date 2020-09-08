#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Repository.Entity;

using SymuDNATests.Classes;

#endregion

namespace SymuTests.Repository.Entity
{
    [TestClass]
    public class ActivityTests
    {
        private readonly TestTask _task = new TestTask(1);

        private readonly Knowledge _knowledge =
            new Knowledge(1, "K1", 10);

        [TestMethod]
        public void AddKnowledgeTest()
        {
            Assert.IsFalse(_task.Knowledges.Contains(_knowledge));
            _task.AddKnowledge(_knowledge);
            Assert.IsTrue(_task.Knowledges.Contains(_knowledge));
            _task.AddKnowledge(_knowledge); //handle duplicate
            Assert.AreEqual(1, _task.Knowledges.Count);
        }
    }
}