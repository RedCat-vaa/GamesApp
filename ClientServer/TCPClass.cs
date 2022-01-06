using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using DBLibrary;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;


namespace ClientServer
{

    public class ServerEventArgs
    {
        public string Message { get; set; }
        public ServerEventArgs(string Message)
        {
            this.Message = Message;
        }
    }
    
    public enum AuthOrReg
    {
        Auth,
        Reg
    }
    public class ClientApp
    {
        TcpClient tcpClient;
        public string serverIp {get;set;}
        public string serverPort { get; set; }

        public StringBuilder stringConnection { get; set; }
        public ClientApp(string IpAdress, string Port)
        {
            tcpClient = new TcpClient();
            serverIp = IpAdress;
            serverPort = Port;
            stringConnection = new StringBuilder();
        }

        public string ConnectionServer(string login, string password, AuthOrReg typeConnection)
        {
            if(tcpClient!=null)
            {
                try
                {
                    DataConnection dataConnection;
                    IPAddress adr = IPAddress.Parse(serverIp);
                    tcpClient.Connect(adr, Convert.ToInt32(serverPort));
                    NetworkStream currentStream = tcpClient.GetStream();
                    byte[] sendServer;
                    if (typeConnection == AuthOrReg.Auth)
                    {
                        dataConnection = new DataConnection { Login = login, Password = password, Type = "Auth" };
                    }
                    else
                    {
                        dataConnection = new DataConnection { Login = login, Password = password, Type = "Reg" };
                       
                    }
                    stringConnection.Clear();
                    stringConnection.Append(Newtonsoft.Json.JsonConvert.SerializeObject(dataConnection));
                    sendServer = Encoding.UTF8.GetBytes(stringConnection.ToString());


                    /*if (typeConnection == AuthOrReg.Auth)
                    {
                       sendServer = Encoding.UTF8.GetBytes($"{login}_|_{password}_|_Auth");
                    }    
                    else
                    {
                        sendServer = Encoding.UTF8.GetBytes($"{login}_|_{password}_|_Reg");
                    }*/

                    currentStream.Write(sendServer, 0, sendServer.Length);

                    byte[] data = new byte[256];
                    int bytes;
                    string message = "";
                    do
                    {
                        bytes = currentStream.Read(data, 0, data.Length);
                        message += Encoding.UTF8.GetString(data, 0, bytes);
                    }
                    while (currentStream.DataAvailable);
                    tcpClient.Close();
                    return message;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                
            }
            return "";
        }

        public static (string, string) OutputIP()
        {
            string json = "";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"InfoServer.json");
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                byte[] array = new byte[fs.Length];
                fs.Read(array, 0, array.Length);
                json = System.Text.Encoding.Default.GetString(array);
            }
            InfoServer? infoServer = Newtonsoft.Json.JsonConvert.DeserializeObject<InfoServer>(json);
            return (infoServer.ip, infoServer.port);
        }


    }
    internal class InfoServer
    {
        [JsonProperty("ip")]
        public string ip { get; set; }
        [JsonProperty("port")]
        public string port { get; set; }

    }

    internal class DataConnection
    {
        [JsonProperty("Login")]
        public string Login { get; set; }
        [JsonProperty("Password")]
        public string Password { get; set; }
        [JsonProperty("Type")]
        public string Type { get; set; }
    }

    public class ServerApp
    {
        public string Response { get; set; }
        TcpListener server { get; set; }

        Users? FindUser = null;

        public delegate void SendMessage(object sender, ServerEventArgs ev);
        public event SendMessage eventSendMessage;

        public void stopServer()
        {
            server.Stop();
        }


        public async void startServerAsync()
        {
            await Task.Run(startServer);
        }

        internal void processAuth(DataConnection dataConnection, NetworkStream stream, TcpClient client)
        {
            FindUser = null;
            using (DB db = new DB())
            {
                var users = db.Users.Where(user => user.Login == dataConnection.Login);
                if (users.Count() == 0)
                {
                    Response = "Пользователь не найден!";
                }
                else
                {
                    foreach (Users user in users)
                    {
                        if (user.Password == dataConnection.Password)
                        {
                            FindUser = user;
                        }
                    }
                }
            }

            if (FindUser != null)
            {
                Response = "Success";
            }
            else
            {
                Response = "Неверный пароль";
            }

            byte[] dataResponse = Encoding.UTF8.GetBytes(Response);

            stream.Write(dataResponse, 0, dataResponse.Length);

            stream.Close();
            client.Close();
        }
        internal void processReg(DataConnection dataConnection, NetworkStream stream, TcpClient client)
        {
            //Client
            using (DB db = new DB())
            {
                var users = db.Users.Where(user => user.Login == dataConnection.Login);
                if (users.Count() > 0)
                {
                    Response = "Логин уже занят!";
                    return;
                }
                Users newUser = new Users { Login = dataConnection.Login, Password = dataConnection.Password };
                db.Users.Add(newUser);
                db.SaveChanges();
                Response = "Success";
            }

            byte[] dataResponse = Encoding.UTF8.GetBytes(Response);

            stream.Write(dataResponse, 0, dataResponse.Length);

            stream.Close();
            client.Close();
        }
        public void startServer()
        {
            
            try
            {
                server = new TcpListener(IPAddress.Any, 11000);
                server.Start();

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();
                    eventSendMessage?.Invoke(this, new ServerEventArgs("Выполнено подключение к серверу"));

                    byte[] data = new byte[256];
                    int bytes = stream.Read(data, 0, data.Length);
                    string message = Encoding.UTF8.GetString(data, 0, bytes);
                    try
                    {
                        DataConnection dataConnection = Newtonsoft.Json.JsonConvert.DeserializeObject<DataConnection>(message);
                        if (dataConnection.Type== "Auth")
                        {
                            processAuth(dataConnection, stream, client);
                        }
                        else
                        {
                            processReg(dataConnection, stream, client);
                        }
                    }
                    catch (Exception ex)
                    {
                        eventSendMessage?.Invoke(this, new ServerEventArgs(ex.Message));
                    }
                }
            }
            catch (Exception ex)
            {
                eventSendMessage?.Invoke(this, new ServerEventArgs(ex.Message));
                server.Stop();
            }
            finally
            {
                server.Stop();
            }
              
        }
    }
}
