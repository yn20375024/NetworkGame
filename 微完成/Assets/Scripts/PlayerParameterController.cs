using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParameterController : MonoBehaviour
{
    [SerializeField, ReadOnly]
    bool playerDeadFlg;

    public float hp = 1000.0f;          // 体力
    public float speed = 10.0f;         // 移動速度
    public float jump = 15.0f;          // ジャンプ力(初期値)

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(playerDeadFlg);
        playerDeadFlg = false ;
    }

    // Update is called once per frame
    void Update()
    {
        if(hp <= 0)
        {
            playerDeadFlg = true;
        }
    }

    public void decreaseHp(float decreasehp) 
    {
        hp -= decreasehp;
    }
}
