using PdfiumViewer;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace WebPrinter.Printers
{
    class IumPdfPrinter : SystemImagePrinter
    {
        public override void PrintPdf(Stream stream, PrintOptions options)
        {
            if (PrintByImage)
            {
                base.PrintPdf(stream, options);
                return;
            }

            using (var document = PdfDocument.Load(stream))
            {
                using (var printDocument = document.CreatePrintDocument())
                {
                    SetSystemPrintOptions(printDocument, options);
                    printDocument.Print();
                }
            }
        }

        protected override List<Image> ConvertPdfToImage(Stream stream, int dpi = 300)
        {
            List<Image> imageList = new List<Image>();
            using (var document = PdfDocument.Load(stream))
            {
                for (int pageIndex = 0; pageIndex < document.PageCount; pageIndex++)
                {
                    var image = document.Render(pageIndex, dpi, dpi, PdfRenderFlags.CorrectFromDpi);
                    imageList.Add(image);
                }
            }
            return imageList;
        }
    }
}
