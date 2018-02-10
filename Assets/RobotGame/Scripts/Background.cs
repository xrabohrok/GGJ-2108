using UnityEngine;

public class Background : MonoBehaviour
{
    // Scroll speed
    public float speed = 0.1f;

    void Update()
    {
        // Value of Y change from 0 to 1 by time. return to 0 if it becomes 1 and repeat.
        float y = Mathf.Repeat(Time.time * speed, 1);

        // Create offset that shift value of Y
        Vector3 offset = new Vector3(0, y, 0);

        // Set up offset to materials
        //GetComponent<Renderer>().sharedMaterial.("Sprites-Default", offset);

        GetComponent<Grid>().transform.position = offset;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}