using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaList : MonoBehaviour
{
    // キャラ情報
    [System.Serializable]
    struct CharaData
    {
        public int id;
        public string name;
        public GameObject prefab;
    }

    [SerializeField]
    private CharaData[] charaData = new CharaData[8];

    void Start()
    {
        for (int i = 0; i < charaData.Length; i++)
        {
            charaData[i].id = i;
        }
    }

    // 名前からIDを取得
    public int getCharaIdFromName(string name)
    {
        for (int i = 0; i < charaData.Length; i++)
        {
            if (name == charaData[i].name)
            {
                return charaData[i].id;
            }
        }
        return -1;
    }

    // Idからプレハブを取得
    public GameObject getCharaPrefabFromId(int id)
    {
        if (0 <= id && id <= charaData.Length)
        {
            return charaData[id].prefab;
        }
        return null;
    }
}
