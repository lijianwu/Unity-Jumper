using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour {

    // Use this for initialization
    /// <summary>
    /// 生成方块的颜色大小初始化设置
    /// </summary>

    //随机大小
    private float randSize;
    void Start () {
        
        //随机缩放大小
        randSize = Random.Range(1.0f, 1.5f);
        //随机颜色
        transform.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
        //随机缩放
        transform.localScale = new Vector3(randSize, transform.localScale.y, randSize);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Plane")
        {
            transform.GetComponent<Rigidbody>().isKinematic = true;
        }
       
    }
    //销毁自己
    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Destroy(gameObject, 20f);
        }
    }

}
