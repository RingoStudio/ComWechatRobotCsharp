using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS_WXBOT_COM.events
{
    /// <summary>
    /// 接收消息事件数据类
    /// </summary>
    public class ReceivedMessageEventArgs : System.EventArgs
    {
        #region FIELDS
        /// <summary>
        /// 消息体
        /// </summary>
        public Dictionary<string, string> message { get; private set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public enums.MessageType type { set; get; }
        /// <summary>
        /// 消息来源类型
        /// </summary>
        public enums.RoomType roomType { set; get; }
        #endregion
        /// <summary>
        /// 新建类
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
