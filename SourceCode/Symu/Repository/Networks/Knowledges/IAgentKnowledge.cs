#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

using Symu.Common.Interfaces.Entity;

namespace Symu.Repository.Networks.Knowledges
{
    /// <summary>
    /// Defines how who knows what
    /// </summary>
    public interface IAgentKnowledge
    {
        IId KnowledgeId { get; }
    }
}