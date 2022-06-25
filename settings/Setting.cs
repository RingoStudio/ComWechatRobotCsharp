using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS_WXBOT_COM.settings
{
    /// <summary>
    /// 设置类
    /// </summary>
    internal class Setting
    {
        /// <summary>
        /// 单挑消息最长文本长度
        /// </summary>
        public static int MSG_CONTENT_MAX_LENGTH = 400;
        /// <summary>
        /// 消息发送队列最小时间间隔
        /// </summary>
        public static int MSG_SEND_INTERVAL_MIN = 8 * 1000;
        /// <summary>
        /// 消息发送队列最大时间间隔
        /// </summary>
        public static int MSG_SEND_INTERVAL_MAX = 16 * 1000;
        /// <summary>
        /// 消息接收时间间隔
        /// </summary>
        public static int MSG_RECEIVE_INTERVAL = 100;
        /// <summary>
        /// 插队队列连续发送次数
        /// </summary>
        public static int MSG_SEND_HIGH_TIMES = 3;
    }
}
