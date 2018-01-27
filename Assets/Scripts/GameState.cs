using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GameState : MonoBehaviour {

    enum Control {Robot, Hack};

    public static GameState instance = null;
    public GameObject hackGame;
    public GameObject robotGame;


    //TODO hide mouse 
    //TODO add in reference to robot on grid
    //TODO add in reference to hacking minigame
    private int robotHealth;
    private int playerMoves;


    //Awake is always called before
    void Awake()
    {
        if (instance == null)
            instance = this;
    }

	// Use this for initialization
	void Start ()
    {
        //hackGame = GameObject.Find("<HACKING BOARD>");
        //robotGame = GameObject.Find("<ROBOT GAME>");
	}
	
	// Update is called once per frame
	void Update ()
    {

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

    void ChangeToRobot()
    {
        Cursor.visible = false;
       // hackGame.SetActive(true);

    }

    void ChangeToHack()
    {

        Cursor.visible = true;
        //robotGame.SetActive(true);

    }
}
