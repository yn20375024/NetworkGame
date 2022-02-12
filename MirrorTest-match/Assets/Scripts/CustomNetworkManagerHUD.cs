using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Mirror;

public class CustomNetworkManagerHUD : MonoBehaviour
{
    NetworkManager manager;

    [Header("MatchSelectCanvas")]
    public Canvas matchSelectCanvas;
    public InputField addressInputField;

    [Header("RoomCanvas")]
    public Canvas roomCanvas;

    [Header("CharaSelectCanvas")]
    public Canvas charaSelectCanvas;

    [Header("SceneName")]
    public string matchSceneName = "MatchScene";
    public string gameSceneName = "GameScene";

    private bool isHost = false;
    private bool isClient = false;
    private bool isServer = false;

    [Header("CharaSelect")]
    public int charaId = -1;

    void OnEnable()
    {
        isHost = false;
        isClient = false;
        isServer = false;

        manager = GetComponent<CustomNetworkManager>();
        matchSelectCanvas.gameObject.SetActive(true);
        roomCanvas.gameObject.SetActive(false);
        charaSelectCanvas.gameObject.SetActive(false);
    }

    // =======================
    // MatchSelectCanvas
    // =======================
    // StartHostButton
    public void OnStartHostButton()
    {
        isHost = true;
        manager.StartHost();

        matchSelectCanvas.gameObject.SetActive(false);
        roomCanvas.gameObject.SetActive(true);

        // IPAddressを取得
        string hostname = Dns.GetHostName();
        IPAddress[] adrList = Dns.GetHostAddresses(hostname);
        foreach (IPAddress address in adrList)
        {
            GameObject.Find("IPAddressText").GetComponent<Text>().text = "RoomId=" + address.ToString();
        }
    }

    // StartClientButton
    public void OnStartClientButton()
    {
        isClient = true;

        matchSelectCanvas.gameObject.SetActive(false);

        if (addressInputField.text == "")
        {
            manager.networkAddress = "localhost";
        }
        else
        {
            manager.networkAddress = addressInputField.text;
        }
        manager.StartClient();
    }

    // StartServerButton
    public void OnStartServerButton()
    {
        isServer = true;
        manager.StartServer();

        matchSelectCanvas.gameObject.SetActive(false);
        roomCanvas.gameObject.SetActive(true);

        // IPAddressを取得
        string hostname = Dns.GetHostName();
        IPAddress[] adrList = Dns.GetHostAddresses(hostname);
        foreach (IPAddress address in adrList)
        {
            GameObject.Find("IPAddressText").GetComponent<Text>().text = "RoomId=" + address.ToString();
        }
    }

    // =======================
    // RoomCanvas
    // =======================
    // GameStartButton
    public void OnGameStartButton()
    {
        if (isHost == true || isServer == true)
        {
            // シーンの切り替えをクライアントに命令
            manager.ServerChangeScene(gameSceneName);
        }
    }

    // CancelButton
    public void OnCancelButton()
    {
        // ホスト
        if (isHost== true)
        {
            isHost = false;
            manager.StopHost();
        }
        // クライアント
        else if (isClient == true)
        {
            isClient = false;
            manager.StopClient();
        }
        // サーバー
        else if (isServer == true)
        {
            isServer = false;
            manager.StopServer();
        }
        matchSelectCanvas.gameObject.SetActive(true);
        roomCanvas.gameObject.SetActive(false);
    }

    // CharaSelectButton
    public void OnCharaSelctButton()
    {
        roomCanvas.gameObject.SetActive(false);
        charaSelectCanvas.gameObject.SetActive(true);
    }

    // =======================
    // CharaSelectCanvas
    // =======================
    // DecideButton
    public void OnDecideButton()
    {
        charaSelectCanvas.gameObject.SetActive(false);
        roomCanvas.gameObject.SetActive(true);

        Debug.Log("selectId=" + GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>().selectId);
    }
}

