using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QLKS.Helpers
{
    public static class ExcelExporter
    {
        public static void ExportToExcel(DataTable dataTable, string titleName, string sheetName)
        {
            // Cấu hình EPPlus sử dụng phi thương mại
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = sheetName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss")
            };

            if (sfd.ShowDialog() == true)
            {
                try
                {
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);

                        // 1. Tạo Tiêu đề Báo cáo lớn phía trên
                        worksheet.Cells[1, 1].Value = titleName.ToUpper();
                        worksheet.Cells[1, 1, 1, dataTable.Columns.Count].Merge = true;
                        worksheet.Cells[1, 1].Style.Font.Size = 16;
                        worksheet.Cells[1, 1].Style.Font.Bold = true;
                        worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        // 2. Điền Tiêu đề Cột
                        for (int col = 0; col < dataTable.Columns.Count; col++)
                        {
                            var cell = worksheet.Cells[3, col + 1];
                            cell.Value = dataTable.Columns[col].ColumnName;
                            cell.Style.Font.Bold = true;
                            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }

                        // 3. Điền Dữ liệu
                        for (int row = 0; row < dataTable.Rows.Count; row++)
                        {
                            for (int col = 0; col < dataTable.Columns.Count; col++)
                            {
                                var cell = worksheet.Cells[row + 4, col + 1];
                                cell.Value = dataTable.Rows[row][col];
                                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                // Định dạng ngày tháng hoặc số tiền nếu cần
                                if (dataTable.Rows[row][col] is DateTime)
                                {
                                    cell.Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";
                                }
                                else if (dataTable.Rows[row][col] is decimal || dataTable.Rows[row][col] is double)
                                {
                                    cell.Style.Numberformat.Format = "#,##0.00";
                                }
                            }
                        }

                        // Tự động căn rộng cột theo nội dung
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        // Lưu file vật lý
                        FileInfo fileInfo = new FileInfo(sfd.FileName);
                        package.SaveAs(fileInfo);

                        MessageBox.Show("Xuất file báo cáo Excel thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi khi xuất Excel: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
