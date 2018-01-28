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
    private int playerMoves = 10;

    public int hazardDmg = 1;

    Text clankyHP;
    Text movesLeft;

    SpriteRenderer robotDim;
    SpriteRenderer hackerDim;



    //Awake is always called before
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        this.currentlyRobotGame = true;
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


        robotDim = GameObject.Find("gradientRobotRoom").GetComponent<SpriteRenderer>(); 
        hackerDim = GameObject.Find("gradientHackerRoom").GetComponent<SpriteRenderer>();

         clankyHP = GameObject.Find("BBIntegrityValueText").GetComponent<Text>();
         movesLeft = GameObject.Find("MovesLeftValueText").GetComponent<Text>();
        robotDim.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Default";

    }

    public void PlayerMoved()
    {
        playerMoves--;
        //if (playerMoves <= 0 && currentlyRobotGame)
        //{
         //   ChangeToHack();
        //}


    }
	// Update is called once per frame
	void Update ()
    {
        movesLeft.text = playerMoves.ToString();
        clankyHP.text = robotHealth.ToString();

    if (GameObject.Find("HackingGame").GetComponent<GridMaster>().FunctioningCircuit && !currentlyRobotGame)
        {
            ChangeToRobot();
        }

        if (robotHealth <= 0)
        {
            GameOver();
        }

        if (playerMoves <= 0 && currentlyRobotGame)
        {
            ChangeToHack();
        }

    }

    void ChangeToRobot()
    {
        this.currentlyRobotGame = true;
        robotDim.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Default";
        hackerDim.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Player";

        Cursor.visible = false;

        
         playerMoves = 10 - Mathf.Min(GameObject.Find("HackingGame").GetComponent<GridMaster>().Misses,5);
        //TODO: set up activate && deactivate
        //hackController.SetActive(false);
        //robotController.SetActive(true);
        // hackGame.SetActive(true);

    }

    void ChangeToHack()
    {

        this.currentlyRobotGame = false;
        hackerDim.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Default";
        robotDim.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Player";
        Cursor.visible = true;
        GameObject.Find("HackingGame").GetComponent<GridMaster>().resetBoard();
  
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

    public void PlayerHurt(int dmg)
    {
        robotHealth -= dmg;
    }

}
