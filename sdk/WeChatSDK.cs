using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RS_WXBOT_COM.events;
using Google.Protobuf;

namespace RS_WXBOT_COM.sdk
{
    public class WeChatSDK
    {
        public const string TAG = "WechatSDK";
        private static WeChatSDK _instance;
        #region FIELDS  
        private WeChatRobotCOMLib.WeChatRobotClass wx;
        private Thread receivingT = null;
        private Thread sendingT = null;

        private ConcurrentQueue<model.MessageBody> messages_high = new ConcurrentQueue<model.MessageBody>();
        private ConcurrentQueue<model.MessageBody> messages_low = new ConcurrentQueue<model.MessageBody>();

        private Random intervalRand = new Random();

        public const string MSG_TYPE = "type";
        public const string MSG_SENDER = "sender";
        public const string MSG_WXID = "wxid";
        public const string MSG_MESSAGE = "message";
        public const string MSG_FILEPATH = "filepath";

        public const string MSG_NICK_SPLITER = "<nick\\>";

        private bool _Stop_Thread_Flag = false;

        public int SendQueueLen { get => messages_high.Count + messages_low.Count; }
        #endregion

        #region EVENT
        /// <summary>
        /// 消息事件回调
        /// </summary>
        /// <typeparam name="TEventArgs">事件参数类型</typeparam>
        /// <param name="sender">Bot Id</param>
        /// <param name="eventArgs">事件参数</param>
        public delegate ValueTask ReceivedMessageCallBackHandler<in TEventArgs>(TEventArgs eventArgs) where TEventArgs : System.EventArgs;

        public event ReceivedMessageCallBackHandler<ReceivedMessageEventArgs> OnReceivedMessageAsync;
        private async void RaiseReceivedMessage(Dictionary<string, string> content, enums.MessageType messageType, enums.RoomType roomType)
        {
            if (content == null || content.Count <= 0)
                return;
            if (OnReceivedMessageAsync == null)
                return;
            await OnReceivedMessageAsync(new ReceivedMessageEventArgs(content, messageType, roomType));
        }
        #endregion

        public static WeChatSDK Instance
        {
            get
            {
                if (_instance == null) _instance = new WeChatSDK();
                return _instance;
            }
        }

        public WeChatSDK() => wx = new WeChatRobotCOMLib.WeChatRobotClass();

        #region PUBLIC
        public bool Stop()
        {
            try
            {
                _Stop_Thread_Flag = true;
                wx.CStopRobotService();
                Thread.Sleep(2000);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 开始发送和接收
        /// </summary>
        public bool Start()
        {
            try
            {
                int idx = 0;
                do
                {
                    wx.CStartRobotService();
                    if (!string.IsNullOrEmpty(GetSelfInfo()))
                    {
                        goto INIT_FINISHED;
                    }
                    idx++;
                } while (idx < 3);
                return false;
            }
            catch (Exception)
            {
                return false;
            }
INIT_FINISHED:

            _Stop_Thread_Flag = false;

            receivingT = new Thread(() =>
            {
                StartReceiving();
            });
            receivingT.Name = "listening";
            receivingT.IsBackground = true;
            receivingT.Start();
            Console.WriteLine($"WechatSDK 启动监听");


            sendingT = new Thread(() =>
            {
                StartSending();
            });
            sendingT.Name = "sending";
            sendingT.IsBackground = true;
            sendingT.Start();
            Console.WriteLine($"WechatSDK 启动发送");

            return true;
        }

        /// <summary>
        /// 发送文本
        /// </summary>
        /// <param name="chatroomID"></param>
        /// <param name="wxID"></param>
        /// <param name="content"></param>
        /// <param name="isLow"></param>
        public void SendTxtMsg(string chatroomID, string wxID, string content, bool isLow)
        {
            if (string.IsNullOrEmpty(content)) return;
            if (string.IsNullOrEmpty(chatroomID))
            {
                if (string.IsNullOrEmpty(wxID)) return;
                AddSendMessage(content, "", wxID, enums.MessageType.Text, isLow);
            }
            else
            {
                AddSendMessage(content, chatroomID, wxID, enums.MessageType.Text, isLow);
            }
        }
        /// <summary>
        /// 发送at
        /// </summary>
        /// <param name="chatroomID"></param>
        /// <param name="wxID"></param>
        /// <param name="content"></param>
        /// <param name="isLow"></param>
        public void SendAtMsg(string chatroomID, string wxID, string content, bool isLow)
        {
            if (string.IsNullOrEmpty(chatroomID) || string.IsNullOrEmpty(wxID) || string.IsNullOrEmpty(content)) return;
            AddSendMessage(content, chatroomID, wxID, enums.MessageType.AT, isLow);
        }
        /// <summary>
        /// 发送at
        /// </summary>
        /// <param name="chatroomID"></param>
        /// <param name="wxIDs">目标wxid列表</param>
        /// <param name="content"></param>
        /// <param name="isLow"></param>
        public void SendAtMsg(string chatroomID, List<string> wxIDs, string content, bool isLow)
        {
            if (string.IsNullOrEmpty(chatroomID) || wxIDs == null || wxIDs.Count <= 0 || string.IsNullOrEmpty(content)) return;
            AddSendMessage(content, chatroomID, wxIDs, isLow);
        }
        /// <summary>
        /// 发送图片
        /// </summary>
        /// <param name="chatroomID"></param>
        /// <param name="wxID"></param>
        /// <param name="path"></param>
        /// <param name="isLow"></param>
        public void SendPic(string chatroomID, string wxID, string path, bool isLow)
        {
            if (string.IsNullOrEmpty(path)) return;
            if (!System.IO.File.Exists(path)) return;
            if (!string.IsNullOrEmpty(chatroomID))
            {
                AddSendMessage(path, chatroomID, "", enums.MessageType.Image, isLow);
                return;
            }
            if (!string.IsNullOrEmpty(wxID))
            {
                AddSendMessage(path, "", wxID, enums.MessageType.Image, isLow);
                return;
            }
        }
        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="chatroomID"></param>
        /// <param name="wxID"></param>
        /// <param name="path"></param>
        /// <param name="isLow"></param>
        public void SendFile(string chatroomID, string wxID, string path, bool isLow)
        {
            if (string.IsNullOrEmpty(path)) return;
            if (!System.IO.File.Exists(path)) return;
            if (!string.IsNullOrEmpty(chatroomID))
            {
                AddSendMessage(path, chatroomID, "", enums.MessageType.FILE, isLow);
                return;
            }
            if (!string.IsNullOrEmpty(wxID))
            {
                AddSendMessage(path, "", wxID, enums.MessageType.FILE, isLow);
                return;
            }
        }
        /// <summary>
        /// 获取我的信息
        /// </summary>
        /// <returns>json string</returns>
        public string GetSelfInfo()
        {
            try
            {
                var data = wx.CGetSelfInfo();
                if (string.IsNullOrEmpty(data)) return "";
                return data;
            }
            catch (Exception)
            {
                return "";
            }
        }
        /// <summary>
        /// 获取通讯录（含联系人、群）
        /// </summary>
        /// <returns>list<dic<wxid, wxNumber,wxNickName, wxRemark>></returns>
        public List<Dictionary<string, string>> GetContacts()
        {
            var result = new List<Dictionary<string, string>>();
            try
            {
                var data = wx.CGetFriendList() as Object[,,];
                string key, val;
                if (data == null || data.Length == 0) return result;
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    var one = new Dictionary<string, string>();
                    for (int j = 0; j < 3; j++)
                    {
                        key = data[i, j, 0] as string;
                        if (string.IsNullOrEmpty(key)) continue;
                        val = data[i, j, 1] as string;
                        if (string.IsNullOrEmpty(val)) val = "";
                        if (one.ContainsKey(key))
                        {
                            one[key] = val;
                        }
                        else
                        {
                            one.Add(key, val);
                        }
                    }
                    result.Add(one);
                }
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }
        /// <summary>
        /// 获取群内成员信息,含ChatroomID/wxID/群昵称/微信昵称
        /// </summary>
        /// <param name="chatroomID"></param>
        /// <returns>list<dic<chatroomID, wxID, displayName, nickName>></returns>
        public Dictionary<string, List<(string wxid, string displayName, string nickName)>> GetChatRoomMembers()
        {
            var result = new Dictionary<string, List<(string wxid, string displayName, string nickName)>>();
            var nickCache = new Dictionary<string, string>();

            try
            {
                var hd = wx.CGetDbHandles() as Object[,,];
                if (hd == null || hd.Length == 0) return result;

                var sql = "";
                //ChatRoom
                sql = $"SELECT ChatRoomName,UserNameList FROM ChatRoom";
                var roomIDs = wx.CExecuteSQL(Convert.ToUInt32(hd[0, 1, 1]),
                   sql) as Object[,];
                //string

                sql = $"SELECT RoomData FROM ChatRoom";
                if (roomIDs == null || roomIDs.Length <= 3) return result;
                var roomData = wx.CExecuteSQL(Convert.ToUInt32(hd[0, 1, 1]),
                   sql) as Object[,];
                //bytes

                //Contact
                sql = $"SELECT UserName, NickName FROM Contact";
                var contactData = wx.CExecuteSQL(Convert.ToUInt32(hd[0, 1, 1]),
                   sql) as Object[,];
                if (contactData != null && contactData.Length > 2)
                {
                    string wxid, nick;
                    for (int i = 1; i < contactData.GetLength(0); i++)
                    {
                        wxid = contactData[i, 0] as string;
                        if (string.IsNullOrEmpty(wxid)) continue;
                        nick = contactData[i, 1] as string;
                        if (string.IsNullOrEmpty(nick)) nick = "";
                        if (!nickCache.ContainsKey(wxid)) nickCache.Add(wxid, nick);
                    }
                }

                string chatroomID, usersStr, namesStr;
                List<string> memberIDs;
                byte[] roomDataBuf;
                protobuf.ChatRoomData roomProtoModel;
                for (int i = 1; i < roomIDs.GetLength(0); i++)
                {
                    //取群号
                    chatroomID = roomIDs[i, 0] as string;
                    if (string.IsNullOrEmpty(chatroomID)) continue;
                    //取WXID列表
                    usersStr = roomIDs[i, 1] as string;
                    if (string.IsNullOrEmpty(usersStr)) continue;
                    memberIDs = usersStr.Split("^G").ToList().Where((a) => !string.IsNullOrEmpty(a)).ToList();

                    //取buff
                    try
                    {
                        roomProtoModel = null;
                        roomDataBuf = roomData[i, 0] as byte[];
                        if (roomDataBuf == null || roomDataBuf.Length == 0) continue;
                        roomProtoModel = protobuf.ChatRoomData.Parser.ParseFrom(roomDataBuf);
                        if (roomProtoModel == null || roomProtoModel.Members == null || roomProtoModel.Members.Count <= 0) continue;

                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    if (!result.ContainsKey(chatroomID)) result.Add(chatroomID, new List<(string wxid, string displayName, string nickName)>());
                    string displayName, nickName;
                    for (int j = 0; j < memberIDs.Count(); j++)
                    {
                        displayName = "";
                        nickName = nickCache.ContainsKey(memberIDs[j]) ? nickCache[memberIDs[j]] : "";
                        foreach (var member in roomProtoModel.Members)
                        {
                            if (member.WxID == memberIDs[j])
                            {
                                if (!string.IsNullOrEmpty(member.DisplayName))
                                {
                                    displayName = member.DisplayName;
                                }
                                break;
                            }
                        }
                        if (string.IsNullOrEmpty(displayName)) displayName = nickName;
                        result[chatroomID].Add((memberIDs[j], displayName, nickName));
                    }

                }
                return result;

            }
            catch (Exception)
            {
                return result;
            }
        }
        #endregion

        #region RECEIVE
        private void StartReceiving()
        {
            /*  Message
             * 保存单条信息的结构
             * messagetype：消息类型
             * sender：发送者wxid；l_sender：`sender`字符数
             * wxid：如果sender是群聊id，则此成员保存具体发送人wxid，否则与`sender`一致；l_wxid：`wxid`字符数
             * message：消息内容，非文本消息是xml格式；l_message：`message`字符数
             * filepath：图片、文件及其他资源的保存路径；l_filepath：`filepath`字符数
             */
            wx.CStartReceiveMessage();
            ReceiveSleep();
            ReceiveSleep();
            while (true)
            {
                if (_Stop_Thread_Flag) break;
                try
                {
                    var raw = wx.CReceiveMessage();
                    if (raw == null)
                    {
                        ReceiveSleep();
                        continue;
                    }
                    var message = raw as System.Object[,];
                    if (message == null || message.Length <= 0)
                    {
                        ReceiveSleep();
                        continue;
                    }
                    var msg = ConvertMessage(message);
                    if (msg == null || msg.Count <= 0)
                    {
                        ReceiveSleep();
                        continue;
                    }
                    var msgType = msg.ContainsKey(MSG_TYPE) ? (enums.MessageType)Convert.ToInt32(msg[MSG_TYPE]) : enums.MessageType.UNKNOW;
                    var sender = msg.ContainsKey(MSG_SENDER) ? msg[MSG_SENDER] : "";
                    var wxid = msg.ContainsKey(MSG_WXID) ? msg[MSG_WXID] : "";
                    enums.RoomType roomType = enums.RoomType.Private;
                    Console.WriteLine(String.Join(",", message));
                    if (msgType == enums.MessageType.ServiceGroup)
                    {
                        roomType = enums.RoomType.Service;
                    }
                    else if (sender != wxid)
                    {
                        roomType = enums.RoomType.Group;
                    }
                    RaiseReceivedMessage(msg, msgType, roomType);
                    ReceiveSleep();
                }
                catch (Exception ex)
                {
                    utils.ConsoleLog.Error(TAG, $"{ex.Message}\n{ex.StackTrace}");
                    ReceiveSleep();
                }
            }
        }
        private static Dictionary<string, string> ConvertMessage(object[,] msg)
        {
            var result = new Dictionary<string, string>();
            string key, value;
            if (msg == null || msg.Length == 0) return result;
            for (int i = 0; i < msg.GetLength(0); i++)
            {
                key = msg[i, 0] as string;
                value = msg[i, 1].ToString();
                if (!string.IsNullOrEmpty(key))
                {
                    result.Add(key, value);
                }
            }
            return result;
        }
        private void ReceiveSleep() => Thread.Sleep(settings.Setting.MSG_RECEIVE_INTERVAL);
        #endregion
        #region SEND
        private void StartSending()
        {
            int high_times = 0;
            model.MessageBody msg = null;
            while (true)
            {
                if (_Stop_Thread_Flag) break;
                high_times = 0;
                try
                {
                    while (true)
                    {
                        high_times++;
                        if (high_times > settings.Setting.MSG_SEND_HIGH_TIMES) break;
                        if (messages_high.TryDequeue(out msg))
                        {
                            SendMsg(msg);
                            SendingSleep();
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (messages_low.TryDequeue(out msg))
                    {
                        SendMsg(msg);
                        SendingSleep();
                    }
                    else
                    {
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    utils.ConsoleLog.Error(TAG, $"{ex.Message}\n{ex.StackTrace}");
                    SendingSleep();
                }
            }
        }
        private void SendingSleep() => Thread.Sleep(intervalRand.Next(settings.Setting.MSG_SEND_INTERVAL_MIN, settings.Setting.MSG_SEND_INTERVAL_MAX));
        private void SendMsg(model.MessageBody msg)
        {
            if (msg == null) return;
            try
            {
                switch (msg.type)
                {
                    case enums.MessageType.Text:
                        if (string.IsNullOrEmpty(msg.chatroomID))
                        {
                            wx.CSendText(msg.wxID, msg.content);

                            utils.ConsoleLog.Processing(TAG, $"TYPE: TEXT, TO: {msg.wxID}, CONTENT: {msg.content}");
                        }
                        else
                        {
                            wx.CSendText(msg.chatroomID, msg.content);
                            utils.ConsoleLog.Processing(TAG, $"TYPE: TEXT, TO: {msg.chatroomID}, CONTENT: {msg.content}");
                        }
                        break;
                    case enums.MessageType.AT:
                        if (msg.wxIDs.Count == 1)
                        {
                            if (msg.content.StartsWith("@"))
                                msg.content = msg.content.Split(MSG_NICK_SPLITER).Last();
                            wx.CSendAtText(msg.chatroomID, msg.wxIDs.First(), msg.content);
                        }
                        else if (msg.wxIDs.Count > 1)
                        {
                            var array = new object[msg.wxIDs.Count];
                            for (int i = 0; i < msg.wxIDs.Count; i++)
                                array[i] = msg.wxIDs[i];
                            msg.content = msg.content.Replace(MSG_NICK_SPLITER, " ");
                            wx.CSendAtText(msg.chatroomID, array, (msg.simpleAt ? "" : "\n----------\n") + msg.content);
                        }
                        utils.ConsoleLog.Processing(TAG, $"TYPE: AT, TO: {msg.chatroomID}, AT: {String.Join("/", msg.wxIDs)} CONTENT: {msg.content}");
                        break;
                    case enums.MessageType.Image:
                        if (string.IsNullOrEmpty(msg.chatroomID))
                        {
                            wx.CSendImage(msg.wxID, msg.content);
                            utils.ConsoleLog.Processing(TAG, $"TYPE: IMG, TO: {msg.wxID}, PATH: {msg.content}");
                        }
                        else
                        {
                            wx.CSendImage(msg.chatroomID, msg.content);
                            utils.ConsoleLog.Processing(TAG, $"TYPE: IMG, TO: {msg.chatroomID}, PATH: {msg.content}");
                        }
                        break;
                    case enums.MessageType.FILE:
                        if (string.IsNullOrEmpty(msg.chatroomID))
                        {
                            wx.CSendFile(msg.wxID, msg.content);
                            utils.ConsoleLog.Processing(TAG, $"TYPE: FILE, TO: {msg.wxID}, PATH: {msg.content}");
                        }
                        else
                        {
                            wx.CSendFile(msg.chatroomID, msg.content);
                            utils.ConsoleLog.Processing(TAG, $"TYPE: FILE, TO: {msg.chatroomID}, PATH: {msg.content}");
                        }
                        break;
                    default:
                        return;
                }
            }
            catch (Exception ex)
            {
                utils.ConsoleLog.Error(TAG, $"{ex.Message}\n{ex.StackTrace}");
            }

        }
        #endregion

        #region PRIVATE
        private void AddSendMessage(string content, string chatroomID, List<string> wxIDs, bool isLow)
        {
            if (string.IsNullOrEmpty(chatroomID) || wxIDs == null || wxIDs.Count == 0) return;
            var target = isLow ? messages_low : messages_high;
            model.MessageBody msg = new model.MessageBody(chatroomID, wxIDs, content, enums.MessageType.AT);
            msg.simpleAt = true;
            target.Enqueue(msg);
        }
        private void AddSendMessage(string content,
             string chatroomID,
             string wxID,
             enums.MessageType type,
             bool isLow)
        {
            if (string.IsNullOrEmpty(content)) return;
            if (string.IsNullOrEmpty(chatroomID) && string.IsNullOrEmpty(wxID)) return;
            var target = isLow ? messages_low : messages_high;

            if (type == enums.MessageType.Text || type == enums.MessageType.AT)
            {
                //文字消息合并处理
                if (content.Length > settings.Setting.MSG_CONTENT_MAX_LENGTH)
                {
                    //单个长消息
                    var index = 1;
                    int len = content.Length / settings.Setting.MSG_CONTENT_MAX_LENGTH + (content.Length % settings.Setting.MSG_CONTENT_MAX_LENGTH > 0 ? 1 : 0);
                    do
                    {
                        if (content.Length > settings.Setting.MSG_CONTENT_MAX_LENGTH)
                        {
                            target.Enqueue(new model.MessageBody(chatroomID,
                                                                 wxID,
                                                                 (index == 1 ? "" : "\n") + content.Substring(0, settings.Setting.MSG_CONTENT_MAX_LENGTH) + $"\n长消息({index}/{len})",
                                                                 type));
                            content = content.Substring(settings.Setting.MSG_CONTENT_MAX_LENGTH);
                            index++;
                        }
                        else
                        {
                            target.Enqueue(new model.MessageBody(chatroomID,
                                                                 wxID,
                                                                 (index == 1 ? "" : "\n") + content + $"\n长消息({index}/{len})",
                                                                 type));
                            break;
                        }
                    } while (true);
                }
                else
                {
                    //合并短消息
                    var flag = false;
                    var newMsg = new model.MessageBody(chatroomID, wxID, content, type);
                    foreach (var message in target)
                    {
                        if (message.Combine(newMsg))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        target.Enqueue(newMsg);
                    }
                }
            }
            else
            {
                target.Enqueue(new model.MessageBody(chatroomID, wxID, content, type));
            }


        }
        #endregion
    }
}
