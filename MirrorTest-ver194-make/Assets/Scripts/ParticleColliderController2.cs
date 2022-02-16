using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleColliderController2 : MonoBehaviour
{
    private uint myPID ;
    private Vector3 forwardVelocity;
    private float damage;
    
    // Start is called before the first frame update
    void Start()
    {
    }

	private void OnParticleCollision(GameObject other)
	{
        if(other.gameObject.tag == "Player"){
		    uint otherID = other.gameObject.GetComponent<PlayerEffectController>().getPId() ;   
            float playerHp = other.gameObject.GetComponent<PlayerParameterController>().hp ;   
            if(otherID != myPID){
                Debug.Log(damage);
                playerHp -= damage;
                other.gameObject.GetComponent<PlayerEffectController>().damage2(forwardVelocity);
            }
        }
	}

    public void setPID(uint setMyPID){
        myPID = setMyPID;
    }
    
    public void setDamage(float setDamage){
        damage = setDamage;
    }

    public void setForwardVelocity(Vector3 setForwardVelocity)
    {
        forwardVelocity = setForwardVelocity;
    }
}
