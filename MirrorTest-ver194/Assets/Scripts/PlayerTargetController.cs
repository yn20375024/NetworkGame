using System;
using UnityEngine;
using Mirror;

public class PlayerTargetController : NetworkBehaviour
{
    [SerializeField, ReadOnly]
    uint PlayerID;
    [SerializeField, ReadOnly]
    uint TargetID;
    [SerializeField, ReadOnly]
    uint NumberOfPlayers;
    [SerializeField, ReadOnly]
    Transform TargetTransform;

    // Start is called before the first frame update
    void Start()
    {
        //Playerタグの付いたオブジェクトを参照する
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        //参照した配列の数(プレイヤー数)
        NumberOfPlayers = (uint)Players.Length;
        //自身のID
        PlayerID = GetComponent<NetworkIdentity>().netId;

        //プレイヤー数が２人以上なら
        if (NumberOfPlayers > 1)
        {
            //他の誰かをターゲット
            for (int i = 0 ; i < NumberOfPlayers ; i++ ){
                TargetID = Players[i].GetComponent<NetworkIdentity>().netId;
                //自身のIDとターゲットIDが違うならターゲット
                if (TargetID != PlayerID) {
                    TargetTransform = Players[i].GetComponent<Transform>();
                    break;
                }
            }
        }
        //プレイヤー数が１人以下なら
        else {
            //中心を向く
            GameObject LookAtObj = GameObject.Find("LookAt") ;
            TargetTransform = LookAtObj.GetComponent<Transform>() ; 
            TargetID = 0;
        }

        //カメラにトランスフォーム挿入
        GameObject MyCamera = GameObject.Find("CameraManager");
        MyCamera.GetComponent<FollowAndLookAtTarget>().LookAt = TargetTransform;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override void OnStartClient()
    {
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        NumberOfPlayers = (uint)Players.Length;
        Debug.Log("Connected Player" + NumberOfPlayers);
    }

    public void ChangeTarget( String mode )
    {
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        NumberOfPlayers = (uint)Players.Length;

        //プレイヤー数が３人以上なら
        if (NumberOfPlayers > 2)
        {
            switch (mode) { 
                case "BACK":
                    //今のターゲット番号が最初のプレイヤーより大きいなら前のプレイヤーに移す
                    if (TargetID > 1)
                    {
                        //ターゲット先がプレイヤーIDと同じでなければ前に1人移る
                        if (PlayerID != (TargetID - 1))
                        {
                            TargetID -= 1;
                        }
                        //ターゲット先がプレイヤーIDと同じなら前に2人移る、移るターゲット先が居ないなら最後の人にターゲットを戻す
                        else
                        {
                            if (1 <= (TargetID - 2))
                            {
                                TargetID -= 2;
                            }
                            else
                            {
                                TargetID = NumberOfPlayers;
                            }

                        }
                    }
                    //今のターゲット番号がプレイヤーと同じまたは少ないなら最後のプレイヤーに戻す
                    else
                    {
                        //最後のプレイヤーでなければ最後のプレイヤーにターゲット
                        if (PlayerID != NumberOfPlayers)
                        {
                            TargetID = NumberOfPlayers;
                        }
                        else
                        {
                            TargetID = (NumberOfPlayers - 1);
                        }
                    }
                    break;
                case "NEXT":
                    //今のターゲット番号がプレイヤーより少ないなら次のプレイヤーに移す
                    if (TargetID < NumberOfPlayers)
                    {
                        //ターゲット先がプレイヤーIDと同じでなければ次に1人移る
                        if (PlayerID != (TargetID + 1))
                        {
                            TargetID += 1;
                        }
                        //ターゲット先がプレイヤーIDと同じなら次に2人移る、移るターゲット先が居ないなら一人目にターゲットを戻す
                        else 
                        {
                            if (NumberOfPlayers >= (TargetID + 2))
                            {
                                TargetID += 2;
                            }
                            else
                            {
                                TargetID = 1;
                            }
                   
                        }
                    }
                    //今のターゲット番号がプレイヤーと同じまたは多いなら最初のプレイヤーに戻す
                    else
                    {
                        //最初のプレイヤーでなければ最初のプレイヤーにターゲット
                        if (PlayerID != 1)
                        {
                            TargetID = 1;
                        }
                        else
                        {
                            TargetID = 2;
                        }
                    }
                    break;
            }

            //ターゲットを移す
            for (int i = 0; i < NumberOfPlayers; i++)
            {
                if (TargetID == Players[i].GetComponent<NetworkIdentity>().netId)
                {
                    TargetTransform = Players[i].GetComponent<Transform>();
                    break;
                }
            }
        }
        //プレイヤー数が２人なら
        else if (NumberOfPlayers == 2)
        {
            if (PlayerID != 1)
            {
                TargetID = 1;
            }
            else
            {
                TargetID = 2;
            }
            //ターゲットを移す
            for (int i = 0; i < NumberOfPlayers; i++)
            {
                if (TargetID == Players[i].GetComponent<NetworkIdentity>().netId)
                {
                    TargetTransform = Players[i].GetComponent<Transform>();
                    break;
                }
            }
        }    
        //プレイヤー数が１人以下なら
        else
        {
            //中心を向く
            GameObject LookAtObj = GameObject.Find("LookAt");
            TargetTransform = LookAtObj.GetComponent<Transform>();
            TargetID = 0;
        }

        //カメラにトランスフォーム挿入
        GameObject MyCamera = GameObject.Find("CameraManager");
        MyCamera.GetComponent<FollowAndLookAtTarget>().LookAt = TargetTransform;
    }
}
