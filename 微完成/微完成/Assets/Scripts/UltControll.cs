using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltControll : MonoBehaviour
{
    private uint myPID;
    private string state;
    private string playerName;
    private PlayerEffectController player;
    private Vector3 forwardVelocity;

    // Start is called before the first frame update
    void Start()
    {
        GameObject searchPlayer = GameObject.Find(playerName);
        player = searchPlayer.GetComponent<PlayerEffectController>();
    }

    // Update is called once per frame
    void Update()
    {
        state = player.getAnim();
        if ((state != "ult") && ((state != "stay")))
        {
            Destroy(this.gameObject);
        }
    }

    public void setState(string setState)
    {
        state = setState;
    }

    public void setPlayerName(string setPlayerName)
    {
        playerName = setPlayerName;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            uint otherID = collider.gameObject.GetComponent<PlayerEffectController>().getPID();
            Debug.Log(otherID);
            Debug.Log(myPID);
            if (otherID != myPID)
            {
                collider.gameObject.GetComponent<PlayerEffectController>().damage2(forwardVelocity);
            }
        }
    }

    public void setPID(uint setMyPID)
    {
        myPID = setMyPID;
    }

    public void setForwardVelocity(Vector3 setForwardVelocity)
    {
        forwardVelocity = setForwardVelocity;
    }
}


