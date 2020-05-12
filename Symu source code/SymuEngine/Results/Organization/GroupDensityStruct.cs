#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuTools;

#endregion

namespace SymuEngine.Results.Organization
{
    /// <summary>
    ///     Group density is used to store information about group density per step : number of groups, max potential number of
    ///     groups, density
    /// </summary>
    /// <remarks>Dynamics of organizations, page 75</remarks>
    public readonly struct GroupDensityStruct
    {
        public GroupDensityStruct(float actualNumber, float maxNumber, ushort step)
        {
            ActualNumber = actualNumber;
            MaxNumber = maxNumber;
            Step = step;
        }

        /// <summary>
        ///     Maximum number of groups
        /// </summary>
        public float MaxNumber { get; }

        /// <summary>
        ///     Real number of groups at the Step
        /// </summary>
        public float ActualNumber { get; }

        public ushort Step { get; }

        /// <summary>
        ///     Density of groups : percentage of actual groups vs the maximum number of groups possible
        /// </summary>
        public float Density => Math.Abs(MaxNumber) < Constants.Tolerance ? 0 : ActualNumber * 100F / MaxNumber;

        public override string ToString()
        {
            return "Density " + Density + " / step" + Step;
        }
    }
}