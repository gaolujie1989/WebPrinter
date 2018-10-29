using Spire.Pdf;
using Spire.Pdf.HtmlConverter;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;

namespace WebPrinter
{
    class PrinterHelper
    {
        private static PrintDocument printDoc = new PrintDocument();

        public static String DefaultPrinter()
        {
            return printDoc.PrinterSettings.PrinterName;
        }

        public static List<String> GetLocalPrinters()
        {
            List<String> fPrinters = new List<String>
            {
                DefaultPrinter()
            };
            foreach (String fPrinterName in PrinterSettings.InstalledPrinters)
            {
                if (!fPrinters.Contains(fPrinterName))
                {
                    fPrinters.Add(fPrinterName);
                }
            }
            return fPrinters;
        }

        public static void PrintHtml(string html, string printerName = null, PdfPageSettings setting = null, PdfHtmlLayoutFormat htmlLayoutFormat = null)
        {
            PdfDocument doc = new PdfDocument();
            if (setting == null)
            {
                setting = new PdfPageSettings();
            }
            if (htmlLayoutFormat == null)
            {
                htmlLayoutFormat = new PdfHtmlLayoutFormat();
            }

            doc.LoadFromHTML(html, false, setting, htmlLayoutFormat);

            if (printerName != null)
            {
                List<String> printers = GetLocalPrinters();
                if (printers.Contains(printerName))
                {
                    doc.PrinterName = printerName;
                }
            }
            doc.PrintDocument.Print();
        }
    }
}
