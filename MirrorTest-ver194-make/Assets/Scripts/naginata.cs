using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class naginata : NetworkBehaviour
{
    //    public float animSpeed = 1.0f;  // アニメーション再生速度設定

    private Vector3 Player_pos;        // プレイヤーのポジション
    private Vector3 past_pos;          // 1つ前のポジション

    private NetworkAnimator m_Animator;        // アニメーター
    public int lrcheck2 = 0;             // L2R2チェック

    public PhysicMaterial slip;
    public PhysicMaterial nonslip;

    // パラメーター
    //    public float hp = 5.0f;         // 体力
    //    public float attack = 5.0f;     // 攻撃力(技の基礎威力に補正を掛ける値)
    public float speed = 10.0f;      // 移動速度
    public float jump = 10.0f;       // ジャンプ力(初期値)

    private PlayerTargetController PT_Control;        // ターゲット
    private PlayerEffectController PE_Control;

    private float horizontal;
    private float vertical;
    private Rigidbody rb;
    private Collider capsuleCollider;

    private float dtap_NowTime = 0f;                //　最初に移動ボタンが押されてからの経過時間
    private Vector2 dtap_Direction = Vector2.zero;  //　移動キーの押した方向
    private int tapCount = 0;
    private bool doubletapflg = false;              // ダブルタップのフラグ   false->なし   true->あり

    Vector3 cameraForward;
    Vector3 moveForward;

    void Start()
    {
        //ローカルプレイヤーの時のみ以下の処理を行う
        if (!isLocalPlayer) return;

        GameObject cManager = GameObject.Find("CameraManager");
        cManager.GetComponent<FollowAndLookAtTarget>().Follow = transform;

        m_Animator = GetComponent<NetworkAnimator>();

        Player_pos = GetComponent<Transform>().position; // 最初のポジションを取得
        past_pos = Player_pos; // 最初のポジションを取得

        PT_Control = GetComponent<PlayerTargetController>();
        PE_Control = GetComponent<PlayerEffectController>();

        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    void FixedUpdate() {
        //ローカルプレイヤーの時のみ以下の処理を行う
        if (!isLocalPlayer) return;
        //方向キー入力時のみ移動
        if ((horizontal != 0.0) || (vertical != 0.0))
        {
            // 移動
            rb.velocity = new Vector3(moveForward.x, rb.velocity.y, moveForward.z);

            // 回転
            Vector3 diff = new Vector3(rb.position.x - past_pos.x, 0, rb.position.z - past_pos.z);           // 移動幅を求める
            if (diff.magnitude > 0.01f) rb.rotation = Quaternion.LookRotation(diff);                         // 移動していたら向きを変える
            past_pos = rb.position;                                                                          // 位置を更新  
            m_Animator.animator.SetInteger("WalkTrigger", 1);
        }
        //入力ない時回転速度ゼロ
        else 
        {
            rb.angularVelocity = Vector3.zero;
            m_Animator.animator.SetInteger("WalkTrigger", 2);
        }
    }


    void Update()
    {
        //ローカルプレイヤーの時のみ以下の処理を行う
        if (!isLocalPlayer) return;

        // 方向キー入力
        horizontal = (Input.GetAxis("Horizontal"));   // -1 ~ 1が格納される  マイナス → 左 / プラス → 右
        vertical = (Input.GetAxis("Vertical"));     //                     マイナス → 下 / プラス → 上

        //物理マテリアルの変更
        if ((checkIsGround(rb.position, 0.3f) == true))
        {
            capsuleCollider.material = slip;
        }
        else 
        {
            capsuleCollider.material = nonslip;
        }

        DoubleTap();                            // ダブルタップ
        if (doubletapflg == true)
        {
            // ダッシュアニメーションtrue
            m_Animator.animator.SetInteger("DashTrigger", 1);
        }
        else
        {
            // ダッシュアニメーションfalse
            m_Animator.animator.SetInteger("DashTrigger", 2);
        }

        // カメラの方向に合わせた正面を設定
        cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        // カメラの方向に合わせた移動方向を決定
        moveForward = vertical * cameraForward * speed + horizontal * Camera.main.transform.right * speed;

        // ダブルタップ中のダッシュ移動
        if (doubletapflg == true)
        {
            // カメラの方向に合わせた移動方向を決定
            moveForward = vertical * cameraForward * (speed * 10) + horizontal * Camera.main.transform.right * (speed * 10);
        }

        // どのボタンが押されたかチェック
        KeyCheck();

        // ジャンプ着地処理
        if(m_Animator.animator.GetInteger("Jump_Seq") == 1 ){
            //飛び上がり
            if(rb.velocity.y <= 0){
                m_Animator.animator.SetInteger("Jump_Seq", 2);
            }
        }else if(m_Animator.animator.GetInteger("Jump_Seq") == 2 ){
            //着地
            if(rb.velocity.y == 0){
                m_Animator.animator.SetInteger("Jump_Seq", 3);
            }
        }else{
            //stayに戻る
            m_Animator.animator.SetInteger("Jump_Seq", 0);
        }

        // 吹き飛び着地処理
        if (m_Animator.animator.GetInteger("Damage2-3_Seq") == 1)
        {
            //飛び上がり
            if (rb.velocity.y <= 0)
            {
                m_Animator.animator.SetInteger("Damage2-3_Seq", 2);
            }
        }
        else if (m_Animator.animator.GetInteger("Damage2-3_Seq") == 2)
        {
            //着地
            if (rb.velocity.y == 0)
            {
                m_Animator.animator.SetInteger("Damage2-3_Seq", 3);
            }
        }
        else
        {
            //stayに戻る
            m_Animator.animator.SetInteger("Damage2-3_Seq", 0);
        }

        // L2R2アニメーション（長押し用）
        if (lrcheck2 == 1)
        {
            m_Animator.animator.SetInteger("Guard", 1);
        }
        else if (lrcheck2 == 2)
        {
            m_Animator.animator.SetInteger("Ult", 1);
        }
        else
        {
            m_Animator.animator.SetInteger("Guard", 2);
            m_Animator.animator.SetInteger("Ult", 2);
        }


        //end時、アニメーションのフラグをfalseにする
        if (PE_Control.getAnim() == "tilt1end")
        {
            m_Animator.animator.SetBool("Tilt1_Flg", false);
        }
        //end時、アニメーションのフラグをfalseにする
        if (PE_Control.getAnim() == "damage1end")
        {
            m_Animator.animator.SetBool("Damage1_Flg", false);
        }
        //end時、アニメーションのフラグをfalseにする
        if (PE_Control.getAnim() == "tilt2end")
        {
            m_Animator.animator.SetBool("Tlit2_Counter_Flg", false);
        }
        //end時、アニメーションのフラグカウントを0にする
        if (PE_Control.getAnim() == "jabend")
        {
            m_Animator.animator.SetInteger("Jab_Seq", 0);
        }
        //end時、アニメーションのフラグカウントを0にする
        if (PE_Control.getAnim() == "tilt2end")
        {
            m_Animator.animator.SetInteger("Tilt2_Seq", 0);
        }
        //end時、アニメーションのフラグカウントを0にする
        if (PE_Control.getAnim() == "damage2-3end")
        {
            m_Animator.animator.SetInteger("Damage2-3_Seq", 0);
        }
        //end時、アニメーションのフラグカウントを0にする
        if (PE_Control.getAnim() == "ultend")
        {
            m_Animator.animator.SetInteger("Ult", 0);
            lrcheck2 = 0 ;
        }
    }
    
    //着地判定
    bool checkIsGround(Vector3 position, float distance)
    {
        //レイキャストを地面に放つ
        Ray ray = new Ray(position, Vector3.down);
        //Raycastの判定を返す
        return Physics.Raycast(ray, distance);
    }

    // 連続タップ検知
    void DoubleTap()
    {
        const float dtap_LimitTime = 0.3f;      //　次に移動ボタンが押されるまでの時間
        float limitAngle = 3f;                  //　最初に押した方向との違いの限度角度

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 1回目押下
        if (tapCount == 0)
        {
            if (h != 0 || v != 0)
            {
                //　移動キーの方向ベクトルを取得
                dtap_Direction = new Vector2(h, v);
                dtap_NowTime = 0f;
                tapCount++;
            }
        }

        // 1度離す
        if (tapCount == 1)
        {
            if (h == 0 && v == 0)
            {
                tapCount++;
            }
        }

        // 2回目押下
        if (tapCount == 2)
        {
            if (h != 0 || v != 0)
            {
                //　2回目に移動キーを押した時の方向ベクトルを取得
                var nowDirection = new Vector2(h, v);

                //　押した方向がリミットの角度を越えていない　かつ　制限時間内に移動キーが押されていれば走る
                if (Vector2.Angle(nowDirection, dtap_Direction) < limitAngle && dtap_NowTime <= dtap_LimitTime)
                {
                    doubletapflg = true;
                    Debug.Log("ダブルタップ");
                    tapCount++;
                }
            }
        }

        //　最初の移動キーを押していれば時間計測
        if (tapCount != 0)
        {
            //　時間計測
            dtap_NowTime += Time.deltaTime;

            if (dtap_NowTime > dtap_LimitTime)
            {
                doubletapflg = false;
                tapCount = 0;
            }
        }
    }

    // ボタンとアクションの対応
    void KeyCheck()
    {
        //【 〇　弱攻撃 】
        if (Input.GetKeyDown("joystick button 1") || Input.GetKeyDown(KeyCode.Z) ) 
        {
            if (m_Animator.animator.GetInteger("Jab_Seq") == 0) {
                m_Animator.animator.SetInteger("Jab_Seq", 1);
            }
            else if ((m_Animator.animator.GetInteger("Jab_Seq") == 1) && (PE_Control.getAnim() == "jabwait1") || (PE_Control.getAnim() == "jab1"))
            {
                m_Animator.animator.SetInteger("Jab_Seq", 2);
            }
            else if ((m_Animator.animator.GetInteger("Jab_Seq") == 2) && (PE_Control.getAnim() == "jabwait2") || (PE_Control.getAnim() == "jab2"))
            {
                m_Animator.animator.SetInteger("Jab_Seq", 3);
            }
            else {
                m_Animator.animator.SetInteger("Jab_Seq", 0);
            }
        }

        //【 △　強攻撃１ 】
        if (Input.GetKeyDown("joystick button 0"))
        {
            m_Animator.animator.SetBool("Tilt1_Flg", true);
        }

        //【 □　強攻撃２ 】
        if (Input.GetKeyDown("joystick button 3") || Input.GetKeyDown(KeyCode.C))
        {
            if (m_Animator.animator.GetInteger("Tilt2_Seq") == 0){
                m_Animator.animator.SetInteger("Tilt2_Seq", 1);
            }
        }

        //【 ×　ジャンプ 】
        // スペースキーでジャンプ
        if ((Input.GetKeyDown(KeyCode.Space)) || (Input.GetKeyDown("joystick button 2")))
        {
            // 接地しているならジャンプ
            if( (checkIsGround(rb.position, 0.3f) == true) && (m_Animator.animator.GetInteger("Jump_Seq") == 0 ) )
            {
                m_Animator.animator.SetInteger("Jump_Seq", 1);
                capsuleCollider.material = nonslip;
                rb.AddForce(new Vector3(0.0f, jump, 0.0f), ForceMode.Impulse);
            }
        }

        //【 L2+R2　必殺技 】
        if (Input.GetKeyDown("joystick button 4") && Input.GetKeyDown("joystick button 5") || Input.GetKeyDown(KeyCode.X))
        {
            lrcheck2 = 2;
            Debug.Log("必殺技");
        }
        //【 R2　ダッシュ 】
        else if (Input.GetKeyDown("joystick button 5"))
        {
            doubletapflg = true;
            Debug.Log("ダッシュ");
        }
        //【 L2　ガード 下げ】
        else if (Input.GetKeyDown("joystick button 4") || Input.GetKeyDown(KeyCode.V))
        {
            lrcheck2 = 1;
            Debug.Log("ガード");
        }
        //【 L2　ガード 上げ】
        else if (Input.GetKeyUp("joystick button 4") || Input.GetKeyUp(KeyCode.V))
        {
            lrcheck2 = 0;
        }

        //【 L1　ターゲット切り替え左 】
        if (Input.GetKeyDown("joystick button 6"))
        {
            PT_Control.ChangeTarget("BACK");
            Debug.Log("ターゲット切り替え左");
        }

        //【 R1　ターゲット切り替え右 】
        if (Input.GetKeyDown("joystick button 7"))
        {
            PT_Control.ChangeTarget("NEXT");
            Debug.Log("ターゲット切り替え右");
        }
    }

    /* ------------------------------------------------ */
    // Winアニメーション      
    //m_Animator.animator.SetBool("Win", false);
    //m_Animator.animator.SetBool("Win", true);
    // ダメージアニメーション      
    //m_Animator.SetTrigger("Damage1");
    //m_Animator.SetTrigger("Damage2");
    //m_Animator.SetTrigger("Damage3");
    //m_Animator.SetTrigger("Damage4");
    /* ------------------------------------------------ */
}

