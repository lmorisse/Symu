#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;

#endregion

namespace SymuEngine.Environment.TimeStep
{
    /// <summary>
    ///     TimeStep is used to manage TimeStep depending on the TimeStepType
    /// </summary>
    public class TimeStep
    {
        public const byte constWeek = 7;
        public const byte constMonth = 30;
        public const byte constQuarter = 90;
        public const ushort constYear = 365;
        private ushort _step;

        public TimeStepType Type { get; set; } = TimeStepType.Daily;
        public bool IsWorkingDay { get; private set; }
        public bool IsEndOfWeek { get; private set; }
        public bool IsEndOfQuarter { get; private set; }
        public bool IsEndOfMonth { get; private set; }
        public bool IsEndOfYear { get; private set; }

        public ushort Step
        {
            get => _step;
            set
            {
                _step = value;
                var day = Day;
                IsWorkingDay = SetIsWorkingDay(day);
                IsEndOfWeek = SetIsEndOfWeek(day);
                IsEndOfMonth = SetIsEndOfMonth(day);
                IsEndOfQuarter = SetIsEndOfQuarter(day);
                IsEndOfYear = SetIsEndOfYear(day);
            }
        }

        /// <summary>
        ///     Give the day corresponding to the Step given the TimeSTepType
        ///     Prefer using Step when possible
        /// </summary>
        public byte Year
        {
            get
            {
                switch (Type)
                {
                    case TimeStepType.Weekly:
                        return (byte) Math.Floor(Step / 52.0F);
                    case TimeStepType.Monthly:
                        return (byte) Math.Floor(Step / 12.0F);
                    case TimeStepType.Daily:
                        return (byte) Math.Floor((float) Step / constYear);
                    //case TimeStepType.Yearly:
                    default:
                        return (byte) Step;
                }
            }
        }

        /// <summary>
        ///     Give the year corresponding to the Step given the TimeSTepType
        ///     Prefer using Step when possible
        /// </summary>
        public ushort Day
        {
            get
            {
                switch (Type)
                {
                    case TimeStepType.Weekly:
                        return (ushort) (Step * constWeek);
                    case TimeStepType.Monthly:
                        return (ushort) (Step * constMonth);
                    case TimeStepType.Yearly:
                        return (ushort) (Step * constYear);
                    //case TimeStepType.Intraday:
                    //case TimeStepType.Yearly:
                    default:
                        return Step;
                }
            }
        }

        private static bool SetIsEndOfWeek(ushort day)
        {
            return day % constWeek == 0;
        }

        private static bool SetIsEndOfMonth(ushort day)
        {
            return day % constMonth == 0;
        }

        private static bool SetIsEndOfQuarter(ushort day)
        {
            return day % constQuarter == 0;
        }

        private static bool SetIsEndOfYear(ushort day)
        {
            return day % constYear == 0;
        }

        public static bool SetIsWorkingDay(ushort day)
        {
            return ConvertDoubleToDateTime(day).DayOfWeek != DayOfWeek.Saturday &&
                   ConvertDoubleToDateTime(day).DayOfWeek != DayOfWeek.Sunday;
        }

        public static DateTime ConvertDoubleToDateTime(double days)
        {
            return new DateTime(new TimeSpan(Convert.ToInt32(Math.Floor(days)), 0, 0, 0).Ticks);
        }
    }
}