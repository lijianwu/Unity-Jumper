using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Jump : MonoBehaviour {

    //力度
    public float force = 200f; 
    //按压力
    public float pressForce;
    //cube预制体
    public GameObject cubePrefabs;
    public GameObject cylinderPrefab;
    //是否添加预制体
    private bool canAdd = false;
    //是否接触到地面
    public bool isPlane = false;
    //cube生成方向
    public int dir = 0;
    //cube物体
    public GameObject cube;
    //跳跃方向
    public Vector3 jumpPos;
    int nameCount = 0;
    //跳跃距离
    private Vector3 jumpDistance;
    
    //当前cube
    public Transform currentCube;
    //当前Cube的缩放
    private Vector3 currentScale;
    //分数
    private int score = 0;
    public Text scoreTxt;

    //颜色
    private Color color;

    //地面
    private GameObject plane;
    //开始游戏
    public bool startGame = false;


    //相机位置
    private Vector3 cameraPos;
    //相机移动位置
    public GameObject cameraPointLeft;
    public GameObject cameraPointRight;

    //JumpPanel
    private GameObject jumpPanel;

    //声音
    private AudioSource audio;
    //动画
    private Animation animation;
    //粒子效果
    public ParticleSystem particleSystem1;
    public ParticleSystem particleSystem2;

    void Start () {

        //初始化一些参数
        currentScale = new Vector3(1, 0.5f, 1);
        isPlane = true;
        pressForce = 0f;
        currentCube.name = "StartPoint";
        jumpDistance = Vector3.zero;
        cameraPos = Camera.main.transform.position;

        jumpPanel = GameObject.Find("JumpPanel");
        plane = GameObject.Find("Plane");

        audio = GetComponent<AudioSource>();
        animation = GetComponent<Animation>();
        
	}
	
	// Update is called once per frame
	void Update () {

        if(startGame)
            JumpToCube();

     
    }

    private void OnCollisionEnter(Collision collision)
    {
        //碰到地面
        if (collision.gameObject.name == "Plane")
        {
            transform.GetComponent<Rigidbody>().freezeRotation = false;
            isPlane = true;
            //结束游戏
            GameObject.Find("ApplicantManager").transform.GetComponent<ApplicatioManager>().Endjump();
        }
        //碰到cube
        if (collision.gameObject.name.StartsWith("Cube"))
        {
            //碰到cube但是掉下来了
            if (transform.position.y < 0.85f)
                return;
            isPlane = false;

            //如果跳跃的不是当前Cube
            if (currentCube.name != collision.transform.name)
            {    
                //不是第一个Cube
                if (collision.transform.name != "Cube")
                {
                    //分数加1
                    score++;
                    scoreTxt.text = "分数:" + " " + score.ToString();
                    //每跳跃五次更改一次背景颜色
                    if (score % 5 == 0)
                        StartCoroutine(ChangeBgColor());
                    //生成下一个Cube
                    CreateCube(collision.transform.position);                   
                }
                //移动相机
                if (dir == 0)
                {  
                    cameraPos = cameraPointLeft.transform.position;
                    cameraPos.y = Camera.main.transform.localPosition.y;    
                }
                if (dir == 1)
                {
                    cameraPos = cameraPointRight.transform.position;
                    cameraPos.y = Camera.main.transform.localPosition.y;
                }
                //移动相机
                StartCoroutine(MoveObject(Camera.main.transform.localPosition, cameraPos, 0.5f));
                //播放粒子效果
                if(startGame)
                    particleSystem1.Play();
            }           
            //获取当前Cube
            currentCube = collision.transform;
        }
        
    }

    //跳跃
    private void JumpToCube()
    { 
        if(isPlane == false)
        {
            //如果是Android平台
            AndroidJump();
            //鼠标按下，随机生成颜色
            if(Input.GetMouseButtonDown(0))
            {
                color = Random.ColorHSV();
                currentScale = currentCube.transform.localScale;
                //播放音效
                if (audio.isPlaying == false)
                {
                    audio.pitch = 5f;
                    audio.Play();
                }

            }
            //鼠标按住
            if (Input.GetMouseButton(0))
            {               
                canAdd = false;
                
                pressForce += 0.008f;
                if (pressForce >= 2)
                    pressForce = 2;

                //开启Cube动画
                AnimationStart();
                //播放声音
                if (audio.isPlaying == false)
                {
                    audio.pitch = 5f;
                    audio.Play();
                }

            }
            //鼠标松开
            if (Input.GetMouseButtonUp(0))
            {
                //播放声音
                if (audio.isPlaying)
                    audio.Stop();
                
                //计算下次跳跃的向量
                jumpPos = cube.transform.position - transform.position;
                //添加力
                transform.GetComponent<Rigidbody>().AddForce(Vector3.up * force * 2f * pressForce);
                transform.GetComponent<Rigidbody>().AddForce(jumpPos.normalized * force * pressForce);

                //根据力的大小播放动画
                if (pressForce >= 0.3f)
                {
                    animation.Play("Jump");
                }

                //播放粒子效果
                particleSystem1.Play();
                //particleSystem2.Play();

                //重置按压力
                pressForce = 0;
                //可以添加
                canAdd = true;

                //设置回cube的原来的缩放
                if (currentCube.tag == "Cube")
                    currentCube.transform.localScale = currentScale;
                else
                {
                    currentScale.y = 0.25f;
                    currentCube.transform.localScale = currentScale;
                }
                
                
            }
            
        }

        
    }

    //生成Cube
    private void CreateCube(Vector3 pos)
    {
        GameObject prefab;
        if(canAdd == true)
        {
            //随机形状的cube
            if (Random.Range(0, 3) == 2)
            {
                prefab = cylinderPrefab;
            }
            else
            {
                prefab = cubePrefabs;
            }
            //控制Cube生成的方向
            dir = Random.Range(0, 2);
            if(dir == 0)
                cube = Instantiate(prefab, pos + new Vector3(0,5,Random.Range(1.5f,3f)), Quaternion.identity);
            else
                cube = Instantiate(prefab, pos + new Vector3(-Random.Range(1.5f, 3f), 5, 0), Quaternion.identity);
            //给实例化的Cube添加名称
            nameCount++;
            cube.name = "Cube" + nameCount.ToString();
            //设置父物体
            cube.transform.parent = jumpPanel.transform;
        }
    }
    //动画和颜色效果
    private void AnimationStart()
    {
        currentCube.GetComponent<MeshRenderer>().material.color = Color.Lerp(currentCube.GetComponent<MeshRenderer>().material.color, color, Time.deltaTime);
        if (currentCube.tag == "Cube")
        {
            if (currentCube.localScale.y > 0.3f)
                currentCube.transform.localScale = Vector3.Lerp(currentCube.localScale, new Vector3(currentCube.transform.localScale.x, 0.002f, currentCube.transform.localScale.z), Time.deltaTime);
        }
        else
        {
            if (currentCube.localScale.y > 0.1f)
                currentCube.transform.localScale = Vector3.Lerp(currentCube.localScale, new Vector3(currentCube.transform.localScale.x, 0.002f, currentCube.transform.localScale.z), Time.deltaTime);
        }
    }
    //修改背景颜色
    IEnumerator ChangeBgColor()
    {
        float dur = 0.0f;
        float time = 5f;
        Color color = Random.ColorHSV();
        while (dur <= time)
        {
            dur += Time.deltaTime;
            plane.GetComponent<MeshRenderer>().material.color = Color.Lerp(plane.GetComponent<MeshRenderer>().material.color,color,dur/time);
                   
            yield return null;
        }
    }

    //在time时间内移动物体
    private IEnumerator MoveObject(Vector3 startPos, Vector3 endPos, float time)
    {
        var dur = 0.0f;
        while (dur <= time)
        {
            dur += Time.deltaTime;
            Camera.main.transform.localPosition = Vector3.Lerp(startPos, endPos, dur / time);
            yield return null;
        }
        StopAllCoroutines();
    }
    //场景循环
    void ChangeScene()
    {
        plane.transform.position = transform.position + new Vector3(-40, 0, 40);
    }
    //安卓点击
    void AndroidJump()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                color = Random.ColorHSV();
                currentScale = currentCube.transform.localScale;
                //播放音效
                if (audio.isPlaying == false)
                {
                    audio.pitch = 5f;
                    audio.Play();
                }
            }
            if (Input.touches[0].phase == TouchPhase.Stationary)
            {
                canAdd = false;

                pressForce += 0.008f;
                if (pressForce >= 2)
                    pressForce = 2;
                //开启动画
                AnimationStart();
                if (audio.isPlaying == false)
                {
                    audio.pitch = 5f;
                    audio.Play();
                }

            }
            if (Input.touches[0].phase == TouchPhase.Ended)
            {
                //计算下次跳跃的向量
                jumpPos = cube.transform.position - transform.position;
                //添加力
                transform.GetComponent<Rigidbody>().AddForce(Vector3.up * force * 2f * pressForce);
                transform.GetComponent<Rigidbody>().AddForce(jumpPos.normalized * force * pressForce);
                //重置按压力
                pressForce = 0;
                //可以添加
                canAdd = true;
                //设置回cube的原来的缩放
                if (currentCube.tag == "Cube")
                    currentCube.transform.localScale = currentScale;
                else
                {
                    currentScale.y = 0.25f;
                    currentCube.transform.localScale = currentScale;
                }
            }

        }
    }
}
 