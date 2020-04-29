#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using SymuEngine.Common;

#endregion

namespace SymuEngine.Classes.Agent.Models.CognitiveArchitecture.Knowledge
{
    /// <summary>
    ///     Describe the knowledge model :
    ///     How to generate knowledge Network
    /// </summary>
    public class KnowledgeModel : ModelEntity
    {
        public RandomGenerator RandomGenerator { get; set; } = RandomGenerator.RandomBinary;
    }
}