#region Licence

// Description: SymuBiz - SymuGroupAndInteraction
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Organization;
using Symu.Common;

#endregion

namespace SymuMessageAndTask.Classes
{
    public class ExampleOrganization : Organization
    {

        private float _costOfTask = 1F;

        private float _initialCapacity = 1F;

        private int _numberOfTasks = 1;

        private float _switchingContextCost = 1F;
        private int _workersCount = 5;

        public int WorkersCount
        {
            get => _workersCount;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("WorkersCount should be > 0");
                }

                _workersCount = value;
            }
        }

        public float InitialCapacity
        {
            get => _initialCapacity;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("InitialCapacity should be >= 0");
                }

                _initialCapacity = value;
            }
        }

        public int NumberOfTasks
        {
            get => _numberOfTasks;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("NumberOfMessages should be >= 0");
                }

                _numberOfTasks = value;
            }
        }

        public float CostOfTask
        {
            get => _costOfTask;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("CostOfTask should be >= 0");
                }

                _costOfTask = value;
            }
        }

        public float SwitchingContextCost
        {
            get => _switchingContextCost;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("SwitchingContextCost should be >= 1");
                }

                _switchingContextCost = value;
            }
        }
        public ExampleOrganization(): base("symu")
        {
            Murphies.SetOff();
        }

        public override Organization Clone()
        {
            var clone = new ExampleOrganization();
            CopyTo(clone);
            clone.CostOfTask = CostOfTask;
            clone.InitialCapacity = InitialCapacity;
            clone.NumberOfTasks = NumberOfTasks;
            clone.SwitchingContextCost = SwitchingContextCost;
            clone.WorkersCount = WorkersCount;
            return clone;
        }
    }
}