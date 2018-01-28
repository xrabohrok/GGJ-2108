﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Clickable))]
public class DragZone : MonoBehaviour
{
    private bool lastClick;
    private Clickable clicker;
    public Draggable currentDraggable;
//    private Draggable currentDraggable;

    public Draggable CurrentDraggable
    {
        get { return currentDraggable; }
    }

    // Use this for initialization
	void Start ()
	{
	    clicker = GetComponent<Clickable>();
        clicker.setClickReleaseCallback(SnapDraggablesToPos);
	}
	

    private void SnapDraggablesToPos()
    {
//if there is a tile that is draggable in the mouse set, take it and grab it
        foreach (var clicked in clicker.selectionSet())
        {
            var draggable = clicked.GetComponent<Draggable>();
            if (draggable != null)
            {
                if (draggable != CurrentDraggable)
                {
                    draggable.CurrentDragZone.setDraggable(null);
                }
                draggable.snapTo(this.transform.position, this);
            }
        }
    }

    public void setDraggable(Draggable draggable)
    {
        //overlap destruction
        if (currentDraggable != null && currentDraggable != draggable && draggable != null)
        {
            currentDraggable.funeralRites();
            GameObject.Destroy(currentDraggable.gameObject);
            currentDraggable = null;
        }

        currentDraggable = draggable;

    }
}
