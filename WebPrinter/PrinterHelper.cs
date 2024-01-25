using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using WebPrinter.Printers;

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
            var printer = new SpirePdfPrinter();
            printer.PrintHtml(html, options);
        }

        public static void PrintImage(string filePath, PrintOptions options)
        {
            CreatePrinter(options.PrintEngine).PrintImage(filePath, options);
        }

        public static void PrintImage(Stream stream, PrintOptions options)
        {
            CreatePrinter(options.PrintEngine).PrintImage(stream, options);
        }

        public static void PrintPdf(string filePath, PrintOptions options)
        {
            CreatePrinter(options.PrintEngine).PrintPdf(filePath, options);
        }

        public static void PrintPdf(Stream stream, PrintOptions options)
        {
            CreatePrinter(options.PrintEngine).PrintPdf(stream, options);
        }

        public static IPrinter CreatePrinter(string name)
        {
            switch (name.ToUpper())
            {
                case "EVOPDF":
                    return new EvoPdfPrinter();
                case "IUMPDF":
                    return new IumPdfPrinter();
                case "SPIREPDF":
                    return new SpirePdfPrinter();
                default:
                    return new SystemImagePrinter();
            }
        }

        public static List<String> GetPrintEngines()
        {
            return new List<string>()
            {
                "SPIREPDF",
                "IUMPDF",
                "EVOPDF",
            };
        }
    }

}
