#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;

#endregion

namespace Symu.Environment
{
    /// <summary>
    ///     The Schedule is your simulation’s representation of time.
    ///     It is a discrete event Schedule.
    ///     Schedule is used to manage time depending on the TimeStepType
    /// </summary>
    public class Schedule
    {
        public const byte ConstWeek = 7;
        public const byte ConstMonth = 30;
        public const byte ConstQuarter = 90;
        public const ushort ConstYear = 365;
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
                    case TimeStepType.Intraday:
                    case TimeStepType.Daily:
                        return (byte) Math.Floor((float) Step / ConstYear);
                    case TimeStepType.Yearly:
                        return (byte) Step;
                    default:
                        throw new ArgumentOutOfRangeException();
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
                    case TimeStepType.Intraday:
                    case TimeStepType.Daily:
                        return Step;
                    case TimeStepType.Weekly:
                        return (ushort) (Step * ConstWeek);
                    case TimeStepType.Monthly:
                        return (ushort) (Step * ConstMonth);
                    case TimeStepType.Yearly:
                        return (ushort) (Step * ConstYear);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static bool SetIsEndOfWeek(ushort day)
        {
            return day % ConstWeek == 0;
        }

        private static bool SetIsEndOfMonth(ushort day)
        {
            return day % ConstMonth == 0;
        }

        private static bool SetIsEndOfQuarter(ushort day)
        {
            return day % ConstQuarter == 0;
        }

        private static bool SetIsEndOfYear(ushort day)
        {
            return day % ConstYear == 0;
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