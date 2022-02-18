using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBallController : MonoBehaviour
{
    private string state;
    private string playerName;
    private PlayerEffectController player;

    // Start is called before the first frame update
    void Start()
    {
        GameObject searchPlayer = GameObject.Find(playerName);
        player = searchPlayer.GetComponent<PlayerEffectController>() ; 
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

    public void setPlayerName(string setPlayerName)
    {
        playerName = setPlayerName;
    }
}
