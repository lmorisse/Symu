#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;

#endregion

namespace SymuEngine.Classes.Blockers
{
    /// <summary>
    ///     Blocker are used to block the tasks in progress, when the system is sub-optimal
    /// </summary>
    public class Blocker
    {
        /// <summary>
        ///     Constructor without parameter
        /// </summary>
        /// <param name="type">type of the blocker</param>
        /// <param name="step">step of creation of the blocker</param>
        public Blocker(int type, ushort step)
        {
            Type = type;
            InitialStep = step;
            LastRecoverStep = step;
            Parameter = null;
            Parameter2 = null;
        }

        /// <summary>
        ///     Constructor with one parameter
        /// </summary>
        /// <param name="type">type of the blocker</param>
        /// <param name="step">step of creation of the blocker</param>
        /// <param name="parameter"></param>
        public Blocker(int type, ushort step, object parameter) : this(type, step)
        {
            Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        }

        /// <summary>
        ///     Constructor with two parameters
        /// </summary>
        /// <param name="type">type of the blocker</param>
        /// <param name="step">step of creation of the blocker</param>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        public Blocker(int type, ushort step, object parameter1, object parameter2) : this(type, step, parameter1)
        {
            Parameter2 = parameter2 ?? throw new ArgumentNullException(nameof(parameter2));
        }

        /// <summary>
        ///     Blocker may have different sources
        ///     Type are used to type the blocker
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        ///     Blocker have two parameters to store information
        /// </summary>
        public object Parameter { get; set; }

        /// <summary>
        ///     Blocker have two parameters to store information
        /// </summary>
        public object Parameter2 { get; set; }

        /// <summary>
        ///     Step of creation of the blocker
        /// </summary>
        public ushort InitialStep { get; set; }

        /// <summary>
        ///     Agent may try several times to unblock a blocker
        ///     Last step the blocker has try to be recovered
        /// </summary>
        public ushort LastRecoverStep { get; set; }

        /// <summary>
        ///     Agent may try several times to unblock a blocker
        ///     Number of trials to recover this blocker
        ///     Used in case there is a limit number of tries after which the blocker is removed
        /// </summary>
        public byte NumberOfTries { get; set; }

        public bool Equals(int type, ushort lastRecoverStep)
        {
            return LastRecoverStep == lastRecoverStep && Type == type;
        }

        public bool Equals(ushort lastRecoverStep)
        {
            return LastRecoverStep == lastRecoverStep;
        }

        public bool Equals(int type)
        {
            return Type == type;
        }

        /// <summary>
        ///     Update last recover step and number of tries
        /// </summary>
        /// <param name="step"></param>
        public void Update(ushort step)
        {
            LastRecoverStep = step;
            NumberOfTries++;
        }

        public override bool Equals(object obj)
        {
            return obj is Blocker blocker
                   && blocker.Type == Type
                   && blocker.Parameter == Parameter
                   && blocker.Parameter2 == Parameter2
                   && blocker.InitialStep == InitialStep;
        }
    }
}