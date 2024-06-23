using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using CronAbsence.Infrastructure.Service.Excel;
using CronAbsence.Domain.Interfaces;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace CronAbsence.Tests
{
    public class ExcelReaderServiceTests
    {
        private ExcelReaderService _excelReaderService;

        [SetUp]
        public void Setup()
        {
            _excelReaderService = new ExcelReaderService();
        }

        [Test]
        public async Task ReadDataAsync_ShouldReturnDataTable_WhenCsvFileExists()
        {
            // Arrange
            string csvFilePath = Path.Combine(Path.GetTempPath(), "testFile.csv");
            string csvContent = "Column1;Column2\nValue1;Value2\nValue3;Value4";
            await File.WriteAllTextAsync(csvFilePath, csvContent);

            // Act
            DataTable result = await _excelReaderService.ReadDataAsync(csvFilePath);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(2, result.Columns.Count);
            ClassicAssert.AreEqual("Column1", result.Columns[0].ColumnName);
            ClassicAssert.AreEqual("Column2", result.Columns[1].ColumnName);
            ClassicAssert.AreEqual(2, result.Rows.Count);
            ClassicAssert.AreEqual("Value1", result.Rows[0][0]);
            ClassicAssert.AreEqual("Value2", result.Rows[0][1]);
            ClassicAssert.AreEqual("Value3", result.Rows[1][0]);
            ClassicAssert.AreEqual("Value4", result.Rows[1][1]);

            // Cleanup
            File.Delete(csvFilePath);
        }

        [Test]
        public void ReadDataAsync_ShouldThrowFileNotFoundException_WhenCsvFileDoesNotExist()
        {
            // Arrange
            string csvFilePath = Path.Combine(Path.GetTempPath(), "nonExistentFile.csv");

            // Act & Assert
            var ex = ClassicAssert.ThrowsAsync<FileNotFoundException>(async () => await _excelReaderService.ReadDataAsync(csvFilePath));
            ClassicAssert.That(ex.Message, Does.Contain("CSV file not found at the specified path."));
            ClassicAssert.AreEqual(csvFilePath, ex.FileName);
        }

        [Test]
        public async Task ReadDataAsync_ShouldHandleEmptyCsvFile()
        {
            // Arrange
            string csvFilePath = Path.Combine(Path.GetTempPath(), "emptyFile.csv");
            await File.WriteAllTextAsync(csvFilePath, string.Empty);

            // Act
            DataTable result = await _excelReaderService.ReadDataAsync(csvFilePath);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(0, result.Columns.Count);
            ClassicAssert.AreEqual(0, result.Rows.Count);

            // Cleanup
            File.Delete(csvFilePath);
        }

        [Test]
        public async Task ReadDataAsync_ShouldHandleCsvFileWithOnlyHeaders()
        {
            // Arrange
            string csvFilePath = Path.Combine(Path.GetTempPath(), "headersOnlyFile.csv");
            string csvContent = "Column1;Column2";
            await File.WriteAllTextAsync(csvFilePath, csvContent);

            // Act
            DataTable result = await _excelReaderService.ReadDataAsync(csvFilePath);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(2, result.Columns.Count);
            ClassicAssert.AreEqual("Column1", result.Columns[0].ColumnName);
            ClassicAssert.AreEqual("Column2", result.Columns[1].ColumnName);
            ClassicAssert.AreEqual(0, result.Rows.Count);

            // Cleanup
            File.Delete(csvFilePath);
        }

        [Test]
        public async Task ReadDataAsync_ShouldHandleCsvFileWithExtraSpaces()
        {
            // Arrange
            string csvFilePath = Path.Combine(Path.GetTempPath(), "extraSpacesFile.csv");
            string csvContent = "  Column1  ;  Column2  \n  Value1  ;  Value2  \n  Value3  ;  Value4  ";
            await File.WriteAllTextAsync(csvFilePath, csvContent);

            // Act
            DataTable result = await _excelReaderService.ReadDataAsync(csvFilePath);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(2, result.Columns.Count);
            ClassicAssert.AreEqual("Column1", result.Columns[0].ColumnName);
            ClassicAssert.AreEqual("Column2", result.Columns[1].ColumnName);
            ClassicAssert.AreEqual(2, result.Rows.Count);
            ClassicAssert.AreEqual("Value1", result.Rows[0][0]);
            ClassicAssert.AreEqual("Value2", result.Rows[0][1]);
            ClassicAssert.AreEqual("Value3", result.Rows[1][0]);
            ClassicAssert.AreEqual("Value4", result.Rows[1][1]);

            // Cleanup
            File.Delete(csvFilePath);
        }

        [Test]
        public async Task ReadDataAsync_ShouldHandleCsvFileWithMissingValues()
        {
            // Arrange
            string csvFilePath = Path.Combine(Path.GetTempPath(), "missingValuesFile.csv");
            string csvContent = "Column1;Column2\nValue1;\n;Value2";
            await File.WriteAllTextAsync(csvFilePath, csvContent);

            // Act
            DataTable result = await _excelReaderService.ReadDataAsync(csvFilePath);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(2, result.Columns.Count);
            ClassicAssert.AreEqual("Column1", result.Columns[0].ColumnName);
            ClassicAssert.AreEqual("Column2", result.Columns[1].ColumnName);
            ClassicAssert.AreEqual(2, result.Rows.Count);
            ClassicAssert.AreEqual("Value1", result.Rows[0][0]);
            ClassicAssert.IsTrue(result.Rows[0][1] == DBNull.Value || result.Rows[0][1].ToString() == string.Empty);
            ClassicAssert.IsTrue(result.Rows[1][0] == DBNull.Value || result.Rows[1][0].ToString() == string.Empty);
            ClassicAssert.AreEqual("Value2", result.Rows[1][1]);

            // Cleanup
            File.Delete(csvFilePath);
        }
    }
}
