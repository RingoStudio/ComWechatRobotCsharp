using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS_WXBOT_COM.model
{
    /// <summary>
    /// 微信成员信息类
    /// </summary>
    public class WxUserInfo
    {
        /// <summary>
        /// 原始 id
        /// </summary>
        public string WxId { get; set; }
        /// <summary>
        /// 修改过的微信号
        /// </summary>
        public string WxNumber { get; set; }
        public string WxV3 { get; set; }
        public string WxRemark { get; set; }
        public string WxNickName { get; set; }
        public string WxBigAvatar { get; set; }
        public string WxSmallAvatar { get; set; }
        public string WxSignature { get; set; }
        public string WxNation { get; set; }
        public string WxProvince { get; set; }
        public string WxCity { get; set; }
        public string WxBackground { get; set; }
        public string PhoneNumber { get; set; }
    }
}
