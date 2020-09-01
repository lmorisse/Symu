﻿#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Common.Interfaces;
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.DNA.OneModeNetworks;
using Symu.DNA.TwoModesNetworks.AgentResource;

#endregion

namespace Symu.Repository.Entity
{
    /// <summary>
    ///     Define who is using a resource and how
    /// </summary>
    public class AgentResource : IAgentResource
    {
        public AgentResource(IId resourceId, IResourceUsage resourceUsage, float resourceAllocation=100)
        {
            ResourceId = resourceId;
            ResourceUsage = resourceUsage;
            ResourceAllocation = resourceAllocation;
        }

        /// <summary>
        ///     The unique agentId of the resource
        /// </summary>
        public IId ResourceId { get; }

        /// <summary>
        ///     Define how the AgentId is using the resource
        /// </summary>
        public IResourceUsage ResourceUsage { get; }


        private float _resourceAllocation;

        /// <summary>
        ///     Allocation of capacity per resource
        ///     capacity allocation ranging from [0; 100]
        /// </summary>
        public float ResourceAllocation
        {
            get => _resourceAllocation;
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentOutOfRangeException("Allocation should be between [0;100]");
                }

                _resourceAllocation = value;
            }
        }

        public IAgentResource Clone()
        {
            return new AgentResource(ResourceId, (ResourceUsage)ResourceUsage, ResourceAllocation);
        }

        public bool Equals(IResourceUsage resourceUsage)
        {
            return ResourceUsage.Equals(resourceUsage);
        }

        public bool Equals(IId resourceId)
        {
            return ResourceId.Equals(resourceId);
        }

        public bool Equals(IId resourceId, IResourceUsage resourceUsage)
        {
            return Equals(resourceUsage) && ResourceId.Equals(resourceId);
        }

        public override bool Equals(object obj)
        {
            return obj is AgentResource agentResource &&
                   ResourceId.Equals(agentResource.ResourceId) &&
                   ResourceUsage == agentResource.ResourceUsage;
        }
    }
}