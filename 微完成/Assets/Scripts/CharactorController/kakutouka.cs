using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class kakutouka : NetworkBehaviour
{
    private Vector3 Player_pos;        // プレイヤーのポジション
    private Vector3 past_pos;          // 1つ前のポジション

    private NetworkAnimator m_Animator;        // アニメーター
    public int lrcheck2 = 0;             // L2R2チェック

    public PhysicMaterial slip;
    public PhysicMaterial nonslip;

    private PlayerTargetController PT_Control;          // ターゲット
    private PlayerEffectController PE_Control;          // エフェクト
    private PlayerParameterController PP_Control;       // パラメータ

    private float horizontal;
    private float vertical;
    private Rigidbody rb;
    private Collider capsuleCollider;

    private float dtap_NowTime = 0.0f;                //　最初に移動ボタンが押されてからの経過時間
    private Vector2 dtap_Direction = Vector2.zero;  //　移動キーの押した方向
    private int tapCount = 0;
    private bool doubletapflg = false;              // ダブルタップのフラグ   false->なし   true->あり

    Vector3 cameraForward;
    Vector3 moveForward;
    Vector3 dashForward;

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
        PP_Control = GetComponent<PlayerParameterController>();

        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    void FixedUpdate()
    {
        //ローカルプレイヤーの時のみ以下の処理を行う
        if (!isLocalPlayer) return;

        PhysicalProcess();      //Rigidbodyの演算処理
    }

    void Update()
    {
        //ローカルプレイヤーの時のみ以下の処理を行う
        if (!isLocalPlayer) return;

        DoubleTap();            //ダブルタップ
        KeyCheck();             //キーチェック                     ---キャラ個別
        EndFlgReset();          //アニメーションフラグのリセット    ---キャラ個別
        MoveProcess();          //移動時の設定、計算
        CommonAnimation();      //共通アニメーション
    }

    // ボタンとアクションの対応
    void KeyCheck()
    {
        //【 〇　弱攻撃 】
        if (Input.GetKeyDown("joystick button 1")
            && (PE_Control.getAnim() == "jab1" || PE_Control.getAnim() == "jab2" || PE_Control.getAnim() == "jab1wait" || PE_Control.getAnim() == "jab2wait"
            || PE_Control.getAnim() == "walk" || PE_Control.getAnim() == "stay"))
        {
            if (m_Animator.animator.GetInteger("Jab_Seq") == 0)
            {
                rb.velocity = Vector3.zero;
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
            else
            {
                m_Animator.animator.SetInteger("Jab_Seq", 0);
            }
        }

        //【 △　強攻撃１ 】
        if (Input.GetKeyDown("joystick button 0")
            && (PE_Control.getAnim() == "walk" || PE_Control.getAnim() == "stay"))
        {
            rb.velocity = Vector3.zero;
            m_Animator.animator.SetBool("Tilt1_Flg", true);
        }

        //【 □　強攻撃２ 】
        if (Input.GetKeyDown("joystick button 3")
            && (PE_Control.getAnim() == "walk" || PE_Control.getAnim() == "stay"))
        {
            if (transform.Find("aura(Clone)") == null)
            {
                rb.velocity = Vector3.zero;
                if (m_Animator.animator.GetInteger("Tilt2_Seq") == 0)
                {
                    m_Animator.animator.SetInteger("Tilt2_Seq", 1);
                }
            }
        }

        //【 ×　ジャンプ 】
        // walkかstayならスペースキーでジャンプ
        if ((Input.GetKeyDown("joystick button 2"))
            && (PE_Control.getAnim() == "stay" || PE_Control.getAnim() == "walk"))
        {
            // 接地しているならジャンプ
            if ((checkIsGround(rb.position, 0.3f) == true) && (m_Animator.animator.GetInteger("Jump_Seq") == 0))
            {
                m_Animator.animator.SetInteger("Jump_Seq", 1);
                capsuleCollider.material = slip;
                rb.AddForce(new Vector3(0.0f, PP_Control.jump, 0.0f), ForceMode.Impulse);
            }
        }

        //【 L2+R2　必殺技 】
        if (Input.GetKeyDown("joystick button 4") && Input.GetKeyDown("joystick button 5")
            && (PE_Control.getAnim() == "walk" || PE_Control.getAnim() == "stay"))
        {
            rb.velocity = Vector3.zero;
            lrcheck2 = 2;
            Debug.Log("必殺技");
        }
        //【 R2　ダッシュ 】
        else if (Input.GetKeyDown("joystick button 5")
                && (PE_Control.getAnim() == "walk" || PE_Control.getAnim() == "stay"))
        {
            m_Animator.animator.SetBool("Dash_Flg", true);
        }
        //【 L2　ガード 下げ】
        else if (Input.GetKeyDown("joystick button 4")
                && (PE_Control.getAnim() == "walk" || PE_Control.getAnim() == "stay"))
        {
            rb.velocity = Vector3.zero;
            lrcheck2 = 1;
            Debug.Log("ガード");
        }
        //【 L2　ガード 上げ】
        else if (Input.GetKeyUp("joystick button 4"))
        {
            lrcheck2 = 0;
        }

        //【 L1　ターゲット切り替え左 】
        if (Input.GetKeyDown("joystick button 6"))
        {
            PT_Control.ChangeTarget("BACK");
        }

        //【 R1　ターゲット切り替え右 】
        if (Input.GetKeyDown("joystick button 7"))
        {
            PT_Control.ChangeTarget("NEXT");
        }
    }

    //アニメーションフラグのリセット
    void EndFlgReset()
    {
        if (PE_Control.getAnim() == "start")
        {
            m_Animator.animator.SetBool("Damage1_Flg", false);
            m_Animator.animator.SetInteger("Damage2-3_Seq", 0);
            m_Animator.animator.SetBool("Tilt1_Flg", false);
            m_Animator.animator.SetInteger("Tilt2_Seq", 0);
            m_Animator.animator.SetInteger("WalkTrigger", 2);
            m_Animator.animator.SetBool("Dash_Flg", false);
            m_Animator.animator.SetInteger("Jab_Seq", 0);
            m_Animator.animator.SetInteger("Jump_Seq", 0);
            m_Animator.animator.SetInteger("Ult", 0);
            lrcheck2 = 0;
            m_Animator.animator.SetBool("End_Flg", false);
            m_Animator.animator.SetBool("Start_Flg", true);
        }
        else if (PE_Control.getAnim() == "end")
        {
            m_Animator.animator.SetBool("Start_Flg", false);
            m_Animator.animator.SetBool("End_Flg", true);
        }

        if (PE_Control.getAnim() == "damage1")
        {
            Debug.Log("d1-in");
            m_Animator.animator.SetBool("Damage1_Flg", false);
        }

        if (PE_Control.getAnim() == "damage2")
        {
            Debug.Log("d2-in");
            m_Animator.animator.SetInteger("Damage2-3_Seq", 2);
        }
    }

    //移動時の設定と計算
    void MoveProcess()
    {
        /*--------------------------------------
         物理挙動
         --------------------------------------*/
        // カメラの方向に合わせた正面を設定
        cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        // カメラの方向に合わせた移動方向を決定
        moveForward = vertical * cameraForward * PP_Control.speed + horizontal * Camera.main.transform.right * PP_Control.speed;

        // 移動速度強化
        if (transform.Find("aura(Clone)") != null)
        {
            moveForward *= 1.5f;
        }

        //ダッシュする速度。ダッシュなら移動速度に加算
        if (PE_Control.getAnim() == "dash")
        {
            dashForward = rb.transform.forward * 30;
        }
        else
        {
            dashForward = Vector3.zero;
        }

        //物理マテリアルの変更
        if ((checkIsGround(rb.position, 0.3f) == true))
        {
            //地面なら滑らない
            capsuleCollider.material = nonslip;
        }
        else
        {
            //空中なら滑る
            capsuleCollider.material = slip;
        }
    }

    //Rigidbodyの演算処理
    void PhysicalProcess()
    {
        //歩き、ジャンプ中で方向キー入力時、またはdashの時のみ移動
        if (((horizontal != 0.0) || (vertical != 0.0)) || (PE_Control.getAnim() == "dash"))
        {
            rb.velocity = new Vector3(moveForward.x + dashForward.x, rb.velocity.y, moveForward.z + dashForward.z);

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
        }

        //ジャンプ中落下速度を上げる
        if ((PE_Control.getAnim() == "jumploop_up") || (PE_Control.getAnim() == "jumploop_down"))
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - 0.5f, rb.velocity.z);
        }
    }

    //キャラクター共通アニメーション
    void CommonAnimation()
    {
        /*--------------------------------------
          アニメーション共通常時処理
          --------------------------------------*/
        //静止常時処理
        if (PE_Control.getAnim() == "stay")
        {
            //上下左右キー入力を受け付ける
            horizontal = (Input.GetAxis("Horizontal"));
            vertical = (Input.GetAxis("Vertical"));
            //入力があるならwalkにする
            if (((horizontal != 0.0f) || (vertical != 0.0f)))
            {
                m_Animator.animator.SetInteger("WalkTrigger", 1);
            }
            //ダブルタップならdashにする
            if (doubletapflg == true)
            {
                m_Animator.animator.SetBool("Dash_Flg", true);
            }
        }
        //歩行常時処理
        else if (PE_Control.getAnim() == "walk")
        {
            //上下左右キー入力を受け付ける
            horizontal = (Input.GetAxis("Horizontal"));
            vertical = (Input.GetAxis("Vertical"));
            //ダブルタップならdashにする
            if (doubletapflg == true)
            {
                m_Animator.animator.SetBool("Dash_Flg", true);
            }
            //入力が無ければstayにする
            if ((horizontal == 0.0f) && (vertical == 0.0f))
            {
                m_Animator.animator.SetInteger("WalkTrigger", 2);
            }
        }
        else if ((PE_Control.getAnim() == "jumploop_up") || (PE_Control.getAnim() == "jumploop_down"))
        {
            //上下左右キー入力を受け付ける
            horizontal = (Input.GetAxis("Horizontal"));
            vertical = (Input.GetAxis("Vertical"));
        }
        else
        {
            horizontal = 0.0f;
            vertical = 0.0f;
        }

        // ジャンプ常時処理
        if (m_Animator.animator.GetInteger("Jump_Seq") == 1)
        {
            //飛び上がり
            if (rb.velocity.y < 0)
            {
                m_Animator.animator.SetInteger("Jump_Seq", 2);
            }
        }
        else if (m_Animator.animator.GetInteger("Jump_Seq") == 2)
        {
            //着地
            if ((rb.velocity.y == 0) || (checkIsGround(rb.position, 0.3f) == true))
            {
                m_Animator.animator.SetInteger("Jump_Seq", 3);
            }
        }

        // 吹き飛び常時処理
        if (m_Animator.animator.GetInteger("Damage2-3_Seq") == 2)
        {
            //着地
            if ((rb.velocity.y == 0) || (checkIsGround(rb.position, 1.0f) == true))
            {
                m_Animator.animator.SetInteger("Damage2-3_Seq", 3);
            }
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

    //1か-1に変換したGetAxisの成分を取得
    int changeGetAxis(float h_or_v)
    {
        int changed_h_or_v;
        if (h_or_v > 0)
        {
            changed_h_or_v = 1;
        }
        else if (h_or_v < 0)
        {
            changed_h_or_v = -1;
        }
        else
        {
            changed_h_or_v = 0;
        }
        return changed_h_or_v;
    }

    //着地判定
    bool checkIsGround(Vector3 position, float distance)
    {
        //レイキャストを地面に放つ
        Ray ray = new Ray(position, Vector3.down);
        //Raycastの判定を返す
        return Physics.Raycast(ray, distance);
    }
}

