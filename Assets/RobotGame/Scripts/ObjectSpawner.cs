using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour {

    public static ObjectSpawner instance;
    public List<ObjectScroller> randomObjects;

	void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
	}
	
	void Update () {
		if (this.randomObjects.Count < 2)
        {
            int randTime = Random.Range(2, 4);
        }
	}
}
