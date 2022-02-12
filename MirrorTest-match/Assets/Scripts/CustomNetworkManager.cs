using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkManager
{
    // 開始、停止
    public override void OnStartHost()
    {
        Debug.Log("OnStartHost");
        base.OnStartHost();
    }

    public override void OnStopHost()
    {
        Debug.Log("OnStopHost");
        base.OnStopHost();
    }
   
    public override void OnStartClient()
    {
        Debug.Log("OnStartClient");
        base.OnStartClient();
    }

    public override void OnStopClient()
    {
        Debug.Log("OnStopClient");
        base.OnStopClient();
    }

    public override void OnStartServer()
    {
        Debug.Log("OnStartServer");
        base.OnStartServer();

        NetworkServer.ReplaceHandler<PlayerInfoMessage>(ReceivedInfo);
    }

    public override void OnStopServer()
    {
        Debug.Log("OnStopServer");
        base.OnStopServer();
    }

    // クライアント
    public override void OnClientConnect()
    {
        Debug.Log("OnClientConnect");
        base.OnClientConnect();

        GetComponent<CustomNetworkManagerHUD>().roomCanvas.gameObject.SetActive(true);
    }

    public override void OnClientDisconnect()
    {
        Debug.Log("OnClientDisconnect");
        base.OnClientDisconnect();

        GetComponent<CustomNetworkManagerHUD>().matchSelectCanvas.gameObject.SetActive(true);
    }

    public override void OnClientNotReady()
    {
        Debug.Log("OnClientNotReady");
        base.OnClientNotReady();

        PlayerInfoMessage msg = new PlayerInfoMessage()
        {
            type = GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>().selectId
        };
        NetworkClient.Send(msg);
    }

    public override void OnClientSceneChanged()
    {
        Debug.Log("OnClientSceneChanged");
        base.OnClientSceneChanged();
    }

    // サーバー
    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("OnServerConnect");
        base.OnServerConnect(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("OnServerDisconnect");
        base.OnServerDisconnect(conn);
    }

    int playerType = -1;
    private void ReceivedInfo(NetworkConnection conn, PlayerInfoMessage msg)
    {
        playerType = msg.type;

        Debug.Log("playerType=" + playerType);

        OnServerAddPlayer(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        GameObject playerPrefab = GameObject.Find("PlayerInfo");
        playerPrefab = playerPrefab.GetComponent<CharaList>().getCharaPrefabFromId(playerType);

        GameObject player;
        Transform startPos = GetStartPosition();
        player = (GameObject)Instantiate(playerPrefab, startPos.position, startPos.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        Debug.Log("OnServerSceneChanged");
        base.OnServerSceneChanged(sceneName);
    }
}

