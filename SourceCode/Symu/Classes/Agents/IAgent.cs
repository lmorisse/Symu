#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace Symu.Classes.Agents
{
    /// <summary>
    ///     The Agent interface
    /// </summary>
    public interface IAgent
    {
        //todo => c#8 IAgentId AgentId;
        /// <summary>
        /// Clone an agent
        /// </summary>
        /// <returns></returns>
        IAgent Clone();
        /// <summary>
        /// Set an agent's property value by its name
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        void SetProperty(string propertyName, float value);
    }
}