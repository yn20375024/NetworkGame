using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    public GameObject particleObject;
    private Vector3 throwPower;
    private Rigidbody rb;

    private uint myPID;
    private float damage;

    void Start(){
        rb = GetComponent<Rigidbody>();
        rb.AddForce(throwPower, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        bool hitflg = true;
        if (other.gameObject.tag == "Player")
        {
            uint otherID = other.gameObject.GetComponent<PlayerEffectController>().getPID();

            if (otherID != myPID)
            {
                other.gameObject.GetComponent<PlayerParameterController>().decreaseHp(damage);
                other.gameObject.GetComponent<PlayerEffectController>().damage1();
            }
            else 
            {
                hitflg = false;
            }
        }

        if (hitflg == true) {
            Instantiate(particleObject, this.transform.position, Quaternion.identity); //パーティクル用ゲームオブジェクト生成
            Destroy(this.gameObject); //衝突したゲームオブジェクトを削除
        }
    }

    public void setThrowPower(Vector3 setThrowPower)
    {
        throwPower = setThrowPower;
    }

    public void setDamage(float setDamage)
    {
        damage = setDamage;
    }

    public void setPID(uint setMyPID)
    {
        myPID = setMyPID;
    }
}