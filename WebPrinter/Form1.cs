using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebPrinter
{
    public partial class Form1 : Form
    {
        private HttpHelper httpHelper;

        private PrintOptions printOptions = new PrintOptions();

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
        }

        private void PrintTest_Click(object sender, EventArgs e)
        {
            string pdf = Directory.GetCurrentDirectory() + "/test.pdf";
            PrinterHelper.PrintPdf(pdf, new PrintOptions()
            {
                PrinterName = printerListBox.SelectedItem.ToString()
            });
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
                    Action getSelectedPrinter = () =>
                    {
                        PrinterHelper.PrintHtml(postData["print_html"], new PrintOptions()
                        {
                            PrinterName = printerListBox.SelectedItem.ToString()
                        });
                    };
                    if (printerListBox.InvokeRequired)
                    {
                        printerListBox.Invoke(getSelectedPrinter);     
                    } else
                    {
                        getSelectedPrinter.Invoke();
                    }
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

                    Action getSelectedPrinter = () =>
                    {
                        PrinterHelper.PrintPdf(fileName, new PrintOptions()
                        {
                            PrinterName = printerListBox.SelectedItem.ToString()
                        });
                        File.Delete(fileName);
                    };
                    if (printerListBox.InvokeRequired)
                    {
                        printerListBox.Invoke(getSelectedPrinter);
                    }
                    else
                    {
                        getSelectedPrinter.Invoke();
                    }
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
    }
}
