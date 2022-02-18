using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBallController : MonoBehaviour
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
        state = player.getAnim();
        if (state != "guard")
        {
            Destroy(this.gameObject);
        }
        else
        { 
        
        }
    }

    public void setState(string setState) {
        state = setState;
    }

    public void setPID(uint setPID)
    {
        myPID = setPID;
    }
}
