#region Licence

// Description: SymuBiz - Symu
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
                IsEndOfYear = SetIsEndOfYear(_step);
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

        private bool SetIsEndOfWeek(ushort step)
        {
            switch (Type)
            {
                case TimeStepType.Intraday:
                case TimeStepType.Daily:
                    return step % ConstWeek == 0;
                case TimeStepType.Weekly:
                    return true;
                case TimeStepType.Monthly:
                case TimeStepType.Yearly:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool SetIsEndOfMonth(ushort step)
        {
            switch (Type)
            {
                case TimeStepType.Intraday:
                case TimeStepType.Daily:
                    return step % ConstMonth == 0;
                case TimeStepType.Weekly:
                    return step % 4 == 0;
                case TimeStepType.Monthly:
                    return true;
                case TimeStepType.Yearly:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool SetIsEndOfQuarter(ushort step)
        {
            switch (Type)
            {
                case TimeStepType.Intraday:
                case TimeStepType.Daily:
                    return step % ConstQuarter == 0;
                case TimeStepType.Weekly:
                    return step % 12 == 0;
                case TimeStepType.Monthly:
                    return step % 3 == 0;
                case TimeStepType.Yearly:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool SetIsEndOfYear(ushort step)
        {
            switch (Type)
            {
                case TimeStepType.Intraday:
                case TimeStepType.Daily:
                    return step % ConstYear == 0;
                case TimeStepType.Weekly:
                    return step % 52 == 0;
                case TimeStepType.Monthly:
                    return step % 12 == 0;
                case TimeStepType.Yearly:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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



        public static int FrequencyFactor(TimeStepType frequency)
        {
            var frequencyFactor = 1;
            switch (frequency)
            {
                case TimeStepType.Intraday:
                case TimeStepType.Daily:
                    frequencyFactor = ConstYear;
                    break;
                case TimeStepType.Weekly:
                    frequencyFactor = 52;
                    break;
                case TimeStepType.Monthly:
                    frequencyFactor = 12;
                    break;
                case TimeStepType.Yearly:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(frequency), frequency, null);
            }

            return frequencyFactor;
        }
    }
}