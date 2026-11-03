using UnitConverter.Api.Models;

namespace UnitConverter.Api.Services;

public sealed class ConversionService
{
    private static readonly Dictionary<string, Dictionary<string, double>> LinearUnits =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Length"] = new(StringComparer.OrdinalIgnoreCase)
            {
                ["Millimeter (mm)"] = 0.001,
                ["Centimeter (cm)"] = 0.01,
                ["Meter (m)"] = 1,
                ["Kilometer (km)"] = 1000,
                ["Inch (in)"] = 0.0254,
                ["Foot (ft)"] = 0.3048,
                ["Mile (mi)"] = 1609.344
            },
            ["Weight"] = new(StringComparer.OrdinalIgnoreCase)
            {
                ["Milligram (mg)"] = 0.000001,
                ["Gram (g)"] = 0.001,
                ["Kilogram (kg)"] = 1,
                ["Metric ton (t)"] = 1000,
                ["Ounce (oz)"] = 0.028349523125,
                ["Pound (lb)"] = 0.45359237
            },
            ["Time"] = new(StringComparer.OrdinalIgnoreCase)
            {
                ["Millisecond (ms)"] = 0.001,
                ["Second (s)"] = 1,
                ["Minute (min)"] = 60,
                ["Hour (h)"] = 3600,
                ["Day (d)"] = 86400,
                ["Week (wk)"] = 604800
            },
            ["Data"] = new(StringComparer.OrdinalIgnoreCase)
            {
                ["Byte (B)"] = 1,
                ["Kilobyte (KB)"] = 1000,
                ["Megabyte (MB)"] = 1_000_000,
                ["Gigabyte (GB)"] = 1_000_000_000,
                ["Kibibyte (KiB)"] = 1024,
                ["Mebibyte (MiB)"] = 1_048_576,
                ["Gibibyte (GiB)"] = 1_073_741_824
            }
        };

    public IReadOnlyCollection<string> Categories => LinearUnits.Keys
        .Append("Temperature").ToArray();

    public IReadOnlyList<string> GetUnits(string category)
    {
        if (category.Equals("Temperature", StringComparison.OrdinalIgnoreCase))
            return ["Celsius (°C)", "Fahrenheit (°F)", "Kelvin (K)"];

        if (LinearUnits.TryGetValue(category, out var units))
            return units.Keys.ToArray();

        throw new ArgumentException($"Unknown category: {category}", nameof(category));
    }

    public ConversionResult Convert(double value, string category, string fromUnit, string toUnit)
    {
        if (!double.IsFinite(value))
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be a finite number.");

        if (category.Equals("Temperature", StringComparison.OrdinalIgnoreCase))
        {
            var celsius = ToCelsius(value, fromUnit);
            var result = FromCelsius(celsius, toUnit);
            return new ConversionResult(value, fromUnit, result, toUnit, "Temperature");
        }

        if (!LinearUnits.TryGetValue(category, out var units))
            throw new ArgumentException($"Unknown category: {category}", nameof(category));

        if (!units.TryGetValue(fromUnit, out var fromFactor))
            throw new ArgumentException($"Unknown source unit: {fromUnit}", nameof(fromUnit));
        if (!units.TryGetValue(toUnit, out var toFactor))
            throw new ArgumentException($"Unknown target unit: {toUnit}", nameof(toUnit));

        var converted = value * fromFactor / toFactor;
        return new ConversionResult(value, fromUnit, converted, toUnit, category);
    }

    private static double ToCelsius(double value, string unit) => unit switch
    {
        "Celsius (°C)" => value,
        "Fahrenheit (°F)" => (value - 32) * 5 / 9,
        "Kelvin (K)" => value - 273.15,
        _ => throw new ArgumentException($"Unknown temperature unit: {unit}", nameof(unit))
    };

    private static double FromCelsius(double celsius, string unit) => unit switch
    {
        "Celsius (°C)" => celsius,
        "Fahrenheit (°F)" => celsius * 9 / 5 + 32,
        "Kelvin (K)" => celsius + 273.15,
        _ => throw new ArgumentException($"Unknown temperature unit: {unit}", nameof(unit))
    };
}
