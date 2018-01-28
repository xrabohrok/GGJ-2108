using UnityEngine;

[RequireComponent(typeof(Clickable))]
public class DragZone : MonoBehaviour
{
    private bool lastClick;
    private Clickable clicker;
    private Draggable currentDraggable;
    private bool locked;

    public Draggable CurrentDraggable
    {
        get { return currentDraggable; }
    }

    public bool Locked
    {
        get { return locked; }
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
        if (!locked)
        {
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
    }

    public void setDraggable(Draggable draggable)
    {
        //overlap destruction
        if (currentDraggable != null && currentDraggable != draggable && draggable != null)
        {
            GameObject.Destroy(currentDraggable.gameObject);
            currentDraggable = null;
        }

        currentDraggable = draggable;

    }

    public void nukeStuff()
    {
        if (currentDraggable != null)
        {
            GameObject.Destroy(currentDraggable.gameObject);
        }
    }

    public void LockDraggable()
    {
        locked = true;
    }

    public void UnlockDraggable()
    {
        locked = false;
    }
}
