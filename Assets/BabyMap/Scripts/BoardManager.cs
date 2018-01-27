using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

namespace BabyMap

{

    public class BoardManager : MonoBehaviour
    {
        // Using Serializable allows us to embed a class with sub properties in the inspector.
        [Serializable]
        public class Count
        {
            public int minimum;             //Minimum value for our Count class.
            public int maximum;             //Maximum value for our Count class.


            //Assignment constructor.
            public Count(int min, int max)
            {
                minimum = min;
                maximum = max;
            }
        }


        public int columns;                                         //Number of columns in our game board.
        public int rows;                                            //Number of rows in our game board.
        public Count wallCount = new Count(5, 9);                       //Lower and upper limit for our random number of walls per level.
        public Count foodCount = new Count(1, 5);                       //Lower and upper limit for our random number of food items per level.
        public IntVector2 exit;
        //public GameObject[] floorTiles;                                 //Array of floor prefabs.

        private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
        private List<Vector3> gridPositions = new List<Vector3>();	//A list of possible locations to place tiles.

        public TileType[,] fullMap;
        private Dictionary<IntVector2, Dictionary<IntVector2, int>> vertices = new Dictionary<IntVector2, Dictionary<IntVector2, int>>();

        private String[,] room; 


        //Clears our list gridPositions and prepares it to generate a new board.
        void InitialiseList()
        {
            //Clear our list gridPositions.
            gridPositions.Clear();

            //Loop through x axis (columns).
            for (int x = 1; x < columns - 1; x++)
            {
                //Within each column, loop through y axis (rows).
                for (int y = 1; y < rows - 1; y++)
                {
                    //At each index add a new Vector3 to our list with the x and y coordinates of that position.
                    gridPositions.Add(new Vector3(x, y, 0f));
                }
            }
        }


        //Sets up the outer walls and floor (background) of the game board.
        void BoardSetup()
        {
            //Instantiate Board and set boardHolder to its transform.
            boardHolder = new GameObject("Board").transform;

            GameObject instance;

            room = new String[columns, rows];

            int topOfRoom = rows - 1;
            int rightOfRoom = columns - 1;

            room[0, topOfRoom] = "robotRoom_topHorizontal_wall";
            room[1, topOfRoom] = "robotRoom_topHorizontal_wall";
            room[2, topOfRoom] = "robotRoom_rightDown_wall";

            for(int i = 5; i <columns; i++)
            {
                room[i, topOfRoom] = "robotRoom_topHorizontal_wall";
            }

            for (int i = 2; i < topOfRoom; i ++) {
                room[2, i] = "robotRoom_rightVertical_wall";
            }

            for (int i = 0; i < 6; i++)
            {
                room[5, i] = "robotRoom_leftVertical_wall";
            }

            room[5, 2] = "robotRoom_leftDown_wall";

            room[6, 2] = "robotRoom_topHorizontal_wall";
            room[7, 2] = "robotRoom_topHorizontal_wall";

            for(int i = 8; i < columns; i++)
            {
                room[i, 5] = "robotRoom_bottomHorizontal_wall";
            }

          

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    instance = Instantiate(Resources.Load("robotRoom_floor"), new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(boardHolder);


                    if (room[x,y] != null)
                    {
                        instance = Instantiate(Resources.Load(room[x, y]), new Vector3(x, y, 0f), Quaternion.Euler(0,0,0)) as GameObject;
                        instance.transform.SetParent(boardHolder);
                    }
                }

            }
        }


        


        //RandomPosition returns a random position from our list gridPositions.
        Vector3 RandomPosition()
        {
            //Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
            int randomIndex = Random.Range(0, gridPositions.Count);

            //Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
            Vector3 randomPosition = gridPositions[randomIndex];

            //Remove the entry at randomIndex from the list so that it can't be re-used.
            gridPositions.RemoveAt(randomIndex);

            //Return the randomly selected Vector3 position.
            return randomPosition;
        }


        //LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
        void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
        {
            //Choose a random number of objects to instantiate within the minimum and maximum limits
            int objectCount = Random.Range(minimum, maximum + 1);

            //Instantiate objects until the randomly chosen limit objectCount is reached
            for (int i = 0; i < objectCount; i++)
            {
                //Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
                Vector3 randomPosition = RandomPosition();

                //Choose a random tile from tileArray and assign it to tileChoice
                GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

                //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
                Instantiate(tileChoice, randomPosition, Quaternion.identity);
            }
        }


        //SetupScene initializes our level and calls the previous functions to lay out the game board
        public void SetupScene()
        {
            //Creates the outer walls and floor.
            BoardSetup();

            //Reset our list of gridpositions.
            InitialiseList();
            
            this.exit = new IntVector2(columns - 1, rows - 1);

            Create2DTileArray();
        }

        public enum TileType
        {
            floor, wall, hazard, goalPoint
        }

        public void Create2DTileArray()
        {
            this.fullMap = new TileType[this.rows, this.columns];

            // When Rich is done with creating the map manually, pull the info from it to here.
            //foreach (GameObject tile in this.floorTiles)
            //    this.fullMap[(int)tile.transform.position.x, (int)tile.transform.position.y] = TileType.floor;

            //foreach (GameObject tile in this.wallTiles)
            //    this.fullMap[(int)tile.transform.position.x, (int)tile.transform.position.y] = TileType.wall;

            //foreach (GameObject tile in this.enemyTiles)
            //    this.fullMap[(int)tile.transform.position.x, (int)tile.transform.position.y] = TileType.hazard;

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    this.CreateVertices(new IntVector2(i, j));
                }
            }
        }

        public List<Vector3> Djikstras(IntVector2 startPos, IntVector2 endPos)
        {
            Dictionary<IntVector2, IntVector2> previous = new Dictionary<IntVector2, IntVector2>();
            Dictionary<IntVector2, int> distances = new Dictionary<IntVector2, int>();
            List<IntVector2> nodes = new List<IntVector2>();
            List<Vector3> path = new List<Vector3>();

            foreach (KeyValuePair<IntVector2, Dictionary<IntVector2, int>> vertex in this.vertices)
            {
                if (vertex.Key == startPos)
                { distances[vertex.Key] = 0; } // Distance from the origin to itself is zero.
                else
                { distances[vertex.Key] = int.MaxValue; }
                nodes.Add(vertex.Key);
            }

            while (nodes.Count > 0)
            {
                nodes.Sort((x, y) => distances[x] - distances[y]);
                while (this.fullMap[nodes[0].X, nodes[0].Y] != TileType.floor
                    && nodes.Count > 0)
                {
                    nodes.RemoveAt(0);
                }

                IntVector2 smallest = nodes[0];
                Debug.Log("Adding: " + smallest);
                nodes.Remove(smallest);

                if (smallest == endPos) // Then we're done and have a completed path.
                {
                    while (previous.ContainsKey(smallest))
                    {
                        path.Add(new Vector3(smallest.X, smallest.Y, 0));
                        smallest = previous[smallest];
                    }
                    break;
                }

                if (distances[smallest] == int.MaxValue)
                {
                    break;
                }

                foreach (KeyValuePair<IntVector2, int> neighbor in vertices[smallest])
                {
                    int alt = distances[smallest] + neighbor.Value;
                    if (alt < distances[neighbor.Key])
                    {
                        distances[neighbor.Key] = alt;
                        previous[neighbor.Key] = smallest;
                    }
                }
            }

            path.Reverse();
            return path;
        }

        public List<IntVector2> ConvertPathToMoves(List<Vector3> path)
        {
            List<IntVector2> returner = new List<IntVector2>();
            for (int i = 0; i < path.Count - 1; i++)
            {
                returner.Add(new IntVector2(Mathf.RoundToInt(path[i + 1].x - path[i].x), Mathf.RoundToInt(path[i + 1].y - path[i].y)));
            }

            return null;
        }

        private void CreateVertices(IntVector2 location)
        {
            //Location, cost of edge.
            Dictionary<IntVector2, int> edges = new Dictionary<IntVector2, int>();
            if (location.X > 0 && fullMap[location.x - 1, location.y] == TileType.floor)
                edges.Add(new IntVector2(location.X - 1, location.Y), 10);
            if (location.X < this.columns - 1 && fullMap[location.x + 1, location.y] == TileType.floor)
                edges.Add(new IntVector2(location.X + 1, location.Y), 10);

            if (location.Y > 0 && fullMap[location.x, location.y - 1] == TileType.floor)
                edges.Add(new IntVector2(location.X, location.Y - 1), 10);
            if (location.Y < this.rows - 1 && fullMap[location.x, location.y + 1] == TileType.floor)
                edges.Add(new IntVector2(location.X, location.Y + 1), 10);

            this.vertices[location] = edges;
        }
    }
}
