#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Repository.Networks.Activities;

#endregion

namespace SymuEngineTests.Repository.Networks.Activities
{
    [TestClass]
    public class ActivityTests
    {
        private readonly Activity _activity = new Activity("a");

        private readonly SymuEngine.Repository.Networks.Knowledge.Repository.Knowledge _knowledge =
            new SymuEngine.Repository.Networks.Knowledge.Repository.Knowledge(1, "k", 10);

        [TestMethod]
        public void AddKnowledgeTest()
        {
            Assert.IsFalse(_activity.Knowledges.Contains(_knowledge));
            _activity.AddKnowledge(_knowledge);
            Assert.IsTrue(_activity.Knowledges.Contains(_knowledge));
            _activity.AddKnowledge(_knowledge); //handle duplicate
            Assert.AreEqual(1, _activity.Knowledges.Count);
        }
    }
}