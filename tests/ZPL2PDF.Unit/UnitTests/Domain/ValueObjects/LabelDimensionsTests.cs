using FluentAssertions;
using Xunit;
// Avoid collision with ZPL2PDF.LabelDimensions (Shared / dimension extractor DTO).
using DomainLabelDimensions = ZPL2PDF.Domain.ValueObjects.LabelDimensions;

namespace ZPL2PDF.Tests.UnitTests.Domain.ValueObjects
{
    /// <summary>
    /// Unit tests for <see cref="ZPL2PDF.Domain.ValueObjects.LabelDimensions"/>.
    /// </summary>
    public class LabelDimensionsTests
    {
        [Fact]
        public void DefaultConstructor_SetsReasonableDefaults()
        {
            var d = new DomainLabelDimensions();

            d.Width.Should().Be(0);
            d.Height.Should().Be(0);
            d.Unit.Should().Be("mm");
            d.Dpi.Should().Be(203);
        }

        [Fact]
        public void Constructor_WithValidParameters_SetsProperties()
        {
            var d = new DomainLabelDimensions(100, 50, "cm", 300);

            d.Width.Should().Be(100);
            d.Height.Should().Be(50);
            d.Unit.Should().Be("cm");
            d.Dpi.Should().Be(300);
        }

        [Theory]
        [InlineData(10, 20, "mm", 203)]
        [InlineData(1, 2, "cm", 203)]
        [InlineData(4, 2, "in", 300)]
        public void IsValid_WithPositiveDimensionsAndKnownUnit_ReturnsTrue(
            double w, double h, string unit, int dpi)
        {
            var d = new DomainLabelDimensions(w, h, unit, dpi);
            d.IsValid().Should().BeTrue();
            d.GetValidationError().Should().BeEmpty();
        }

        [Fact]
        public void IsValid_WithZeroWidth_ReturnsFalse()
        {
            var d = new DomainLabelDimensions(0, 20, "mm", 203);
            d.IsValid().Should().BeFalse();
            d.GetValidationError().Should().Contain("Width");
        }

        [Fact]
        public void IsValid_WithZeroHeight_ReturnsFalse()
        {
            var d = new DomainLabelDimensions(10, 0, "mm", 203);
            d.IsValid().Should().BeFalse();
            d.GetValidationError().Should().Contain("Height");
        }

        [Fact]
        public void IsValid_WithEmptyUnit_ReturnsFalse()
        {
            var d = new DomainLabelDimensions(10, 20, "  ", 203);
            d.IsValid().Should().BeFalse();
            d.GetValidationError().Should().Contain("Unit");
        }

        [Fact]
        public void IsValid_WithZeroDpi_ReturnsFalse()
        {
            var d = new DomainLabelDimensions(10, 20, "mm", 0);
            d.IsValid().Should().BeFalse();
            d.GetValidationError().Should().Contain("DPI");
        }

        [Fact]
        public void IsValid_WithInvalidUnit_ReturnsFalse()
        {
            var d = new DomainLabelDimensions(10, 20, "px", 203);
            d.IsValid().Should().BeFalse();
            d.GetValidationError().Should().Contain("Invalid unit");
        }

        [Theory]
        [InlineData("mm", true)]
        [InlineData("MM", true)]
        [InlineData("cm", true)]
        [InlineData("in", true)]
        [InlineData("px", false)]
        [InlineData("", false)]
        [InlineData("   ", false)]
        public void IsValidUnit_MatchesExpected(string unit, bool expected)
        {
            DomainLabelDimensions.IsValidUnit(unit).Should().Be(expected);
        }

        [Fact]
        public void ToMillimeters_FromMm_ReturnsEquivalentValues()
        {
            var d = new DomainLabelDimensions(12.5, 8, "mm", 203);
            var mm = d.ToMillimeters();

            mm.Width.Should().BeApproximately(12.5, 0.001);
            mm.Height.Should().BeApproximately(8, 0.001);
            mm.Unit.Should().Be("mm");
            mm.Dpi.Should().Be(203);
        }

        [Fact]
        public void ToMillimeters_FromCm_ConvertsCorrectly()
        {
            var d = new DomainLabelDimensions(2, 3, "cm", 203);
            var mm = d.ToMillimeters();

            mm.Width.Should().BeApproximately(20, 0.001);
            mm.Height.Should().BeApproximately(30, 0.001);
            mm.Unit.Should().Be("mm");
        }

        [Fact]
        public void ToCentimeters_FromMm_ConvertsCorrectly()
        {
            var d = new DomainLabelDimensions(100, 50, "mm", 203);
            var cm = d.ToCentimeters();

            cm.Unit.Should().Be("cm");
            cm.Width.Should().BeApproximately(10, 0.001);
            cm.Height.Should().BeApproximately(5, 0.001);
        }

        [Fact]
        public void ToInches_FromMm_ConvertsCorrectly()
        {
            var d = new DomainLabelDimensions(25.4, 50.8, "mm", 203);
            var inches = d.ToInches();

            inches.Unit.Should().Be("in");
            inches.Width.Should().BeApproximately(1, 0.01);
            inches.Height.Should().BeApproximately(2, 0.01);
        }

        [Fact]
        public void ToPoints_UsesDpiAndMillimeters()
        {
            // 25.4 mm @ 203 dpi => 1 inch => 203 points width
            var d = new DomainLabelDimensions(25.4, 25.4, "mm", 203);
            (int w, int h) = d.ToPoints();

            w.Should().Be(203);
            h.Should().Be(203);
        }

        [Fact]
        public void Clone_ReturnsEqualButDistinctInstance()
        {
            var original = new DomainLabelDimensions(7, 8, "in", 300);
            var copy = original.Clone();

            copy.Should().NotBeSameAs(original);
            copy.Should().Be(original);
        }

        [Fact]
        public void Equals_WithSameValues_ReturnsTrue()
        {
            var a = new DomainLabelDimensions(10, 20, "MM", 203);
            var b = new DomainLabelDimensions(10, 20, "mm", 203);

            a.Equals(b).Should().BeTrue();
            (a == b).Should().BeFalse(); // no operator overload
        }

        [Fact]
        public void Equals_WithDifferentValues_ReturnsFalse()
        {
            var a = new DomainLabelDimensions(10, 20, "mm", 203);
            var b = new DomainLabelDimensions(11, 20, "mm", 203);

            a.Equals(b).Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_WithSameValues_ReturnsSameHash()
        {
            var a = new DomainLabelDimensions(10, 20, "mm", 203);
            var b = new DomainLabelDimensions(10, 20, "MM", 203);

            a.GetHashCode().Should().Be(b.GetHashCode());
        }

        [Fact]
        public void ToString_IncludesDimensionsAndUnit()
        {
            var d = new DomainLabelDimensions(100, 50, "mm", 203);
            var s = d.ToString();

            s.Should().Contain("100");
            s.Should().Contain("50");
            s.Should().Contain("mm");
        }
    }
}
