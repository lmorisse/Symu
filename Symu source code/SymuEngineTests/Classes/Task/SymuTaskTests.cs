﻿using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Task;
using SymuEngine.Repository;
using SymuTools;
using SymuEngine.Repository.Networks.Knowledges;

namespace SymuEngineTests.Classes.Task
{
    [TestClass]
    public class SymuTaskTests
    {
        private readonly SymuTask _task = new SymuTask(0);
        private readonly MurphyTask _model = new MurphyTask();
        private readonly List<Knowledge> _knowledges = new List<Knowledge>();

        [TestInitialize]
        public void Initialize()
        {
            for (var i = 0; i < 2; i++)
            {
                // knowledge length of 10 is arbitrary in this example
                var knowledge = new Knowledge((ushort)i, i.ToString(), 10);
                _knowledges.Add(knowledge);
            }
        }
        /// <summary>
        /// With RequiredRatio = 0
        /// </summary>
        [TestMethod]
        public void SetKnowledgesBitsTest()
        {
            _model.RequiredRatio = 0;
            _task.SetKnowledgesBits(_model, _knowledges,1);
            foreach (var knowledgeBits in _task.KnowledgesBits.List)
            {
                Assert.AreEqual(0, knowledgeBits.GetRequired().Length);
                Assert.AreEqual(0, knowledgeBits.GetMandatory().Length);
            }
        }
        /// <summary>
        /// With RequiredRatio = 1 & RequiredMandatoryRatio =0
        /// Complexity = 1
        /// </summary>
        [TestMethod]
        public void SetKnowledgesBitsTest1()
        {
            _model.RequiredRatio = 1;
            _model.RequiredMandatoryRatio = 0;
            _task.SetKnowledgesBits(_model, _knowledges, 1);
            foreach (var knowledgeBits in _task.KnowledgesBits.List)
            {
                Assert.AreEqual(10, knowledgeBits.GetRequired().Length);
                Assert.AreEqual(0, knowledgeBits.GetMandatory().Length);
            }
        }
        /// <summary>
        /// With RequiredRatio = 1 & RequiredMandatoryRatio =1
        /// Complexity = 1
        /// </summary>
        [TestMethod]
        public void SetKnowledgesBitsTest2()
        {
            _model.RequiredRatio = 1;
            _model.RequiredMandatoryRatio = 1;
            _task.SetKnowledgesBits(_model, _knowledges, 1);
            foreach (var knowledgeBits in _task.KnowledgesBits.List)
            {
                Assert.AreEqual(10, knowledgeBits.GetRequired().Length);
                Assert.AreEqual(10, knowledgeBits.GetMandatory().Length);
            }
        }
        /// <summary>
        /// With RequiredRatio = 1 & RequiredMandatoryRatio =1
        /// Complexity = 0
        /// </summary>
        [TestMethod]
        public void SetKnowledgesBitsTest3()
        {
            _model.RequiredRatio = 1;
            _model.RequiredMandatoryRatio = 1;
            _task.SetKnowledgesBits(_model, _knowledges, 0);
            foreach (var knowledgeBits in _task.KnowledgesBits.List)
            {
                Assert.AreEqual(0, knowledgeBits.GetRequired().Length);
                Assert.AreEqual(0, knowledgeBits.GetMandatory().Length);
            }
        }
        /// <summary>
        /// With RequiredRatio = 1 & RequiredMandatoryRatio =1
        /// Complexity = 0.5F
        /// </summary>
        [TestMethod]
        public void SetKnowledgesBitsTest4()
        {
            _model.RequiredRatio = 1;
            _model.RequiredMandatoryRatio = 1;
            _task.SetKnowledgesBits(_model, _knowledges, 0.5F);
            foreach (var knowledgeBits in _task.KnowledgesBits.List)
            {
                Assert.IsTrue(knowledgeBits.GetRequired().Length < 10 && knowledgeBits.GetRequired().Length > 0);
                Assert.IsTrue(knowledgeBits.GetMandatory().Length < 10 && knowledgeBits.GetMandatory().Length > 0);
            }
        }
    }
}