using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using UnityEngine.UI;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using TMPro;

public class Lobby : MonoBehaviour
{
    static public Lobby lobby;

    [SerializeField]
    public TextMeshProUGUI myIp, pcCode;

    [SerializeField]
    Button btnCreate, btnEnter, pcCreate,pcClose;

    [SerializeField]
    GameObject painelEnter, painelCreate, pcWaiting;

    private void Awake()
    {
        lobby = GetComponent<Lobby>();
    }

    #region Login

    public void DisableButtons()
    {
        btnCreate.interactable = false;
        btnEnter.interactable = false;
    }
    public void EnableButtons()
    {
        btnCreate.interactable = true;
        btnEnter.interactable = true;
    }

    public void Enter()
    {
        painelCreate.SetActive(false);
        painelEnter.SetActive(true);
        
        //Reseta o outro painel
        CloseRoom();
    }
    public void Create()
    {
        painelCreate.SetActive(true);
        painelEnter.SetActive(false);
    }

    #endregion

    #region PCreate
        
    public void CreateRoom()
    {
        //Chamar o user createRoom
        User.user.CreateRoom();

        pcCreate.interactable = false;
        pcClose.interactable = true;
        pcWaiting.SetActive(true);
        pcCode.text = User.user.ip +":"+ User.user.port;

    }

    public void CloseRoom()
    {
        //Fechar a sala (user)
        User.user.CloseRoom();

        pcCreate.interactable = true;
        pcClose.interactable = false;
        pcWaiting.SetActive(false);
        pcCode.text = "";
    }
    #endregion
}
