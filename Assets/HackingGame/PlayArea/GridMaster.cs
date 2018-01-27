﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public List<TileDef> poweredSet;

    public List<DragZone> spawns;

    private GameObject source;
    private GameObject sink;

#if UNITY_EDITOR
    private float lastSize;
    private int lastVCount;
    private int lastHCount;
    private float lastSpacing;
    private Vector3 offset;
    private bool functioning;
    public bool FunctioningCircuit
    {
        get { return functioning; }
    }
#endif

    // Use this for initialization
    void Start () {
		//center offset
	    var voffset = (size * verticalCount + spacing * (verticalCount - 1))/2;
	    var hoffset = (size * horizontalCount + spacing * (horizontalCount - 1))/2;
        offset = new Vector3(hoffset, voffset, 0);

        generateBoard();
    }

    private void generateBoard()
    {
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
                thingy.name = "tile:(" + i.ToString() + " , " + j.ToString() + ")";
                arrayedRefs[j].Add(thingy.gameObject);
            }
        }

        //of these, choose two that are not on the edge, these are the points that will be connected
        var sourceY = Mathf.FloorToInt( Random.value * (verticalCount - 2)) + 1;
        var sourceX = Mathf.FloorToInt(Random.value * (horizontalCount - 2)) + 1;

        int sinkX = sourceX;
        while (sinkX == sourceX)
        {
            sinkX = Mathf.FloorToInt( Random.value * (horizontalCount - 2)) + 1;
        }

        int sinkY = sourceY;
        while (sinkY == sourceY)
        {
            sinkY = Mathf.FloorToInt(Random.value * (verticalCount - 2)) + 1;
        }

        var dir = Mathf.FloorToInt(Random.value * 4);
        source = Instantiate(tilePrefab, arrayedRefs[sourceY][sourceX].transform.position, Quaternion.identity).gameObject;
        setupSourcedTile(sourceY, sourceX, dir, source.GetComponent<Draggable>(), true);

        dir = Mathf.FloorToInt(Random.value * 4);
        sink = Instantiate(tilePrefab, arrayedRefs[sinkX][sinkY].transform.position, Quaternion.identity).gameObject;
        setupSourcedTile(sinkY, sinkX, dir, sink.GetComponent<Draggable>(), false);
    }

    private void setupSourcedTile(int indexX, int indexY, int dir, Draggable tile, bool sourcePowered)
    {
        var sourcePos = arrayedRefs[indexX][indexY].GetComponent<DragZone>();
        var currPiece = tile;
        sourcePos.setDraggable(currPiece);
        currPiece.snapTo(sourcePos.transform.position, sourcePos);
        var currTile = poweredSet[dir];
        var circuit = currPiece.GetComponent<CircuitTile>();
        circuit.Initializer(currTile.left, currTile.right, currTile.top, currTile.down,
            false, currTile.tile, currTile.poweredTile);
        circuit.sourcedPower = sourcePowered;
    }


    // Update is called once per frame
    void Update () {
        handleTileSpawner();

        //initial pass (clear trees)
        for (int j = 0; j < verticalCount; j++)
        {
            for (int i = 0; i < horizontalCount; i++)
            {
                var thingy = getCircuitRef(j, i);
                if (thingy != null)
                {
                    thingy.clearLinks();
                    thingy.setUnpowered();
                }
            }
        }

        //build tree
        for (int j = 0; j < verticalCount; j++)
        {
            for (int i = 0; i < horizontalCount; i++)
            {
                var thisTile = getCircuitRef(j, i);
                if(thisTile != null)
                {

                    //up
                    if (thisTile.ports[0] && j < verticalCount - 1)
                    {
                        linkNeighbors(thisTile, i, j + 1);
                    }                
                    //right
                    if (thisTile.ports[1] && i < horizontalCount - 1)
                    {
                        linkNeighbors(thisTile, i + 1, j);
                    }                
                    //down
                    if (thisTile.ports[1] && j > 0)
                    {
                        linkNeighbors(thisTile, i, j-1);
                    }                
                    //left
                    if (thisTile.ports[1] && i > 0)
                    {
                        linkNeighbors(thisTile, i- 1, j);
                    }
                }
            }
        }

        //crawl tree
        var goal = sink.GetComponent<CircuitTile>();
        var unprocessed = new List<CircuitTile>();
        var processed = new List<CircuitTile>();
        unprocessed.Add(source.GetComponent<CircuitTile>());
        var misses = 0;
        functioning = false;
        while (unprocessed.Any())
        {
            var current = unprocessed[0];
            current.setPowered();

            if (current == goal)
            {
                functioning = true;
            }

            //count busted links
            var openPorts = 0;
            foreach (var currentPort in current.ports)
            {
                if (currentPort)
                {
                    openPorts++;
                }
            }
            misses += openPorts - current.neighbors.Count;

            foreach (var neighbor in current.neighbors)
            {
                if (!unprocessed.Contains(neighbor) && !processed.Contains(neighbor))
                {
                    unprocessed.Add(neighbor);
                }
            }
            unprocessed.Remove(current);
            processed.Add(current);
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

    private void linkNeighbors(CircuitTile thisTile, int i, int j)
    {
        var neighbor = getCircuitRef(i, j);
        if (neighbor != null)
        {
            thisTile.addLink(neighbor);
            neighbor.addLink(thisTile);
        }
    }

    private CircuitTile getCircuitRef(int j, int i)
    {
        var thisDraggable = arrayedRefs[j][i].GetComponent<DragZone>().CurrentDraggable;
        if (thisDraggable != null)
        {
            return thisDraggable.GetComponent<CircuitTile>();
        }
        return null;
    }

    private void handleTileSpawner()
    {
        foreach (var spawn in spawns)
        {
            if (spawn.CurrentDraggable == null)
            {
                var ind = Mathf.FloorToInt(Random.value * drawSet.Count);
                var draw = drawSet[ind];
                var newTile = GameObject.Instantiate(tilePrefab, spawn.transform.position, Quaternion.identity);
                var tile = newTile.GetComponent<CircuitTile>();
                spawn.setDraggable(tile.gameObject.GetComponent<Draggable>());
                tile.GetComponent<Draggable>().snapTo(spawn.transform.position, spawn);
                tile.Initializer(draw.left, draw.right, draw.top, draw.down, draw.leak,
                    draw.tile, draw.poweredTile);
            }
        }
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
