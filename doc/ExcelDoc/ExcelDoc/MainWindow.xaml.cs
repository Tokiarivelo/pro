using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace ExcelDoc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Worksheets.Add("Worksheet1");

                var excelWorksheet = excel.Workbook.Worksheets["Worksheet1"];

                List<string[]> headerRow = new List<string[]>{
                      new[] { "ID", "First Name", "Last Name", "DOB" }
                    };
                string headerRange = "A1:" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + "1";

                excelWorksheet.Cells[headerRange].LoadFromArrays(headerRow);

                excelWorksheet.Cells[headerRange].Style.Font.Bold = true;
                excelWorksheet.Cells[headerRange].Style.Font.Size = 14;
                excelWorksheet.Cells[headerRange].Style.Fill.PatternType = ExcelFillStyle.DarkGrid;
                excelWorksheet.Cells[headerRange].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                excelWorksheet.Cells[headerRange].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                excelWorksheet.Cells[headerRange].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSkyBlue);

                IEnumerable<object[]> cellData= new List<object[]>{
                      new object[] {0, 0, 0, 1},
                      new object[] {10,7.32,7.42,1},
                      new object[] {20,5.01,5.24,1},
                      new object[] {30,3.97,4.16,1},
                      new object[] {40,3.97,4.16,1},
                      new object[] {50,3.97,4.16,1},
                      new object[] {60,3.97,4.16,1},
                      new object[] {70,3.97,4.16,1},
                      new object[] {80,3.97,4.16,1},
                      new object[] {90,3.97,4.16,1},
                      new object[] {100,3.97,4.16,1}
                };
                excelWorksheet.Cells[2, 1].LoadFromArrays(cellData);
                excelWorksheet.Cells.AutoFitColumns();
                FileInfo excelFile = new FileInfo(@"C:\Users\toky\Desktop\newDep\test.xlsx");
                excel.SaveAs(excelFile);

                bool isExcelInstalled = Type.GetTypeFromProgID("Excel.Application") != null;
                if (isExcelInstalled)
                {
                    System.Diagnostics.Process.Start(excelFile.ToString());
                }
            }
        }
    }
}
