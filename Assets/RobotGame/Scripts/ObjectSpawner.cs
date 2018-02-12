using System.Collections;
using System.Collections.Generic;
using RobotGame;
using UnityEngine;


// Used by ObjectSpawner to track when obstacles will show.
public class IncomingObject
{
    public GameObject objectScroller;
    public float timeUntilSpawn;    // In seconds.

    public IncomingObject()
    {
        timeUntilSpawn = 0f;
    }
}


public class ObjectSpawner : MonoBehaviour {

    public static ObjectSpawner instance;
    public List<IncomingObject> randomObjects;
    public const float minTimeToNextSpawn = 4f;
    public const float maxTimeToNextSpawn = 8f;
    public List<GameObject> allPrefabs;

    void Awake() {
        if (instance == null)
        {   instance = this;    }
        else
        { Destroy(this.gameObject); }

        // This list will unfortunately need to be updated with each added prefab.
        // The total number (for randomly picking one) does NOT need updated. -mw
        this.allPrefabs.Add(GameObject.Find("Wires"));
        this.allPrefabs.Add(GameObject.Find("TeddyBear"));



        this.randomObjects = new List<IncomingObject>();
	}
	
	void Update ()
    {
        // Reduce time until the next one spawns
        foreach(IncomingObject obstacle in this.randomObjects){
            obstacle.timeUntilSpawn -= Time.deltaTime;
        }

        // If there's an object to spawn, do it and remove it from the list.
        if(this.randomObjects.Count > 0 
            && this.randomObjects[0].timeUntilSpawn < 0)
        {
            GameObject newObj = GameObject.Instantiate(this.randomObjects[0].objectScroller);
            newObj.transform.position = new Vector3(0, newObj.GetComponent<ObjectScroller>().startAtY, 0);
            this.randomObjects.RemoveAt(0);
        }

        // If there's less than two objects ready to spawn, queue up a new one.
        if (this.randomObjects.Count < 2)
        {
            this.AddNewObject();
        }   
	}



    void AddNewObject()
    {
        IncomingObject nextObj = new IncomingObject();

        // Decide how long until it spawns.
        float addedWait = 0f;
        if (this.randomObjects.Count != 0)
        {
            addedWait = this.randomObjects[this.randomObjects.Count - 1].timeUntilSpawn;
        }
        nextObj.timeUntilSpawn = addedWait + Random.Range(minTimeToNextSpawn, maxTimeToNextSpawn);

        // Decide randomly which prefab to use.
        nextObj.objectScroller = this.allPrefabs[Random.Range(0, this.allPrefabs.Count)];

        this.randomObjects.Add(nextObj);
        Debug.Log("Object added: " + nextObj.timeUntilSpawn + ", " + nextObj.objectScroller.name);
    }
}
