using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

namespace BabyMap
{
    //The abstract keyword enables you to create classes and class members that are incomplete and must be implemented in a derived class.
    public abstract class MovingObject : MonoBehaviour
    {
        public BoardManager board;
        public IntVector2 position;

        private Rigidbody2D rb2D;               //The Rigidbody2D component attached to this object.
        private float inverseMoveTime = 2f;
        public bool busyHandlingInput = false;

        protected void Awake()
        {
            
        }

        //Protected, virtual functions can be overridden by inheriting classes.
        protected virtual void Start()
        {
            board = BoardManager.instance;
            this.position = board.start;
            rb2D = gameObject.GetComponent<Rigidbody2D>();
            this.rb2D.position = new Vector2(this.position.x, this.position.y);
        }


        //Move returns true if it is able to move and false if not. 
        //Move takes parameters for x direction, y direction and a RaycastHit2D to check collision.
        protected TileType Move(IntVector2 direction)
        {
            // Calculate end position based on the direction parameters passed in when calling Move.
            IntVector2 end = this.position + direction;

            if(board.fullMap[end.x, end.y] == TileType.Floor)
            {
                this.position = end;
                StartCoroutine(SmoothMovement(new Vector3(end.x, end.y, 0f)));
                if (GameObject.Find("GameManager").GetComponent<GameState>().currentlyRobotGame)
                {
                    GameObject.Find("GameManager").GetComponent<GameState>().PlayerMoved();
                }
            }

            return board.fullMap[end.x, end.y];
        }

        //Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
        protected IEnumerator SmoothMovement(Vector3 end)
        {
            this.busyHandlingInput = true;
            //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
            //Square magnitude is used instead of magnitude because it's computationally cheaper.
            float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //While that distance is greater than a very small amount (Epsilon, almost zero):
            while (sqrRemainingDistance > float.Epsilon)
            {
                //Find a new position proportionally closer to the end, based on the moveTime
                Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

                //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
                rb2D.MovePosition(newPostion);

                //Recalculate the remaining distance after moving.
                sqrRemainingDistance = (transform.position - end).sqrMagnitude;

                //Return and loop until sqrRemainingDistance is close enough to zero to end the function
                yield return null;
            }
            this.busyHandlingInput = false;
        }


        //The virtual keyword means AttemptMove can be overridden by inheriting classes using the override keyword.
        //AttemptMove takes a generic parameter T to specify the type of component we expect our unit to interact with if blocked (Player for Enemies, Wall for Player).
        protected virtual void AttemptMove(int xDir, int yDir)
        {
            IntVector2 direction = new IntVector2(xDir, yDir);
            IntVector2 end = this.position + direction;
            TileType nextTile;

            // Don't move diagonally.
            if (direction.x != 0)
                direction.y = 0;

            // Ignore any moves that push us off the map.
            if (end.x >= 0 && end.x < board.columns && end.y >= 0 && end.y < board.rows)
            {
                nextTile = Move(direction);

                // Handle if we walked into a hazard or goal.
                if (nextTile != TileType.Floor)
                    OnCantMove(nextTile);
            }
        }


        //The abstract modifier indicates that the thing being modified has a missing or incomplete implementation.
        //OnCantMove will be overriden by functions in the inheriting classes.
        protected abstract void OnCantMove(TileType type);
    }
}
