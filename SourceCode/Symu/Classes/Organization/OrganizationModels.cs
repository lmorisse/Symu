#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Symu.Common.Classes;
using Symu.DNA.GraphNetworks.TwoModesNetworks.Sphere;
using Symu.Engine;
using Symu.Repository.Entities;

#endregion

namespace Symu.Classes.Organization
{
    public class LearningModelEntity : ModelEntity
    {
        public override ModelEntity Clone()
        {
            var clone = new LearningModelEntity();
            CopyTo(clone);
            return clone;
        }
    }
    public class ForgettingModelEntity : ModelEntity
    {
        public override ModelEntity Clone()
        {
            var clone = new ForgettingModelEntity();
            CopyTo(clone);
            return clone;
        }
    }
    public class InfluenceModelEntity : ModelEntity
    {
        public override ModelEntity Clone()
        {
            var clone = new InfluenceModelEntity();
            CopyTo(clone);
            return clone;
        }
    }
    public class BeliefModelEntity : ModelEntity
    {
        public override ModelEntity Clone()
        {
            var clone = new BeliefModelEntity();
            CopyTo(clone);
            return clone;
        }
    }
    public class KnowledgeModelEntity : ModelEntity
    {
        public override ModelEntity Clone()
        {
            var clone = new KnowledgeModelEntity();
            CopyTo(clone);
            return clone;
        }
    }
    /// <summary>
    ///     List of the models used by the organizationEntity
    /// </summary>
    public class OrganizationModels
    {
        public List<ModelEntity> List { get; }= new List<ModelEntity>();

        public OrganizationModels()
        {
            Add(new LearningModelEntity());
            Add(new ForgettingModelEntity());
            Add(new InfluenceModelEntity());
            Add(new BeliefModelEntity());
            Add(new KnowledgeModelEntity());
            Add(new InteractionSphereModel());
        }

        public void Add(ModelEntity model)
        {
            if (!List.Contains(model))
            {
                List.Add(model);
            }
        }

        /// <summary>
        ///     Agent knowledge learning model
        /// </summary>
        public LearningModelEntity Learning => Get<LearningModelEntity>();//{ get; set; } = new LearningModelEntity();

        /// <summary>
        ///     Agent knowledge forgetting model
        /// </summary>
        public ForgettingModelEntity Forgetting => Get<ForgettingModelEntity>();//{ get; set; } = new ForgettingModelEntity();

        /// <summary>
        ///     Agent influence model
        /// </summary>
        public InfluenceModelEntity Influence => Get<InfluenceModelEntity>();//{ get; set; } = new InfluenceModelEntity();

        /// <summary>
        ///     Agent influence model
        /// </summary>
        public BeliefModelEntity Beliefs => Get<BeliefModelEntity>();//{ get; set; } = new BeliefModelEntity();

        /// <summary>
        ///     Agent knowledge model
        /// </summary>
        public KnowledgeModelEntity Knowledge => Get<KnowledgeModelEntity>();//{ get; set; } = new KnowledgeModelEntity();

        /// <summary>
        ///     Impact level of agent's belief on how agent will accept to do the task
        /// </summary>
        public BeliefWeightLevel BeliefWeightLevel { get; set; } = BeliefWeightLevel.RandomWeight;

        public InteractionSphereModel InteractionSphere => Get<InteractionSphereModel>();// { get; set; } = new InteractionSphereModel();

        /// <summary>
        ///     Random generator modes in order to create random network
        /// </summary>
        public RandomGenerator Generator { get; set; } = RandomGenerator.RandomUniform;

        /// <summary>
        ///     Define the ratio of task splitting in intraday mode
        /// </summary>
        public float Intraday { get; set; } = 0.01F;

        public OrganizationModels Clone()
        {
            var clone = new OrganizationModels();
            clone.List.Clear();
            foreach (var model in List)
            {
                clone.Add(model.Clone());
            }
            clone.Generator = Generator;
            clone.BeliefWeightLevel = BeliefWeightLevel;
            return clone;
        }

        public TModel Get<TModel>() where TModel : ModelEntity
        {
            return List.OfType<TModel>().FirstOrDefault();
        }

        /// <summary>
        ///     Clone all models on
        /// </summary>
        /// <param name="rate"></param>
        public void SetOn(float rate)
        {
            foreach (var model in List)
            {
                model.On=true;
                model.RateOfAgentsOn = rate;
            }
        }

        /// <summary>
        ///     Clone all models off
        /// </summary>
        public void SetOff()
        {
            List.ForEach(x => x.On =false);
        }
    }
}