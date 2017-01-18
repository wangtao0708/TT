using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Xml;
using System.Security.Cryptography;
using System.Text;
using LitJson;

namespace tt2.Wx.Web2
{
    public class WxData
    {


        private SortedDictionary<string, object> _ValuesDictionary = new SortedDictionary<string, object>();//采用排序的Dictionary的好处是方便对数据包进行签名，不用再签名之前再做一次排序
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="objValue"></param>
        public void SetValue(string strKey, object objValue)
        {
            _ValuesDictionary[strKey] = objValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetValue(string key)
        {
            object o = null;
            _ValuesDictionary.TryGetValue(key, out o);
            return o;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsSet(string key)
        {
            object o = null;
            _ValuesDictionary.TryGetValue(key, out o);
            if (null != o)
                return true;
            else
                return false;
        }


        /// <summary>
        /// Dictionary格式转化成url参数格式 return url格式串, 该串不包含sign字段值
        /// </summary>
        /// <returns></returns>
        public string ToUrl()
        {
            string strUrl = "";
            try
            {
                foreach (KeyValuePair<string, object> kvp in _ValuesDictionary)
                {
                    if (kvp.Value == null)
                    {
                        throw new Exception("内部含有值为null的字段!");
                    }
                    if (kvp.Key != "sign" && kvp.Value.ToString() != "")
                    {
                        strUrl += kvp.Key + "=" + kvp.Value + "&";
                    }
                }
                strUrl = strUrl.Trim('&');
                return strUrl;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SortedDictionary<string, object> FromXml(string xml)
        {
            try
            {
                if (string.IsNullOrEmpty(xml))
                {
                    throw new Exception("将空的xml串转换为WxPayData不合法!");
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                XmlNode xmlNode = xmlDoc.FirstChild;//获取到根节点<xml>
                XmlNodeList nodes = xmlNode.ChildNodes;
                foreach (XmlNode xn in nodes)
                {
                    XmlElement xe = (XmlElement)xn;
                    _ValuesDictionary[xe.Name] = xe.InnerText;//获取xml的键值对到WxPayData内部的数据中
                }

                try
                {
                    //2015-06-29 错误是没有签名
                    if (_ValuesDictionary["return_code"].ToString() != "SUCCESS")
                    {
                        return _ValuesDictionary;
                    }
                    CheckSign();//验证签名,不通过会抛异常
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                return _ValuesDictionary;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string ToXml()
        {
            try
            {
                //数据为空时不能转化为xml格式
                if (0 == _ValuesDictionary.Count)
                {
                    throw new Exception("WxPayData数据为空!");
                }

                string xml = "<xml>";
                foreach (KeyValuePair<string, object> pair in _ValuesDictionary)
                {
                    //字段值不能为null，会影响后续流程
                    if (pair.Value == null)
                    {
                        throw new Exception("WxPayData内部含有值为null的字段!");
                    }

                    if (pair.Value.GetType() == typeof(int))
                    {
                        xml += "<" + pair.Key + ">" + pair.Value + "</" + pair.Key + ">";
                    }
                    else if (pair.Value.GetType() == typeof(string))
                    {
                        xml += "<" + pair.Key + ">" + "<![CDATA[" + pair.Value + "]]></" + pair.Key + ">";
                    }
                    else//除了string和int类型不能含有其他数据类型
                    {
                        throw new Exception("WxPayData字段数据类型错误!");
                    }
                }
                xml += "</xml>";
                return xml;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public string ToJson()
        {
            try
            {
                string jsonStr = JsonMapper.ToJson(_ValuesDictionary);
                return jsonStr;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #region 检查签名
        /// <summary>
        /// 检查签名
        /// </summary>
        /// <returns></returns>
        public bool CheckSign()
        {
            try
            {
                //如果没有设置签名，则跳过检测
                if (!IsSet("sign"))
                {
                    throw new Exception("签名不存在!");
                }
                //如果设置了签名但是签名为空，则抛异常
                else if (GetValue("sign") == null || GetValue("sign").ToString() == "")
                {
                    throw new Exception("签名存在但不合法!");
                }

                //获取接收到的签名
                string return_sign = GetValue("sign").ToString();

                //在本地计算新的签名
                string cal_sign = MakeSign();

                if (cal_sign == return_sign)
                {
                    return true;
                }
                throw new Exception("签名验证错误!");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion 检查签名

        #region MD5签名
        /// <summary>
        /// MD5签名
        /// </summary>
        /// <returns></returns>
        public string MakeSign()
        {
            try
            {
                //转url格式
                string str = ToUrl();
                //在string后加入API KEY
                str += "&key=" + WxConfig.ApiKey;
                //MD5加密
                var md5 = MD5.Create();
                var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                var sb = new StringBuilder();
                foreach (byte b in bs)
                {
                    sb.Append(b.ToString("x2"));
                }
                //所有字符转为大写
                return sb.ToString().ToUpper();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion MD5签名

    }
}