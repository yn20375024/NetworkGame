using System.Collections;
using UnityEngine;
using Mirror;

// 必要なコンポーネントを自動追加
//[RequireComponent(typeof(Animator))]
//[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CharacterController))]

public class CharaController : NetworkBehaviour
{
//    public float animSpeed = 1.0f;  // アニメーション再生速度設定

    private CharacterController characon;
    private Vector3 Player_pos;        // プレイヤーのポジション
    private Vector3 past_pos;          // 1つ前のポジション

    // パラメーター
//    public float hp = 5.0f;         // 体力
//    public float attack = 5.0f;     // 攻撃力(技の基礎威力に補正を掛ける値)
    public float speed = 10.0f;      // 移動速度
    public float jump = 10.0f;       // ジャンプ力(初期値)
    public float gravity = 15.0f;     // 重力

    // キャラクターコントローラ（カプセルコライダ）の参照
//    private CapsuleCollider col;
    private Rigidbody rb;

    // CapsuleColliderで設定されているコライダのHeiht、Centerの初期値を収める変数
//    private float orgColHight;
//    private Vector3 orgVectColCenter;

//    private Animator anim;						    	// キャラにアタッチされるアニメーターへの参照
//    private AnimatorStateInfo currentBaseState;			// base layerで使われる、アニメーターの現在の状態の参照

    void Start()
    {
        if (!isLocalPlayer) return;

        GameObject cManager = GameObject.Find("CameraManager");
        cManager.GetComponent<FollowAndLookAtTarget>().Follow = transform;

        characon = GetComponent<CharacterController>();

        // Animatorコンポーネントを取得する
//        anim = GetComponent<Animator>();

        // CapsuleColliderコンポーネントを取得する（カプセル型コリジョン）
//        col = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();

        // CapsuleColliderコンポーネントのHeight、Centerの初期値を保存する
//        orgColHight = col.height;
//        orgVectColCenter = col.center;

        Player_pos = GetComponent<Transform>().position; // 最初のポジションを取得
        past_pos = Player_pos; // 最初のポジションを取得
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        // 方向キー入力
        float h = (Input.GetAxis("Horizontal"));   // -1 ~ 1が格納される  マイナス → 左 / プラス → 右
        float v = (Input.GetAxis("Vertical"));     //                     マイナス → 下 / プラス → 上

        DoubleTap();                            // ダブルタップ

        // カメラの方向に合わせた正面を設定
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        // カメラの方向に合わせた移動方向を決定
        Vector3 moveForward = v * cameraForward * speed + h * Camera.main.transform.right * speed;

        // ダブルタップ中のダッシュ移動
        if (doubletapflg == true)
        {
            // カメラの方向に合わせた移動方向を決定
            moveForward = v * cameraForward * (speed * 10) + h * Camera.main.transform.right * (speed * 10);
        }

        // 重力計算
        if (!characon.isGrounded) Player_pos.y -= gravity * Time.deltaTime;
        // 移動
        characon.Move(new Vector3(moveForward.x * Time.deltaTime, Player_pos.y * Time.deltaTime, moveForward.z * Time.deltaTime));

        // 回転
        Vector3 diff = new Vector3(transform.position.x - past_pos.x, 0, transform.position.z - past_pos.z);    // 移動幅を求める
        if (diff.magnitude > 0.01f) transform.rotation = Quaternion.LookRotation(diff);                         // 移動していたら向きを変える
        past_pos = transform.position;                                                                          // 位置を更新

        // どのボタンが押されたかチェック
        KeyCheck();
   }

    private float dtap_NowTime = 0f;                //　最初に移動ボタンが押されてからの経過時間
    private Vector2 dtap_Direction = Vector2.zero;  //　移動キーの押した方向
    private int tapCount = 0;
    private bool doubletapflg = false;              // ダブルタップのフラグ   false->なし   true->あり

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
        if (Input.GetKeyDown("joystick button 1"))
        {
            Debug.Log("弱攻撃");
        }

        //【 △　強攻撃１ 】
        if (Input.GetKeyDown("joystick button 0"))
        {
            Debug.Log("強攻撃１");
        }

        //【 □　強攻撃２ 】
        if (Input.GetKeyDown("joystick button 3"))
        {
            Debug.Log("強攻撃２");
        }

        //【 ×　ジャンプ 】
        // スペースキーでジャンプ
        if ((Input.GetKeyDown(KeyCode.Space)) || (Input.GetKeyDown("joystick button 2")))
        {
            // 接地しているなら
            if (characon.isGrounded)
            {
                // ジャンプ力を設定
                Player_pos.y = jump;
            }
        }

        //【 L2+R2　必殺技 】
        if (Input.GetKeyDown("joystick button 4") && Input.GetKeyDown("joystick button 5"))
        {
            Debug.Log("必殺技");
        }
        //【 L2　ガード 】
        else if (Input.GetKeyDown("joystick button 4"))
        {
            Debug.Log("ガード");
        }
        //【 R2　投げ 】
        else if (Input.GetKeyDown("joystick button 5"))
        {
            Debug.Log("投げ");
        }

        //【 L1　ターゲット切り替え左 】
        if (Input.GetKeyDown("joystick button 6"))
        {
            Debug.Log("ターゲット切り替え左");
        }

        //【 R1　ターゲット切り替え右 】
        if (Input.GetKeyDown("joystick button 7"))
        {
            Debug.Log("ターゲット切り替え右");
        }
    }

}
