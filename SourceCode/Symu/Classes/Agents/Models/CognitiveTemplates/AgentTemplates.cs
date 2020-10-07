#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

#endregion

#region using directives

using System.Collections.Generic;
using System.Linq;

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
        private readonly List<CognitiveArchitectureTemplate> _templates = new List<CognitiveArchitectureTemplate>();

        public AgentTemplates()
        {
            Add(Standard);
            Add(Human);
            Add(Promoter);
            Add(Internet);
        }

        public StandardAgentTemplate Standard { get; } = new StandardAgentTemplate();
        public SimpleHumanTemplate Human { get; } = new SimpleHumanTemplate();
        public PromoterTemplate Promoter { get; } = new PromoterTemplate();
        public InternetAccessTemplate Internet { get; } = new InternetAccessTemplate();

        public void Add(CognitiveArchitectureTemplate template)
        {
            if (!_templates.Contains(template))
            {
                _templates.Add(template);
            }
        }

        public TTemplate Get<TTemplate>() where TTemplate : CognitiveArchitectureTemplate
        {
            return _templates.OfType<TTemplate>().FirstOrDefault();
        }
    }
}