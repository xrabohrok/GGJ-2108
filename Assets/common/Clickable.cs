using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Clickable : MonoBehaviour
{
    private static ClickMaster masterClicker; 

    private Collider2D collider;
    private ClickMaster clickMaster;

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
    }

    public void ReportMouseUp()
    {
        clicked = false;
    }

    public List<Clickable> selectionSet()
    {
        return clickMaster.hoverElements;
    }
}
