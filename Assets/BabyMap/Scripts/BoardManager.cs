using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

namespace BabyMap

{


    public enum TileType
    {
        Floor, Wall, Hazard, Goal
    }


    public class BoardManager : MonoBehaviour
    {


        public static BoardManager instance = null;


        public int columns;                                         //Number of columns in our game board.
        public int rows;                                            //Number of rows in our game board.
        public IntVector2 exit;

        private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.

        public TileType[,] fullMap;
        private Dictionary<IntVector2, Dictionary<IntVector2, int>> vertices = new Dictionary<IntVector2, Dictionary<IntVector2, int>>();

        private String[,] room;

        public void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
        }

        //Sets up the outer walls and floor (background) of the game board.
        public void BoardSetup()
        {
            //Instantiate Board and set boardHolder to its transform.
            boardHolder = new GameObject("Board").transform;

            GameObject instance = null;

            room = new String[columns, rows];

            int topOfRoom = rows - 1;
            int rightOfRoom = columns - 1;

            room[0, topOfRoom] = "robotRoom_topHorizontal_wall";
            fullMap[0, topOfRoom] = TileType.Wall;
            room[1, topOfRoom] = "robotRoom_topHorizontal_wall";
            fullMap[1 , topOfRoom] = TileType.Wall;
            room[2, topOfRoom] = "robotRoom_leftVertical_wall";
            fullMap[2, topOfRoom] = TileType.Wall;

            room[5, topOfRoom] = "robotRoom_rightBlind_wall";
            fullMap[5, topOfRoom] = TileType.Wall;
            for (int i = 6; i <columns; i++)
            {
                room[i, topOfRoom] = "robotRoom_topHorizontal_wall";
                fullMap[i,topOfRoom] = TileType.Wall;
            }

            room[2, 2] = "robotRoom_downCorner_wall";
            for (int i = 3; i < topOfRoom; i ++) {
                room[2, i] = "robotRoom_leftVertical_wall";
                fullMap[2, i] = TileType.Wall;
            }

            for (int i = 0; i < 5; i++)
            {
                room[5, i] = "robotRoom_leftVertical_wall";
                fullMap[5, i] = TileType.Wall;
            }
            room[5, 5] = "robotRoom_topBlind_wall";
            fullMap[5,5] = TileType.Wall;

            room[5, 2] = "robotRoom_leftDown_wall";
            fullMap[5, 2] = TileType.Wall;

            room[6, 2] = "robotRoom_topHorizontal_wall";
            fullMap[6, 2] = TileType.Wall;
            room[7, 2] = "robotRoom_leftCorner_wall";
            fullMap[7,2] = TileType.Wall;

            room[8, 5] = "robotRoom_rightBlind_wall";
            fullMap[8, 5] = TileType.Wall;
            for (int i = 9; i < columns; i++)
            {
                room[i, 5] = "robotRoom_topHorizontal_wall";
                fullMap[i , 5] = TileType.Wall;
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
        
        //SetupScene initializes our level and calls the previous functions to lay out the game board
        public void SetupScene()
        {
            this.InitializeFullMap();
            this.BoardSetup();
                        
            this.exit = new IntVector2(columns - 1, rows - 1);

            Create2DTileArray();
        }

        public void Create2DTileArray()
        {
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    this.CreateVertices(new IntVector2(i, j));
                }
            }
        }

        // Currently the fullMap is used for Djikstras and collisions, and room is used for displaying sprites.
        // Obviously this should change in the future. -mw
        public void InitializeFullMap()
        {
            this.fullMap = new TileType[this.columns, this.rows];
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    this.fullMap[i, j] = TileType.Floor;
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
                while (this.fullMap[nodes[0].X, nodes[0].Y] != TileType.Floor
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
            if (location.X > 0 && (fullMap[location.x - 1, location.y] == TileType.Floor))
                edges.Add(new IntVector2(location.X - 1, location.Y), 10);
            if ((location.x < this.columns - 1) && (fullMap[location.x + 1, location.y] == TileType.Floor))
                edges.Add(new IntVector2(location.X + 1, location.Y), 10);

            if (location.Y > 0 && (fullMap[location.x, location.y - 1] == TileType.Floor))
                edges.Add(new IntVector2(location.X, location.Y - 1), 10);
            if ((location.Y < this.rows - 1) && (fullMap[location.x, location.y + 1] == TileType.Floor))
                edges.Add(new IntVector2(location.X, location.Y + 1), 10);

            this.vertices[location] = edges;
        }
    }
}
