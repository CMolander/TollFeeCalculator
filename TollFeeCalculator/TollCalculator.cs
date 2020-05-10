using System;
using System.Collections.Generic;
using System.Linq;
using TollFeeCalculator.VehicleTypes;

namespace TollFeeCalculator
{
    public class TollCalculator
    {
        /**
     * Calculate the total toll fee for one day
     *
     * @param vehicle - the vehicle
     * @param dates   - date and time of all passes on one day
     * @return - the total toll fee for that day
     */

        public int GetTollFee(IVehicle vehicle, DateTime[] dates)
        {
            var totalFee = 0;
            const int maximumTollFee = 60;
            const int maximumDiffMinutes = 60;

            for (int i = 0; i < dates.Length; i++)
            {
                var fee = GetTollFee(dates[i], vehicle);

                var isNumberOfPassagesMoreThanOne = i >= 1;

                if (isNumberOfPassagesMoreThanOne)
                {
                    var diffMinutes = (dates[i] - dates[i - 1]).TotalMinutes;

                    var previousFee = diffMinutes <= maximumDiffMinutes ? GetTollFee(dates[i - 1], vehicle) : 0;

                    totalFee = fee > previousFee ? totalFee + (fee - previousFee) : totalFee;
                }
                else
                {
                    totalFee = fee;
                }
            }

            return totalFee > maximumTollFee ? maximumTollFee : totalFee;
        }

        private bool IsTollFreeVehicle(IVehicle vehicle)
        {
            if (vehicle == null) return false;
            var vehicleType = vehicle.GetVehicleType();
            return vehicleType.Equals(TollFreeVehicles.Motorbike.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Tractor.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Emergency.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Diplomat.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Foreign.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Military.ToString());
        }

        private int GetTollFee(DateTime date, IVehicle vehicle)
        {
            if (IsTollFreeDate(date) || IsTollFreeVehicle(vehicle)) return 0;

            var time = date.TimeOfDay;

            var tollFee8TimeIntervals = new List<TimeSpan[]>
            {
                new[] {new TimeSpan(6, 0, 0), new TimeSpan(6, 29, 0)},
                new[] {new TimeSpan(8, 30, 0), new TimeSpan(14, 59, 0)},
                new[] {new TimeSpan(18, 0, 0), new TimeSpan(18, 29, 0)}
            };

            var tollFee13TimeIntervals = new List<TimeSpan[]>
            {
                new[] {new TimeSpan(6, 30, 0), new TimeSpan(6, 59, 0)},
                new[] {new TimeSpan(8, 0, 0), new TimeSpan(8, 29, 0)},
                new[] {new TimeSpan(15, 0, 0), new TimeSpan(15, 29, 0)},
                new[] {new TimeSpan(17, 0, 0), new TimeSpan(17, 59, 0)}
            };

            var tollFee18TimeIntervals = new List<TimeSpan[]>
            {
                new[] {new TimeSpan(7, 0, 0), new TimeSpan(7, 59, 0)},
                new[] {new TimeSpan(15, 30, 0), new TimeSpan(16, 59, 0)}
            };

            if (IsBetweenTimeRanges(time, tollFee8TimeIntervals))
            {
                return 8;
            }

            if (IsBetweenTimeRanges(time, tollFee13TimeIntervals))
            {
                return 13;
            }

            if (IsBetweenTimeRanges(time, tollFee18TimeIntervals))
            {
                return 18;
            }

            return 0;
        }

        private bool IsBetweenTimeRanges(TimeSpan time, IEnumerable<TimeSpan[]> timeIntervals)
        {
            return timeIntervals.Any(timeInterval => timeInterval[0] <= time && time <= timeInterval[1]);
        }

        private bool IsTollFreeDate(DateTime date)
        {
            var year = date.Year;
            var month = date.Month;
            var day = date.Day;

            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) return true;

            if (year == 2013)
            {
                if (month == 1 && day == 1 ||
                    month == 3 && (day == 28 || day == 29) ||
                    month == 4 && (day == 1 || day == 30) ||
                    month == 5 && (day == 1 || day == 8 || day == 9) ||
                    month == 6 && (day == 5 || day == 6 || day == 21) ||
                    month == 7 ||
                    month == 11 && day == 1 ||
                    month == 12 && (day == 24 || day == 25 || day == 26 || day == 31))
                {
                    return true;
                }
            }

            if (year == 2019)
            {
                if (month == 1 && day == 1 ||
                    month == 3 && (day == 29 || day == 30) ||
                    month == 4 && (day == 2 || day == 30) ||
                    month == 5 && (day == 1 || day == 29 || day == 30) ||
                    month == 6 && (day == 5 || day == 6 || day == 21) ||
                    month == 7 ||
                    month == 11 && day == 1 ||
                    month == 12 && (day == 24 || day == 25 || day == 26 || day == 31))
                {
                    return true;
                }
            }

            if (year == 2020)
            {
                if (month == 1 && day == 1 ||
                    month == 3 && (day == 28 || day == 29) ||
                    month == 4 && (day == 9 || day == 10 || day == 13 || day == 30) ||
                    month == 5 && (day == 1 || day == 20 || day == 21) ||
                    month == 6 && (day == 5 || day == 19) ||
                    month == 7 ||
                    month == 12 && (day == 24 || day == 25 || day == 26 || day == 31))
                {
                    return true;
                }
            }
            return false;
        }

        private enum TollFreeVehicles
        {
            Motorbike = 0,
            Tractor = 1,
            Emergency = 2,
            Diplomat = 3,
            Foreign = 4,
            Military = 5
        }
    }
}