using UnityEngine;

class CircuitTile : MonoBehaviour
{
    public void move(Vector3 newloc)
    {
        this.transform.position = new Vector3(newloc.x, newloc.y, 0);
    }
}