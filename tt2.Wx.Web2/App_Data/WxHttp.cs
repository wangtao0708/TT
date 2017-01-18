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
    public class WxHttp
    {

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开    
            return true;
        }
        public static string Get(string strUrl)
        {
            System.GC.Collect();
            string strResult = "";
            HttpWebRequest hwrGet = null;
            HttpWebResponse hwpGet = null;
            //请求strUrl以获取数据
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
                hwrGet = (HttpWebRequest)WebRequest.Create(strUrl);
                hwrGet.Method = "GET";
                //设置代理
                //WebProxy proxy = new WebProxy();
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);
                //hwrGet.Proxy = proxy;
                //获取服务器返回
                hwpGet = (HttpWebResponse)hwrGet.GetResponse();
                //获取HTTP返回数据
                StreamReader sr = new StreamReader(hwpGet.GetResponseStream(), Encoding.UTF8);
                strResult = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException ex)
            {
                System.Threading.Thread.ResetAbort();
                throw new Exception(ex.Message .ToString ());
                
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                }
                throw new Exception(e.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //关闭连接和流
                if (hwpGet != null)
                {
                    hwpGet.Close();
                }
                if (hwrGet != null)
                {
                    hwrGet.Abort();
                }
            }
            return strResult;
        }


    }
}