using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*SuperWebSocket is used to handle WebSocket server functionalities.
Newtonsoft.Json is used for JSON serialization/deserialization.*/
using SuperWebSocket;
using Newtonsoft.Json;

namespace websocket_server
{
    class Program
    {
        //lsession: List to keep track of WebSocket sessions
        //private static List<WebSocketSession> lsession = new List<WebSocketSession>();
        //registeredUsers: List to keep track of registered users
        private static List<websocket_userregister> registeredUsers = new List<websocket_userregister>();
        private static int userCounter = 0;
        private static WebSocketServer wsServer;

        static void Main(string[] args)
        {
            initwebsocket();
        }

        private static void initwebsocket()
        {
            wsServer = new WebSocketServer();
            int port = 8808;
            wsServer.Setup(port);
            wsServer.NewSessionConnected += WsServer_NewSessionConnected;
            wsServer.NewMessageReceived += WsServer_NewMessageReceived;
            wsServer.NewDataReceived += WsServer_NewDataReceived;
            wsServer.SessionClosed += WsServer_SessionClosed;
            wsServer.Start();
            Console.WriteLine("Server is runing on port " + port + " . Press Enter to Exit...");
            Console.ReadKey();
            wsServer.Stop();
        }

        //Logs the new session ID when a client connects and auto-registers the user
        private static void WsServer_NewSessionConnected(WebSocketSession session)
        {
            //lsession.Add(session);
            userCounter++;
            string username = "user" + userCounter;
            registeredUsers.Add(new websocket_userregister() { mysession = session, username = username });
            session.Send("Welcome, " + username);
            SendActiveUsersToAll();
            Console.WriteLine("WsServer_NewSessionConnected: " + session.SessionID + " as " + username);
        }

        private static void SendActiveUsersToAll()
        {
            string activeUsers = JsonConvert.SerializeObject(registeredUsers.Select(x => x.username).ToList());
            foreach (var user in registeredUsers)
            {
                user.mysession.Send("Active Users: " + activeUsers);
            }
        }

        //Logs the received message
        private static void WsServer_NewMessageReceived(WebSocketSession session, string value)
        {
            Console.WriteLine("WsServer_NewMessageReceived: " + value);

            try
            {
                ////Deserializes the message to a websocket_userregister object
                //var userRegister = JsonConvert.DeserializeObject<websocket_userregister>(value);

                ////Checks if the session is already registered; if not, registers it and sends a success code
                //bool checksession_reg = registeredUsers.Exists(x => x.mysession == session && x.username == userRegister.username);

                ////If the session is registered, attempts to find the target user(pm_target) and sends a command to the target user’s session.
                ////If the target user is not found, sends an error code
                //if (checksession_reg == false)
                //{
                //    registeredUsers.Add(new websocket_userregister() { 
                //        mysession = session, 
                //        username = userRegister.username 
                //    });
                //    session.Send("Scode 51");
                //}
                //else
                //{
                //    try
                //    {
                //        websocket_userregister targetUser = registeredUsers.Find(p => p.username == userRegister.pm_target);
                //        if(targetUser != null)
                //        {
                //            targetUser.mysession.Send("my command is " + userRegister.pm_cmd + " period "+ userRegister.pm_period + " package "+ userRegister.pm_package);
                //        }
                //        else
                //        {
                //            session.Send("Ecode 63");
                //        }
                //    }catch(Exception ex)
                //    {
                //        session.Send("Ecode 67");
                //    }
                /*
                for (int i = 0; i < registeredUsers.Count; i++)
                {
                    if (registeredUsers[i].mysession.SessionID != session.SessionID)
                    {
                        int sessionindex = registeredUsers.FindIndex(x => x.mysession == session);
                        msg_from mf = new msg_from() { datapackage = value, myusername = registeredUsers[sessionindex].myusername };
                        string _json = JsonConvert.SerializeObject(mf);
                        registeredUsers[i].mysession.Send(_json);
                    }
                }
                */
                //}

                // Broadcast the received message to all connected clients
                foreach (var user in registeredUsers)
                {
                    user.mysession.Send(value);
                }
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine("JSON Deserialization Error: " + jsonEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private static void WsServer_NewDataReceived(WebSocketSession session, byte[] value)
        {
            Console.WriteLine("WsServer_NewDataReceived");
        }

        private static void WsServer_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
            bool checksession_reg = registeredUsers.Exists(x => x.mysession == session);
            if (checksession_reg == true)
            {
                int sessionindex = registeredUsers.FindIndex(x => x.mysession == session);
                registeredUsers.RemoveAt(sessionindex);
            }
            SendActiveUsersToAll();
            Console.WriteLine("SessionClosed");
        }
    }
}
