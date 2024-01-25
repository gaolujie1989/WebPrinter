using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Print;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;

namespace WebPrinter.Printers
{
    class SpirePdfPrinter : SystemImagePrinter
    {
        public override void PrintPdf(Stream stream, PrintOptions options)
        {
            using (var doc = new PdfDocument())
            {
                doc.LoadFromStream(stream);
                SetPrintOptions(doc.PrintSettings, options);
                doc.Print();
            }
        }

        public override void PrintImage(Stream stream, PrintOptions options)
        {
            using (var doc = CreatePdfFromImage(stream))
            {
                SetPrintOptions(doc.PrintSettings, options);
                doc.Print();
            }
        }

        public void PrintHtml(string html, PrintOptions options)
        {
            using (var doc = new PdfDocument())
            {
                doc.LoadFromHTML(html, false, false, true);
                SetPrintOptions(doc.PrintSettings, options);
                doc.Print();
            }
        }

        public PdfDocument CreatePdfFromImage(Stream imageStream)
        {
            Image sourceImage = Image.FromStream(imageStream, false, false);
            float imageWidth = sourceImage.Width;
            float imageHeight = sourceImage.Height;

            PdfImage imagePdf = PdfImage.FromStream(imageStream);

            PdfDocument pdfDoc = new PdfDocument();
            pdfDoc.PageSettings.Width = imageWidth;
            pdfDoc.PageSettings.Height = imageHeight;
            pdfDoc.PageSettings.Margins.All = 0;

            PdfPageBase newPage = pdfDoc.Pages.Add();
            newPage.Canvas.DrawImage(imagePdf, 0, 0, imageWidth, imageHeight);
            return pdfDoc;
        }

        private void SetPrintOptions(PdfPrintSettings settings, PrintOptions options)
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
            if (options.Landscape)
            {
                settings.Landscape = options.Landscape;
            }
        }

        private PaperSize GetPaperSize(string paperName)
        {
            using (var doc = new PrintDocument())
            {
                foreach (PaperSize ps in doc.PrinterSettings.PaperSizes)
                {
                    if (ps.PaperName.Equals(paperName, StringComparison.OrdinalIgnoreCase))
                    {
                        return ps;
                    }
                }
            }
            return null;
        }
    }
}
