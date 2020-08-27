#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Repository.Entity;

#endregion

namespace Symu.Classes.Agents.Models.CognitiveModels
{
    /// <summary>
    ///     Knowledge & Beliefs from Construct Software
    ///     Knowledge and knowledge Transactive memory
    ///     Beliefs and beliefs transactive memory
    ///     Referral
    /// </summary>
    /// <remarks>Knowledge & Beliefs from Construct Software</remarks>
    public class KnowledgeAndBeliefs
    {
        /// <summary>
        ///     Clone KnowledgeAndBeliefs
        /// </summary>
        /// <param name="knowledgeAndBeliefs"></param>
        public void CopyTo(KnowledgeAndBeliefs knowledgeAndBeliefs)
        {
            if (knowledgeAndBeliefs is null)
            {
                throw new ArgumentNullException(nameof(knowledgeAndBeliefs));
            }

            knowledgeAndBeliefs.HasInitialKnowledge = HasInitialKnowledge;
            knowledgeAndBeliefs.HasKnowledge = HasKnowledge;
            knowledgeAndBeliefs.HasInitialBelief = HasInitialBelief;
            knowledgeAndBeliefs.DefaultBeliefLevel = DefaultBeliefLevel;
            knowledgeAndBeliefs.HasBelief = HasBelief;
        }

        #region Knowledge

        /// <summary>
        ///     This parameter specify whether agents of this class can store knowledge
        /// </summary>
        public bool HasKnowledge { get; set; }

        /// <summary>
        ///     This parameter specify whether agents of this class has initial knowledge
        /// </summary>
        public bool HasInitialKnowledge { get; set; }

        #endregion

        #region Beliefs

        /// <summary>
        ///     This parameter specify whether agents of this class can store beliefs
        /// </summary>
        public bool HasBelief { get; set; }

        /// <summary>
        ///     This parameter specify whether agents of this class has initial beliefs
        /// </summary>
        public bool HasInitialBelief { get; set; }

        /// <summary>
        ///     Default belief level use to create new belief during symu
        /// </summary>
        public BeliefLevel DefaultBeliefLevel { get; set; } = BeliefLevel.NeitherAgreeNorDisagree;

        #endregion

        #region Transactive memories

        #endregion

        #region Referral

        #endregion
    }
}