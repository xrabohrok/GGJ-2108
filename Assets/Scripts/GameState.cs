//GameState.cs
//Used to keep track of state of robot and hacking gameboard

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour {


    public static GameState instance = null;
    //refers to the Hacking Game Board
    public GameObject hackGame;
    //refers to the Robot Game Board
    public GameObject robotGame;
    public Boolean currentlyRobotGame;

   
    Animator anim;
    public float restartDelay = 5f;
    float restartTimer;
    float timeLeft = 10.0f;

    //Used for music change
    public AudioClip robotMusic;
    public AudioClip hackMusic;

    //Health before robot dies
    private int robotHealth = 5;
    //Player moves
    private int playerMoves = 5;

    //Decrement player movement
    public void PlayerMoved()
    {
        playerMoves--;

        if (playerMoves <= 0)
        {
            ChangeToHack();
        }
    }


    //Awake is always called before
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        this.currentlyRobotGame = false;
    }

	// Use this for initialization
	void Start ()
    {

        //TODO add in reference to robot on grid
        //hackGame = GameObject.Find("<HACKING BOARD>");
        //TODO add in reference to hacking minigame
        //robotGame = GameObject.Find("<ROBOT GAME>");

        //Text needs initialized before this will work and I don't know how to do that. -mw
        //controlSwitchText.text = "Controlling Robot";



    }

	
	// Update is called once per frame
	void Update ()
    {
		
	if (GameObject.Find("HackingGame").GetComponent<GridMaster>().FunctioningCircuit)
        {
           // playerMoves = GameObject.Find("HackingGame").GetComponent<GridMaster>().Misses + 5;
            ChangeToRobot();
        }


            if (robotHealth <= 0)
        {
            GameOver();
        }

	}

    void ChangeToRobot()
    {
        Cursor.visible = false;
        this.currentlyRobotGame = true;
        playerMoves = GameObject.Find("HackingGame").GetComponent<GridMaster>().Misses + 5;
        //TODO: set up activate && deactivate
        //hackController.SetActive(false);
        //robotController.SetActive(true);
        // hackGame.SetActive(true);

    }

    void ChangeToHack()
    {

        Cursor.visible = true;
        GameObject.Find("HackingGame").GetComponent<GridMaster>().resetBoard();
  
        //TODO: set up activate && deactivate
        //hackController.SetActive(true);
        //robotController.SetActive(false);

        //robotGame.SetActive(true);
        this.currentlyRobotGame = false;

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

}
