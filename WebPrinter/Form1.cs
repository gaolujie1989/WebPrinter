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

        private void button1_Click(object sender, EventArgs e)
        {
            string text = File.ReadAllText(@"E:\Code\C#\WebPrinter\WebPrinter\text.html");
            PrinterHelper.PrintHtml(text, printerListBox.SelectedItem.ToString());
        }


        private void StartPrintService()
        {
            httpHelper = new HttpHelper(1989);
            httpHelper.RequestHandler = HandlePrintRequest;
            httpHelper.Start();
        }

        private void HandlePrintRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            Dictionary<string, string> resultData = new Dictionary<string, string>();
            int statusCode = 200;

            if (request.HttpMethod == "OPTION")
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
                    string selectedPrinter = null;
                    Action getSelectedPrinter = () =>
                    {
                        selectedPrinter = printerListBox.SelectedItem.ToString();
                        PrinterHelper.PrintHtml(postData["print_html"], selectedPrinter);
                    };
                    if (printerListBox.InvokeRequired)
                    {
                        printerListBox.Invoke(getSelectedPrinter);     
                    } else
                    {
                        getSelectedPrinter.Invoke();
                    }
                    resultData.Add("result", "Success");
                    statusCode = 200;
                }
                else
                {
                    resultData.Add("result", "Invalid Request Data");
                    statusCode = 400;
                }
            }
            else
            {
                resultData.Add("result", "Skip");
                statusCode = 404;
            }
            httpHelper.SendJsonResponse(response, resultData, statusCode);
        }
    }
}
