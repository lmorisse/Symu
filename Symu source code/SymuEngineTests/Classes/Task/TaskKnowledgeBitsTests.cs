#region Licence

// Description: Symu - SymuEngineTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Task;

#endregion

namespace SymuEngineTests.Classes.Task
{
    [TestClass]
    public class TaskKnowledgeBitsTests
    {
        private readonly TaskKnowledgesBits _knowledgeBits = new TaskKnowledgesBits();
        private TaskKnowledgeBits _bits;


        [TestInitialize]
        public void Initialize()
        {
            _bits = new TaskKnowledgeBits
            {
                KnowledgeId = 1
            };
            _bits.SetRequired(new byte[] {1, 2});
            _bits.SetMandatory(new byte[] {1, 2});
            _knowledgeBits.Add(_bits);
        }

        [TestMethod]
        public void RemoveFirstMandatoryTest()
        {
            _knowledgeBits.RemoveFirstMandatory(1);
            Assert.AreEqual(2, _bits.GetMandatory()[0]);
            _knowledgeBits.RemoveFirstMandatory(1);
            Assert.AreEqual(0, _bits.GetMandatory().Length);
            _knowledgeBits.RemoveFirstMandatory(1);
            Assert.AreEqual(0, _bits.GetMandatory().Length);
        }

        [TestMethod]
        public void RemoveFirstRequiredTest()
        {
            _knowledgeBits.RemoveFirstRequired(1);
            Assert.AreEqual(2, _bits.GetRequired()[0]);
            _knowledgeBits.RemoveFirstRequired(1);
            Assert.AreEqual(0, _bits.GetRequired().Length);
            _knowledgeBits.RemoveFirstRequired(1);
            Assert.AreEqual(0, _bits.GetRequired().Length);
        }
    }
}