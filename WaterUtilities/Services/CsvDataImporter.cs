using System.Globalization;
using ILogger = Serilog.ILogger;
using CsvHelper;
using Elsa.Workflows.Runtime;
using WaterUtilities.Models;

namespace WaterUtilities.Services;

public interface ICsvDataImporter
{
    public IEnumerable<dynamic> LoadDynamicCsvData(string filePath);
    public IEnumerable<dynamic> LoadAquaPPCsvData(string filePath);
}

public class CsvDataImporter : ICsvDataImporter
{
    private readonly ILogger _logger;

    public CsvDataImporter(ILogger logger)
    {
        _logger = logger;
        _logger.Debug("Initialized CsvDataImporter service");
    }

    public IEnumerable<dynamic> LoadDynamicCsvData(string filePath)
    {
        try
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<dynamic>();
            return records;
        }
        catch (ArgumentException ex)
        {
            _logger.Error("File path is empty, {ex}", ex);
            return [];
        }
        catch (FileNotFoundException)
        {
            _logger.Error("File not found");
            return [];
        }
        catch (DirectoryNotFoundException)
        {
            _logger.Error("Directory not found");
            return [];
        }
        catch (CsvHelperException ex)
        {
            _logger.Error("CsvDataImporter could not load dynamic csv data, {ex.Message}", ex.Message);
            return [];
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error when parsing CsvData");
            return [];
        }
    }


    public IEnumerable<dynamic> LoadAquaPPCsvData(string filePath)
    {
        try
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<WaterQualityData>();
            return records;
        }
        catch (ArgumentException ex)
        {
            _logger.Error("File path is empty, {ex}", ex);
            return [];
        }
        catch (FileNotFoundException)
        {
            _logger.Error("File not found");
            return [];
        }
        catch (DirectoryNotFoundException)
        {
            _logger.Error("Directory not found");
            return [];
        }
        catch (CsvHelperException ex)
        {
            _logger.Error("CsvDataImporter could not load dynamic csv data, {ex.Message}", ex.Message);
            return [];
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error when parsing CsvData");
            return [];
        }
    }
    
}