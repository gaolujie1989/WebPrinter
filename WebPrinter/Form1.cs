using Newtonsoft.Json;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace WebPrinter
{
    public partial class Form1 : Form
    {
        private HttpHelper httpHelper;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadPrinters();
            StartPrintService();
        }

        private void LoadPrinters()
        {
            var printers = PrinterHelper.GetLocalPrinters();
            printerListBox.DataSource = printers;

            Properties.Settings.Default.PrinterName = printerListBox.SelectedItem.ToString();
            Properties.Settings.Default.Save();
        }

        private void PrinterListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox listBox = sender as ListBox;
            Properties.Settings.Default.PrinterName = listBox.SelectedItem.ToString();
            Properties.Settings.Default.Save();
        }

        private void LandscapeBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            Properties.Settings.Default.Landscape = checkBox.Checked;
            Properties.Settings.Default.Save();
        }

        private PrintOptions GetPrintOptions()
        {
            return new PrintOptions()
            {
                PrinterName = Properties.Settings.Default.PrinterName,
                Landscape = Properties.Settings.Default.Landscape,
            };
        }

        private void PrintPdfTestBtn_Click(object sender, EventArgs e)
        {
            string pdf = Directory.GetCurrentDirectory() + "/test.pdf";
            PrinterHelper.PrintPdf(pdf, GetPrintOptions());
        }

        private void PrintImageTestBtn_Click(object sender, EventArgs e)
        {
            string image = Directory.GetCurrentDirectory() + "/test.png";
            string pdf = image + ".pdf";
            Stream stream = File.OpenRead(image);
            CreatePdfFromImage(stream, pdf);
            PrinterHelper.PrintPdf(pdf, GetPrintOptions());
        }

        private void StartPrintService()
        {
            httpHelper = new HttpHelper(19890)
            {
                RequestHandler = HandlePrintRequest
            };
            httpHelper.Start();
        }

        private void HandlePrintRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            Dictionary<string, string> resultData = new Dictionary<string, string>();
            int statusCode = 200;

            if (request.HttpMethod == "OPTIONS")
            {
                httpHelper.SendJsonResponse(response, resultData, statusCode);
                return;
            }

            if (request.Url.AbsolutePath == "/print")
            {
                string requestConent;
                Dictionary<string, string> postData = new Dictionary<string, string>();
                using (Stream inputStream = request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(inputStream, Encoding.UTF8))
                    {
                        requestConent = readStream.ReadToEnd();
                        postData = JsonConvert.DeserializeObject<Dictionary<string, string>>(requestConent);
                    }
                }

                if (postData.ContainsKey("print_html"))
                {
                    PrinterHelper.PrintHtml(postData["print_html"], GetPrintOptions());
                    resultData.Add("result", "Success");
                    resultData.Add("status", "200");
                    statusCode = 200;
                }
                else if (postData.ContainsKey("print_pdf_base64"))
                {
                    string pdfFileDir = Directory.GetCurrentDirectory() + "/pdf/";
                    if (!Directory.Exists(pdfFileDir))
                    {
                        Directory.CreateDirectory(pdfFileDir);
                    }
                    string fileName = pdfFileDir + DateTime.Now.ToString("yyyyMMddHHmmss_") + (new Random()).Next(1000, 9999).ToString() + ".pdf";
                    byte[] pdfBytes = Convert.FromBase64String(postData["print_pdf_base64"]);
                    using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.CreateNew)))
                    {
                        writer.Write(pdfBytes);
                    }

                    PrinterHelper.PrintPdf(fileName, GetPrintOptions());
                    File.Delete(fileName);
                    resultData.Add("result", "Success");
                    resultData.Add("status", "200");
                    statusCode = 200;
                }
                else if (postData.ContainsKey("print_image_base64"))
                {
                    string pdfFileDir = Directory.GetCurrentDirectory() + "/pdf/";
                    if (!Directory.Exists(pdfFileDir))
                    {
                        Directory.CreateDirectory(pdfFileDir);
                    }
                    string fileName = pdfFileDir + DateTime.Now.ToString("yyyyMMddHHmmss_") + (new Random()).Next(1000, 9999).ToString() + ".pdf";
                    byte[] imageBytes = Convert.FromBase64String(postData["print_image_base64"]);
                    Stream stream = new MemoryStream(imageBytes);
                    CreatePdfFromImage(stream, fileName);
     
                    PrinterHelper.PrintPdf(fileName, GetPrintOptions());
                    File.Delete(fileName);
                    resultData.Add("result", "Success");
                    resultData.Add("status", "200");
                    statusCode = 200;
                }
                else
                {
                    resultData.Add("result", "Invalid Request Data");
                    resultData.Add("status", "400");
                    statusCode = 400;
                }
            }
            else if (request.Url.AbsolutePath == "/test")
            {
                resultData.Add("result", "Success");
                resultData.Add("status", "200");
                statusCode = 200;
            } else
            {
                resultData.Add("result", "Skip");
                resultData.Add("status", "404");
                statusCode = 404;
            }
            httpHelper.SendJsonResponse(response, resultData, statusCode);
        }

        private void CreatePdfFromImage(Stream imageStream, string pdfFilePath)
        {
            Image sourceImage = Image.FromStream(imageStream, false, false);
            float imageWidth = sourceImage.Width;
            float imageHeight = sourceImage.Height;

            PdfImage imagePdf = PdfImage.FromStream(imageStream);

            PdfDocument newDocument = new PdfDocument();
            //删除第一页，破解水印
            //newDocument.Pages.Add();
            //newDocument.Pages.RemoveAt(0
            //设置文档纸张尺寸
            newDocument.PageSettings.Width = imageWidth;
            newDocument.PageSettings.Height = imageHeight;
            newDocument.PageSettings.Margins.All = 0;

            PdfPageBase newPage = newDocument.Pages.Add();
            newPage.Canvas.DrawImage(imagePdf, 0, 0, imageWidth, imageHeight);
            newDocument.SaveToFile(pdfFilePath);
        }
    }
}
