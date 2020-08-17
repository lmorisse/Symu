#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

using Symu.Tools.Interfaces;

namespace Symu.Classes.Agents
{
    /// <summary>
    /// ClassId is the implementation of IClassId, the interface for the unique identifier of the class of the agent
    /// </summary>
    public struct ClassId : IClassId
    {
        /// <summary>
        ///     Class Key of the agent
        /// </summary>
        public byte Id { get; set; }

        public ClassId(byte id)
        {
            Id = id;
        }

        public bool Equals(IClassId classId)
        {
            return Id == ((ClassId)classId).Id;
        }
    }
}