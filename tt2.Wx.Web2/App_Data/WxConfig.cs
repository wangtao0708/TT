using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tt2.Wx.Web2
{
    public class WxConfig
    {
        #region 都江堰测试
        //public const string AppID = "wxd5879c37385a3917";
        //public const string AppSecret = "ccef9856a7b04a68e2c825917e31704e";
        //public const string MchID = "1431787602";
        //public const string ApiKey = "ZRTT1606zrtt1606ZRTT1606zrtt1606";
        #endregion 都江堰测试
        #region 都江堰正式
        public const string AppID = "wx6c01eed0a34a58cc";
        public const string AppSecret = "79bf5e479f1a9c6a288ec4642aa1f42b";
        public const string MchID = "1348908801";
        public const string ApiKey = "ZRTT1606zrtt1606ZRTT1606zrtt1606";
        #endregion 都江堰正式
        public const string AuthorizeUrl = "https://open.weixin.qq.com/connect/oauth2/authorize?";
        public const string AccessTokenUrl = "https://api.weixin.qq.com/sns/oauth2/access_token?";
        public const string UserUrl = "https://api.weixin.qq.com/sns/userinfo?";

        public const string NotifyUrl = "http://nfyapi.nihaott.com/Weixin/UnifiedPayNotify";

        //public const string SslcertPath = @"WxCert\test\apiclient_cert.p12";//测试
        public const string SslcertPath = @"WxCert\apiclient_cert.p12";//正式
        //public const string SslcertPassword = "1431787602";//测试
        public const string SslcertPassword = "1348908801";//正式

        public const string IP = "123.57.248.30";

        //=======【代理服务器设置】===================================
        /* 默认IP和端口号分别为0.0.0.0和0，此时不开启代理（如有需要才设置）
        */
        public const string PROXY_URL = "http://0.0.0.0:0";

        //=======【上报信息配置】===================================
        /* 测速上报等级，0.关闭上报; 1.仅错误时上报; 2.全量上报
        */
        public const int REPORT_LEVENL = 0;

        //=======【日志级别】===================================
        /* 日志等级，0.不输出日志；1.只输出错误信息; 2.输出错误和正常信息; 3.输出错误信息、正常信息和调试信息
        */
        public const int LOG_LEVENL = 3;

    }
}