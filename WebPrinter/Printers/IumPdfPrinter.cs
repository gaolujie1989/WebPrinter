using PdfiumViewer;
using System.IO;

namespace WebPrinter.Printers
{
    class IumPdfPrinter : SystemImagePrinter
    {
        public override void PrintPdf(Stream stream, PrintOptions options)
        {
            using (var document = PdfDocument.Load(stream))
            {
                using (var printDocument = document.CreatePrintDocument())
                {
                    SetSystemPrintOptions(printDocument, options);
                    printDocument.Print();
                }
            }
        }
    }
}
