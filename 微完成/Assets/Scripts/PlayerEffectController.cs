using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerEffectController : NetworkBehaviour
{
    public string playerType;
    public Transform parentTransform;
    public GameObject jumpPrefab;
    public GameObject guardPrefab;
    public GameObject damagePrefab1;
    public GameObject damagePrefab2;
    public GameObject effectPrefab1;
    public GameObject effectPrefab2;
    public GameObject effectPrefab3;
    public GameObject effectPrefab4;
    public GameObject effectPrefab5;

    private Transform playerTransform;
    private NetworkAnimator playerAnimator;
    private PlayerTargetController playerTarget;
    private Rigidbody rb;
    private string animState;
    private string animTag;
    private uint playerID;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GetComponent<Transform>();
        playerAnimator = GetComponent<NetworkAnimator>();
        playerTarget = GetComponent<PlayerTargetController>();
        rb = GetComponent<Rigidbody>();
        playerID = playerTarget.getPID();
        animState = "";
        animTag = "";
    }

    // Update is called once per frame
    void Update()
    {
        //別のアニメーションに切り替わったとき
        if (animState != getAnimState())
        {
            //アニメーション状態を保存
            animState = getAnimState();
            //アニメーションのタグを保存
            animTag = getAnimTag();

            if (animState == "jumpin")
            {
                Quaternion make_rotation = Quaternion.Euler(270.0f, 0.0f, 0.0f);
                GameObject obj = Instantiate(jumpPrefab, playerTransform.position, make_rotation);
            }
            if (animState == "guard")
            {
                Vector3 make_position = new Vector3(playerTransform.position.x, playerTransform.position.y + 2.5f, playerTransform.position.z);
                GameObject obj = Instantiate(guardPrefab, make_position, Quaternion.identity);
                obj.GetComponent<GuardBallController>().setPID(playerID);
            }

            switch(playerType){
                case "ガンマン":
                    gunmanEffectPattern();
                    break;
                case "サモナー":
                    summonerEffectPattern();
                    break;
                case "忍者":
                    ninjaEffectPattern();
                    break;
                case "格闘家":
                    fighterEffectPattern();
                    break;
                case "海賊":
                    piratesEffectPattern();
                    break;
                case "薙刀":
                    naginataEffectPattern();
                    break;
                case "騎士":
                    knightEffectPattern();
                    break;
                case "魔法使い":
                    magicianEffectPattern();
                    break;
            }

        }
    }
    
    //ガンマンエフェクトパターン
    void gunmanEffectPattern()
    {
        if (animState == "ult")
        {
            Vector3 make_position = new Vector3(playerTransform.position.x + (playerTransform.forward.x * 3), playerTransform.position.y + 3.0f, playerTransform.position.z + (playerTransform.forward.z * 3));
            GameObject obj = Instantiate(effectPrefab2, make_position, playerTransform.rotation);
        }

        if (animState == "jab1" || animState == "jab2")
        {
            Vector3 make_position = new Vector3(playerTransform.position.x + (playerTransform.forward.x * 3), playerTransform.position.y + 3.0f, playerTransform.position.z + (playerTransform.forward.z * 3));
            GameObject obj = Instantiate(effectPrefab1, make_position, playerTransform.rotation);
            obj.GetComponent<ParticleScript>().setThrowPower(new Vector3(playerTransform.forward.x * 200.0f, 0.0f, playerTransform.forward.z * 200.0f));
            obj.GetComponent<ParticleScript>().setPID(playerID);
            obj.GetComponent<ParticleScript>().setDamage(5.0f);
        }

        if (animState == "tilt2end")
        {
            Vector3 make_position = new Vector3(playerTransform.position.x + (playerTransform.forward.x * 2), playerTransform.position.y + 5.0f, playerTransform.position.z + (playerTransform.forward.z * 2));
            GameObject obj = Instantiate(effectPrefab3, make_position, playerTransform.rotation);
            obj.GetComponent<ParticleScript>().setThrowPower(new Vector3(playerTransform.forward.x * 5.0f, 5.0f, playerTransform.forward.z * 5.0f));

        }
    }

    //サモナーエフェクトパターン
    void summonerEffectPattern()
    {
    }    

    //忍者エフェクトパターン
    void ninjaEffectPattern()
    {
    }

    //格闘家エフェクトパターン
    void fighterEffectPattern()
    {
        if ((animState == "jab1") || (animState == "jab2"))
        {
            Vector3 make_position = new Vector3(playerTransform.position.x + (playerTransform.forward.x * 2), playerTransform.position.y + 2.5f, playerTransform.position.z + (playerTransform.forward.z * 2));
            GameObject obj = Instantiate(effectPrefab1, make_position, playerTransform.rotation);
            obj.GetComponent<ImpactController>().setPID(playerID);
            obj.GetComponent<ImpactController>().setDamage(5.0f);
            obj.GetComponent<ImpactController>().setPlayerName(gameObject.name);
            obj.GetComponent<ImpactController>().setState("jab1");
        }

        if (animState == "jab3")
        {
            Vector3 make_position = new Vector3(playerTransform.position.x + (playerTransform.forward.x * 2), playerTransform.position.y + 2.5f, playerTransform.position.z + (playerTransform.forward.z * 2));
            GameObject obj = Instantiate(effectPrefab1, make_position, playerTransform.rotation);
            obj.GetComponent<ImpactController>().setPID(playerID);
            obj.GetComponent<ImpactController>().OnKnockBack();
            obj.GetComponent<ImpactController>().setForwardVelocity(new Vector3(playerTransform.forward.x * 8.0f, 5.0f, playerTransform.forward.z * 8.0f));
            obj.GetComponent<ImpactController>().setDamage(5.0f);
            obj.GetComponent<ImpactController>().setPlayerName(gameObject.name);
            obj.GetComponent<ImpactController>().setState("jab3");
        }

        if (animState == "tilt1")
        {
            Invoke("fighterEffectPattern_tilt1", 0.05f);
            Invoke("fighterEffectPattern_tilt1", 0.1f);
            Invoke("fighterEffectPattern_tilt1", 0.15f);
            Invoke("fighterEffectPattern_tilt1", 0.2f);
            Invoke("fighterEffectPattern_tilt1", 0.25f);
            Invoke("fighterEffectPattern_tilt1", 0.3f);
            Invoke("fighterEffectPattern_tilt1_finish", 0.6f);
        }

        if (animState == "tilt2_1")
        {
            GameObject obj = Instantiate(effectPrefab2, parentTransform);
            obj.transform.SetParent(parentTransform);
        }

        if (animState == "ult")
        {
            Invoke("fighterEffectPattern_ult", 0.25f);
        }
    }
    // 格闘家強攻撃１連打
    void fighterEffectPattern_tilt1()
    {
        Vector3 make_position = new Vector3(playerTransform.position.x + (playerTransform.forward.x * 2), playerTransform.position.y + 2.5f, playerTransform.position.z + (playerTransform.forward.z * 2));
        make_position.x += Random.Range(-1.0f, 1.0f);
        make_position.y += Random.Range(-1.0f, 1.0f);
        GameObject obj = Instantiate(effectPrefab1, make_position, playerTransform.rotation);
        obj.GetComponent<ImpactController>().setPID(playerID);
        obj.GetComponent<ImpactController>().setDamage(5.0f);
        obj.GetComponent<ImpactController>().setPlayerName(gameObject.name);
        obj.GetComponent<ImpactController>().setState("tilt1");
    }
    // 格闘家強攻撃１アッパー
    void fighterEffectPattern_tilt1_finish()
    {
        Vector3 make_position = new Vector3(playerTransform.position.x + (playerTransform.forward.x * 4), playerTransform.position.y + 2.7f, playerTransform.position.z + (playerTransform.forward.z * 4));
        Quaternion make_rotation = Quaternion.Euler(-60, 0, 0);
        GameObject obj = Instantiate(effectPrefab1, make_position, playerTransform.rotation * make_rotation);
        obj.GetComponent<ImpactController>().setPID(playerID);
        obj.GetComponent<ImpactController>().OnKnockBack();
        obj.GetComponent<ImpactController>().setForwardVelocity(new Vector3(playerTransform.forward.x * 7.0f, 12.0f, playerTransform.forward.z * 7.0f));
        obj.GetComponent<ImpactController>().setDamage(5.0f);
        obj.GetComponent<ImpactController>().setPlayerName(gameObject.name);
        obj.GetComponent<ImpactController>().setState("tilt1");
    }
    // 格闘家必殺技
    void fighterEffectPattern_ult()
    {
        Vector3 make_position = new Vector3(playerTransform.position.x + (playerTransform.forward.x * 4), playerTransform.position.y + 2.5f, playerTransform.position.z + (playerTransform.forward.z * 4));
        GameObject obj = Instantiate(effectPrefab3, make_position, playerTransform.rotation);
        obj.GetComponent<ImpactController>().setPID(playerID);
        obj.GetComponent<ImpactController>().OnKnockBack();
        obj.GetComponent<ImpactController>().setForwardVelocity(new Vector3(playerTransform.forward.x * 10.0f, 10.0f, playerTransform.forward.z * 10.0f));
        obj.GetComponent<ImpactController>().setDamage(5.0f);
        obj.GetComponent<ImpactController>().setPlayerName(gameObject.name);
        obj.GetComponent<ImpactController>().setState("ult");

        Vector3 make_position2 = new Vector3(playerTransform.position.x + (playerTransform.forward.x * 3), playerTransform.position.y + 2.5f, playerTransform.position.z + (playerTransform.forward.z * 3));
        GameObject obj2 = Instantiate(effectPrefab1, make_position2, playerTransform.rotation);
        obj2.GetComponent<ImpactController>().setPID(playerID);
        obj.GetComponent<ImpactController>().OnKnockBack();
        obj.GetComponent<ImpactController>().setForwardVelocity(new Vector3(playerTransform.forward.x * 10.0f, 10.0f, playerTransform.forward.z * 10.0f));
        obj2.GetComponent<ImpactController>().setDamage(5.0f);
        obj2.GetComponent<ImpactController>().setPlayerName(gameObject.name);
        obj2.GetComponent<ImpactController>().setState("ult");
    }

    //海賊エフェクトパターン
    void piratesEffectPattern()
    {
        switch (animState)
        {
            case "jab1":
            case "jab2":
            case "tilt1":
                GameObject obj = Instantiate(effectPrefab3, parentTransform);
                obj.transform.SetParent(parentTransform);
                obj.GetComponent<ParticleColliderController1>().setPID(playerID);
                obj.GetComponent<ParticleColliderController1>().setDamage(5.0f);
                break;
        }

        if (animState == "tilt2_2"){
                GameObject obj = Instantiate(effectPrefab4, parentTransform);
                obj.transform.SetParent(parentTransform);
                obj.GetComponent<ParticleColliderController2>().setPID(playerID);
                obj.GetComponent<ParticleColliderController2>().setForwardVelocity(new Vector3(playerTransform.forward.x * 10.0f, 10.0f, playerTransform.forward.z * 10.0f));
                obj.GetComponent<ParticleColliderController2>().setDamage(5.0f);
                obj.GetComponent<RollControll>().setPlayerName(gameObject.name);
        }

        if (animState == "ult")
        {
            Invoke(nameof(kaizokuult), 0.5f);
        }
    }

    void kaizokuult()
    {
        Vector3 make1_position = new Vector3(playerTransform.position.x, playerTransform.position.y + (float)9.0, playerTransform.position.z);
        Vector3 make2_position = new Vector3(playerTransform.position.x, playerTransform.position.y + (float)6.0, playerTransform.position.z);
        Vector3 make3_position = new Vector3(playerTransform.position.x, playerTransform.position.y + (float)3.0, playerTransform.position.z);
        GameObject obj1 = Instantiate(effectPrefab1, make3_position, Quaternion.Euler(-90, 0, 0));
        GameObject obj2 = Instantiate(effectPrefab2, make2_position, Quaternion.Euler(-90, 0, 0));
        GameObject obj3 = Instantiate(effectPrefab5, make1_position, Quaternion.Euler(-90, 0, 0));
        obj1.GetComponent<ParticleColliderController2>().setPID(playerID);
        obj2.GetComponent<ParticleColliderController2>().setPID(playerID);
        obj3.GetComponent<ParticleColliderController2>().setPID(playerID);
        obj1.GetComponent<UltControll>().setPlayerName(gameObject.name);
        obj2.GetComponent<UltControll>().setPlayerName(gameObject.name);
        obj3.GetComponent<UltControll>().setPlayerName(gameObject.name);
        obj1.GetComponent<UltControll>().setForwardVelocity(new Vector3(playerTransform.forward.x * 10.0f, 10.0f, playerTransform.forward.z * 10.0f));
        obj2.GetComponent<UltControll>().setForwardVelocity(new Vector3(playerTransform.forward.x * 10.0f, 10.0f, playerTransform.forward.z * 10.0f));
        obj3.GetComponent<UltControll>().setForwardVelocity(new Vector3(playerTransform.forward.x * 10.0f, 10.0f, playerTransform.forward.z * 10.0f));
    }

    //薙刀エフェクトパターン
    void naginataEffectPattern()
    {
        switch (animState)
        {
            case "jab1":
            case "jab2":
            case "jab3":
            case "tilt2":
            case "tilt2_2":
                GameObject obj = Instantiate(effectPrefab1, parentTransform);
                obj.transform.SetParent(parentTransform);
                obj.GetComponent<ParticleColliderController1>().setPID(playerID);
                obj.GetComponent<ParticleColliderController1>().setDamage(5.0f);
                break;
        }

        if (animState == "tilt1")
        {
            GameObject obj = Instantiate(effectPrefab2, parentTransform);
            obj.transform.SetParent(parentTransform);
            obj.GetComponent<ParticleColliderController2>().setPID(playerID);
            obj.GetComponent<ParticleColliderController2>().setForwardVelocity(new Vector3(playerTransform.forward.x * 3.0f, 3.0f, playerTransform.forward.z * 3.0f));
            obj.GetComponent<ParticleColliderController2>().setDamage(10.0f);
        }

        if (animState == "ult")
        {
            GameObject obj = Instantiate(effectPrefab2, parentTransform);
            obj.transform.SetParent(parentTransform);
            obj.GetComponent<ParticleColliderController2>().setPID(playerID);
            obj.GetComponent<ParticleColliderController2>().setForwardVelocity(new Vector3(playerTransform.forward.x * 10.0f, 10.0f, playerTransform.forward.z * 10.0f));
            obj.GetComponent<ParticleColliderController2>().setDamage(50.0f);
        }

        if (animState == "tilt2_1")
        {
            Vector3 make_position = new Vector3(playerTransform.position.x + (playerTransform.forward.x * 2), playerTransform.position.y + 2.0f, playerTransform.position.z + (playerTransform.forward.z * 2));
            GameObject obj = Instantiate(effectPrefab3, make_position, playerTransform.rotation);
            obj.GetComponent<CounterController>().setPID(playerID);
        }
    }

    //騎士エフェクトパターン
    void knightEffectPattern()
    {
    }

    //魔法使いエフェクトパターン
    void magicianEffectPattern()
    {
    }

    //ダメージ発生関数１
    public void damage1()
    {
        //ガード状態時
        if (animState == "guard") 
        {

        }
        //カウンター状態時
        else if (animState == "tilt2_1" && animTag == "counter")
        {
            playerAnimator.animator.SetBool("Tilt2_Counter_Flg", true);
        }
        //ダメージ受けた時
        else 
        {
            playerAnimator.animator.SetBool("Damage1_Flg", true);
            Instantiate(damagePrefab1, playerTransform);
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
        else if (animState == "tilt2_1" && animTag == "counter")
        {
            playerAnimator.animator.SetBool("Tilt2_Counter_Flg", true);
        }
        //ダメージ受けた時
        else
        {
            rb.AddForce(forwardVelocity, ForceMode.Impulse);
            playerAnimator.animator.SetInteger("Damage2-3_Seq", 1);
            Instantiate(damagePrefab2, playerTransform);
        }
    }

    //publicプレイヤーID取得
    public uint getPID()
    {
        return playerID;
    }

    //publicアニメーション取得
    public string getAnim()
    {
        return animState;
    }

    //アニメーションタグ取得
    private string getAnimTag()
    {
        string nowTag = "";

        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsTag("counter"))
        {
            nowTag = "counter";
        }

        return nowTag;
    }

    //アニメーション状態取得
    private string getAnimState()
    {
        string nowAnim = "";
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("start"))
        {
            nowAnim = "start";
        }
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("end"))
        {
            nowAnim = "end";
        }
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
        if (playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName("tilt2"))
        {
            nowAnim = "tilt2";
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
