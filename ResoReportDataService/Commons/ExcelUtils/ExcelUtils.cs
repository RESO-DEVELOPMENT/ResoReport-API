using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace ResoReportDataService.Commons.ExcelUtils
{
    public static class ExcelUtils
    {
        
        /// <summary>
        /// Format:
        /// - currency: ₫#,##0.00 
        /// - float | double: #,##0.00
        /// - int | number: #,##
        /// </summary>
        /// <param name="excelModel"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static FileStreamResult ExportExcel<T>(ExcelModel<T> excelModel)
        {
            int count = 0;
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add(excelModel.SheetTitle);
                var startHeaderChar = 'A';
                var startHeaderNumber = 1;

                #region Set Headers

                //Set STT column
                worksheet.Cells["" + (startHeaderChar++) + (startHeaderNumber)].Value = "STT";

                foreach (var columnConfig in excelModel.ColumnConfigs)
                {
                    worksheet.Cells["" + (startHeaderChar++) + (startHeaderNumber)].Value = columnConfig.Title;
                }

                #endregion

                #region Set style for rows and columns

                var endHeaderChar = --startHeaderChar;
                var endHeaderNumber = startHeaderNumber;
                startHeaderChar = 'A';
                startHeaderNumber = 1;

                worksheet.Cells["" + startHeaderChar + startHeaderNumber +
                                ":" + endHeaderChar + endHeaderNumber].Style.Font.Bold = true;
                worksheet.Cells["" + startHeaderChar + startHeaderNumber +
                                ":" + endHeaderChar + endHeaderNumber].AutoFitColumns();
                worksheet.Cells["" + startHeaderChar + startHeaderNumber +
                                ":" + endHeaderChar + endHeaderNumber]
                    .Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells["" + startHeaderChar + startHeaderNumber +
                                ":" + endHeaderChar + endHeaderNumber]
                    .Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.GreenYellow);
                worksheet.View.FreezePanes(2, 1);

                #endregion

                #region Set value for cells

                foreach (var data in excelModel.DataSources)
                {
                    worksheet.Cells["" + (startHeaderChar++) + (++startHeaderNumber)].Value = ++count;
                    foreach (var (column, index)in excelModel.ColumnConfigs.Select((column, i) => (column, i)))
                    {
                        char headerChar = startHeaderChar;
                        headerChar += (char)index;

                        if (
                            column.ValueType.Trim().ToLower().Equals("double") ||
                            column.ValueType.Trim().ToLower().Equals("float"))
                        {
                            worksheet.Cells["" + (headerChar) + (startHeaderNumber)].Style.Numberformat.Format =
                                "#,##0.00";
                        }
                        else if (column.ValueType.Trim().ToLower().Equals("currency"))
                        {
                            worksheet.Cells["" + (headerChar) + (startHeaderNumber)].Style.Numberformat.Format =
                                "#,##0.00₫";
                        }
                        else if (column.ValueType.Trim().ToLower().Equals("int") ||
                                 column.ValueType.Trim().ToLower().Equals("number"))
                        {
                            worksheet.Cells["" + (headerChar) + (startHeaderNumber)].Style.Numberformat.Format = "#,##";
                        }

                        worksheet.Cells["" + (headerChar) + (startHeaderNumber)].Value =
                            column.Render != null
                                ? column.Render(data)
                                : data.GetType().GetProperties()
                                    .SingleOrDefault(x => x.Name.ToLower().Equals(column.DataIndex.ToLower()))
                                    ?.GetValue(data, null) ?? "N/A";
                    }

                    startHeaderChar = 'A';
                }

                #region Set style for excel

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                worksheet.Cells[worksheet.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[worksheet.Dimension.Address].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[worksheet.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[worksheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                #endregion

                excelPackage.SaveAs(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                var fileDownloadName = excelModel.SheetTitle + ".xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return new FileStreamResult(memoryStream, contentType)
                {
                    FileDownloadName = fileDownloadName
                };

                #endregion
            }
        }

        public static FileStreamResult ExportExcelWithTemplate()
        {
            return null;
        }
    }
}