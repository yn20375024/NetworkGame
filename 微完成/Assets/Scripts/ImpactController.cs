using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactController : MonoBehaviour
{
    private string state;
    private string playerName;
    private PlayerEffectController player;

    private uint playerID;
    private Transform objTransform;
    private bool kb_flg = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject searchPlayer = GameObject.Find(playerName);
        player = searchPlayer.GetComponent<PlayerEffectController>();
    }

    // Update is called once per frame
    void Update()
    {
//        state = player.getAnim();
        if ((state == "jab1") || (state == "jab2"))
        {
            Invoke("DestroyObj", 0.25f);
        }
        else if (state == "jab3")
        {
            Invoke("DestroyObj", 0.6f);
        }
        else if (state == "tilt1")
        {
            Invoke("DestroyObj", 0.1f);
        }
        else if (state == "ult")
        {
            Invoke("DestroyObj", 1.0f);
        }
    }

    void DestroyObj()
    {
        Destroy(this.gameObject);
    }

    public void setState(string setState)
    {
        state = setState;
    }

    public void setPlayerName(string setPlayerName)
    {
        playerName = setPlayerName;
    }

    // 当たり判定
    private uint myPID;
    private float damage;
    private Vector3 forwardVelocity;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            uint otherID = other.gameObject.GetComponent<PlayerEffectController>().getPId();
            if (otherID != myPID)
            {
                other.gameObject.GetComponent<PlayerParameterController>().decreaseHp(damage);
                if (kb_flg == false)
                {
                    other.gameObject.GetComponent<PlayerEffectController>().damage1();
                }
                else
                {
                    other.gameObject.GetComponent<PlayerEffectController>().damage2(forwardVelocity);
                }
            }
        }
    }

    public void setDamage(float setDamage)
    {
        damage = setDamage;
    }

    public void setPID(uint setMyPID)
    {
        myPID = setMyPID;
    }

    public void setForwardVelocity(Vector3 setForwardVelocity)
    {
        forwardVelocity = setForwardVelocity;
    }

    public void OnKnockBack()
    {
        kb_flg = true;
    }
}
