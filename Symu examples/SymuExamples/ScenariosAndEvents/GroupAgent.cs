#region Licence

// Description: SymuBiz - SymuScenariosAndEvents
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents;
using Symu.Common.Interfaces;
using Symu.Environment;

#endregion

namespace SymuExamples.ScenariosAndEvents
{
    public sealed class GroupAgent : ReactiveAgent
    {
        public const byte Class = 1;

        /// <summary>
        ///     Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        private GroupAgent(SymuEnvironment environment) : base(
            ClassId, environment)
        {
        }

        public static IClassId ClassId => new ClassId(Class);

        /// <summary>
        ///     Factory method to create an agent
        ///     Call the Initialize method
        /// </summary>
        /// <returns></returns>
        public static GroupAgent CreateInstance(SymuEnvironment environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var agent = new GroupAgent(environment);
            agent.Initialize();
            return agent;
        }
    }
}