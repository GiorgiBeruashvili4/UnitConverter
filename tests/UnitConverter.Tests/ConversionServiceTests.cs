using UnitConverter.Api.Services;
using Xunit;

namespace UnitConverter.Tests;

public sealed class ConversionServiceTests
{
    private readonly ConversionService _service = new();

    [Fact]
    public void Convert_MetersToKilometers_ReturnsExpectedValue()
    {
        var result = _service.Convert(1500, "Length", "Meter (m)", "Kilometer (km)");
        Assert.Equal(1.5, result.Result, 10);
    }

    [Fact]
    public void Convert_CelsiusToFahrenheit_ReturnsExpectedValue()
    {
        var result = _service.Convert(100, "Temperature", "Celsius (°C)", "Fahrenheit (°F)");
        Assert.Equal(212, result.Result, 10);
    }

    [Fact]
    public void Convert_KilogramsToPounds_ReturnsExpectedValue()
    {
        var result = _service.Convert(1, "Weight", "Kilogram (kg)", "Pound (lb)");
        Assert.Equal(2.2046226218, result.Result, 8);
    }

    [Fact]
    public void Convert_UnknownCategory_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.Convert(1, "Volume", "Liter", "Milliliter"));
    }

    [Fact]
    public void Convert_NonFiniteValue_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _service.Convert(double.NaN, "Length", "Meter (m)", "Foot (ft)"));
    }
}
