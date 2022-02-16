using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleColliderController1 : MonoBehaviour
{
    private uint myPID ;
    private Vector3 forwardVelocity;

	private void OnParticleCollision(GameObject other)
	{
        if(other.gameObject.tag == "Player"){
		    uint otherID = other.gameObject.GetComponent<PlayerEffectController>().getPId() ;   
            if(otherID != myPID){
                other.gameObject.GetComponent<PlayerEffectController>().damage1();
            }
        }
	}

    public void setPID(uint setMyPID){
        myPID = setMyPID;
    }

    public void setForwardVelocity(Vector3 setForwardVelocity)
    {
        forwardVelocity = setForwardVelocity;
    }
}
