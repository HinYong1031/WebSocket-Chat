using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperWebSocket;
namespace websocket_server
{
    class websocket_userregister
    {
        public WebSocketSession mysession { get; set; }
        public string username { get; set; }
        public string pm_target { get; set; }
        public string pm_cmd { get; set; }
        public string pm_period { get; set; }
        public string pm_package { get; set; }
    }
}
