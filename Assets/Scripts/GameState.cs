<<<<<<< HEAD
﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GameState : MonoBehaviour {

    enum Control {Robot, Hack};

    public static GameState instance = null;

    //TODO hide mouse 
    //TODO add in reference to robot on grid
    //TODO add in reference to hacking minigame
    private int robotHealth;
    private int playerMoves;
    private int inControl;
=======
﻿//GameState.cs
//Used to keep track of state of robot and hacking gameboard

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour {

   // enum Control {Robot, Hack};

    public static GameState instance = null;
    //refers to the Hacking Game Board
    public GameObject hackGame;
    //refers to the Robot Game Board
    public GameObject robotGame;
    public Text controlSwitchText;
    Animator anim;
    public float restartDelay = 5f;
    float restartTimer;

    //Used for music change
    public AudioClip robotMusic;
    public AudioClip hackMusic;

    //Health before robot dies
    private int robotHealth = 5;
    //Player moves
    private int playerMoves = 5;
    
>>>>>>> 87c0837db0f592241eba50b95f2cdc35a7982d81


    //Awake is always called before
    void Awake()
    {
        if (instance == null)
            instance = this;
    }

	// Use this for initialization
	void Start ()
    {
<<<<<<< HEAD
		
	}
=======
        //TODO add in reference to robot on grid
        //hackGame = GameObject.Find("<HACKING BOARD>");
        //TODO add in reference to hacking minigame
        //robotGame = GameObject.Find("<ROBOT GAME>");

        controlSwitchText.text = "Controlling Robot";
       


    }
>>>>>>> 87c0837db0f592241eba50b95f2cdc35a7982d81
	
	// Update is called once per frame
	void Update ()
    {
<<<<<<< HEAD
		
	}

    Control ChangeControl (Control con)
    {
        if (con == Control.Hack)
        {
            con = Control.Robot;
        }
        else if (con == Control.Robot)
        {
            con = Control.Hack;
        }

        return con;
    }
=======
        if (robotHealth <= 0)
        {
            GameOver();
        }

        if (playerMoves <= 0)
        {
            ChangeToHack();
        }
	}

    void ChangeToRobot()
    {
        Cursor.visible = false;

        controlSwitchText.text = "Controlling Robot";
        //TODO: set up activate && deactivate
        //hackController.SetActive(false);
        //robotController.SetActive(true);
        // hackGame.SetActive(true);

    }

    void ChangeToHack()
    {

        Cursor.visible = true;

        controlSwitchText.text = "Hacking";
        //TODO: set up activate && deactivate
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
            SceneManager.LoadScene("Game");
        }

    }


>>>>>>> 87c0837db0f592241eba50b95f2cdc35a7982d81
}
