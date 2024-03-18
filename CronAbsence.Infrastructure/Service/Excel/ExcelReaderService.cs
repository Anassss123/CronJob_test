using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace CronAbsence.Infrastructure.Service.Excel
{
    public class ExcelReaderService : IExcelReaderService
    {
        public DataTable ReadData(FileInfo file)
        {
            DataTable dataTable = new DataTable();

            using (var package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                // Add columns to the DataTable based on the number of columns in the Excel file
                for (int col = 1; col <= colCount; col++)
                {
                    dataTable.Columns.Add($"Column {col}");
                }

                // Read data from Excel file and populate the DataTable
                for (int row = 1; row <= rowCount; row++)
                {
                    DataRow dataRow = dataTable.NewRow();

                    for (int col = 1; col <= colCount; col++)
                    {
                        dataRow[col - 1] = worksheet.Cells[row, col].Value?.ToString() ?? "NULL";
                    }

                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }

        public async Task<object[,]> ReadDataAsync(FileInfo file)
        {
            using (var package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                object[,] data = new object[rowCount, colCount];

                for (int row = 1; row <= rowCount; row++)
                {
                    for (int col = 1; col <= colCount; col++)
                    {
                        data[row - 1, col - 1] = worksheet.Cells[row, col].Value;
                    }
                }

                return data;
            }
        }
        public void DisplayDataAsync(DataTable dataTable)
        {
             foreach (DataRow row in dataTable.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    Console.Write(item.ToString() + "\t");
                }
                Console.WriteLine();
            }
        }
    }
}
