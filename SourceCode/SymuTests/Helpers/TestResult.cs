#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Common.Classes;
using Symu.Common.Interfaces;
using Symu.Environment;
using Symu.Results;

#endregion

namespace SymuTests.Helpers
{
    public class TestResult : IResult
    {
        public bool Result { get; private set; }

        /// <summary>
        ///     If set to true, Tasks will be filled with value and stored during the simulation
        /// </summary>
        public bool On { get; set; }

        /// <summary>
        ///     Frequency of the results
        /// </summary>
        public TimeStepType Frequency { get; set; }

        public void SetResults()
        {
            Result = true;
        }

        public void Clear()
        {
            Result = false;
        }

        public void CopyTo(object clone)
        {
            if (clone != null)
            {
                ((TestResult) clone).Result = Result;
            }
        }

        public IResult Clone()
        {
            var test = new TestResult();
            CopyTo(test);
            return test;
        }
    }
}