using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Web.UI;
using System.Web.UI.WebControls;
using System.Runtime.Serialization;
using System.Net;
using System.Web.Security;
using LitJson;

using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq.Expressions;
using System.IO;
using System.Text;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Xml;
using System.Security.Cryptography;

namespace tt2.Wx.Web2.Controllers
{
    public class BaseController : Controller
    {

        #region 默认时间
        /// <summary>
        /// 默认时间
        /// </summary>
        public readonly DateTime DefaultTime = new DateTime(1900, 01, 01, 00, 00, 00);
        #endregion 默认时间

        #region Application

        #region 微信普通访问令牌

        #region 获得微信普通访问令牌
        /// <summary>
        /// 获得微信普通访问令牌
        /// </summary>
        /// <returns>微信普通访问令牌</returns>
        public string GetOrdinaryAccessToken()
        {
            string strAccessToken = "";
            try
            {
                if (System.Web.HttpContext.Current.Application["zr_tt_WxOrdinaryAccessToken"] != null)
                {
                    strAccessToken = System.Web.HttpContext.Current.Application["zr_tt_WxOrdinaryAccessToken"].ToString();
                }
                return strAccessToken;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion 获得微信普通访问令牌

        #region 设置微信普通访问令牌
        /// <summary>
        /// 设置微信普通访问令牌
        /// </summary>
        /// <param name="strAccessToken">微信普通访问令牌</param>
        public void SetOrdinaryAccessToken(string strAccessToken)
        {
            try
            {
                System.Web.HttpContext.Current.Application["zr_tt_WxOrdinaryAccessToken"] = strAccessToken;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion 设置微信普通访问令牌

        #region 获得微信普通访问令牌到期时间
        /// <summary>
        /// 获得微信普通访问令牌到期时间
        /// </summary>
        /// <returns>客户端授权时间</returns>
        public DateTime GetOrdinaryAccessTokenExpire()
        {
            DateTime dtmExpire = DefaultTime;
            try
            {
                if (System.Web.HttpContext.Current.Application["zr_tt_WxOrdinaryAccessTokenExpire"] != null)
                {
                    dtmExpire = Convert.ToDateTime(System.Web.HttpContext.Current.Application["zr_tt_WxOrdinaryAccessTokenExpire"]);
                }
                return dtmExpire;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion 获得微信普通访问令牌到期时间

        #region 设置微信普通访问令牌到期时间
        /// <summary>
        /// 设置微信普通访问令牌到期时间
        /// </summary>
        /// <param name="dtmCreate">客户端授权时间</param>
        public void SetOrdinaryAccessTokenExpire(DateTime dtmExpire)
        {
            try
            {
                System.Web.HttpContext.Current.Application["zr_tt_WxOrdinaryAccessTokenExpire"] = dtmExpire;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion 设置微信普通访问令牌到期时间

        #endregion 微信普通访问令牌

        #region 微信JS接口的临时票据

        #region 获得微信JS接口的临时票据
        /// <summary>
        /// 获得微信JS接口的临时票据
        /// </summary>
        /// <returns>微信JS接口的临时票据</returns>
        public Dictionary<string, object> GetJsapiTicket(string strUrl)
        {
            Dictionary<string, object> dicWxJsapiTicket = new Dictionary<string, object>();
            try
            {
                if (System.Web.HttpContext.Current.Application[strUrl] != null)
                {
                    dicWxJsapiTicket = (Dictionary<string, object>)System.Web.HttpContext.Current.Application[strUrl];
                }
                return dicWxJsapiTicket;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion 获得微信JS接口的临时票据

        #region 设置微信JS接口的临时票据
        /// <summary>
        /// 设置微信JS接口的临时票据
        /// </summary>
        /// <param name="strJsapiTicket">微信JS接口的临时票据</param>
        public void SetJsapiTicket(string strUrl, Dictionary<string, object> dicWxJsapiTicket)
        {
            try
            {
                System.Web.HttpContext.Current.Application[strUrl] = dicWxJsapiTicket;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion 设置微信JS接口的临时票据

        #endregion 微信JS接口的临时票据

        #endregion Application

        #region 哈希加密
        /// <summary>
        /// 哈希加密
        /// </summary>
        /// <param name="strOriginal">原文</param>
        /// <returns>密文</returns>
        public string GetSwcSH1(string strOriginal)
        {
            string strCiphertext = "";
            try
            {
                SHA1 shaAlgorithm = SHA1.Create();
                byte[] bytHash = shaAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(strOriginal));
                for (int i = 0; i < bytHash.Length; i++)
                {
                    strCiphertext += bytHash[i].ToString("x2").ToUpperInvariant();
                }
                return strCiphertext;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion 哈希加密

        #region 写入日志
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="strFileName">文件名称</param>
        /// <param name="strClassName">类别名称</param>
        /// <param name="strContent">内容</param>
        public static void WriteLog(string strFileName, string strClassName, string strContent)
        {
            string strPath = "";
            try
            {
                strPath = System.Web.Hosting.HostingEnvironment.MapPath("~/") + "UtilityLog";
                if (!Directory.Exists(strPath)) Directory.CreateDirectory(strPath); //如果日志目录不存在就创建
                string strNowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");//获取当前系统时间
                strFileName = strPath + "//" + strFileName + DateTime.Now.ToString("yyyy-MM-dd") + ".log";//用日期对日志文件命名
                StreamWriter stwLog = System.IO.File.AppendText(strFileName);//创建或打开日志文件，向日志文件末尾追加记录
                strContent = strNowTime + " " + strClassName + ": " + strContent;
                stwLog.WriteLine(strContent);//向日志文件写入内容
                stwLog.Close();//关闭日志文件
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion 写入日志

    }
}