using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

namespace RobotGame
{
    //The abstract keyword enables you to create classes and class members that are incomplete and must be implemented in a derived class.
    public abstract class RobotObject : MonoBehaviour
    {
        
        private Rigidbody2D rb2D;               //The Rigidbody2D component attached to this object.
        private float inverseMoveTime = 3.3f;
        public bool busyHandlingInput = false;

        protected void Awake()
        {

        }

        //Protected, virtual functions can be overridden by inheriting classes.
        protected virtual void Start()
        {
            
            rb2D = gameObject.GetComponent<Rigidbody2D>();
           
        }

    }
}
