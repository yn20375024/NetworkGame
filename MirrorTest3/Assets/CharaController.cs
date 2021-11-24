using System.Collections;
using UnityEngine;
using Mirror;

// 必要なコンポーネントを自動追加
//[RequireComponent(typeof(Animator))]
//[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]

public class CharaController : NetworkBehaviour
{
//    public float animSpeed = 1.0f;  // アニメーション再生速度設定

    private Vector3 Player_pos;     // プレイヤーのポジション

    // パラメーター
//    public float hp = 5.0f;         // 体力
//    public float attack = 5.0f;     // 攻撃力(技の基礎威力に補正を掛ける値)
    public float speed = 5.0f;      // 移動速度
//    public float jump = 5.0f;       // ジャンプ力(初期値)
//    private float jumpPower;        // ジャンプ力(値変更用)
//    public float gravity = 0.2f;     // 重力

    // キャラクターコントローラ（カプセルコライダ）の参照
//    private CapsuleCollider col;
    private Rigidbody rb;

    // CapsuleColliderで設定されているコライダのHeiht、Centerの初期値を収める変数
//    private float orgColHight;
//    private Vector3 orgVectColCenter;

//    private Animator anim;						    	// キャラにアタッチされるアニメーターへの参照
//    private AnimatorStateInfo currentBaseState;			// base layerで使われる、アニメーターの現在の状態の参照

    // フラグ
//    private bool flg_jump;           // ジャンプフラグ(true - ジャンプ中、ジャンプ不可/false - ジャンプ可)

    // Start is called before the first frame update
    void Start()
    {
        // Animatorコンポーネントを取得する
//        anim = GetComponent<Animator>();

        // CapsuleColliderコンポーネントを取得する（カプセル型コリジョン）
//        col = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();

        // CapsuleColliderコンポーネントのHeight、Centerの初期値を保存する
//        orgColHight = col.height;
//        orgVectColCenter = col.center;

        Player_pos = GetComponent<Transform>().position; // 最初のポジションを取得

//        jumpPower = jump;       // ジャンプ力
//        flg_jump = false;       // ジャンプフラグ
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        float h = Input.GetAxis("Horizontal");   // x方向のInputの値を取得
        float v = Input.GetAxis("Vertical");     // z方向のInputの値を取得

        // 移動/回転
        rb.velocity = new Vector3(h * speed, 0, v * speed);         // 移動
        Vector3 diff = new Vector3(transform.position.x - Player_pos.x, 0, transform.position.z - Player_pos.z);             // 移動幅を求める
        if (diff.magnitude > 0.01f)                                 // 移動していたら向きを変える
        {
            transform.rotation = Quaternion.LookRotation(diff);
        }
        Player_pos = transform.position;                            // 位置を更新

        // どのボタンが押されたかチェック
//        KeyCheck();

        // アクション管理
//        ActJump();
    }

    // ボタンとアクションの対応
/*    void KeyCheck()
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
        if (Input.GetKeyDown("joystick button 2"))
        {
            Debug.Log("ジャンプ");
            if (flg_jump == false)
            {
                flg_jump = true;
                jumpPower = jump;
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

    // ジャンプ
    void ActJump()
    {
        if (flg_jump == true)
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
            jumpPower -= gravity;

            // 着地判定
            // レイキャストをキャラクターのセンターから落とす
            Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
            var distance = 1.0f;
            // 落下中にレイキャストが他オブジェクトと交わったら着地
            if (jumpPower <= 0 && Physics.Raycast(ray, distance) == true)
            {
                Debug.Log("着地した");
                flg_jump = false;
            }
        }
    }*/
}
