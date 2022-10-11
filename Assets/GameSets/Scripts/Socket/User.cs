using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking.PlayerConnection;
using UnityEngine.UI;
using System.Net;
using System.IO;
using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using TMPro;
using System.Text;
using System.Threading.Tasks;

public enum UserType
{
    host, guest
}

public class User : MonoBehaviour
{
    public static User user;

    public int points = 0;

    public string userName, vsName;

    public bool isConnected = false;
    public bool isMyTurn = false;

    public string ip,vsIp;
    public int port = 4848;

    TcpListener server;
    Socket socket;
    Socket handler;

    public UserType type;

    private void Awake()
    {
        user = GetComponent<User>();    
    }

    public void SetName(string s)
    {
        Debug.Log(s);
        userName = s;
        if (userName.Length > 0)
        {
            Debug.Log("Ativa os botões");
            Lobby.lobby.EnableButtons();
        }
        else
        {
            Debug.Log("Desativa os botões");
            Lobby.lobby.DisableButtons();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        //GetIPAddress();
        ip = GetLocalIPv4();
        //Debug.Log("IP: "+GetLocalIPv4());
    }

    public string GetIPAddress()
    {
        string ip = new WebClient().DownloadString("https://icanhazip.com/");
        ip = ip.Split("\n")[0];
        Lobby.lobby.myIp.text = ip;
        this.ip = ip;
        return ip;
    }
    public string GetLocalIPv4()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                Lobby.lobby.myIp.text = ip.ToString();
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
  
    public void CreateRoom()
    {
        Debug.Log("Criando Sala");
        type = UserType.host;
        isMyTurn = true;
        ServerSocket();
    }
    public void ConnectToRoom()
    {
        Debug.Log("Conectando à Sala");
        type = UserType.guest;
        isMyTurn = false;
        ServerSocket();
    }

     async Task<Socket> ServerSocket()
    {
        Debug.Log("Criando Socket\n");

        IPEndPoint ipEndPoint;
        if (type == UserType.host)
        {
            ipEndPoint = new(IPAddress.Parse(ip), port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipEndPoint);
            socket.Listen(10);
            Debug.Log("Aguardando cliente\n");
            handler = await socket.AcceptAsync();
            SceneManager.LoadScene(1);
        }
        else
        {
            //Conecta ao server
            Debug.Log("Conectando a " + vsIp);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(vsIp, port);
            handler = socket;
            Debug.Log("Conectado a " + vsIp);
            SceneManager.LoadScene(1);
        }
        
        Debug.Log(handler.RemoteEndPoint.ToString());
        isConnected = true;

        //GameLoop
        while (isConnected)
        {
            Debug.Log("Recebendo mensagem do cliente\n");

            // Receive message.
            var buffer = new byte[1024];

            var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, received);
            Debug.Log("Recebido:" + response);

            if (response.Contains("close"))
            {
                handler.Close();
                isConnected = false;
                break;
            }

            if (GameLogic(response))
            {
                //Ok
            }
            else
            {
                //Não entendi a mensagem
            }
            
        }
        return null;
    }

    public void CloseRoom()
    {
        if (server != null)
        {
            server.Stop();
            Debug.Log("Fechando Socket");
        }
    }

    string MessageDecrypt(byte[] bytes, NetworkStream stream)
    {
        string data="";
        int i;

        // Vê todas as mensagens enviadas pelo cliente
        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
        {
            // Bytes -> ASCII
            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
            Debug.Log("Received:" + data);


            // Envia uma resposta ao cliente
            byte[] msg = System.Text.Encoding.ASCII.GetBytes("ok");
            stream.Write(msg, 0, msg.Length);
            Debug.Log("Sent:" + data);
        }

        return data;
    }

    byte[] MessageEncrypt(string msg)
    {
        byte[] bytes = System.Text.Encoding.ASCII.GetBytes(msg);
        return bytes;
    }


    bool GameLogic(string data)
    {
        Debug.Log(data);

        //Info Section
        /// Exemplo de mensagem aguardada: MyIp=123.456.789.012
        bool isOk = false;

        //Apresentação
        if (data.Contains("name:"))
        {
            SendName();
            vsName = data.Split(":")[1];
            isMyTurn = true;
            CardsManager.cardManager.Unblock();
            isOk = true;
            CardsManager.cardManager.vsName.text = vsName;
        }
        
        //apresentação pt2
        if (data.Contains("hostName:"))
        {
            vsName = data.Split(":")[1];
            CardsManager.cardManager.vsName.text = vsName;
            isMyTurn = false;
            CardsManager.cardManager.Block();
            isOk = true;
        }

        if (data.Contains("card:"))
        {
            int cardShow = int.Parse(data.Split(":")[1]);
            CardsManager.cardManager.cards[cardShow].onClick();
            isOk = true;
        }

        if (data.Contains("sync:"))
        {
            string order = data.Split(":")[1];
            string []oSplited = order.Split(",");
            int[] vec = new int[oSplited.Length];
            Debug.Log("Recebeu " + oSplited.Length + " Indexes");
            for(int i=0;i< oSplited.Length; i++)
            {
                vec[i] = int.Parse(oSplited[i]);
            }
            CardsManager.cardManager.Order(vec);
            isOk = true;
        }


        return isOk;
    }

    public void SendSocket(string message)
    {
        handler.Send(MessageEncrypt(message));
    }

    public void SendName()
    {
        handler.Send(MessageEncrypt("hostName:"+userName));
    }

    public void WinPlay()
    {
        if(isMyTurn)
            handler.Send(MessageEncrypt("win"));
    }

    public void LosePlay()
    {
        if (isMyTurn)
        {
            handler.Send(MessageEncrypt("lose"));

            isMyTurn = false;
            CardsManager.cardManager.Block();
        }
        else
        {
            isMyTurn = true;
            CardsManager.cardManager.Unblock();
        }
    }

    public void EndGame()
    {
        CloseRoom();
        CardsManager.cardManager.endGamePanel.SetActive(true);

        if (points > 4)
        {
            CardsManager.cardManager.winImage.SetActive(true);
        }
        if (points <= 4)
        {
            CardsManager.cardManager.loseImage.SetActive(true);
        }

    }

}
