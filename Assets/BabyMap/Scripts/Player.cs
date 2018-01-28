﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//Allows us to use UI.
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace BabyMap
{
    //Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
    public class Player : MovingObject
    {
        public static Player instance;

        public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
        public AudioClip moveSound1;                //1 of 2 Audio clips to play when soundPlayer moves.
        public AudioClip moveSound2;                //2 of 2 Audio clips to play when soundPlayer moves.
        public AudioClip gameOverSound;             //Audio clip to play when soundPlayer dies.

        private Animator animator;                  //Used to store a reference to the Player's animator component
        private SpriteRenderer spriteRenderer;
        List<IntVector2> moveList;

        public void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
            
        }

        //Start overrides the Start function of MovingObject
        protected override void Start()
        {
            //Get a component reference to the Player's animator component
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            this.moveList = new List<IntVector2>();
            
            //Call the Start function of the MovingObject base class.
            base.Start();
        }


        //This function is called when the behaviour becomes disabled or inactive.
        private void OnDisable()
        {}


        private void Update()
        {
            //If it's not the soundPlayer's turn, exit the function.
            if (GameState.instance.currentlyRobotGame && !this.busyHandlingInput)
            {
                int horizontal = 0;
                int vertical = 0;

                //Get input from the input manager, round it to an integer.
                horizontal = (int)(Input.GetAxisRaw("Horizontal"));
                vertical = (int)(Input.GetAxisRaw("Vertical"));

                if (horizontal != 0)        //Can't move diagonally
                    vertical = 0;

                if (horizontal != 0 || vertical != 0)
                {
                    AttemptMove(horizontal, vertical);
                    // Possibly loop and don't count the input if the soundPlayer moved into a wall.
                }
            }

            else if(!GameState.instance.currentlyRobotGame && !this.busyHandlingInput)
            {
                // Djikstras:
                //if (this.moveList.Count == 0)
                //{
                //    IntVector2 exit = GameManager.instance.boardScript.exit;
                //    IntVector2 exitPos = new IntVector2(Mathf.RoundToInt(exit.x), Mathf.RoundToInt(exit.y));
                //    IntVector2 position = new IntVector2(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y));

                //    List<Vector3> djikstrasResult = GameManager.instance.boardScript.Djikstras(position, exitPos);
                //    moveList = GameManager.instance.boardScript.ConvertPathToMoves(djikstrasResult);

                //}
                //else
                //{
                //    AttemptMove(moveList[0].x, moveList[0].y);
                //    moveList.RemoveAt(0);
                //}

                //Move Randomly:
                IntVector2 direction;
                TileType nextTile = TileType.Wall;

                while (nextTile == TileType.Wall)
                {
                    direction = new IntVector2(Random.Range(-1, 2), Random.Range(-1, 2));
                    // Don't move diagonally.
                    if (direction.x != 0)
                        direction.y = 0;

                    IntVector2 end = this.position + direction;
                    if (end.x >= 0 && end.x < board.columns && end.y >= 0 && end.y < board.rows)
                    {
                        //Set canMove to true if Move was successful, false if failed.

                        TriggerClankyWalkAnimation(direction);
                        nextTile = Move(direction);
                    }
                }

                if (nextTile != TileType.Floor)
                    OnCantMove(nextTile);
            }

        }

        private void TriggerClankyWalkAnimation(IntVector2 direction)
        {
            TriggerClankyWalkAnimation(direction.X, direction.Y);

        }

        private void TriggerClankyWalkAnimation(int x, int y)
        {
            if (x == 1)
            {
                spriteRenderer.flipX = true;
                animator.SetTrigger("BBWalkSide");
            }
            else if (x == -1)
            {
                spriteRenderer.flipX = false;
                animator.SetTrigger("BBWalkSide");

            }
            else if (y == -1)
            {
                animator.SetTrigger("BBWalkUp");
            }
            else if (y == 1)
            {
                animator.SetTrigger("BBWalkAway");
            }

        }

        public void TriggerClankyHurtAnimation()
        {

            animator.SetTrigger("BBHurt");

        }


        //AttemptMove overrides the AttemptMove function in the base class MovingObject
        //AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
        protected override void AttemptMove(int xDir, int yDir)
        {
            //Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
            TriggerClankyWalkAnimation(xDir, yDir);
            base.AttemptMove(xDir, yDir);
        }


        //OnCantMove overrides the abstract function OnCantMove in MovingObject.
        protected override void OnCantMove(TileType tile)
        {
            if (tile == TileType.Goal)
            {
                // good shit
            }
            else if (tile == TileType.Hazard)
            {
                // bad shit
                GameManager.instance.GameOver();
            }
        }


        //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
        private void OnTriggerEnter2D(Collider2D other)
        {
            //Check if the tag of the trigger collided with is Exit.
            if (other.tag == "Exit")
            {
                //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
                Invoke("Restart", restartLevelDelay);

                //Disable the soundPlayer object since level is over.
                enabled = false;
            }
            else if (other.tag == "Hazard")
            {
                TriggerClankyHurtAnimation();
                GameState.instance.PlayerHurt(GameState.instance.hazardDmg);
                other.gameObject.SetActive(false);
            }

        }


        //Restart reloads the scene when called.
        private void Restart()
        {
            //Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replace the existing one
            //and not load all the scene object in the current scene.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }

    }
}

