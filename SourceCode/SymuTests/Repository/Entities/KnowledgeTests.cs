using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Organization;
using Symu.Common.Classes;
using Symu.Repository.Entities;
using SymuTests.Helpers;

namespace SymuTests.Repository.Entities
{
    [TestClass()]
    public class KnowledgeTests : BaseTestClass
    {
        private Knowledge _knowledge;

        [TestInitialize]
        public void Initialize()
        {
        }
        /// <summary>
        /// model Off
        /// </summary>
        [TestMethod()]
        public void KnowledgeTest()
        {
            Organization.Models.Beliefs.On = false;
            _knowledge = new Knowledge(Network, Organization.Models, "1", 1);
            Assert.AreEqual(1, _knowledge.Length);
            Assert.IsNull(_knowledge.AssociatedBelief);
        }
        /// <summary>
        /// model On
        /// </summary>
        [TestMethod()]
        public void KnowledgeTest1()
        {
            Organization.Models.Beliefs.On = true;
            _knowledge = new Knowledge(Network, Organization.Models, "1", 1);
            Assert.AreEqual(1, _knowledge.Length);
            Assert.IsNotNull(_knowledge.AssociatedBelief);
            Assert.AreEqual(_knowledge.EntityId, _knowledge.AssociatedBelief.KnowledgeId);
        }
        /// <summary>
        /// model off
        /// </summary>
        [TestMethod()]
        public void CloneTest()
        {
            Organization.Models.Beliefs.On = false;
            _knowledge = new Knowledge(Network, Organization.Models, "1", 1);
            var clone = (Knowledge)_knowledge.Clone();
            Assert.AreEqual(_knowledge.Length, clone.Length);
            Assert.IsNull(clone.AssociatedBelief);
        }        
        /// <summary>
        /// model on
        /// </summary>
        [TestMethod()]
        public void CloneTest1()
        {
            Organization.Models.Beliefs.On = true;
            _knowledge = new Knowledge(Network, Organization.Models, "1", 1);
            var clone = (Knowledge)_knowledge.Clone();
            Assert.AreEqual(_knowledge.Length, clone.Length);
            Assert.AreEqual(_knowledge.AssociatedBelief, clone.AssociatedBelief);
        }
        /// <summary>
        /// mix, in case of a change in Models.Beliefs 
        /// </summary>
        [TestMethod()]
        public void CloneTest2()
        {
            Organization.Models.Beliefs.On = false;
            _knowledge = new Knowledge(Network, Organization.Models, "1", 1);
            Organization.Models.Beliefs.On = true;
            var clone = (Knowledge)_knowledge.Clone();
            Assert.AreEqual(_knowledge.Length, clone.Length);
            Assert.IsNotNull(clone.AssociatedBelief);
            Assert.AreEqual(_knowledge.AssociatedBelief, clone.AssociatedBelief);
        }
    }
}