using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UnitConverter.Api.Models;
using UnitConverter.Api.Services;

namespace UnitConverter.Api.Pages;

public sealed class IndexModel(ConversionService conversionService) : PageModel
{
    [BindProperty] public string Category { get; set; } = "Length";
    [BindProperty] public double InputValue { get; set; } = 1;
    [BindProperty] public string FromUnit { get; set; } = "Meter (m)";
    [BindProperty] public string ToUnit { get; set; } = "Kilometer (km)";

    public IReadOnlyCollection<string> Categories { get; private set; } = [];
    public IReadOnlyList<string> Units { get; private set; } = [];
    public ConversionResult? Result { get; private set; }
    public string? ErrorMessage { get; private set; }

    public void OnGet() => LoadOptions();

    public void OnPost()
    {
        LoadOptions();
        try
        {
            Result = conversionService.Convert(InputValue, Category, FromUnit, ToUnit);
        }
        catch (ArgumentException ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    private void LoadOptions()
    {
        Categories = conversionService.Categories;
        try { Units = conversionService.GetUnits(Category); }
        catch (ArgumentException) { Category = "Length"; Units = conversionService.GetUnits(Category); }
        if (!Units.Contains(FromUnit)) FromUnit = Units[0];
        if (!Units.Contains(ToUnit)) ToUnit = Units[Math.Min(1, Units.Count - 1)];
    }
}
