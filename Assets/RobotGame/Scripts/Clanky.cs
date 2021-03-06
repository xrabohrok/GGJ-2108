﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//Allows us to use UI.
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace RobotGame
{
    //Clanky inherits from RobotObject, our base class for robots.
    public class Clanky : RobotObject
    {
        public static Clanky instance = null;

        public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
        public AudioClip moveSound1;                //1 of 2 Audio clips to play when soundPlayer moves.
        public AudioClip moveSound2;                //2 of 2 Audio clips to play when soundPlayer moves.
        public AudioClip gameOverSound;             //Audio clip to play when soundPlayer dies.

        private Animator animator;                  //Used to store a reference to the Player's animator component
        private SpriteRenderer spriteRenderer;
        List<Vector2Int> moveList;

        //Is Clanky Powered up or down?
        private bool powered = false;

        //Time in seconds
        private int powerTimer = 2;

        public float currentSpeed = 5;

        private float normalSpeed = 5;
        private float reducedSpeed = 2.5f;
        private float increasedSpeed = 10;

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

            //Call the Start function of the MovingObject base class.
            base.Start();
        }


        //This function is called when the behaviour becomes disabled or inactive.
        private void OnDisable()
        { }


        private void Update()
        {
            if (!powered)
            {
                currentSpeed = normalSpeed;
            }

        }

        ////This will set clanky to idle instead of just triggering 
        //public void SetClankyIdle()
        //{
        //    animator.SetTrigger("");
        //   // animator.SetTrigger("BBIdle");
        //}

        ////This will trigger the idle animation which will revert to the "default" state
        //public void TriggerClankyIdle()
        //{
        //    animator.SetTrigger("BBIdle");
        //}

        //This will set clanky to positive instead of just triggering 
        public void SetClankyPositive()
        {

            // animator.SetTrigger("BBHurt");
            animator.SetTrigger("ClankyPositive");


        }

        //This will trigger the positive animation which will revert to the "default" state
        public void TriggerClankyPositive()
        {

            animator.SetTrigger("ClankyPositive");
            currentSpeed = increasedSpeed;
            powered = true;
            StartCoroutine(PoweredRoutine());

        }

        //This will set clanky to hurt instead of just triggering 
        public void SetClankyNegative()
        {

            // animator.SetTrigger("BBHurt");

        }

        //This will trigger the hurt animation which will revert to the "default" state
        public void TriggerClankyNegative()
        {
            animator.SetTrigger("ClankyNegative");
            currentSpeed = reducedSpeed;
            powered = true;
            StartCoroutine(PoweredRoutine());
        }

        public void SetNormalSpeed(float newSpeed)
        {
            normalSpeed = newSpeed;
        }

        public void SetIncreasedSpeed(float newSpeed)
        {
            increasedSpeed = newSpeed;
        }

        public void SetReducedSpeed(float newSpeed)
        {
            reducedSpeed = newSpeed;
        }

        public void SetPowerTimer(int newTimer)
        {
            powerTimer = newTimer;
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
            else if (other.tag == "NegativeEvent")
            {
                TriggerClankyNegative();

                // other.gameObject.SetActive(false);
            }
            else if (other.tag == "PositiveEvent")
            {
                TriggerClankyPositive();

                // other.gameObject.SetActive(false);
            }

        }

        IEnumerator PoweredRoutine()
        {
            yield return new WaitForSeconds(powerTimer);
            powered = false;
        }

    }
}

