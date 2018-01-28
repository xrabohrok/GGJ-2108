using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer),typeof(Draggable))]
class CircuitTile : MonoBehaviour
{
    private Draggable dragger;
    private SpriteRenderer sRenderer;
    private Sprite currSprite;
    public bool[] ports;
    private bool leakable;
    private Sprite offSprite;
    private Sprite onSprite;
    public bool sourcedPower = false;
    public List<CircuitTile> neighbors;
    private bool currenlyPowered;

    public bool CurrenlyPowered
    {
        get { return currenlyPowered; }
    }

    void Start()
    {
        dragger = GetComponent<Draggable>();
        sRenderer = GetComponent<SpriteRenderer>();
        neighbors = new List<CircuitTile>();
    }

    public void Initializer(bool left, bool right, bool top, bool bottom, bool leak, Sprite off, Sprite on)
    {
        //css order
        ports = new bool[4];
        ports[0] = top;
        ports[1] = right;
        ports[2] = bottom;
        ports[3] = left;

        leakable = leak;
        offSprite = off;
        onSprite = on;

        if (sRenderer == null)
        {
            sRenderer = GetComponent<SpriteRenderer>();
        }
        sRenderer.sprite = off;
    }

    public void clearLinks()
    {
        neighbors = new List<CircuitTile>();
    }

    public void addLink(CircuitTile link)
    {
        if (!neighbors.Contains(link))
        {
            neighbors.Add(link);
        }
    }

    public void setPowered()
    {
        currenlyPowered = true;
        sRenderer.sprite = onSprite;
    }

    public void setUnpowered()
    {
        currenlyPowered = false;
        sRenderer.sprite = offSprite;
    }


}