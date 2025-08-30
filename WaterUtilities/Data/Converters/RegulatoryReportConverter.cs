using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WaterUtilities.Models;

namespace WaterUtilities.Data.Converters;

public class ReportDataConverter : ValueConverter<Dictionary<string, object>, string>
{
    public ReportDataConverter()
        : base(
            v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
            v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, JsonSerializerOptions.Default)!)
    {
    }
}

public class ReportStatusConverter : ValueConverter<RegulatoryReportStatus, string>
{
    public ReportStatusConverter()
        : base(
            v => v.ToString(),
            // Change from string to enum value
            v => Enum.Parse<RegulatoryReportStatus>(v)
            )
    {
    }
}