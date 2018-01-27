using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackingGameDriver : MonoBehaviour {

    private CircuitTile draggedThing = null;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	    if (draggedThing != null)
	    {
	        draggedThing.move(Camera.main.ScreenToWorldPoint(Input.mousePosition));
	    }

	    if (Input.GetMouseButtonUp(0))
	    {
	        draggedThing = null;
	    }
    }
}