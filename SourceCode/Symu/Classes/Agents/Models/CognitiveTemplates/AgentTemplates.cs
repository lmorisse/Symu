#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

#endregion

namespace Symu.Classes.Agents.Models.CognitiveTemplates
{
    /// <summary>
    ///     List of all available agent templates
    /// </summary>
    /// <example>
    ///     Human
    ///     ...
    /// </example>
    public class AgentTemplates
    {
        public StandardAgentTemplate Standard { get; } = new StandardAgentTemplate();
        public SimpleHumanTemplate Human { get; } = new SimpleHumanTemplate();
        public PromoterTemplate Promoter { get; } = new PromoterTemplate();
        public InternetAccessTemplate Internet { get; } = new InternetAccessTemplate();
    }
}