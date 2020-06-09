#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Environment;
using Symu.Results;

#endregion

namespace SymuTests.Helpers
{
    public class TestResult : SymuResults
    {
        public TestResult(SymuEnvironment environment) : base(environment)
        {
        }

        public bool Result { get; private set; }

        protected override void HandleResults()
        {
            Result = true;
        }

        public override void Clear()
        {
            Result = false;
        }

        public override void CopyTo(object clone)
        {
            if (clone != null)
            {
                ((TestResult) clone).Result = Result;
            }
        }

        public override SymuResults Clone()
        {
            var test = new TestResult(Environment);
            CopyTo(test);
            return test;
        }
    }
}