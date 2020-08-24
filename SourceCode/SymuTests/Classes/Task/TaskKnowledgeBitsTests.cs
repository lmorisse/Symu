#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Task;
using Symu.Common.Interfaces.Entity;

#endregion

namespace SymuTests.Classes.Task
{
    [TestClass]
    public class TaskKnowledgeBitsTests
    {
        private readonly UId _knowledgeId = new UId(1);
        private readonly TaskKnowledgesBits _knowledgeBits = new TaskKnowledgesBits();
        private TaskKnowledgeBits _bits;


        [TestInitialize]
        public void Initialize()
        {
            _bits = new TaskKnowledgeBits
            {
                KnowledgeId = _knowledgeId
            };
            _bits.SetRequired(new byte[] {1, 2});
            _bits.SetMandatory(new byte[] {1, 2});
            _knowledgeBits.Add(_bits);
        }

        [TestMethod]
        public void RemoveFirstMandatoryTest()
        {
            _knowledgeBits.RemoveFirstMandatory(_knowledgeId);
            Assert.AreEqual(2, _bits.GetMandatory()[0]);
            _knowledgeBits.RemoveFirstMandatory(_knowledgeId);
            Assert.AreEqual(0, _bits.GetMandatory().Length);
            _knowledgeBits.RemoveFirstMandatory(_knowledgeId);
            Assert.AreEqual(0, _bits.GetMandatory().Length);
        }

        [TestMethod]
        public void RemoveFirstRequiredTest()
        {
            _knowledgeBits.RemoveFirstRequired(_knowledgeId);
            Assert.AreEqual(2, _bits.GetRequired()[0]);
            _knowledgeBits.RemoveFirstRequired(_knowledgeId);
            Assert.AreEqual(0, _bits.GetRequired().Length);
            _knowledgeBits.RemoveFirstRequired(_knowledgeId);
            Assert.AreEqual(0, _bits.GetRequired().Length);
        }
    }
}