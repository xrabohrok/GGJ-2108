using UnityEngine;

namespace RobotGame
{
    public class ObjectScroller : MonoBehaviour
    {
        //The Y point to reset the scrolling object
        public float resetAtY = 1;

        public float startAtY = 0;


        void Update()
        {
  

            Renderer objectRenderer = GetComponent<Renderer>();

            // Value of Y change from 0 to resetAtY by time. return to 0 if it becomes greater then resetAtY.
            //float offsetY = Mathf.Repeat(Time.time * speed, resetAtY);

            //Debug.Log("OffsetY:" + offsetY);

            float currentY = objectRenderer.transform.position.y;
            //Debug.Log("CurrentY:" + currentY);

            //Turns the speed from human Friendly to the real speed
            float realSpeed = GetSpeed() / 1000;

            float newY = currentY + realSpeed;

            if (newY >= resetAtY)
            {
                newY = startAtY;
            }

            // Debug.Log("NewY:" + newY);

            // Create offset that shift value of Y
            Vector3 offset = new Vector3(0, newY, 0);

            objectRenderer.transform.position = offset;
        }

        public float GetSpeed()
        {
            return GameObject.Find("Clanky").GetComponent<Clanky>().currentSpeed;
        }
    }
}