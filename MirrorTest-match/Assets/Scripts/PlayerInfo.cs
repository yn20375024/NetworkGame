using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Mirror;

public static class PlayerType
{
    public const int KAKUTOUKA = 0;
    public const int NAGINATA = 1;
    public const int KISHI = 2;
}

[System.Serializable]
public struct PlayerInfoMessage : NetworkMessage
{
    public int type;
}

public class PlayerInfo : MonoBehaviour
{
    public static int type = -1;

    EventSystem eventSystem;
    GameObject selectObj;
    private string text;
    public int selectId = -1;

    public void OnCharaChanged()
    {
        eventSystem = EventSystem.current;
        selectObj = eventSystem.currentSelectedGameObject;

        text = selectObj.transform.Find("Text").GetComponent<Text>().text;
        selectId = GetComponent<CharaList>().getCharaIdFromName(text);
    }

    public int getType()
    {
        return type;
    }
}

