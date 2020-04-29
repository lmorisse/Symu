#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;

#endregion

namespace SymuEngine.Classes.Agent.Models.Templates
{
    /// <summary>
    ///     Set all the CognitiveArchitecture parameters for the AgentTemplate
    /// </summary>
    public abstract class CognitiveArchitectureTemplate
    {
        public CognitiveArchitecture.CognitiveArchitecture Cognitive { get; set; } =
            new CognitiveArchitecture.CognitiveArchitecture(null, new AgentId(0, 1), 0);

        public void Set(CognitiveArchitecture.CognitiveArchitecture cognitive)
        {
            if (cognitive is null)
            {
                throw new ArgumentNullException(nameof(cognitive));
            }

            Cognitive.CopyTo(cognitive);
        }
    }
}