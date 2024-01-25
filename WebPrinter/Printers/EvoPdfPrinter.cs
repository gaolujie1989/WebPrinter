using EvoPdf.PdfPrint;
using System.Drawing.Printing;
using System.IO;

namespace WebPrinter.Printers
{
    class EvoPdfPrinter : SystemImagePrinter
    {
        public override void PrintPdf(Stream stream, PrintOptions options)
        {
            PdfPrint printer = new PdfPrint();
            SetPrintOptions(printer, options);
            printer.Print(stream);
        }

        private void SetPrintOptions(PdfPrint printer, PrintOptions options)
        {
            printer.PrinterSettings.PrinterName = options.PrinterName;
            printer.DefaultPageSettings.Landscape = options.Landscape;
            printer.DefaultPageSettings.Margins = options.Margins;
            printer.UseHardMargins = false;
            printer.ShowStatusDialog = false;
        }
    }
}
