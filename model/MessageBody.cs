using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS_WXBOT_COM.model
{
    /// <summary>
    /// 发送消息类
    /// </summary>
    internal class MessageBody
    {
        /// <summary>
        /// 消息内容（文字/图片路径/文件路径）
        /// </summary>
        public string content { get; set; } = "";
        /// <summary>
        /// 要发送的群聊ID
        /// </summary>
        public string chatroomID { get; set; }
        /// <summary>
        /// 要发送的个人ID
        /// 当消息为群聊且带有AT时，此处存放其中一个AT目标ID
        /// </summary>
        public string wxID { get; set; }
        /// <summary>
        /// 要AT的个人ID列表
        /// </summary>
        public List<string> wxIDs { get; set; } = new List<string>();
        /// <summary>
        /// 消息类型
        /// </summary>
        public enums.MessageType type { get; set; }
        /// <summary>
        /// 是否是简易AT（已弃用）
        /// </summary>
        public bool simpleAt { get; set; } = false;
        /// <summary>
        /// 目前是最后的消息
        /// </summary>
        public bool latest { get; set; } = true;
        #region PUBLIC
        /// <summary>
        /// 检查新消息是否可以合并
        /// </summary>
        /// <param name="chatroomID">群聊ID</param>
        /// <param name="wxID">AT目标 或 私聊ID</param>
        /// <param name="content">消息内容</param>
        /// <param name="type">消息类型</param>
        /// <returns></returns>
        public bool CheckCombine(string chatroomID,
            string wxID,
            string content,
            enums.MessageType type)
        {
            switch (type)
            {
                case enums.MessageType.Text:
                case enums.MessageType.AT:
                    //文字/AT消息可以进行合并
                    break;
                default:
                    return false;
            }

            if (string.IsNullOrEmpty(chatroomID))
            {
                /*
                 * 发送给私聊的消息，合并判定条件：
                 * 1. 相同的私聊ID
                 * 2. 相同的消息类型
                 * 3. 消息文字合并后不超过设定的最高长度             
                 */
                return wxID == this.wxID &&
                        type == this.type &&
                        latest &&
                        (this.content.Length + content.Length) <= settings.Setting.MSG_CONTENT_MAX_LENGTH;
            }
            else
            {
                if (string.IsNullOrEmpty(wxID))
                {
                    /*
                     * 发送给群聊的消息，且不含有AT目标，合并判定条件：
                     * 1. 相同的群聊ID
                     * 2. 不含有AT目标ID
                     * 3. 消息文字合并后不超过设定的最高长度             
                     */
                    return chatroomID == this.chatroomID &&
                             string.IsNullOrEmpty(this.wxID) &&
                             type == this.type &&
                             latest &&
                             (this.content.Length + content.Length) <= settings.Setting.MSG_CONTENT_MAX_LENGTH;
                }
                else
                {
                    /*
                     * 发送给群聊的消息，且含有AT目标，合并判定条件：
                     * 1. 相同的群聊ID
                     * 2. 含有AT目标ID
                     * 3. 消息文字合并后不超过设定的最高长度             
                     */
                    return chatroomID == this.chatroomID &&
                             !string.IsNullOrEmpty(this.wxID) &&
                             type == this.type &&
                             latest &&
                             (this.content.Length + content.Length) <= settings.Setting.MSG_CONTENT_MAX_LENGTH;
                }
            }

        }
        /// <summary>
        /// 检查新消息是否可以合并
        /// </summary>
        /// <param name="msg">发送消息类</param>
        /// <returns></returns>
        public bool CheckCombine(MessageBody msg) => CheckCombine(msg.chatroomID,
                                                                  msg.wxID,
                                                                  msg.content,
                                                                  msg.type);
        /// <summary>
        /// 新建消息
        /// </summary>
        /// <param name="chatroomID">群聊ID</param>
        /// <param name="wxID">AT目标 或 私聊ID</param>
        /// <param name="content">消息内容</param>
        /// <param name="type">消息类型</param>
        public MessageBody(string chatroomID,
                        string wxID,
                        string content,
                        enums.MessageType type)
        {
            this.chatroomID = chatroomID;
            this.wxID = wxID;
            this.content = content;
            this.type = type;
            if (type == enums.MessageType.AT && !string.IsNullOrEmpty(chatroomID) && !string.IsNullOrEmpty(wxID))
            {
                //AT消息类型，向AT消息列表中放置AT目标ID
                if (!this.wxIDs.Contains(wxID)) this.wxIDs.Add(wxID);
            }
        }
        /// <summary>
        /// 新建消息
        /// </summary>
        /// <param name="chatroomID">群聊ID</param>
        /// <param name="wxIDs">AT目标</param>
        /// <param name="content">消息内容</param>
        /// <param name="type">消息类型（应为AT）</param>
        public MessageBody(string chatroomID,
                     List<string> wxIDs,
                     string content,
                     enums.MessageType type)
        {
            this.chatroomID = chatroomID;
            this.wxID = wxIDs.First();
            this.content = content;
            this.type = type;
            foreach (var id in wxIDs)
            {
                if (string.IsNullOrEmpty(id)) continue;
                if (!this.wxIDs.Contains(id)) this.wxIDs.Add(id);
            }
            this.latest = false;
        }
        /// <summary>
        /// 合并新消息
        /// </summary>
        /// <param name="msg">要合并的消息</param>
        /// <returns>TRUE-合并成功，FALSE-合并失败</returns>
        public bool Combine(MessageBody msg)
        {
            if (!CheckCombine(msg)) return false;

            content += "\n----------\n";
            content += msg.content;

            if (type == enums.MessageType.AT && !string.IsNullOrEmpty(chatroomID) && !string.IsNullOrEmpty(wxID))
            {
                if (!this.wxIDs.Contains(msg.wxID)) this.wxIDs.Add(msg.wxID);
            }
            else
            {
                this.wxID = msg.wxID;
            }

            return true;
        }
        #endregion
    }
}
