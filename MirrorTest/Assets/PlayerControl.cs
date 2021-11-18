using UnityEngine;
using Mirror;

public class PlayerControl : NetworkBehaviour
{
    // 定期更新時に呼ばれる
    void FixedUpdate()
    {
        // ローカルプレイヤーの時
        if (isLocalPlayer) {
            // 操作
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            CmdMoveSphere(x, z);
        }
    }

    // 球の移動
    [Command]
    void CmdMoveSphere(float x, float z) 
    {
        Vector3 v = new Vector3(x, 0, z) * 5f;
        GetComponent<Rigidbody>().AddForce(v);
    }
}
