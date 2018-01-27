using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Clickable))]
public class Draggable : MonoBehaviour
{
    public bool requireDragZone;
    private bool draggable;
    private Clickable clicker;
    private Vector3 lastGoodPos;

    public bool isDraggable
    {
        get { return draggable; }
    }

    public void draggableEnable()
    {
        draggable = true;
    }

    public void draggableDisable()
    {
        draggable = false;
    }

	// Use this for initialization
	void Start ()
	{
	    clicker = GetComponent<Clickable>();
        clicker.setClickDownCallback(() =>
        {
            lastGoodPos = this.transform.position;
        });
        clicker.setClickReleaseCallback(() =>
        {
            if (requireDragZone)
            {
                if (clicker.selectionSet().All(c => c.GetComponent<DragZone>() == null))
                {
                    this.transform.position = lastGoodPos;
                }
            }
        });
	    draggable = true;

	}

    public void snapTo(Vector3 newPos)
    {
        this.transform.position = new Vector3(newPos.x, newPos.y, this.transform.position.z);
    }
	
	// Update is called once per frame
	void Update () {
	    if (clicker.Clicked && draggable)
	    {
	        var mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	        this.transform.position = new Vector3( mouseWorldPoint.x, mouseWorldPoint.y, 0);
	    }
	}
}
