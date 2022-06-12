using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS_WXBOT_COM.model
{
    internal class MessageBody
    {
        public string content { get; set; } = "";
        public string chatroomID { get; set; }
        public string wxID { get; set; }
        public List<string> wxIDs { get; set; } = new List<string>();
        public enums.MessageType type { get; set; }
        public bool simpleAt { get; set; } = false;
        /// <summary>
        /// 目前是最后的消息
        /// </summary>
        public bool latest { get; set; } = true;
        #region PUBLIC
        /// <summary>
        /// 检查新消息是否可以合并
        /// </summary>
        /// <param name="chatroomID"></param>
        /// <param name="wxID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CheckCombine(string chatroomID,
            string wxID,
            string content,
            enums.MessageType type)
        {
            switch (type)
            {
                case enums.MessageType.Text: break;
                case enums.MessageType.AT: break;
                default: return false;
            }

            if (string.IsNullOrEmpty(chatroomID))
            {
                return wxID == this.wxID &&
                        type == this.type &&
                        latest &&
                        (this.content.Length + content.Length) <= settings.Setting.MSG_CONTENT_MAX_LENGTH;
            }
            else
            {
                if (string.IsNullOrEmpty(wxID))
                {
                    return chatroomID == this.chatroomID &&
                             string.IsNullOrEmpty(this.wxID) &&
                             type == this.type &&
                             latest &&
                             (this.content.Length + content.Length) <= settings.Setting.MSG_CONTENT_MAX_LENGTH;
                }
                else
                {
                    return chatroomID == this.chatroomID &&
                             !string.IsNullOrEmpty(this.wxID) &&
                             type == this.type &&
                             latest &&
                             (this.content.Length + content.Length) <= settings.Setting.MSG_CONTENT_MAX_LENGTH;
                }
            }

        }
        public bool CheckCombine(MessageBody msg) => CheckCombine(msg.chatroomID,
                                                                  msg.wxID,
                                                                  msg.content,
                                                                  msg.type);
        /// <summary>
        /// 新建消息
        /// </summary>
        /// <param name="chatroomID"></param>
        /// <param name="wxID"></param>
        /// <param name="content"></param>
        /// <param name="type"></param>
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
                if (!this.wxIDs.Contains(wxID)) this.wxIDs.Add(wxID);
            }
        }
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
        public MessageBody() { }
        /// <summary>
        /// 合并新消息
        /// </summary>
        /// <param name="msg">要合并的消息</param>
        /// <returns>消息长度超长后其余部分组成的消息</returns>
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
