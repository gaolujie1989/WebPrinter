using Spire.Pdf;
using Spire.Pdf.HtmlConverter;
using Spire.Pdf.Print;
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
            using (var doc = new PdfDocument())
            {
                doc.LoadFromHTML(html, false, new PdfPageSettings(), new PdfHtmlLayoutFormat());
                SetPrintSettingOptions(doc.PrintSettings, options);
                doc.Print();
            }
        }

        public static void PrintPdf(string fileName, PrintOptions options = null)
        {
            using (var doc = new PdfDocument())
            {
                doc.LoadFromFile(fileName);
                SetPrintSettingOptions(doc.PrintSettings, options);
                doc.Print();
            }
        }

        private static void SetPrintSettingOptions(PdfPrintSettings settings, PrintOptions options)
        {
            //不弹出打印对话框
            settings.PrintController = new StandardPrintController();
            //关闭自动横向
            settings.SelectSinglePageLayout(PdfSinglePageScalingMode.FitSize, false);
            if (!String.IsNullOrEmpty(options.PrinterName))
            {
                settings.PrinterName = options.PrinterName;
            }
            if (!String.IsNullOrEmpty(options.PaperName))
            {
                var ps = GetPaperSize(options.PaperName);
                if (ps != null)
                {
                    settings.PaperSize = ps;
                }
            }
            if (options.Duplex && settings.CanDuplex)
            {
                settings.Duplex = Duplex.Horizontal;
            }
            if (options.Landscape)
            {
                settings.Landscape = options.Landscape;
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
