using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Clickable : MonoBehaviour
{
    private static ClickMaster masterClicker; 

    private Collider2D collider;
    private ClickMaster clickMaster;

    private bool hoveredOver;

    public Collider2D Collider
    {
        get { return collider; }
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
}
