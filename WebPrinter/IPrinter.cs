using System.IO;

namespace WebPrinter
{
    interface IPrinter
    {
        void PrintPdf(string filePath, PrintOptions options);

        void PrintPdf(Stream stream, PrintOptions options);

        void PrintImage(string filePath, PrintOptions options);

        void PrintImage(Stream stream, PrintOptions options);
    }
}
