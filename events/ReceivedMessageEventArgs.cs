using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS_WXBOT_COM.events
{
    public class ReceivedMessageEventArgs : System.EventArgs
    {
        #region FIELDS
        public Dictionary<string, string> message { get; private set; }

        public enums.MessageType type { set; get; }
        public enums.RoomType roomType { set; get; }
        #endregion
        /// <summary>
        /// 消息事件参数
        /// </summary>
        /// <param name="message">消息体</param>
        /// <param name="type">消息类型</param>
        /// <param name="roomType">来源类型</param>
        internal ReceivedMessageEventArgs(Dictionary<string, string> message,
            enums.MessageType type,
            enums.RoomType roomType)
        {
            this.message = message;
            this.type = type;
            this.roomType = roomType;
        }

    }
}
