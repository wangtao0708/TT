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

namespace tt2.Wx.Web2.Controllers
{
    public class WeixinController : BaseController
    {

        #region 微信访问入口页，获取用户UnionID和OpenID
        /// <summary>
        /// 微信访问入口页，获取用户UnionID和OpenID
        /// </summary>
        /// <param name="intVisit">访问：1首页；2定制；3我的；4管家主页；5商家主页</param>
        /// <param name="strTargetID">目标编码：4管家编码；5商家编码</param>
        /// <returns></returns>
        public ActionResult Index(int intVisit=0,string strTargetID="")
        {
            ViewBag.UnionID = "";
            ViewBag.OpenID = "";
            ViewBag.Visit = intVisit;
            ViewBag.TargetID = strTargetID;
            ViewBag.ErrorMessage = "";
            string strWxCode = "", strVisitResult="", strAccessToken="";
            Dictionary<string, object> dicResult = null;
            try
            {
                if (string.IsNullOrEmpty(Request.QueryString["code"]))
                {
                    #region 获取Code
                    string strMyUrl = "http://" + Request.Url.Host + Request.Path + "?intVisit=" + intVisit + "&strTargetID=" + strTargetID;
                    WxData wxdCode = new WxData();
                    wxdCode.SetValue("appid", WxConfig.AppID);
                    wxdCode.SetValue("redirect_uri", HttpUtility.UrlEncode(strMyUrl));
                    wxdCode.SetValue("response_type", "code");
                    //wxdAuthorize.SetValue("scope", "snsapi_base");
                    wxdCode.SetValue("scope", "snsapi_userinfo");
                    wxdCode.SetValue("state", "STATE" + "#wechat_redirect");
                    Response.Redirect(WxConfig.AuthorizeUrl + wxdCode.ToUrl());
                    #endregion 获取Code
                }
                else
                {
                    strWxCode = Request.QueryString["code"].Trim();
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    #region 获取OpenID
                    WxData wxdOpen = new WxData();
                    wxdOpen.SetValue("appid", WxConfig.AppID);
                    wxdOpen.SetValue("secret", WxConfig.AppSecret);
                    wxdOpen.SetValue("code", strWxCode);
                    wxdOpen.SetValue("grant_type", "authorization_code");
                    strVisitResult = WxHttp.Get(WxConfig.AccessTokenUrl + wxdOpen.ToUrl());
                    dicResult = (Dictionary<string, object>)jss.Deserialize(strVisitResult, typeof(Dictionary<string, object>));
                    if (dicResult.ContainsKey("errcode")) throw new Exception("获取网页用户授权出错：" + dicResult["errmsg"].ToString());
                    strAccessToken = dicResult["access_token"].ToString();
                    ViewBag.OpenID = dicResult["openid"].ToString();
                    #endregion 获取OpenID

                    #region 获取UnionID
                    strVisitResult = WxHttp.Get(WxConfig.UserUrl + "access_token=" + strAccessToken + "&openid=" + ViewBag.OpenID + "&lang=zh_CN");
                    dicResult = (Dictionary<string, object>)jss.Deserialize(strVisitResult, typeof(Dictionary<string, object>));
                    if (dicResult.ContainsKey("errcode")) throw new Exception("获取用户信息出错：" + dicResult["errmsg"].ToString());
                    ViewBag.UnionID = dicResult["unionid"].ToString();
                    #endregion 获取UnionID

                }
            }
            catch (Exception ex)
            {
                WriteLog("Weixin", "Index", "Exception：" + ex.Message.ToString());
                ViewBag.ErrorMessage = ex.Message.ToString();
            }
            return View();
        }
        #endregion 微信访问入口页，获取用户UnionID和OpenID

        #region 微信JS接口授权
        /// <summary>
        /// 微信JS接口授权
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult WxJsapiAuthorize()
        {
            string strUrl = "";
            string strGet = "";
            string strAccessToken = "";//令牌
            string strJsapiTicket = "";//票据
            DateTime dtmTicketExpire = DefaultTime;//票据到期时间
            string strResult = "";
            JsonData jsdResult;
            JsonResult jsrReturn = new JsonResult();
            string strNonce = "";
            string strTimestamp = "";
            string strSignature = "";
            try
            {
                if (string.IsNullOrEmpty(Request.Form["strUrl"])) throw new Exception("strUrl为空！");
                strUrl = Request.Form["strUrl"].Trim();
                WriteLog("Weixin", "WxJsapiAuthorize", "strUrl：" + strUrl);
                Dictionary<string, object> dicTicket = new Dictionary<string, object>();
                dicTicket = GetJsapiTicket(strUrl);
                if (dicTicket.Count<=0)
                {
                    #region 获得微信JS接口的临时票据
                    strGet = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + WxConfig.AppID + "&secret=" + WxConfig.AppSecret;
                    strResult = WxHttp.Get(strGet);
                    jsdResult = JsonMapper.ToObject(strResult);
                    strAccessToken = (string)jsdResult["access_token"];
                    WriteLog("Weixin", "WxJsapiAuthorize", "strAccessToken：" + strAccessToken);
                    strGet = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + strAccessToken + "&type=jsapi";
                    dtmTicketExpire = DateTime.Now;
                    strResult = WxHttp.Get(strGet);
                    jsdResult = JsonMapper.ToObject(strResult);
                    strJsapiTicket = (string)jsdResult["ticket"];
                    WriteLog("Weixin", "WxJsapiAuthorize", "strJsapiTicket：" + strJsapiTicket);
                    dtmTicketExpire = dtmTicketExpire.AddMinutes(110);
                    dicTicket = new Dictionary<string, object>();
                    dicTicket.Add("Ticket", strJsapiTicket);
                    dicTicket.Add("Expire", dtmTicketExpire);
                    SetJsapiTicket(strUrl,dicTicket);
                    #endregion 获得微信JS接口的临时票据
                }
                else
                {
                    if (dicTicket.ContainsKey("Ticket")) strJsapiTicket= (string)dicTicket["Ticket"];
                    if (dicTicket.ContainsKey("Expire")) dtmTicketExpire = (DateTime)dicTicket["Expire"];
                    if(dtmTicketExpire <= DateTime.Now || strJsapiTicket == ""){
                        #region 获得微信JS接口的临时票据
                        strGet = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + WxConfig.AppID + "&secret=" + WxConfig.AppSecret;
                        strResult = WxHttp.Get(strGet);
                        jsdResult = JsonMapper.ToObject(strResult);
                        strAccessToken = (string)jsdResult["access_token"];
                        WriteLog("Weixin", "WxJsapiAuthorize", "strAccessToken：" + strAccessToken);
                        strGet = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + strAccessToken + "&type=jsapi";
                        dtmTicketExpire = DateTime.Now;
                        strResult = WxHttp.Get(strGet);
                        jsdResult = JsonMapper.ToObject(strResult);
                        strJsapiTicket = (string)jsdResult["ticket"];
                        WriteLog("Weixin", "WxJsapiAuthorize", "strJsapiTicket：" + strJsapiTicket);
                        dtmTicketExpire = dtmTicketExpire.AddMinutes(110);
                        dicTicket = new Dictionary<string, object>();
                        dicTicket.Add("Ticket", strJsapiTicket);
                        dicTicket.Add("Expire", dtmTicketExpire);
                        SetJsapiTicket(strUrl, dicTicket);
                        #endregion 获得微信JS接口的临时票据
                    }
                }
                WxData wxdTicket = new WxData();
                strNonce = WxApi.GenerateNonceStr();
                wxdTicket.SetValue("noncestr", strNonce);
                wxdTicket.SetValue("jsapi_ticket", strJsapiTicket);
                strTimestamp = WxApi.GenerateTimeStamp();
                wxdTicket.SetValue("timestamp", strTimestamp);
                wxdTicket.SetValue("url", strUrl);
                string strParam = wxdTicket.ToUrl();
                WriteLog("Weixin", "WxJsapiAuthorize", "strParam：" + strParam);
                strSignature = GetSwcSH1(strParam);// FormsAuthentication.HashPasswordForStoringInConfigFile(strParam, "SHA1");
                WriteLog("Weixin", "WxJsapiAuthorize", "strSignature：" + strSignature);
                jsrReturn.Data = new { Result = 1, AppID = WxConfig.AppID, Timestamp = strTimestamp, Noncestr = strNonce, Signature = strSignature };
            }
            catch (Exception ex)
            {
                WriteLog("Weixin", "WxJsapiAuthorize", "Exception：" + ex.Message.ToString());
                jsrReturn.Data = new { Result = -99, Message = ex.Message.ToString() };
            }
            return jsrReturn;
        }
        #endregion 微信JS接口授权




    }
}