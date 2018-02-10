using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Clickable : MonoBehaviour
{
    private static ClickMaster masterClicker; 

    private Collider2D collider;

    public int priority;

    public AudioClip clickDownSound;
    public AudioClip clickUpSound;

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
            masterClicker = FindObjectOfType<ClickMaster>();
        }

	    masterClicker.register(this);
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

        if (clickDownSound != null)
        {
            masterClicker.playSound(clickDownSound);
        }

        if(clickDownCallback != null)
        {
            clickDownCallback.Invoke();
        }
    }

    public void ReportMouseUp()
    {
        clicked = false;

        if (clickUpSound != null)
        {
            masterClicker.playSound(clickUpSound);
        }

        if (clickReleaseCallback != null)
        {
            clickReleaseCallback.Invoke();
        }
    }

    public List<Clickable> selectionSet()
    {
        return masterClicker.hoverElements;
    }

    public Clickable selected()
    {
        return masterClicker.CurrClickable;
    }

    private void die()
    {
        masterClicker.deRegister(this);
    }
}
