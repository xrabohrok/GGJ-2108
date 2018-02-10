using UnityEngine;

public class ObjectScroller : MonoBehaviour
{
    // Scroll speed
    public float speed = 0.25f;

    //The Y point to reset the scrolling object
    public float resetAtY = 1;

    void Update()
    {
        // Value of Y change from 0 to resetAtY by time. return to 0 if it becomes greater then resetAtY.
        float y = Mathf.Repeat(Time.time * speed, resetAtY);

        // Create offset that shift value of Y
        Vector3 offset = new Vector3(0, y, 0);

        GetComponent<Renderer>().transform.position = offset;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}