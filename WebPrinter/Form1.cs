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

        private string printTestFile;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadPrinters();
            LoadPrintEngines();
            LoadPrintTestFiles();
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

        private void LoadPrintEngines()
        {
            var printEngines = PrinterHelper.GetPrintEngines();
            printEngineBox.DataSource = printEngines;

            Properties.Settings.Default.PrintEngine = printEngineBox.SelectedItem.ToString();
            Properties.Settings.Default.Save();
        }

        private void PrintEngineBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            Properties.Settings.Default.PrintEngine = comboBox.SelectedItem.ToString();
            Properties.Settings.Default.Save();
        }

        private void LoadPrintTestFiles()
        {
            var printTestFiles = new List<string>()
            {
                "bwaren.pdf",
                "dhl.pdf",
                "vcds.png"
            };
            printTestFileBox.DataSource = printTestFiles;
            printTestFile = printTestFileBox.SelectedItem.ToString();
        }

        private void PrintTestFileBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            printTestFile = comboBox.SelectedItem.ToString();
        }

        private void PrintTestBtn_Click(object sender, EventArgs e)
        {
            string filePath = Directory.GetCurrentDirectory() + "/TestFiles/" + printTestFile;
            if (filePath.ToLower().EndsWith(".pdf"))
            {
                PrinterHelper.PrintPdf(filePath, GetPrintOptions());
            }
            else
            {
                PrinterHelper.PrintImage(filePath, GetPrintOptions());
            }
        }

        private PrintOptions GetPrintOptions()
        {
            return new PrintOptions()
            {
                PrintEngine = Properties.Settings.Default.PrintEngine,
                PrinterName = Properties.Settings.Default.PrinterName,
                Landscape = Properties.Settings.Default.Landscape,
            };
        }

        private void PrintBWarenTestBtn_Click(object sender, EventArgs e)
        {
            string pdf = Directory.GetCurrentDirectory() + "/test-bwaren.pdf";
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
                    byte[] pdfBytes = Convert.FromBase64String(postData["print_pdf_base64"]);
                    Stream stream = new MemoryStream(pdfBytes);
                    PrinterHelper.PrintPdf(stream, GetPrintOptions());
                    resultData.Add("result", "Success");
                    resultData.Add("status", "200");
                    statusCode = 200;
                }
                else if (postData.ContainsKey("print_image_base64"))
                {
                    byte[] imageBytes = Convert.FromBase64String(postData["print_image_base64"]);
                    Stream stream = new MemoryStream(imageBytes);
                    PrinterHelper.PrintImage(stream, GetPrintOptions());
                    resultData.Add("result", "Success");
                    resultData.Add("status", "200");
                    statusCode = 200;
                }
                else if (postData.ContainsKey("test"))
                {
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
            else
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
            newDocument.PageSettings.Width = imageWidth;
            newDocument.PageSettings.Height = imageHeight;
            newDocument.PageSettings.Margins.All = 0;

            PdfPageBase newPage = newDocument.Pages.Add();
            newPage.Canvas.DrawImage(imagePdf, 0, 0, imageWidth, imageHeight);
            newDocument.SaveToFile(pdfFilePath);
        }
    }
}
