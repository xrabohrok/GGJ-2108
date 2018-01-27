using UnityEngine;

[RequireComponent(typeof(SpriteRenderer),typeof(Draggable))]
class CircuitTile : MonoBehaviour
{
    private Draggable dragger;
    private SpriteRenderer sRenderer;
    private Sprite currSprite;
    private bool[] ports;
    private bool leakable;
    private Sprite offSprite;
    private Sprite onSprite;
    public bool sourcedPower = false;

    void Start()
    {
        dragger = GetComponent<Draggable>();
        sRenderer = GetComponent<SpriteRenderer>();

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


}