using System;
using System.Collections.Generic;
using TollFeeCalculator;
using Xunit;

namespace Test.Unit.TollFeeCalculator
{
    public class TollCalculatorTest
    {
        public static IEnumerable<object[]> TollFee8DateTimes => new[]
        {
            new object[] { new[] { new DateTime(2020, 04, 23, 6, 0, 0) }},
            new object[] { new[] { new DateTime(2020, 04, 23, 6, 29, 0) }},
            new object[] { new[] { new DateTime(2020, 04, 23, 8, 30, 0) }},
            new object[] { new[] { new DateTime(2020, 04, 23, 12, 00, 0) }},
            new object[] { new[] { new DateTime(2020, 04, 23, 14, 59, 0) }},
            new object[] { new[] { new DateTime(2020, 04, 23, 18, 00, 0) }},
            new object[] { new[] { new DateTime(2020, 04, 23, 18, 29, 0) }}
        };

        public static IEnumerable<object[]> TollFee13DateTimes => new[]
        {
            new object[] { new[] { new DateTime(2020, 04, 23, 6, 30, 0) }},
            new object[] { new[] { new DateTime(2020, 04, 23, 6, 59, 0) }},
            new object[] { new[] { new DateTime(2020, 04, 23, 8, 00, 0) }},
            new object[] { new[] { new DateTime(2020, 04, 23, 8, 29, 0) }},
            new object[] { new[] { new DateTime(2020, 04, 23, 15, 00, 0) }},
            new object[] { new[] { new DateTime(2020, 04, 23, 15, 29, 0) }},
            new object[] { new[] { new DateTime(2020, 04, 23, 17, 00, 0) }},
            new object[] { new[] { new DateTime(2020, 04, 23, 17, 59, 0) }}
        };

        public static IEnumerable<object[]> TollFee18DateTimes => new[]
        {
            new object[] { new[] { new DateTime(2020, 04, 23, 7, 00, 0) }},
            new object[] { new[] { new DateTime(2020, 04, 23, 7, 59, 0) }},
            new object[] { new[] { new DateTime(2020, 04, 23, 15, 30, 0) }},
            new object[] { new[] { new DateTime(2020, 04, 23, 16, 59, 0) }}
        };

        public static IEnumerable<object[]> TollFreeDateTimes => new[]
        {
            new object[] { new[] { new DateTime(2020, 04, 23, 5, 59, 0) }},
            new object[] { new[] { new DateTime(2020, 04, 23, 18, 30, 0) }},
            new object[] { new[] { new DateTime(2020, 04, 23, 21, 00, 0) }}
        };

        public static IEnumerable<object[]> TollFreeDates => new[]
        {
            new object[] { new[] { new DateTime(2019, 07, 23, 15, 00, 0) }},
            new object[] { new[] { new DateTime(2013, 01, 01, 10, 00, 0) }},
            new object[] { new[] { new DateTime(2020, 12, 24, 7, 00, 0) }},
            new object[] { new[] { new DateTime(2020, 05, 01, 7, 00, 0) }},
            new object[] { new[] { new DateTime(2019, 06, 05, 10, 00, 0) }}
        };

        public static IEnumerable<object[]> TollFreeVehicles => new[]
        {
            new object[] { new Motorbike() }
        };

        [Theory]
        [MemberData(nameof(TollFee8DateTimes))]
        public void ShouldReturnTollFee8(DateTime[] dateTimes)
        {
            // Arrange.
            var vehicle = new Car();
            const int expectedTollFee = 8;

            // Act.
            var result = new TollCalculator().GetTollFee(vehicle, dateTimes);
                
            // Assert.
            Assert.Equal(expectedTollFee, result);
        }

        [Theory]
        [MemberData(nameof(TollFee13DateTimes))]
        public void ShouldReturnTollFee13(DateTime[] dateTimes)
        {
            // Arrange.
            var vehicle = new Car();
            const int expectedTollFee = 13;

            // Act.
            var result = new TollCalculator().GetTollFee(vehicle, dateTimes);

            // Assert.
            Assert.Equal(expectedTollFee, result);
        }

        [Theory]
        [MemberData(nameof(TollFee18DateTimes))]
        public void ShouldReturnTollFee18(DateTime[] dateTimes)
        {
            // Arrange.
            var vehicle = new Car();
            const int expectedTollFee = 18;

            // Act.
            var result = new TollCalculator().GetTollFee(vehicle, dateTimes);

            // Assert.
            Assert.Equal(expectedTollFee, result);
        }

        [Fact]
        public void ShouldReturnMaximumDailyToll()
        {
            // Arrange.
            var vehicle = new Car();
            const int expectedTollFee = 60;

            var dateTimes = new[]
            {
                new DateTime(2020, 04, 23, 6, 10, 0,10),
                new DateTime(2020, 04, 23, 7, 20, 0),
                new DateTime(2020, 04, 23, 10, 00, 0),
                new DateTime(2020, 04, 23, 13, 00, 0),
                new DateTime(2020, 04, 23, 16, 00, 0),
                new DateTime(2020, 04, 23, 18, 00, 0)
            };

            // Act.
            var result = new TollCalculator().GetTollFee(vehicle, dateTimes);

            // Assert.
            Assert.Equal(expectedTollFee, result);
        }

        [Theory]
        [MemberData(nameof(TollFreeVehicles))]
        public void ShouldReturnZeroTollForTollFreeVehicle(IVehicle tollFreeVehicle)
        {
            // Arrange.
            const int expectedTollFee = 0;

            var dateTimes = new[]
            {
                new DateTime(2020, 04, 23, 10, 0, 0),
                new DateTime(2020, 04, 23, 16, 0, 0)
            };

            // Act.
            var result = new TollCalculator().GetTollFee(tollFreeVehicle, dateTimes);

            // Assert.
            Assert.Equal(expectedTollFee, result);
        }

        [Theory]
        [MemberData(nameof(TollFreeDateTimes))]
        public void ShouldReturnZeroTollForTollFreeDateTimes(DateTime[] dateTimes)
        {
            // Arrange.
            var vehicle = new Car();
            const int expectedTollFee = 0;

            // Act.
            var result = new TollCalculator().GetTollFee(vehicle, dateTimes);

            // Assert.
            Assert.Equal(expectedTollFee, result);
        }

        [Theory]
        [MemberData(nameof(TollFreeDates))]
        public void ShouldReturnZeroTollForTollFreeDates(DateTime[] dateTimes)
        {
            // Arrange.
            var vehicle = new Car();
            const int expectedTollFee = 0;

            // Act.
            var result = new TollCalculator().GetTollFee(vehicle, dateTimes);

            // Assert.
            Assert.Equal(expectedTollFee, result);
        }

        [Fact]
        public void ShouldReturnMaximumTollWithin60Minutes()
        {
            // Arrange.
            var vehicle = new Car();
            const int expectedTollFee = 44;

            var dateTimes = new[]
            {
                new DateTime(2020, 04, 23, 08, 00, 0),
                new DateTime(2020, 04, 23, 08, 15, 0),
                new DateTime(2020, 04, 23, 15, 15, 0),
                new DateTime(2020, 04, 23, 15, 40, 0),
                new DateTime(2020, 04, 23, 17, 00, 0)
            };

            // Act.
            var result = new TollCalculator().GetTollFee(vehicle, dateTimes);

            // Assert.
            Assert.Equal(expectedTollFee, result);
        }
    }
}