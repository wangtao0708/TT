using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace tt2.Wx.Web2
{
    public class HttpService
    {

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开    
            return true;
        }

        public static string Post(string xml, string url, bool isUseCert, int intTimeOut)
        {
            System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接
            string result = "";//返回结果
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream reqStream = null;

            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(CheckValidationResult);
                }
                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.Timeout = intTimeOut * 1000;

                //设置代理服务器
                //WebProxy proxy = new WebProxy();                          //定义一个网关对象
                //proxy.Address = new Uri(WxConfig.PROXY_URL);              //网关服务器端口:端口
                //request.Proxy = proxy;

                //设置POST的数据类型和长度
                request.ContentType = "text/xml";
                byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                request.ContentLength = data.Length;
                //是否使用证书
                if (isUseCert)
                {
                    //string strPath = HttpContext.Current.Request.PhysicalApplicationPath;
                    //strPath += WxConfig.SslcertPath;
                    //string strPath1 = System.Web.HttpContext.Current.Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath.ToString());//获取程序根目录
                    //string strPath2 = strPath1 + "/WxCert/apiclient_cert.p12";
                    X509Certificate2 cert = new X509Certificate2(@"D:\WxCert\apiclient_cert.p12", WxConfig.SslcertPassword, X509KeyStorageFlags.MachineKeySet);
                    request.ClientCertificates.Add(cert);
                }
                //往服务器写入数据
                reqStream = request.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
                //获取服务端返回
                response = (HttpWebResponse)request.GetResponse();
                //获取服务端返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException ex)
            {
                System.Threading.Thread.ResetAbort();
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                }
                throw new Exception(e.ToString());
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }

        /// <summary>
        /// 处理http GET请求，返回数据
        /// </summary>
        /// <param name="url">请求的url地址</param>
        /// <returns>http GET成功后返回的数据，失败抛WebException异常</returns>
        public static string Get(string url)
        {
            System.GC.Collect();
            string result = "";

            HttpWebRequest request = null;
            HttpWebResponse response = null;

            //请求url以获取数据
            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "GET";

                //设置代理
                //WebProxy proxy = new WebProxy();
                //proxy.Address = new Uri(WxConfig.PROXY_URL);
                //request.Proxy = proxy;

                //获取服务器返回
                response = (HttpWebResponse)request.GetResponse();

                //获取HTTP返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException ex)
            {
                System.Threading.Thread.ResetAbort();
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                }
                throw new Exception(e.ToString());
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }

        public static string HttpPostVisit(string strUrl, string strContentType, string strContent)
        {
            System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接
            string strResult = "";//返回结果
            HttpWebRequest reqPost = null;
            HttpWebResponse repPost = null;
            Stream stmRequest = null;
            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (strUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(CheckValidationResult);
                }
                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                reqPost = (HttpWebRequest)WebRequest.Create(strUrl);
                reqPost.Method = "POST";
                reqPost.Timeout = 30 * 1000;
                //设置代理服务器
                //WebProxy proxy = new WebProxy();                          //定义一个网关对象
                //proxy.Address = new Uri(WxConfig.PROXY_URL);              //网关服务器端口:端口
                //reqPost.Proxy = proxy;

                //设置POST的数据类型和长度
                reqPost.ContentType = strContentType;
                byte[] bytData = System.Text.Encoding.UTF8.GetBytes(strContent);
                reqPost.ContentLength = bytData.Length;
                //往服务器写入数据
                stmRequest = reqPost.GetRequestStream();
                stmRequest.Write(bytData, 0, bytData.Length);
                stmRequest.Close();

                //获取服务端返回
                repPost = (HttpWebResponse)reqPost.GetResponse();

                //获取服务端返回数据
                StreamReader sr = new StreamReader(repPost.GetResponseStream(), Encoding.UTF8);
                strResult = sr.ReadToEnd().Trim();
                sr.Close();
                return strResult;
            }
            catch (System.Threading.ThreadAbortException ex)
            {
                System.Threading.Thread.ResetAbort();
                throw ex;
            }
            catch (WebException ex)
            {
                //if (ex.Status == WebExceptionStatus.ProtocolError)
                //{
                //}
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //关闭连接和流
                if (repPost != null) repPost.Close();
                if (reqPost != null) reqPost.Abort();
            }
            
        }

    }
}