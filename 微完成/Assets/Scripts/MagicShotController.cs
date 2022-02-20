using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicShotController : MonoBehaviour
{
    private string state;
    private PlayerEffectController player;
    private uint myPID;

    private Vector3 throwPower;
    private Rigidbody rb;

    Vector3 startPosition;

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
                state = player.getAnim();

                rb = GetComponent<Rigidbody>();
                rb.AddForce(throwPower, ForceMode.Impulse);
                EffController();

                target = player.GetComponent<PlayerTargetController>().getTargetTransform();
                break;
            }
        }
    }

    public void setThrowPower(Vector3 setThrowPower)
    {
        throwPower = setThrowPower;
    }

    //速度
    Vector3 velocity;
    // 加速度
    public Vector3 acceleration;
    // ターゲットをセットする
    public Transform target;
    // 着弾時間
    float period = 5f;

    // Update is called once per frame
    void Update()
    {
        if (state == "tilt1")
        {
            acceleration = Vector3.zero;

            //ターゲットと自分自身の差
            target = player.GetComponent<PlayerTargetController>().getTargetTransform();
            Vector3 diff = new Vector3(target.position.x - transform.position.x, (target.position.y - transform.position.y) + 2.0f, target.position.z - transform.position.z);

            //加速度を求めてるらしい
            acceleration += (diff - velocity * period) * 2f / (period * period);

            // 着弾時間を徐々に減らしていく
            period -= Time.deltaTime;

            // 速度の計算
            velocity += acceleration * Time.deltaTime;

            // 移動処理
            rb.MovePosition(transform.position + velocity * Time.deltaTime);
        }
    }

    public void setTargetTranform(Transform arg_target)
    {
        target = arg_target;
    }

    void EffController()
    {
        state = player.getAnim();
        if (state == "jab1")
        {
            Invoke("DestroyObj", 3.0f);
        }

        if (state == "tilt1")
        {
            Invoke("DestroyObj", 10.0f);
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
        Destroy(this.gameObject);
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
