using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace WebPrinter
{
    class HttpHelper
    {
        private HttpListener httpListener = null;

        public Action<HttpListenerRequest, HttpListenerResponse> RequestHandler { get; set; } = null;

        public HttpHelper(int port, string ip = "127.0.0.1", string prefix = "http")
        {
            httpListener = new HttpListener();
            httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            var uriPrefix = prefix + "://" + ip + ":" + port.ToString() + "/";
            httpListener.Prefixes.Add(uriPrefix);
        }

        public void Start()
        {
            httpListener.Start();
            httpListener.BeginGetContext(new AsyncCallback(GetContextCallBack), httpListener);
        }

        public void Stop()
        {
            httpListener.Stop();
        }

        public void GetContextCallBack(IAsyncResult ar)
        {
            try
            {
                httpListener = ar.AsyncState as HttpListener;
                HttpListenerContext context = httpListener.EndGetContext(ar);
                httpListener.BeginGetContext(new AsyncCallback(GetContextCallBack), httpListener);

                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                RequestHandler?.Invoke(request, response);
            }
            catch (Exception ex) { }
        }

        public void SendJsonResponse(HttpListenerResponse response, object data, int statusCode)
        {
            response.StatusCode = statusCode;
            response.ContentType = "application/json";
            Dictionary<string, string> crossHeaders = new Dictionary<string, string>() {
                { "Access-Control-Allow-Headers", "x-requested-with, Content-Type, origin, authorization, accept, client-security-token, X-Access-Token, X-XH"},
                { "Access-Control-Allow-Methods", "POST, GET, OPTIONS, DELETE, PUT"},
                { "Access-Control-Allow-Origin", "*"},
                { "Access-Control-Max-Age", "86400"},
            };
            foreach (var d in crossHeaders)
            {
                response.AddHeader(d.Key, d.Value);
            }
            byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            response.ContentLength64 = buffer.Length;

            using (Stream output = response.OutputStream)
            {
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }
        }
    }

    /// <summary>
    /// HttpListenner监听Post请求参数值实体
    /// </summary>
    public class HttpListenerPostValue
    {
        /// <summary>
        /// 0=> 参数
        /// 1=> 文件
        /// </summary>
        public int type = 0;
        public string name;
        public byte[] datas;
    }

    /// <summary>
    /// 获取Post请求中的参数和值帮助类
    /// </summary>
    public class HttpListenerPostParaHelper
    {
        private HttpListenerRequest request;

        public HttpListenerPostParaHelper(HttpListenerRequest request)
        {
            this.request = request;
        }

        private bool CompareBytes(byte[] source, byte[] comparison)
        {
            try
            {
                int count = source.Length;
                if (source.Length != comparison.Length)
                    return false;
                for (int i = 0; i < count; i++)
                    if (source[i] != comparison[i])
                        return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private byte[] ReadLineAsBytes(Stream SourceStream)
        {
            var resultStream = new MemoryStream();
            while (true)
            {
                int data = SourceStream.ReadByte();
                resultStream.WriteByte((byte)data);
                if (data == 10)
                    break;
            }
            resultStream.Position = 0;
            byte[] dataBytes = new byte[resultStream.Length];
            resultStream.Read(dataBytes, 0, dataBytes.Length);
            return dataBytes;
        }

        /// <summary>
        /// 获取Post过来的参数和数据
        /// </summary>
        /// <returns></returns>
        public List<HttpListenerPostValue> GetHttpListenerPostValue()
        {
            try
            {
                List<HttpListenerPostValue> HttpListenerPostValueList = new List<HttpListenerPostValue>();
                if (request.ContentType.Length > 20 && string.Compare(request.ContentType.Substring(0, 20), "multipart/form-data;", true) == 0)
                {
                    string[] HttpListenerPostValue = request.ContentType.Split(';').Skip(1).ToArray();
                    string boundary = string.Join(";", HttpListenerPostValue).Replace("boundary=", "").Trim();
                    byte[] ChunkBoundary = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
                    byte[] EndBoundary = Encoding.UTF8.GetBytes("--" + boundary + "--\r\n");
                    Stream SourceStream = request.InputStream;
                    var resultStream = new MemoryStream();
                    bool CanMoveNext = true;
                    HttpListenerPostValue data = null;
                    while (CanMoveNext)
                    {
                        byte[] currentChunk = ReadLineAsBytes(SourceStream);
                        if (!Encoding.UTF8.GetString(currentChunk).Equals("\r\n"))
                            resultStream.Write(currentChunk, 0, currentChunk.Length);
                        if (CompareBytes(ChunkBoundary, currentChunk))
                        {
                            byte[] result = new byte[resultStream.Length - ChunkBoundary.Length];
                            resultStream.Position = 0;
                            resultStream.Read(result, 0, result.Length);
                            CanMoveNext = true;
                            if (result.Length > 0)
                                data.datas = result;
                            data = new HttpListenerPostValue();
                            HttpListenerPostValueList.Add(data);
                            resultStream.Dispose();
                            resultStream = new MemoryStream();

                        }
                        else if (Encoding.UTF8.GetString(currentChunk).Contains("Content-Disposition"))
                        {
                            byte[] result = new byte[resultStream.Length - 2];
                            resultStream.Position = 0;
                            resultStream.Read(result, 0, result.Length);
                            CanMoveNext = true;
                            data.name = Encoding.UTF8.GetString(result).Replace("Content-Disposition: form-data; name=\"", "").Replace("\"", "").Split(';')[0];
                            resultStream.Dispose();
                            resultStream = new MemoryStream();
                        }
                        else if (Encoding.UTF8.GetString(currentChunk).Contains("Content-Type"))
                        {
                            CanMoveNext = true;
                            data.type = 1;
                            resultStream.Dispose();
                            resultStream = new MemoryStream();
                        }
                        else if (CompareBytes(EndBoundary, currentChunk))
                        {
                            byte[] result = new byte[resultStream.Length - EndBoundary.Length - 2];
                            resultStream.Position = 0;
                            resultStream.Read(result, 0, result.Length);
                            data.datas = result;
                            resultStream.Dispose();
                            CanMoveNext = false;
                        }
                    }
                }
                return HttpListenerPostValueList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

}
