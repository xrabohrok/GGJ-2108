using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Clickable : MonoBehaviour
{
    private static ClickMaster masterClicker; 

    private Collider2D collider;
    private ClickMaster clickMaster;

    private Action clickDownCallback;
    public void setClickDownCallback(Action thing)
    {
        clickDownCallback = thing;
    }

    private Action clickReleaseCallback;
    public void setClickReleaseCallback(Action thing)
    {
        clickReleaseCallback = thing;
    }

    private bool hoveredOver;
    private bool clicked;

    public Collider2D Collider
    {
        get { return collider; }
    }

    public bool Clicked
    {
        get { return clicked; }
    }

    // Use this for initialization
    void Start ()
	{
	    collider = GetComponent<Collider2D>();
        if(masterClicker == null)
        {
            clickMaster = FindObjectOfType<ClickMaster>();
        }

	    clickMaster.register(this);
	}

    void OnDestroy()
    {
        die();
    }

    public void ReportHover()
    {
        hoveredOver = true;
    }

    public void ReportExitHover()
    {
        hoveredOver = false;
    }

    public void ReportMouseDown()
    {
        clicked = true;
        if(clickDownCallback != null)
        {
            clickDownCallback.Invoke();
        }
    }

    public void ReportMouseUp()
    {
        clicked = false;
        if (clickReleaseCallback != null)
        {
            clickReleaseCallback.Invoke();
        }
    }

    public List<Clickable> selectionSet()
    {
        return clickMaster.hoverElements;
    }

    private void die()
    {
        clickMaster.deRegister(this);
    }
}
