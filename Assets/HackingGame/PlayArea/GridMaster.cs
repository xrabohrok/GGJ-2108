using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMaster : MonoBehaviour
{
    //whatever this is, it needs a CircuitTile on it somewhere
    public Transform GoodSlotPrefab;

    public int verticalCount = 10;
    public int horizontalCount = 10;

    public float spacing = 5.0f;
    public float size = 10.0f;

	// Use this for initialization
	void Start () {
		//center offset
	    var voffset = (size * verticalCount + spacing * (verticalCount - 1))/2;
	    var hoffset = (size * horizontalCount + spacing * (horizontalCount - 1))/2;
        var offset = new Vector3(hoffset, voffset, 0);

	    for (int j = 0; j < verticalCount; j++)
	    {
	        for (int i = 0; i < horizontalCount; i++)
	        {
	            var relativePos = new Vector3(
                    size * i + spacing * i,
                    size * j + spacing * j,
                    0
                    );
	            var tilePos = relativePos + this.transform.position - offset;
	            GameObject.Instantiate(GoodSlotPrefab, tilePos, Quaternion.identity);
	        }
	    }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
