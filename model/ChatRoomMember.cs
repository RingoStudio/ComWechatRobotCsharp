using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS_WXBOT_COM.model
{
    internal class ChatRoomMember
    {
        public string wxID { get; set; }
        public string displayName { get; set; }
        public int state { get; set; }
    }
}
