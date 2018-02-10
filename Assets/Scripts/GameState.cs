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
       
    Animator anim;
    public float restartDelay = 5f;
    float restartTimer;
    float timeLeft = 10.0f;

    //Player moves
    private int playerMoves = 0;


    delegate void HackingConcludedFn();
    HackingConcludedFn hackingConcluded;

    //Awake is always called before
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

	// Use this for initialization
	void Start ()
    {
        this.hackingConcluded = HackingConcluded;
        // If the hacking game exists, pass this.hackingConcluded to it.
    }

	// Update is called once per frame
	void Update ()
    {
        //movesLeft.text = playerMoves.ToString();
        //clankyHP.text = robotHealth.ToString();
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

    }

    public void HackingConcluded()
    {

    }

}
