using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS_WXBOT_COM.enums
{
    public enum RoomType
    {
        /// <summary>
        /// 私聊消息
        /// </summary>
        Private = 0,
        /// <summary>
        /// 群聊消息
        /// </summary>
        Group = 1,
        /// <summary>
        /// 服务号消息
        /// </summary>
        Service = 2,
        /// <summary>
        /// 订阅号消息
        /// </summary>
        Subscription = 3,
        /// <summary>
        /// 系统信息
        /// </summary>
        System=999,

        UNKNOW=99999,
    }
}
