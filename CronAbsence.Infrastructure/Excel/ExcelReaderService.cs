using System.IO;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace CronAbsence.Infrastructure.Excel
{
    public class ExcelReaderService : IExcelReaderService
    {
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
    }
}
