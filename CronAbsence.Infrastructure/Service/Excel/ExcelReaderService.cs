using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using FluentFTP;
using OfficeOpenXml;

namespace CronAbsence.Infrastructure.Service.Excel
{
    public class ExcelReaderService : IExcelReaderService
    {
        public async Task<DataTable> ReadDataAsync(string ftpServer, string ftpUsername, string ftpPassword, string remoteFilePath, string localFilePath)
        {
            DataTable dataTable = new DataTable();

            using (var ftp = new FtpClient(ftpServer, ftpUsername, ftpPassword))
            {
                ftp.Connect();

                // Download the Excel file to the local file path
                ftp.DownloadFile(localFilePath, remoteFilePath);

                // Read data from the downloaded file
                using (var package = new ExcelPackage(new FileInfo(localFilePath)))
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

                // Disconnect from the FTP server
                ftp.Disconnect();
            }

            return dataTable;
        }
    }
}
