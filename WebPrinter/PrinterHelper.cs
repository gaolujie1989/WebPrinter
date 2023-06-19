using Spire.Pdf;
using Spire.Pdf.HtmlConverter;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;

namespace WebPrinter
{
    class PrinterHelper
    {
        public static String DefaultPrinter()
        {
            using (var doc = new PrintDocument())
            {
                return doc.PrinterSettings.PrinterName;
            }
        }

        public static List<String> GetLocalPrinters()
        {
            List<String> printerNames = new List<String>
            {
                DefaultPrinter()
            };
            foreach (String printerName in PrinterSettings.InstalledPrinters)
            {
                if (!printerNames.Contains(printerName))
                {
                    printerNames.Add(printerName);
                }
            }
            return printerNames;
        }

        public static void PrintHtml(string html, PrintOptions options)
        {
            PdfDocument pdf = new PdfDocument();
            pdf.LoadFromHTML(html, false, new PdfPageSettings(), new PdfHtmlLayoutFormat());
            pdf.PageScaling = PdfPrintPageScaling.FitSize;

            var printDoc = pdf.PrintDocument;
            //// 不弹出打印对话框
            printDoc.PrintController = new StandardPrintController();
            SetPrintDocumentOptions(printDoc, options);
            printDoc.Print();
        }

        public static void PrintPdf(string fileName, PrintOptions options = null)
        {
            PdfDocument pdf = new PdfDocument();
            pdf.LoadFromFile(fileName);
            pdf.PageScaling = PdfPrintPageScaling.FitSize;

            var printDoc = pdf.PrintDocument;
            //// 不弹出打印对话框
            printDoc.PrintController = new StandardPrintController();
            SetPrintDocumentOptions(printDoc, options);
            printDoc.Print();
        }

        private static void SetPrintDocumentOptions(PrintDocument document, PrintOptions options)
        {
            if (!String.IsNullOrEmpty(options.PrinterName))
            {
                document.PrinterSettings.PrinterName = options.PrinterName;
            }
            if (!String.IsNullOrEmpty(options.PaperName))
            {
                var ps = GetPaperSize(options.PaperName);
                if (ps != null)
                {
                    document.DefaultPageSettings.PaperSize = ps;
                }
            }
            if (options.Duplex && document.PrinterSettings.CanDuplex)
            {
                document.PrinterSettings.Duplex = Duplex.Horizontal;
            }
            if (options.Landscape)
            {
                document.DefaultPageSettings.Landscape = options.Landscape;
            }
        }

        private static PaperSize GetPaperSize(string paperName)
        {
            using (var doc = new PrintDocument())
            {
                foreach (PaperSize ps in doc.PrinterSettings.PaperSizes)
                {
                    if (ps.PaperName.Equals(paperName, StringComparison.OrdinalIgnoreCase)) {
                        return ps;
                    }
                }
            }
            return null;
        }
    }

    public class PrintOptions
    {
        /// <summary>
        /// 指定打印机，缺省使用系统默认打印机
        /// </summary>
        public string PrinterName { get; set; }

        /// <summary>
        /// 指定纸类型，缺省使用打印机默认纸类型
        /// </summary>
        public string PaperName { get; set; }

        /// <summary>
        /// 是否双面打印，缺省单面打印
        /// </summary>
        public bool Duplex { get; set; }

        /// <summary>
        /// 是否横向打印，缺省竖向打印
        /// </summary>
        public bool Landscape { get; set; }
    }
}
