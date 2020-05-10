using System;
using System.Collections.Generic;
using System.Linq;
using Nager.Date;
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
            return date.Month == 7 || date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday || IsPublicHoliday(date);
        }

        private bool IsPublicHoliday(DateTime date)
        {
            var publicHolidays = DateSystem.GetPublicHoliday(date.Year, CountryCode.SE).ToList();

            // The date before a public holiday is a toll free date, except from these ones.
            var excludedDays = new List<string>
            {
                "Midsommarafton", "Julafton", "Nyårsafton"
            };

            var datesBeforePublicHoliday = publicHolidays.Where(x => !excludedDays.Contains(x.LocalName)).Select(x => x.Date.AddDays(-1)).ToList();

            var tollFreeDates = publicHolidays.Select(x => x.Date).Concat(datesBeforePublicHoliday).ToList();

            return tollFreeDates.Contains(date.Date);
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