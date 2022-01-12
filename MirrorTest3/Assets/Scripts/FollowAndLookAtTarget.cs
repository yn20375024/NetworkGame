using System;
using UnityEngine;

public class FollowAndLookAtTarget : MonoBehaviour
{
    public Transform Follow;        // 追従対象
    public Transform LookAt;        // ロックオン対象

    private const float ANGLE_LIMIT_UP = 60f;       // カメラの傾き制限 - 上
    private const float ANGLE_LIMIT_DOWN = -30f;    // カメラの傾き制限 - 下

    void Start()
    {
    }

    private void LateUpdate()
    {
        if (Follow != null)
        {
            // 追従
            transform.position = Follow.position;

            if (LookAt != null)
            {
                // ロックオン
                transform.LookAt(LookAt, Vector3.up);

                // 傾き制限
                float angle_x = 180f <= transform.eulerAngles.x ? transform.eulerAngles.x - 360 : transform.eulerAngles.x;
                transform.eulerAngles = new Vector3(
                    Mathf.Clamp(angle_x, ANGLE_LIMIT_DOWN, ANGLE_LIMIT_UP),
                    transform.eulerAngles.y,
                    transform.eulerAngles.z
                );
            }
        }
    }
}

