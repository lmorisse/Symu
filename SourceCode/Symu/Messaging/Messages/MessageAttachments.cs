#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using Symu.Common.Interfaces.Entity;
using Symu.Repository.Entity;

#endregion

namespace Symu.Messaging.Messages
{
    /// <summary>
    ///     Manage the message attachments
    ///     Special attachments :
    ///     Knowledge
    ///     Belief
    ///     Referral
    /// </summary>
    public class MessageAttachments
    {
        #region Beliefs

        /// <summary>
        ///     The BeliefBits that are sent
        /// </summary>
        public Bits BeliefBits { get; set; }

        #endregion

        /// <summary>
        ///     Copy attachments to another messageAttachments
        ///     Use to prevent bad behaviour when you use :
        ///     messageA.Attachments = messageB.Attachments
        /// </summary>
        /// <param name="attachments"></param>
        public void Copy(MessageAttachments attachments)
        {
            if (attachments is null)
            {
                throw new ArgumentNullException(nameof(attachments));
            }

            Objects.AddRange(attachments.Objects);
            KnowledgeId = attachments.KnowledgeId;
            KnowledgeBit = attachments.KnowledgeBit;
            KnowledgeBits = attachments.KnowledgeBits;
            BeliefBits = attachments.BeliefBits;
        }

        #region Attachments

        public List<object> Objects { get; } = new List<object>();

        public object First => Objects.Count > 0 ? Objects[0] : null;
        public object Second => Objects.Count > 1 ? Objects[1] : null;
        public object Third => Objects.Count > 2 ? Objects[2] : null;
        public object Fourth => Objects.Count > 3 ? Objects[3] : null;

        public object IndexOf(int index)
        {
            return Objects.Count > index ? Objects[index] : null;
        }

        public void Add(object attachment)
        {
            Objects.Add(attachment);
        }

        #endregion

        #region Knowledge

        /// <summary>
        ///     The knowledge Id that is sent
        /// </summary>
        public IId KnowledgeId { get; set; }

        /// <summary>
        ///     The Knowledge bit index that is asked
        /// </summary>
        public byte KnowledgeBit { get; set; }

        /// <summary>
        ///     The knowledgeBits that are sent
        /// </summary>
        public Bits KnowledgeBits { get; set; }

        #endregion
    }
}