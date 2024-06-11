using System.Data;
using CronAbsence.Domain.Interfaces;

namespace CronAbsence.Infrastructure.Service.Excel
{
    public class ExcelReaderService : IExcelReaderService
    {
        public async Task<DataTable> ReadDataAsync(string csvFilePath)
        {
            DataTable dataTable = new DataTable();

            try
            {
                if (!File.Exists(csvFilePath))
                {
                    throw new FileNotFoundException("CSV file not found at the specified path.", csvFilePath);
                }

                using (var reader = new StreamReader(csvFilePath))
                {
                    string line;
                    bool isFirstLine = true;

                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(';');

                        if (isFirstLine)
                        {
                            foreach (string header in parts)
                            {
                                dataTable.Columns.Add(header.Trim());
                            }

                            isFirstLine = false;
                        }
                        else
                        {
                            DataRow dataRow = dataTable.NewRow();

                            for (int i = 0; i < parts.Length; i++)
                            {
                                dataRow[i] = parts[i].Trim();
                            }

                            dataTable.Rows.Add(dataRow);
                        }
                    }
                }

                return dataTable;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading CSV file: {ex.Message}");
                throw;
            }
        }
    }
}
