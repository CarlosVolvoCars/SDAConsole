using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;

namespace SDAConsole.exporters
{
    public class CsvExporter
    {
        public void ExportResultsToExcel(
            string filePath,
            List<(string nodeName, bool evaluation)> sameSoftwareOnECUList1,
            List<(string nodeName, bool evaluation)> sameSoftwareOnECUList2,
            List<string> ecuDifferences,
            List<(string ecuName, string SWType, string SWPart, string CoMo, string evaluation)> evaluationDetails1,
            List<(string ecuName, string SWType, string SWPart, string CoMo, string evaluation)> evaluationDetails2,
            string baselineName1,
            string baselineName2)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
            {
                // Create the workbook and sheets
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                // Sheet 1: ECU Evaluations 1
                CreateSheet(workbookPart, sheets, $"ECU Evaluations {baselineName1}", sameSoftwareOnECUList1);

                // Sheet 2: ECU Evaluations 2
                CreateSheet(workbookPart, sheets, $"ECU Evaluations {baselineName2}", sameSoftwareOnECUList2);

                // Sheet 3: ECU Differences
                CreateDifferencesSheet(workbookPart, sheets, "ECU Differences", ecuDifferences);

                // Sheet 4: Evaluation Details 1
                CreateSheet(workbookPart, sheets, $"Evaluation Details {baselineName1}", evaluationDetails1);

                // Sheet 5: Evaluation Details 2
                CreateSheet(workbookPart, sheets, $"Evaluation Details {baselineName2}", evaluationDetails2);

                workbookPart.Workbook.Save();
            }
        }

        private static void CreateSheet(WorkbookPart workbookPart, Sheets sheets, string sheetName, List<(string nodeName, bool evaluation)> data)
        {
            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

            // Add header row
            Row headerRow = new Row();
            headerRow.Append(
                CreateTextCell("A1", "ECU"),
                CreateTextCell("B1", "Evaluation")
            );
            sheetData.AppendChild(headerRow);

            // Add data rows
            for (int i = 0; i < data.Count; i++)
            {
                Row dataRow = new Row();
                dataRow.Append(
                    CreateTextCell($"A{i + 2}", data[i].nodeName),
                    CreateTextCell($"B{i + 2}", data[i].evaluation ? "True" : "False")
                );
                sheetData.AppendChild(dataRow);
            }

            // Add the sheet to the workbook
            uint sheetId = (uint)sheets.Count() + 1;
            sheets.Append(new Sheet { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = sheetId, Name = sheetName });
        }

        private static void CreateSheet(WorkbookPart workbookPart, Sheets sheets, string sheetName, List<(string ecuName, string SWType, string SWPart, string CoMo, string evaluation)> data)
        {
            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

            // Add header row
            Row headerRow = new Row();
            headerRow.Append(
                CreateTextCell("A1", "ECU"),
                CreateTextCell("B1", "SW Type"),
                CreateTextCell("C1", "SW Part"),
                CreateTextCell("D1", "CoMo"),
                CreateTextCell("E1", "Evaluation")
            );
            sheetData.AppendChild(headerRow);

            // Add data rows
            for (int i = 0; i < data.Count; i++)
            {
                Row dataRow = new Row();
                dataRow.Append(
                    CreateTextCell($"A{i + 2}", data[i].ecuName),
                    CreateTextCell($"B{i + 2}", data[i].SWType),
                    CreateTextCell($"C{i + 2}", data[i].SWPart),
                    CreateTextCell($"D{i + 2}", data[i].CoMo),
                    CreateTextCell($"E{i + 2}", data[i].evaluation)
                );
                sheetData.AppendChild(dataRow);
            }

            // Add the sheet to the workbook
            uint sheetId = (uint)sheets.Count() + 1;
            sheets.Append(new Sheet { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = sheetId, Name = sheetName });
        }

        private static void CreateDifferencesSheet(WorkbookPart workbookPart, Sheets sheets, string sheetName, List<string> differences)
        {
            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

            // Add header row
            Row headerRow = new Row();
            headerRow.Append(CreateTextCell("A1", "Differences"));
            sheetData.AppendChild(headerRow);

            // Add data rows
            for (int i = 0; i < differences.Count; i++)
            {
                Row dataRow = new Row();
                dataRow.Append(CreateTextCell($"A{i + 2}", differences[i]));
                sheetData.AppendChild(dataRow);
            }

            // Add the sheet to the workbook
            uint sheetId = (uint)sheets.Count() + 1;
            sheets.Append(new Sheet { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = sheetId, Name = sheetName });
        }

        private static Cell CreateTextCell(string cellReference, string cellValue)
        {
            return new Cell
            {
                CellReference = cellReference,
                DataType = CellValues.String,
                CellValue = new CellValue(cellValue)
            };
        }
    }
}