using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleColliderController1 : MonoBehaviour
{
    private uint myPID ;
    private float damage;
    
    // Start is called before the first frame update
    void Start()
    {
    }

	private void OnParticleCollision(GameObject other)
	{
        if(other.gameObject.tag == "Player"){
		    uint otherID = other.gameObject.GetComponent<PlayerEffectController>().getPId() ;   
            if(otherID != myPID){
                other.gameObject.GetComponent<PlayerParameterController>().decreaseHp(damage);
                other.gameObject.GetComponent<PlayerEffectController>().damage1();
            }
        }
	}
    public void setDamage(float setDamage){
        damage = setDamage;
    }

    public void setPID(uint setMyPID){
        myPID = setMyPID;
    }
}
