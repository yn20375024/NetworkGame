using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerEffectController : NetworkBehaviour
{
    public Transform parentTransform;
    public GameObject effectPrefab1;
    public GameObject effectPrefab2;
    public GameObject effectPrefab3;
    public GameObject effectPrefab4;
    public GameObject effectPrefab5;
    public GameObject effectPrefab6;
    public GameObject effectPrefab7;
    public GameObject effectPrefab8;
    public GameObject effectPrefab9;
    public GameObject effectPrefab10;

    private Transform playerTransform;
    private NetworkAnimator playerAnimator;
    private PlayerTargetController playerTarget;
    private Rigidbody rb;
    private string animState;
    private uint playerID;  

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GetComponent<Transform>();
        playerAnimator = GetComponent<NetworkAnimator>();
        playerTarget = GetComponent<PlayerTargetController>();
        rb = GetComponent<Rigidbody>();
        playerID = playerTarget.getPId();
        animState = "";
    }

    // Update is called once per frame
    void Update()
    {
        //別のアニメーションに切り替わったとき
        if (animState != getAnimState())
        {
            //アニメーション状態を保存
            animState = getAnimState();

            switch (animState) {
                case "jab1":
                case "jab2":
                case "jab3":
                case "tilt1":
                case "tilt1_1":
                case "tilt1_2":
                case "tilt2":
                case "tilt2_2":
                    GameObject obj = Instantiate(effectPrefab5, parentTransform);
                    obj.transform.SetParent(parentTransform);
                    obj.GetComponent<ParticleColliderController>().setPID(playerID);
                    break;
            }

            if (animState == "ult") {
                GameObject obj = Instantiate(effectPrefab7, parentTransform);
                obj.transform.SetParent(parentTransform);
                obj.GetComponent<ParticleColliderController>().setPID(playerID);            
            }

            if (animState == "jumpin")
            {
                Quaternion make_rotation = Quaternion.Euler(270.0f, 0.0f, 0.0f);
                GameObject obj = Instantiate(effectPrefab1, playerTransform.position, make_rotation);
            }

            if (animState == "tilt2_1")
            {
                Vector3 make_position = new Vector3(playerTransform.position.x + (playerTransform.forward.x * 2), playerTransform.position.y + 2.0f, playerTransform.position.z + (playerTransform.forward.z * 2));
                GameObject obj = Instantiate(effectPrefab6, make_position, playerTransform.rotation );
                obj.GetComponent<CounterController>().setPlayerName(gameObject.name);
            }

            if (animState == "guard")
            {
                Vector3 make_position = new Vector3(playerTransform.position.x, playerTransform.position.y + 2.5f, playerTransform.position.z);
                GameObject obj = Instantiate(effectPrefab2, make_position, Quaternion.identity);
                obj.GetComponent<GuardBallController>().setPlayerName(gameObject.name);
            }
        }
    }

    //ダメージ発生関数１
    public void damage1()
    {
        //ガード状態時
        if (animState == "guard") 
        {

        }
        //カウンター状態時
        else if (animState == "tilt2_1")
        {
            playerAnimator.animator.SetBool("Tlit2_Counter_Flg", true);
        }
        //ダメージ受けた時
        else 
        {
            playerAnimator.animator.SetBool("Damage1_Flg", true);
            Instantiate(effectPrefab3, playerTransform);
        }

    }

    //ダメージ発生関数２
    public void damage2(Vector3 forwardVelocity)
    {
        //ガード状態時
        if (animState == "guard")
        {

        }
        //カウンター状態時
        else if (animState == "tilt2_1")
        {
            playerAnimator.animator.SetBool("Tlit2_Counter_Flg", true);
        }
        //ダメージ受けた時
        else
        {
            rb.AddForce(forwardVelocity);
            playerAnimator.animator.SetInteger("Damage2-3_Seq", 1);
            Instantiate(effectPrefab4, playerTransform);
        }
    }

    //publicプレイヤーID取得
    public uint getPId()
    {
        return playerID;
    }

    //publicアニメーション取得
    public string getAnim(){
        return animState ;
    }

    //アニメーション状態取得
    private string getAnimState() {
        string nowAnim = "";

        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("walk"))
        {
            nowAnim = "walk";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("stay"))
        {
            nowAnim = "stay";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("jab1"))
        {
            nowAnim = "jab1";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("jabwait1"))
        {
            nowAnim = "jabwait1";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("jab2"))
        {
            nowAnim = "jab2";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("jabwait2"))
        {
            nowAnim = "jabwait2";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("jab3"))
        {
            nowAnim = "jab3";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("jabend"))
        {
            nowAnim = "jabend";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("dash"))
        {
            nowAnim = "dash";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("dashend"))
        {
            nowAnim = "dashend";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("guard"))
        {
            nowAnim = "guard";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("ult"))
        {
            nowAnim = "ult";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("ultend"))
        {
            nowAnim = "ultend";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("damage1"))
        {
            nowAnim = "damage1";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("damage1end"))
        {
            nowAnim = "damage1end";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("damage2"))
        {
            nowAnim = "damage2";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("damageloop"))
        {
            nowAnim = "damageloop";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("damage3"))
        {
            nowAnim = "damage3";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("damage2-3end"))
        {
            nowAnim = "damage2-3end";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("tilt1"))
        {
            nowAnim = "tilt1";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("tilt1end"))
        {
            nowAnim = "tilt1end";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("tilt2_1"))
        {
            nowAnim = "tilt2_1";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("tilt2_2"))
        {
            nowAnim = "tilt2_2";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("tilt2end"))
        {
            nowAnim = "tilt2end";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("win"))
        {
            nowAnim = "win";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("jumpin"))
        {
            nowAnim = "jumpin";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("jumploop_up"))
        {
            nowAnim = "jumploop_up";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("jumploop_down"))
        {
            nowAnim = "jumploop_down";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("jumpout"))
        {
            nowAnim = "jumpout";
        }

        return nowAnim;
    }

}
