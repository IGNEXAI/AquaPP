using ILogger = Serilog.ILogger;
using ExcelDataReader;

namespace WaterUtilities.Services;

public interface IExcelDataImporter
{
    
}

public class ExcelDataImporter
{
    private string _filePath;
    private readonly ILogger _logger;

    public ExcelDataImporter(ILogger logger)
    {
        _logger = logger;
        _logger.Debug("Initialized CsvDataImporter service");
    }

    public void LoadExcelData(string filePath)
    {
        _filePath = filePath;

    }
}