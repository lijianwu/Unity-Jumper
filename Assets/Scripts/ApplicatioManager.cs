using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicatioManager : MonoBehaviour {

    // Use this for initialization
    //Jumper
    private GameObject jumper;
    private Jump jump;
    public GameObject startPanel;
    public GameObject endPanel;


	void Start () {

        jumper = GameObject.FindGameObjectWithTag("Player");
        jump = jumper.GetComponent<Jump>();
        startPanel = GameObject.Find("Canvas/StartPanel");
        //endPanel = GameObject.Find("Canvas/EndPanel");

        startPanel.SetActive(true);
        if(endPanel.activeSelf == true)
            endPanel.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {

		if(jump.isPlane)
        {
            //游戏结束
            jump.startGame = false;
        }
	}
    //游戏开始
    public void StartJump()
    {
        if (startPanel != null && startPanel.activeSelf == true)
            startPanel.SetActive(false);
        jump.startGame = true;

    }
    //游戏结束
    public void Endjump()
    {
        if(endPanel.activeSelf == false)
        {
            endPanel.SetActive(true);
        }
    }
    //重玩游戏
    public void RestartJump()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        #endif
    }
    //退出游戏
    public void QuitJump()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();  
        #endif
    }
}
