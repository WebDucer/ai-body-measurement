using System.Text;
using System.Text.Json;
using BodyMeasurement.Models;

namespace BodyMeasurement.Services;

/// <summary>
/// Implementation of export service for weight data
/// </summary>
public class ExportService : IExportService
{
    /// <summary>
    /// Exports weight entries to CSV format
    /// </summary>
    public async Task<string> ExportToCsvAsync(List<WeightEntry> entries, string language = "en")
    {
        var csv = new StringBuilder();

        // Add headers based on language
        if (language == "de")
        {
            csv.AppendLine("Datum,Gewicht (kg),Gewicht (lbs),Notizen");
        }
        else
        {
            csv.AppendLine("Date,Weight (kg),Weight (lbs),Notes");
        }

        // Add data rows
        foreach (var entry in entries.OrderBy(e => e.Date))
        {
            var weightLbs = WeightConverter.KgToLbs(entry.WeightKg);
            var notes = EscapeCsvField(entry.Notes ?? string.Empty);
            
            csv.AppendLine($"{entry.Date:yyyy-MM-dd},{entry.WeightKg:F1},{weightLbs:F1},{notes}");
        }

        // Save to file
        var fileName = $"weight-data-{DateTime.Now:yyyy-MM-dd}.csv";
        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
        
        await File.WriteAllTextAsync(filePath, csv.ToString());
        
        return filePath;
    }

    /// <summary>
    /// Exports weight entries to JSON format
    /// </summary>
    public async Task<string> ExportToJsonAsync(List<WeightEntry> entries)
    {
        var exportData = new
        {
            exportDate = DateTime.UtcNow,
            totalEntries = entries.Count,
            entries = entries.OrderBy(e => e.Date).Select(e => new
            {
                date = e.Date,
                weightKg = e.WeightKg,
                weightLbs = WeightConverter.KgToLbs(e.WeightKg),
                notes = e.Notes,
                createdAt = e.CreatedAt
            }).ToList()
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(exportData, options);

        // Save to file
        var fileName = $"weight-data-{DateTime.Now:yyyy-MM-dd}.json";
        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
        
        await File.WriteAllTextAsync(filePath, json);
        
        return filePath;
    }

    /// <summary>
    /// Shares a file using the platform's share sheet
    /// </summary>
    public async Task<bool> ShareFileAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Share Weight Data",
                File = new ShareFile(filePath)
            });

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Escapes CSV field to handle special characters
    /// </summary>
    private string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field))
        {
            return string.Empty;
        }

        // If field contains comma, quote, or newline, wrap in quotes and escape quotes
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }

        return field;
    }
}
