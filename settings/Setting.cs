using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS_WXBOT_COM.settings
{
    internal class Setting
    {
        public static int MSG_CONTENT_MAX_LENGTH = 400;
        public static int MSG_SEND_INTERVAL_MIN = 8 * 1000;
        public static int MSG_SEND_INTERVAL_MAX = 16 * 1000;
        public static int MSG_RECEIVE_INTERVAL = 100;
        /// <summary>
        /// 插队队列连续发送次数
        /// </summary>
        public static int MSG_SEND_HIGH_TIMES = 3;
    }
}
