#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Common.Interfaces;

#endregion

namespace Symu.Repository.Networks.Resources
{

    /// <summary>
    ///     Define who is using a resource and how
    /// </summary>
    public class IAgentResource
    {
        public IAgentResource(IAgentId resourceId, byte typeOfUse, float allocation)
        {
            ResourceId = resourceId;
            TypeOfUse = typeOfUse;
            Allocation = allocation;
        }

        /// <summary>
        ///     The unique agentId of the resource
        /// </summary>
        public IAgentId ResourceId { get; }

        /// <summary>
        ///     Define how the AgentId is using the resource
        /// </summary>
        public byte TypeOfUse { get; }

        /// <summary>
        ///     Allocation of capacity per resource
        ///     capacity allocation ranging from [0; 100]
        /// </summary>
        public float Allocation { get; set; }

        public bool IsTypeOfUse(byte typeOfUse)
        {
            return TypeOfUse == typeOfUse;
        }

        public bool IsTypeOfUseAndClassId(byte typeOfUse, IClassId classId)
        {
            return IsTypeOfUse(typeOfUse) && ResourceId.Equals(classId);
        }

        public bool Equals(IAgentId resourceId, byte typeOfUse)
        {
            return IsTypeOfUse(typeOfUse) && ResourceId.Equals(resourceId);
        }

        public bool Equals(IAgentId resourceId)
        {
            return ResourceId.Equals(resourceId);
        }

        public override bool Equals(object obj)
        {
            return obj is IAgentResource agentResource &&
                   ResourceId.Equals(agentResource.ResourceId) &&
                   TypeOfUse == agentResource.TypeOfUse;
        }
    }
}