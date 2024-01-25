using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;

namespace WebPrinter.Printers
{
    class SystemImagePrinter : IPrinter
    {
        public virtual void PrintImage(string filePath, PrintOptions options)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                PrintImage(stream, options);
            }
        }

        public virtual void PrintImage(Stream stream, PrintOptions options)
        {
            PrintDocument printDocument = new PrintDocument();
            SetSystemPrintOptions(printDocument, options);
            printDocument.PrintPage += (sender, e) =>
            {
                using (Image image = Image.FromStream(stream))
                {
                    PrintPage(image, e);
                }
            };
            printDocument.Print();
        }

        private void PrintPage(Image image, PrintPageEventArgs e)
        {
            // 计算图片的缩放比例，以适应页面大小
            float scale = Math.Min(
                e.MarginBounds.Width / (float)image.Width,
                e.MarginBounds.Height / (float)image.Height);

            // 计算图片的新大小
            int scaledWidth = (int)(image.Width * scale);
            int scaledHeight = (int)(image.Height * scale);

            // 计算图片居中时的起始位置
            int x = e.MarginBounds.Left + (e.MarginBounds.Width - scaledWidth) / 2;
            int y = e.MarginBounds.Top + (e.MarginBounds.Height - scaledHeight) / 2;

            // 绘制调整大小后的图片
            e.Graphics.DrawImage(image, x, y, scaledWidth, scaledHeight);
        }

        public void SetSystemPrintOptions(PrintDocument printDocument, PrintOptions options)
        {
            printDocument.PrintController = new StandardPrintController();
            printDocument.PrinterSettings.PrinterName = options.PrinterName;
            printDocument.DefaultPageSettings.Landscape = options.Landscape;
            if (options.Margins != null)
            {
                printDocument.DefaultPageSettings.Margins = options.Margins;
            }

            //// 获取打印机支持的分辨率列表
            //PrinterResolutionCollection resolutions = printDocument.PrinterSettings.PrinterResolutions;

            //// 选择一个高分辨率设置
            //PrinterResolution highResolution = null;
            //foreach (PrinterResolution res in resolutions)
            //{
            //    if (res.Kind == PrinterResolutionKind.High)
            //    {
            //        highResolution = res;
            //        break;
            //    }
            //}
            //// 如果找到高分辨率设置，则应用它
            //if (highResolution != null)
            //{
            //    printDocument.DefaultPageSettings.PrinterResolution = highResolution;
            //}
        }


        public virtual void PrintPdf(string filePath, PrintOptions options)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                PrintPdf(stream, options);
            }
        }

        public virtual void PrintPdf(Stream stream, PrintOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
