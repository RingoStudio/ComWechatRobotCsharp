using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS_WXBOT_COM.enums
{
    public enum MessageType
    {
        /// <summary>
        /// 文字消息
        /// </summary>
        Text = 1,
        /// <summary>
        /// 图片消息
        /// </summary>
        Image = 3,

        USER_LIST_INFO = 1004,
        FILE = 1005,
        AT = 1006,
        /// <summary>
        /// 服务号群发的消息
        /// </summary>
        ServiceGroup = 49,

        WS_HEARTBEAT = 5005,
        GROUP_MEMBER_REPLY_1 = 5010,
        GROUP_MEMBER_REPLY = 5010,
        GROUP_MEMBER_INFO = 5020,
        DEBUG_INFO = 6000,
        USER_INFO = 6500,
        USER_INFO_1 = 6550,
        DEFAULT = 9999,
        /// <summary>
        /// 未定义
        /// </summary>
        UNKNOW = -1,
    }
}
