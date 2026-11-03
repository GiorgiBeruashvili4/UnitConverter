namespace UnitConverter.Api.Models;

public sealed record ConversionResult(
    double InputValue,
    string FromUnit,
    double Result,
    string ToUnit,
    string Category);
