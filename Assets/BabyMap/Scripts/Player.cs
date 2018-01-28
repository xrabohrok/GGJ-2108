using UnityEngine;
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
        
        public AudioClip moveSound1;                //1 of 2 Audio clips to play when player moves.
        public AudioClip moveSound2;                //2 of 2 Audio clips to play when player moves.
        public AudioClip gameOverSound;             //Audio clip to play when player dies.

        private Animator animator;                  //Used to store a reference to the Player's animator component
        List<IntVector2> moveList;
        float timer;

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
            this.moveList = new List<IntVector2>();
            timer = Time.time;

            //Call the Start function of the MovingObject base class.
            base.Start();
        }


        //This function is called when the behaviour becomes disabled or inactive.
        private void OnDisable()
        {}


        private void Update()
        {
            //If it's not the player's turn, exit the function.
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
                    // Possibly loop and don't count the input if the player moved into a wall.
                }
            }

            else if(!GameState.instance.currentlyRobotGame && !this.busyHandlingInput)
            {
                // Move randomly (comment out if we're using djikstra's instead)
                Debug.Log(Time.time - this.timer);
                this.timer = Time.time;
                MoveRandomly();

                // Djikstras:
                /*
                if (this.moveList.Count == 0)
                {
                    IntVector2 exit = GameManager.instance.boardScript.exit;
                    IntVector2 exitPos = new IntVector2(Mathf.RoundToInt(exit.x), Mathf.RoundToInt(exit.y));
                    IntVector2 position = new IntVector2(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y));

                    Djikstras:
                    List<Vector3> djikstrasResult = GameManager.instance.boardScript.Djikstras(position, exitPos);
                    moveList = GameManager.instance.boardScript.ConvertPathToMoves(djikstrasResult);
                }
                else
                {
                    AttemptMove(moveList[0].x, moveList[0].y);
                    moveList.RemoveAt(0);
                }
                */
            }

        }

        //AttemptMove overrides the AttemptMove function in the base class MovingObject
        //AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
        protected override void AttemptMove(int xDir, int yDir)
        {
            //Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
            if(xDir == 1)
            {
                animator.SetTrigger("BBWalkSide");
            }
            else if (xDir == -1)
            {
                animator.SetTrigger("BBWalkSide");
            }
            else  if (yDir == -1)
            {
                animator.SetTrigger("BBWalkUp");
            }
            else if (yDir == 1)
            {
                animator.SetTrigger("BBWalkAway");
            }
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
    }
}

