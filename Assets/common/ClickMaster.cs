using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//make sure the execution order is set to do this before any clicable!
public class ClickMaster : MonoBehaviour {

    private Vector3 mouseLoc;
    public Vector3 MouseLoc { get { return mouseLoc; }  }
    private List<Clickable> lastHovered;
    public List<Clickable> clickables { get; set;  }
    public List<Clickable> hovered { get; set; }

	// Use this for initialization
    void Start()
    {
        clickables = new List<Clickable>();
        hovered = new List<Clickable>();
    }
	
	// Update is called once per frame
	void Update ()
	{
	    lastHovered = hovered;
        hovered = new List<Clickable>();

        if(Cursor.visible)
        {
            mouseLoc = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        //what was the mouse over?
	    foreach (var clickable in clickables)
	    {
	        if (Cursor.visible && clickable.Collider.OverlapPoint(mouseLoc))
	        {
	            clickable.ReportHover();
	            hovered.Add(clickable);
            }
        }

	    foreach (var clickable in lastHovered)
	    {
	        if (!(hovered.Contains(clickable)))
	        {
	            clickable.ReportExitHover();
            }
	    }

	    if (Input.GetMouseButtonDown(0))
	    {
	        foreach (var clicked in hovered)
	        {
	            clicked.ReportMouseDown();
	        }
	    }

	    if (Input.GetMouseButtonUp(0))
	    {
	        foreach (var clicked in hovered)
	        {
	            clicked.ReportMouseUp();
	        }
	    }
    }

    public void register(Clickable clickable)
    {
        clickables.Add(clickable);
    }
}
