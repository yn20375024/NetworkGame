using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraPosition : MonoBehaviour
{
    public GameObject Chara;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // カメラ位置 ( 自分の位置に設定中 )
        Vector3 CamPos = new Vector3(Chara.transform.position.x, Chara.transform.position.y, Chara.transform.position.z);
       transform.position = CamPos;

    }

}
