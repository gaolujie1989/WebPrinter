using System.Drawing.Printing;

namespace WebPrinter
{
    public class PrintOptions
    {
        /// <summary>
        /// 指定Printer类
        /// </summary>
        public string PrintEngine { get; set; }

        /// <summary>
        /// 打印模式，是否用系统默认的图片模式打印
        /// </summary>
        public bool PrintByImage { get; set; } = false;

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

        /// <summary>
        /// 打印边框长度
        /// </summary>
        public Margins Margins { get; set; } = new Margins(0, 0, 0, 0);

        /// <summary>
        /// 打印DPI
        /// </summary>
        public int DPI { get; set; } = 300;
    }
}
