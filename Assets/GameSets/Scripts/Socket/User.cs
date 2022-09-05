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

public class User : MonoBehaviour
{
    public static User user;

    public int points = 0;

    public string userName;

    public bool isConnected = false;
    public bool isMyTurn = false;

    public string ip;
    public int port = 4848;

    TcpListener server;
    Socket socket;
    Socket handler;

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
                Lobby.lobby.myIp.text = ip.ToString()+":"+port;
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
    
    public void CreateRoom()
    {
        Debug.Log("Começando Task socket");
        ServerSocket();
    }

     async Task<Socket> ServerSocket()
    {
        Debug.Log("Criando Socket\n");

        IPEndPoint ipEndPoint = new(IPAddress.Any, port);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(ipEndPoint);
        socket.Listen(10);

        Debug.Log("Aguardando cliente\n");
        handler = await socket.AcceptAsync();

        SceneManager.LoadScene(1);
        
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
                handler.Send(MessageEncrypt("closing"));
                handler.Close();
                isConnected = false;
                break;
            }

            if (GameLogic(response))
            {
                handler.Send(MessageEncrypt("Ok"));
            }
            else
            {
                handler.Send(MessageEncrypt("No"));
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
        /// Exemplo de mensagem aguardada: info:MyIp=123.456.789.012|MyName=Meu Nome
        bool isOk = false;
        if (data.Contains("info:")) {
            //Divide as mensagens por pipes |
            string[] infos = data.Split("|");
            //Para cada mensagem separada por pipe
            foreach (string info in infos)
            {
                if (info.Contains("MyIp="))
                {
                    Enemy.enemy.ip = info.Split("MyIp=")[1];
                    Debug.Log("EnemyIp: " + Enemy.enemy.ip);
                    isOk = true;
                }

                if (info.Contains("MyName="))
                {
                    Enemy.enemy.name = info.Split("MyName=")[1];
                    Debug.Log("EnemyName: " + Enemy.enemy.name);
                    isOk = true;
                }

                if (info.Contains("Card="))
                {
                    //Show card
                    isOk = true;
                }

                if (info.Contains("Play="))
                {
                    if (info.Split("MyName=")[1] == "win")
                    {
                        isOk = true;
                    }
                }
                if (info.Contains("Turn"))
                {
                    isMyTurn = true;
                    isOk = true;
                }

            }

        }

        //Create
        if (data.Contains("Start:"))
        {
            isMyTurn = true;
            CardsManager.cardManager.Unblock();
            isOk = true;
        }
        return isOk;
    }

    public void WinPlay()
    {
        handler.Send(MessageEncrypt("info:Play=win"));
    }

    public void LosePlay()
    {
        handler.Send(MessageEncrypt("info:Turn"));
    }


}
