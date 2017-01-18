using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web;
using System.Net;
using System.IO;

namespace tt2.Wx.Web2
{
    public class WxApi
    {

        public static WxData UnifiedOrder(WxData inputObj, int timeOut = 60)
        {
            try
            {
                string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
                //检测必填参数
                if (!inputObj.IsSet("out_trade_no"))
                {
                    throw new Exception("缺少统一支付接口必填参数out_trade_no！");
                }
                else if (!inputObj.IsSet("body"))
                {
                    throw new Exception("缺少统一支付接口必填参数body！");
                }
                else if (!inputObj.IsSet("total_fee"))
                {
                    throw new Exception("缺少统一支付接口必填参数total_fee！");
                }
                else if (!inputObj.IsSet("trade_type"))
                {
                    throw new Exception("缺少统一支付接口必填参数trade_type！");
                }

                //关联参数
                if (inputObj.GetValue("trade_type").ToString() == "JSAPI" && !inputObj.IsSet("openid"))
                {
                    throw new Exception("统一支付接口中，缺少必填参数openid！trade_type为JSAPI时，openid为必填参数！");
                }
                if (inputObj.GetValue("trade_type").ToString() == "NATIVE" && !inputObj.IsSet("product_id"))
                {
                    throw new Exception("统一支付接口中，缺少必填参数product_id！trade_type为NATIVE时，product_id为必填参数！");
                }

                //异步通知url未设置，则使用配置文件中的url
                if (!inputObj.IsSet("notify_url"))
                {
                    inputObj.SetValue("notify_url", WxConfig.NotifyUrl);//异步通知url
                }

                inputObj.SetValue("appid", WxConfig.AppID);//公众账号ID
                inputObj.SetValue("mch_id", WxConfig.MchID);//商户号
                inputObj.SetValue("spbill_create_ip", WxConfig.IP);//终端ip	  	    
                inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
                //签名
                inputObj.SetValue("sign", inputObj.MakeSign());
                string xml = inputObj.ToXml();
                var start = DateTime.Now;
                string response = HttpService.Post(xml, url, false, timeOut);
                var dtmEnd = DateTime.Now;
                int intTimeCost = (int)((dtmEnd - start).TotalMilliseconds);
                WxData wxdResult = new WxData();
                wxdResult.FromXml(response);
                //ReportCostTime(url, intTimeCost, wxdResult);//测速上报
                return wxdResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static WxData OrderQuery(WxData inputObj, int timeOut = 60)
        {
            try
            {
                string url = "https://api.mch.weixin.qq.com/pay/orderquery";
                //检测必填参数
                if (!inputObj.IsSet("out_trade_no") && !inputObj.IsSet("transaction_id"))
                {
                    throw new Exception("订单查询接口中，out_trade_no、transaction_id至少填一个！");
                }

                inputObj.SetValue("appid", WxConfig.AppID);//公众账号ID
                inputObj.SetValue("mch_id", WxConfig.MchID);//商户号
                inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
                inputObj.SetValue("sign", inputObj.MakeSign());//签名

                string xml = inputObj.ToXml();

                var start = DateTime.Now;
                string response = HttpService.Post(xml, url, false, timeOut);//调用HTTP通信接口提交数据

                var dtmEnd = DateTime.Now;
                int intTimeCost = (int)((dtmEnd - start).TotalMilliseconds);//获得接口耗时

                //将xml格式的数据转化为对象以返回
                WxData wxdResult = new WxData();
                wxdResult.FromXml(response);

                ReportCostTime(url, intTimeCost, wxdResult);//测速上报

                return wxdResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static string GenerateTimeStamp()
        {
            try
            {
                TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                return Convert.ToInt64(ts.TotalSeconds).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GenerateNonceStr()
        {
            try
            {
                return Guid.NewGuid().ToString().Replace("-", "");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void ReportCostTime(string interface_url, int intTimeCost, WxData inputObj)
        {
            try
            {
                //如果不需要进行上报
                if (WxConfig.REPORT_LEVENL == 0) return;

                //如果仅失败上报
                if (WxConfig.REPORT_LEVENL == 1 && inputObj.IsSet("return_code") && inputObj.GetValue("return_code").ToString() == "SUCCESS" && inputObj.IsSet("result_code") && inputObj.GetValue("result_code").ToString() == "SUCCESS") return;

                //上报逻辑
                WxData data = new WxData();
                data.SetValue("interface_url", interface_url);
                data.SetValue("execute_time_", intTimeCost);
                //返回状态码
                if (inputObj.IsSet("return_code"))
                {
                    data.SetValue("return_code", inputObj.GetValue("return_code"));
                }
                //返回信息
                if (inputObj.IsSet("return_msg"))
                {
                    data.SetValue("return_msg", inputObj.GetValue("return_msg"));
                }
                //业务结果
                if (inputObj.IsSet("result_code"))
                {
                    data.SetValue("result_code", inputObj.GetValue("result_code"));
                }
                //错误代码
                if (inputObj.IsSet("err_code"))
                {
                    data.SetValue("err_code", inputObj.GetValue("err_code"));
                }
                //错误代码描述
                if (inputObj.IsSet("err_code_des"))
                {
                    data.SetValue("err_code_des", inputObj.GetValue("err_code_des"));
                }
                //商户订单号
                if (inputObj.IsSet("out_trade_no"))
                {
                    data.SetValue("out_trade_no", inputObj.GetValue("out_trade_no"));
                }
                //设备号
                if (inputObj.IsSet("device_info"))
                {
                    data.SetValue("device_info", inputObj.GetValue("device_info"));
                }

                try
                {
                    Report(data);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static WxData Report(WxData inputObj, int timeOut = 30)
        {
            try
            {
                string url = "https://api.mch.weixin.qq.com/payitil/report";
                //检测必填参数
                if (!inputObj.IsSet("interface_url"))
                {
                    throw new Exception("接口URL，缺少必填参数interface_url！");
                }
                if (!inputObj.IsSet("return_code"))
                {
                    throw new Exception("返回状态码，缺少必填参数return_code！");
                }
                if (!inputObj.IsSet("result_code"))
                {
                    throw new Exception("业务结果，缺少必填参数result_code！");
                }
                if (!inputObj.IsSet("user_ip"))
                {
                    throw new Exception("访问接口IP，缺少必填参数user_ip！");
                }
                if (!inputObj.IsSet("execute_time_"))
                {
                    throw new Exception("接口耗时，缺少必填参数execute_time_！");
                }

                inputObj.SetValue("appid", WxConfig.AppID);//公众账号ID
                inputObj.SetValue("mch_id", WxConfig.MchID);//商户号
                inputObj.SetValue("user_ip", WxConfig.IP);//终端ip
                inputObj.SetValue("time", DateTime.Now.ToString("yyyyMMddHHmmss"));//商户上报时间	 
                inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
                inputObj.SetValue("sign", inputObj.MakeSign());//签名
                string xml = inputObj.ToXml();
                string response = HttpService.Post(xml, url, false, timeOut);
                WxData wxdResult = new WxData();
                wxdResult.FromXml(response);
                return wxdResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static string GenerateOutTradeNo()
        {
            var ran = new Random();
            return string.Format("{0}{1}{2}", WxConfig.MchID, DateTime.Now.ToString("yyyyMMddHHmmss"), ran.Next(999));
        }

        public static WxData Refund(WxData wxdRefund, int intTimeOut = 60)
        {
            string url = "https://api.mch.weixin.qq.com/secapi/pay/refund";
            try
            {
                //检测必填参数
                if (!wxdRefund.IsSet("out_trade_no") && !wxdRefund.IsSet("transaction_id"))
                {
                    throw new Exception("退款申请接口中，out_trade_no、transaction_id至少填一个！");
                }
                else if (!wxdRefund.IsSet("out_refund_no"))
                {
                    throw new Exception("退款申请接口中，缺少必填参数out_refund_no！");
                }
                else if (!wxdRefund.IsSet("total_fee"))
                {
                    throw new Exception("退款申请接口中，缺少必填参数total_fee！");
                }
                else if (!wxdRefund.IsSet("refund_fee"))
                {
                    throw new Exception("退款申请接口中，缺少必填参数refund_fee！");
                }
                else if (!wxdRefund.IsSet("op_user_id"))
                {
                    throw new Exception("退款申请接口中，缺少必填参数op_user_id！");
                }

                wxdRefund.SetValue("appid", WxConfig.AppID);//公众账号ID
                wxdRefund.SetValue("mch_id", WxConfig.MchID);//商户号
                wxdRefund.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));//随机字符串
                wxdRefund.SetValue("sign", wxdRefund.MakeSign());//签名

                string xml = wxdRefund.ToXml();
                var start = DateTime.Now;
                string response = HttpService.Post(xml, url, true, intTimeOut);//调用HTTP通信接口提交数据到API
                //var dtmEnd = DateTime.Now;
                //int intTimeCost = (int)((dtmEnd - start).TotalMilliseconds);//获得接口耗时
                //将xml格式的结果转换为对象以返回
                WxData wxdResult = new WxData();
                wxdResult.FromXml(response);

                //ReportCostTime(url, intTimeCost, wxdResult);//测速上报

                return wxdResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


    }

}