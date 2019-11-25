using System.Collections.Generic;
using System.IO;
using Abp.Dependency;
using OfficeOpenXml;

namespace NewCM.Data
{
    public class ExcelImporterBase: NewCMServiceBase, ITransientDependency
    {
        public List<List<string>> ReadFile(string FilePath, string SheetName, int startRowIndex = 2)
        {
            List<List<string>> Result = new List<List<string>>();

            FileInfo fileInfo = new FileInfo(FilePath);

            using (var excelPackage = new ExcelPackage(fileInfo))
            {
                var sheet = excelPackage.Workbook.Worksheets[SheetName];
                if (sheet == null)
                    sheet = excelPackage.Workbook.Worksheets[1];

                if (sheet != null)
                {
                    for (var rowIndex = 0; rowIndex < sheet.Dimension.Rows - 1; rowIndex++)
                    {
                        List<string> Line = new List<string>();

                        for (var colIndex = 0; colIndex < sheet.Dimension.Columns; colIndex++)
                        {
                            var Value = sheet.Cells[rowIndex + startRowIndex, colIndex + 1].Value;
                            Line.Add(Value != null ? Value.ToString() : "");
                        }

                        Result.Add(Line);
                    }
                }
            }

            try
            {
                //fileInfo.Delete();
            }
            catch { }

            return Result;
        }
    }
}
