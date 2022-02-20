using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactController : MonoBehaviour
{
    private string state;
    private PlayerEffectController player;
    private uint myPID;

    // Start is called before the first frame update
    void Start()
    {
        int playerNum;
        GameObject[] searchPlayers = GameObject.FindGameObjectsWithTag("Player");
        playerNum = searchPlayers.Length;

        for (int i = 0; i < playerNum; i++)
        {
            if (searchPlayers[i].GetComponent<PlayerEffectController>().getPID() == myPID)
            {
                player = searchPlayers[i].GetComponent<PlayerEffectController>();
                break;
            }
        }
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

    public void setPID(uint setPID)
    {
        myPID = setPID;
    }

    // 当たり判定
    private bool kb_flg = false;
    private float damage;
    private Vector3 forwardVelocity;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            uint otherID = other.gameObject.GetComponent<PlayerEffectController>().getPID();
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

    public void setForwardVelocity(Vector3 setForwardVelocity)
    {
        forwardVelocity = setForwardVelocity;
    }

    public void OnKnockBack()
    {
        kb_flg = true;
    }
}
