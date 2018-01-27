﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridMaster : MonoBehaviour
{
    //whatever this is, it needs a CircuitTile on it somewhere
    public Transform GoodSlotPrefab;

    public int verticalCount = 10;
    public int horizontalCount = 10;

    public float spacing = 5.0f;
    public float size = 10.0f;
    public Transform tilePrefab;

    private List<List<GameObject>> arrayedRefs;
    public List<TileDef> drawSet;

    public List<DragZone> spawns;

#if UNITY_EDITOR
    private float lastSize;
    private int lastVCount;
    private int lastHCount;
    private float lastSpacing;
    private Vector3 offset;
#endif

    // Use this for initialization
    void Start () {
		//center offset
	    var voffset = (size * verticalCount + spacing * (verticalCount - 1))/2;
	    var hoffset = (size * horizontalCount + spacing * (horizontalCount - 1))/2;
        offset = new Vector3(hoffset, voffset, 0);

        arrayedRefs = new List<List<GameObject>>();
	    for (int j = 0; j < verticalCount; j++)
	    {
            arrayedRefs.Add(new List<GameObject>());
	        for (int i = 0; i < horizontalCount; i++)
	        {
	            var relativePos = new Vector3(
                    size * i + spacing * i,
                    size * j + spacing * j,
                    0
                    );
	            var tilePos = relativePos + this.transform.position - offset;
	            var thingy = GameObject.Instantiate(GoodSlotPrefab, tilePos, Quaternion.identity);
                arrayedRefs[j].Add(thingy.gameObject);
	        }
	    }
	}



    // Update is called once per frame
    void Update () {
        foreach (var spawn in spawns)
        {
            if (spawn.CurrentDraggable == null)
            {
                var ind = Mathf.FloorToInt(Random.value * drawSet.Count);
                Debug.Log(ind);
                var draw = drawSet[ind];
                var newTile = GameObject.Instantiate(tilePrefab, spawn.transform.position, Quaternion.identity);
                var tile = newTile.GetComponent<CircuitTile>();
                tile.Initializer(draw.left, draw.right, draw.top, draw.down, draw.leak,
                    draw.tile, draw.poweredTile);
                spawn.setDraggable(tile.gameObject.GetComponent<Draggable>());
            }
        }



#if UNITY_EDITOR
        if (lastHCount != horizontalCount ||
            lastSize != size ||
            lastSpacing != spacing ||
            lastVCount != verticalCount)
        {
            for (int j = 0; j < verticalCount; j++)
            {
                arrayedRefs.Add(new List<GameObject>());
                for (int i = 0; i < horizontalCount; i++)
                {
                    var relativePos = new Vector3(
                        size * i + spacing * i,
                        size * j + spacing * j,
                        0
                    );
                    var tilePos = relativePos + this.transform.position - offset;
                    arrayedRefs[j][i].transform.position = tilePos;
                }
            }
        }

        lastSize = size;
        lastVCount = verticalCount;
        lastHCount = horizontalCount;
        lastSpacing = spacing;
#endif

    }

    [Serializable]
    public class TileDef
    {
        public bool top;
        public bool right;
        public bool down;
        public bool left;
        public bool leak;
        public Sprite tile;
        public Sprite poweredTile;
    }
}
