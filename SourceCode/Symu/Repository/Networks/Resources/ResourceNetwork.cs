#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Symu.Common;
using Symu.Common.Interfaces;
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.Repository.Entity;
using Symu.Repository.Networks.Roles;

#endregion

namespace Symu.Repository.Networks.Resources
{
    /// <summary>
    ///     All resources in the network
    /// </summary>
    /// <example>database, products, routines, processes, ...</example>
    public class ResourceNetwork
    {
        /// <summary>
        ///     Repository of all the resources used during the simulation
        /// </summary>
        public ResourceCollection Repository { get; } = new ResourceCollection();

        /// <summary>
        ///     AgentResources.Key = IAgentId
        ///     AgentResources.Value = List of all the resourceId the agentId is using
        /// </summary>
        public ConcurrentDictionary<IAgentId, List<IAgentResource>> AgentResources { get; } =
            new ConcurrentDictionary<IAgentId, List<IAgentResource>>();
        public int Count => AgentResources.Count;

        public bool Any()
        {
            return AgentResources.Any();
        }

        public void Clear()
        {
            Repository.Clear();
            AgentResources.Clear();
        }

        #region Repository
        /// <summary>
        /// Get the resource from its Id
        /// </summary>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public IResource GetResource(IId resourceId)
        {
            return Repository.Get(resourceId);
        }

        /// <summary>
        ///     Add a resource to the repository
        /// </summary>
        public void Add(IResource resource)
        {
            if (Exists(resource))
            {
                return;
            }

            Repository.Add(resource);
        }

        public bool Exists(IResource resource)
        {
            return Repository.Contains(resource);
        }

        public bool Exists(IId resourceId)
        {
            return Repository.Exists(resourceId);
        }

        public void Remove(IResource resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            RemoveResource(resource.Id);
        }
        public void RemoveResource(IId resourceId)
        {
            foreach (var key in AgentResources.Keys)
            {
                AgentResources[key].RemoveAll(n => n.Equals(resourceId));
            }

            Repository.List.RemoveAll(x => x.Id.Equals(resourceId));
        }
        #endregion

        #region Agent resource

        /// <summary>
        ///     Remove agent from network
        /// </summary>
        /// <param name="agentId"></param>
        public void RemoveAgent(IAgentId agentId)
        {
            AgentResources.TryRemove(agentId, out _);
        }
        /// <summary>
        /// Exists agentId
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bool ExistsAgentId(IAgentId agentId)
        {
            return AgentResources.ContainsKey(agentId);
        }

        public bool Exists(IAgentId agentId, IId resourceId)
        {
            return ExistsAgentId(agentId) && AgentResources[agentId].Exists(x => x.Equals(resourceId));
        }

        /// <summary>
        /// Add a resource to the resource repository
        /// Add an agentResource to an agentId 
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="resource"></param>
        /// <param name="agentResource"></param>
        public void Add(IAgentId agentId, IResource resource, IAgentResource agentResource)
        //public void Add(IAgentId agentId, IResource resource, IResourceUsage resourceUsage, float allocation = 100)
        {
            if (resource is null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            Add(resource);
            AddAgentId(agentId);
            //var agentResource = new IAgentResource(resource.Id, resourceUsage, allocation);
            AddAgentResource(agentId, agentResource);
        }

        public void Add(IAgentId agentId, IAgentResource agentResource)
        {
            if (agentResource == null)
            {
                throw new ArgumentNullException(nameof(agentResource));
            }

            AddAgentId(agentId);
            AddAgentResource(agentId, agentResource);
        }

        /// <summary>
        ///     Add a Belief to an AgentId
        ///     AgentId is supposed to be already present in the collection.
        ///     if not use Add method
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="agentResource"></param>
        private void AddAgentResource(IAgentId agentId, IAgentResource agentResource)
        {
            if (AgentResources[agentId].Contains(agentResource))
            {
                return;
            }

            AgentResources[agentId].Add(agentResource);
            // Reallocation
            //var resourceIds = GetResourceIds(agentId, agentResource.ResourceUsage);
            //if (resourceIds is null)
            //{
            //    return;
            //}

            //var resourceCollection = resourceIds.ToList();
            var totalAllocation =
                AgentResources[agentId].Sum(x => x.ResourceAllocation);

            // There is non main object used at 100%
            // Objects are added as things progress, with the good allocation
            if (!(totalAllocation >= 100))
            {
                return;
            }

            foreach (var resource in AgentResources[agentId])
            {
                //Don't use ComponentAllocation[ca] *= 100/ => return 0
                resource.ResourceAllocation = Convert.ToSingle(resource.ResourceAllocation * 100 / totalAllocation);
            }
        }

        public void AddAgentId(IAgentId agentId)
        {
            if (!ExistsAgentId(agentId))
            {
                AgentResources.TryAdd(agentId, new List<IAgentResource>());
            }
        }

        public float GetAllocation(IAgentId agentId, IId resourceId)
        {
            if (HasResource(agentId, resourceId))
            {
                return GetAgentResource(agentId, resourceId).Sum(x => x.ResourceAllocation);
            }

            return 0;
        }

        public float GetAllocation(IAgentId agentId, IId resourceId, IResourceUsage resourceUsage)
        {
            if (HasResource(agentId, resourceId, resourceUsage))
            {
                return GetAgentResource(agentId, resourceId, resourceUsage).ResourceAllocation;
            }

            return 0;
        }
        /// <summary>
        ///     Get the IResource used by an agent via its resourceId
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public IResource GetResource(IAgentId agentId, IId resourceId)
        {
            return HasResource(agentId, resourceId) ? Repository.Get(resourceId) : null;
        }
        /// <summary>
        ///     Get the IResource used by an agent via its resourceId
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public TResource GetResource<TResource>(IAgentId agentId, IId resourceId) where TResource : IResource
        {
            return HasResource(agentId, resourceId) ? (TResource)Repository.Get(resourceId) : default;
        }
        /// <summary>
        ///     Get the IAgentResource used by an agent with a specific type of use
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public List<IAgentResource> GetAgentResource(IAgentId agentId, IId resourceId)
        {
            return HasResource(agentId, resourceId) ? AgentResources[agentId].FindAll(n => n.Equals(resourceId)) : null;
        }
        /// <summary>
        ///     Get the IAgentResource used by an agent with a specific type of use
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="resourceId"></param>
        /// <param name="resourceUsage"></param>
        /// <returns></returns>
        public IAgentResource GetAgentResource(IAgentId agentId, IId resourceId, IResourceUsage resourceUsage)
        {
            return HasResource(agentId, resourceId, resourceUsage) ? AgentResources[agentId].Find(n => n.Equals(resourceId, resourceUsage)) : null;
        }
        public bool HasResource(IAgentId agentId, IId resourceId, IResourceUsage resourceUsage)
        {
            return ExistsAgentId(agentId) && AgentResources[agentId].Exists(n => n.Equals(resourceId, resourceUsage));
        }
        public bool HasResource(IAgentId agentId, IId resourceId)
        {
            return ExistsAgentId(agentId) && AgentResources[agentId].Exists(n => n.Equals(resourceId));
        }
        public bool HasResource(IAgentId agentId, IResourceUsage resourceUsage)
        {
            return ExistsAgentId(agentId) && AgentResources[agentId].Exists(n => n.Equals(resourceUsage));
        }

        /// <summary>
        ///     Get the list of all the resources the agentId is using
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public IEnumerable<IId> GetResourceIds(IAgentId agentId)
        {
            return ExistsAgentId(agentId) ? AgentResources[agentId].Select(x => x.ResourceId) : new List<IId>();
        }

        /// <summary>
        ///     Get the list of all the resources the agentId is using filtered by type of use
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="resourceUsage"></param>
        /// <returns></returns>
        public IEnumerable<IId> GetResourceIds(IAgentId agentId, IResourceUsage resourceUsage)
        {
            return ExistsAgentId(agentId)
                ? AgentResources[agentId].FindAll(n => n.Equals(resourceUsage)).Select(x => x.ResourceId)
                : new List<IId>();
        }

        /// <summary>
        ///     Get all the agents of classId using resourceId
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="resourceUsage"></param>
        /// <param name="agentClassId"></param>
        /// <returns></returns>
        public List<IAgentId> GetAgentIds(IId resourceId, IResourceUsage resourceUsage, IClassId agentClassId)
        {
            return (from agentId 
                in AgentResources.Keys.Where(x => x.ClassId.Equals(agentClassId)) 
                let agentResource = AgentResources[agentId] 
                where agentResource.Exists(x => x.Equals(resourceId, resourceUsage))
                select agentId).ToList();
        }

        /// <summary>
        ///     agentId is added to groupId 
        ///     agentId inherits all the resources of the groupId
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupId"></param>
        public void AddMemberToGroup(IAgentId agentId, IAgentId groupId)
        {
            if (!ExistsAgentId(groupId))
            {
                return;
            }
            AddAgentId(agentId);
            foreach (var groupResourceId in AgentResources[groupId])
            {
                Add(agentId, groupResourceId.Clone());//.ResourceId, groupResourceId.ResourceUsage, groupResourceId.ResourceAllocation);
            }
        }

        /// <summary>
        ///     agentId is removed to groupId 
        ///     agentId looses all the resources of the groupId
        /// </summary>
        public void RemoveMemberFromGroup(IAgentId agentId, IAgentId groupId)
        {
            if (!ExistsAgentId(agentId) || !ExistsAgentId(groupId))
            {
                return;
            }

            foreach (var groupResourceId in AgentResources[groupId])
            {
                AgentResources[agentId].RemoveAll(n => n.Equals(groupResourceId.ResourceId) && n.Equals(groupResourceId.ResourceUsage));
            }
        }

        public void Remove(IAgentId agentId, IId resourceId)
        {
            if (!ExistsAgentId(agentId))
            {
                return;
            }
            AgentResources[agentId].RemoveAll(l => l.Equals(resourceId));
        }
        /// <summary>
        ///     Copy the same networks from an agent to another
        /// </summary>
        /// <param name="fromAgentId"></param>
        /// <param name="toAgentId"></param>
        public void CopyTo(IAgentId fromAgentId, IAgentId toAgentId)
        {
            if (!ExistsAgentId(fromAgentId))
            {
                return;
            }
            AddAgentId(toAgentId);
            AgentResources[toAgentId].Clear();
            foreach (var agentResource in AgentResources[fromAgentId])
            {
                Add(toAgentId, agentResource.Clone());
            }
        }

        /// <summary>
        ///     Make a clone of Portfolios from modeling to Symu
        /// </summary>
        /// <param name="resources"></param>
        public void CopyTo(ResourceNetwork resources)
        {
            if (resources is null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            foreach (var resource in Repository.List)
            {
                resources.Add(resource);
            }
            foreach (var networkPortfolio in AgentResources)
            foreach (var portfolio in networkPortfolio.Value)
            {
                resources.Add(networkPortfolio.Key, portfolio);
            }
        }
        #endregion
    }
}