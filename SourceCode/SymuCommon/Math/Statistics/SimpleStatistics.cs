#region Licence

// Description: SymuBiz - SymuTools
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

#endregion

#region using directives

using System.Collections.Generic;
using MathNet.Numerics.Statistics;

#endregion

namespace Symu.Common.Math.Statistics
{
    public static class SimpleStatistics
    {
        public static float GetStandardDeviation(List<float> values)
        {
            if (values == null || values.Count <= 1)
            {
                return 0;
            }

            return (float) values.StandardDeviation();
        }
    }
}