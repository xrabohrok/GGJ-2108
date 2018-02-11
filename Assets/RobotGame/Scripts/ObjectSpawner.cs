using System.Collections;
using System.Collections.Generic;
using RobotGame;
using UnityEngine;

namespace RobotGame
{
    // Used by ObjectSpawner to track when obstacles will show.
    public class IncomingObject
    {
        public GameObject objectScroller;
        public float timeUntilSpawn; // In seconds.

        public IncomingObject()
        {
            timeUntilSpawn = 0f;
        }
    }


    public class ObjectSpawner : MonoBehaviour
    { 

        public static ObjectSpawner instance;
        public List<IncomingObject> randomObjects;
        public const float minTimeToNextSpawn = 2f;
        public const float maxTimeToNextSpawn = 4f;
        public List<GameObject> allPrefabs;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }

            this.allPrefabs.Add(GameObject.Find("Wires"));
            this.allPrefabs.Add(GameObject.Find("TeddyBear"));
        }

        void Update()
        {
            // Reduce time until the next one spawns
            foreach (IncomingObject obstacle in this.randomObjects)
            {
                obstacle.timeUntilSpawn -= Time.deltaTime;
            }

            // If there's an object to spawn, do it and remove it from the list.
            if (this.randomObjects.Count > 0
                && this.randomObjects[0].timeUntilSpawn < 0)
            {
                GameObject newObj = GameObject.Instantiate(this.randomObjects[0].objectScroller);
                newObj.transform.position = new Vector3(0, newObj.GetComponent<ObjectScroller>().startAtY, 0);
                this.randomObjects.RemoveAt(0);
            }

            // Decide randomly which prefab to use.
            nextObj.objectScroller = this.allPrefabs[Random.Range(0, this.allPrefabs.Count)];

            this.randomObjects.Add(nextObj);
            Debug.Log("Object added: " + nextObj.timeUntilSpawn + ", " + nextObj.objectScroller.name);
        }
    }
}
