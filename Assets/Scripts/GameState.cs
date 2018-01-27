using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour {

   // enum Control {Robot, Hack};

    public static GameState instance = null;
    public GameObject hackGame;
    public GameObject robotGame;
    public Text controlSwitchText;
    Animator anim;
    public float restartDelay = 5f;
    float restartTimer;

    //Used for music change
    public AudioClip robotMusic;
    public AudioClip hackMusic;


    private int robotHealth = 5;
    private int playerMoves = 5;
    


    //Awake is always called before
    void Awake()
    {
        if (instance == null)
            instance = this;
    }

	// Use this for initialization
	void Start ()
    {
        //TODO add in reference to robot on grid
        //hackGame = GameObject.Find("<HACKING BOARD>");
        //TODO add in reference to hacking minigame
        //robotGame = GameObject.Find("<ROBOT GAME>");

        controlSwitchText.text = "Controlling Robot";
       


    }
	
	// Update is called once per frame
	void Update ()
    {
        if (robotHealth <= 0)
        {
            GameOver();
        }
	}

    void ChangeToRobot()
    {
        Cursor.visible = false;

        controlSwitchText.text = "Controlling Robot";
        //hackController.SetActive(false);
        //robotController.SetActive(true);
        // hackGame.SetActive(true);

    }

    void ChangeToHack()
    {

        Cursor.visible = true;

        controlSwitchText.text = "Hacking";
        //hackController.SetActive(true);
        //robotController.SetActive(false);

        
        //robotGame.SetActive(true);

    }

    void GameOver()
    {
        anim.SetTrigger("GameOver");
        restartTimer += Time.deltaTime;
        if (restartTimer >= restartDelay)
        {
            // .. then reload the currently loaded level.
            SceneManager.LoadScene(0);
        }

    }


}
