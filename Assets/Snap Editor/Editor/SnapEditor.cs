using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SnapEditor : EditorWindow
{

    private int tiles = 50;

    public static GameObject levelParent;
    private MeshFilter levelMFilter;
    public static PolygonCollider2D poly;
    private Mesh parentMesh;
    private List<Vector4> collisions;
    private List<Vector2> _points;

    private List<Vector4> debugLines = new List<Vector4>();

    //Layers
    public static int layerActive = 0;
    public static List<bool> layerVisible = new List<bool>();
    public static List<GameObject> layerObj = new List<GameObject>();
    public static List<float> layerDepth = new List<float>();

    public static string _newLayername;

    //Editor Colors
    public static Color SelectionColor = new Color(0.43f, 0.95f, 0.83f);
    private Color SelectedTileColor = new Color(0.98f, 0.4f, 0.06f);

    //Editor Icons
    public static Texture _visibleIcon;
    public static Texture _hiddenIcon;
    private Texture _brushIcon;
    private Texture _brushActiveIcon;
    private Texture _collisionIcon;
    private Texture _smartMatIcon;
    private Texture _bigBrushIcon;
    private Texture _bigBrushActiveIcon;
    private Texture _horBrushIcon;
    private Texture _horBrushActiveIcon;
    private Texture _verBrushIcon;
    private Texture _verBrushActiveIcon;
    private Texture _rectIcon;
    private Texture _rectActiveIcon;
    private Texture _bigMatIcon;

    private bool editMode = false;
    private bool draw = false;
    private bool bigBrush = false;
    private bool horBrush = false;
    private bool verBrush = false;
    private bool rect = false;

    private int activeTile = 0;

    //Tiles
    private Texture[] TileTex = new Texture2D[50];
    private Material[] TileMat = new Material[50];
    private bool[] TileCollision = new bool[50];
    private bool[] TileSmartMat = new bool[50];
    private bool[] TileBigMat = new bool[50];
    private Vector2[] TileBigMatSize = new Vector2[50];

    private Vector2 scrollPos;

    private static EditorWindow layersWindow;

    //RECT
    private Vector2 rectStart;
    private Vector2 rectStart2;
    private Vector2 rectEnd;
    private bool drawRect = false;
    private bool delRect = false;

    private SESettings _settings;

    //LayerMask

    private LayerMask _lmask = Physics.DefaultRaycastLayers;

    [MenuItem("Tools/Snap Editor")]

    public static void ShowWindow()
    {
        EditorWindow w = EditorWindow.GetWindow(typeof(SnapEditor));
        w.titleContent = new GUIContent("Snap Editor");
    }


    void Awake()
    {
        //LoadTiles();
        LevelParent();

        //load icons
        LoadIcons();

        _settings = AssetDatabase.LoadAssetAtPath("Assets/Snap Editor/SESettings.prefab", typeof(SESettings)) as SESettings;

        if (_settings)
        {
            SEStoredTiles scr = AssetDatabase.LoadAssetAtPath(_settings.LastOpenedTileset, typeof(SEStoredTiles)) as SEStoredTiles;
            if (scr)
            {
                TileTex = scr.TileTex;
                TileMat = scr.TileMat;
                TileCollision = scr.TileCollision;
                TileSmartMat = scr.TileSmartMat;
                TileBigMat = scr.TileBigMat;
                TileBigMatSize = scr.TileBigMatSize;
                Debug.Log("Opened last opened Tileset.");
            }else
            Debug.Log("Couldn't find last opened Tileset, please load another Tileset manually.");
        }
        else
        {
            Debug.Log("Couldn't find settings file. Using default settings.");
        }
    }

    void OnGUI()
    {
        if (!levelParent)
        {
            LevelParent();
        }

        //Recreate layer window
        if (!layersWindow)
        {
            layersWindow = EditorWindow.GetWindow(typeof(TPWindow));
            layersWindow.titleContent = new GUIContent("Layers");
        }
        EditorGUILayout.Separator();
        GUILayout.Label("Drawing: ", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();

        if (!editMode && !poly)
        {
            //Brush tool
            GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(72));
            GUILayout.Label("Brush Tool", EditorStyles.miniLabel);
            GUI.backgroundColor = Color.white;
            Texture ic = _brushIcon;
            if (draw) ic = _brushActiveIcon;
            draw = GUILayout.Toggle(draw, new GUIContent(ic, "Draw tiles (D)"), "Button", GUILayout.Height(67), GUILayout.Width(67));
            if (draw)
            {
                rect = false;
            }
            EditorGUILayout.EndVertical();

            GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(72));
            GUILayout.Label("Brush mobifiers", EditorStyles.miniLabel);
            GUI.backgroundColor = Color.white;
            EditorGUILayout.BeginHorizontal();

            ic = _bigBrushIcon;
            if (bigBrush) ic = _bigBrushActiveIcon;
            bigBrush = GUILayout.Toggle(bigBrush, new GUIContent(ic, "Big 3x3 tiles brush (B)"), "Button", GUILayout.Height(32), GUILayout.Width(32));
            if (bigBrush)
            {
                horBrush = false;
                verBrush = false;
            }

            ic = _horBrushIcon;
            if (horBrush) ic = _horBrushActiveIcon;
            horBrush = GUILayout.Toggle(horBrush, new GUIContent(ic, "Horizontal 3x3 tiles brush (H)"), "Button", GUILayout.Height(32), GUILayout.Width(32));
            if (horBrush)
            {
                bigBrush = false;
                verBrush = false;
            }

            ic = _verBrushIcon;
            if (verBrush) ic = _verBrushActiveIcon;
            verBrush = GUILayout.Toggle(verBrush, new GUIContent(ic, "Vertical 3x3 tiles brush (V)"), "Button", GUILayout.Height(32), GUILayout.Width(32));
            if (verBrush)
            {
                bigBrush = false;
                horBrush = false;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            //Rect tool
            GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(72));
            GUILayout.Label("Rect. Tool", EditorStyles.miniLabel);
            GUI.backgroundColor = Color.white;
            ic = _rectIcon;
            if (rect) ic = _rectActiveIcon;
            rect = GUILayout.Toggle(rect, new GUIContent(ic, "Rectangle (R)"), "Button", GUILayout.Height(67), GUILayout.Width(67));
            if (rect)
            {
                draw = false;
            }
            EditorGUILayout.EndVertical();
        }
        else
        {
            if (!editMode) GUILayout.Label("Unbuild level to edit it.", EditorStyles.centeredGreyMiniLabel);
            else GUILayout.Label("Drawing tools disabled while editing tileset.", EditorStyles.centeredGreyMiniLabel);
            draw = false;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();
        GUILayout.Label("Functions: ", EditorStyles.boldLabel);
        GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUI.backgroundColor = Color.white;
        editMode = GUILayout.Toggle(editMode, "Edit Tileset", "Button", GUILayout.Width(96), GUILayout.Height(24));
        EditorGUILayout.Separator();
        if (editMode)
        {
            if (GUILayout.Button("New Tileset")) NewTiles();
            EditorGUILayout.Separator();
            if (GUILayout.Button("Open Tileset")) LoadTiles();
            EditorGUILayout.Separator();
            if (GUILayout.Button("Save Tileset")) SaveTiles();
        }
        else
        {
            {
                if (poly)
                {
                    if (GUILayout.Button("UnBuild Level", GUILayout.Width(96), GUILayout.Height(24))) UnbuildLevel();
                }
            }
            if (levelParent)
            {
                if (!poly)
                    if (GUILayout.Button("Build Level", GUILayout.Width(96), GUILayout.Height(24))) BuildLevel();
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();

        GUILayout.Label("Tiles:", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUI.backgroundColor = new Color(0.3f, 0.3f, 0.3f);
        EditorGUILayout.BeginVertical("Box");
        GUI.backgroundColor = Color.white;
        EditorGUILayout.BeginHorizontal();
        int maxHor = Mathf.FloorToInt((this.position.width - 15) / 100 - 1);
        if (editMode)
        {
            int r = 0;
            for (int i = 0; i < tiles; i++)
            {
                bool row = false;
                if (r > maxHor)
                {
                    row = true;
                    r = 0;
                }
                if (row)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
                EditorGUILayout.BeginVertical("Box");
                if (GUILayout.Button("", GUILayout.Width(64), GUILayout.Height(64)))
                {
                    LoadMaterial(i);
                }
                Rect re = GUILayoutUtility.GetLastRect();
                re = new Rect(re.x + 2, re.y + 2, re.width - 4, re.height - 4);
                if (TileTex[i])
                {
                    if (TileSmartMat[i])
                    {
                        Rect rct = new Rect(0.125f, 0.25f, 0.25f, 0.5f);
                        GUI.DrawTextureWithTexCoords(re, TileTex[i], rct);
                    }
                    else
                    {
                        GUI.DrawTexture(re, TileTex[i]);
                    }

                    int ofst = 0;
                    if (TileCollision[i])
                    {
                        GUI.DrawTexture(new Rect(re.x + 2, re.y + 2, 16, 16), _collisionIcon);
                        ofst += 18;
                    }
                    if (TileSmartMat[i])
                    {
                        GUI.DrawTexture(new Rect(re.x + ofst, re.y + 2, 16, 16), _smartMatIcon);
                        ofst += 18;
                    }
                    if (TileBigMat[i])
                    {
                        GUI.DrawTexture(new Rect(re.x + ofst, re.y + 2, 16, 16), _bigMatIcon);
                        ofst += 18;
                    }
                }

                TileCollision[i] = GUILayout.Toggle(TileCollision[i], "Collision");
                TileSmartMat[i] = GUILayout.Toggle(TileSmartMat[i], "Smart Mat");
                if (TileSmartMat[i])
                {
                    TileBigMat[i] = false;
                }
                TileBigMat[i] = GUILayout.Toggle(TileBigMat[i], "Big Mat");
                if (TileBigMat[i])
                {
                    TileSmartMat[i] = false;
                    GUILayout.Label("Width - Height", EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Separator();
                    TileBigMatSize[i] = new Vector2(EditorGUILayout.FloatField(TileBigMatSize[i].x, GUILayout.Width(24)), EditorGUILayout.FloatField(TileBigMatSize[i].y, GUILayout.Width(24)));
                    EditorGUILayout.Separator();
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                r++;
            }
        }
        else
        {
            int r = 0;
            for (int i = 0; i < tiles; i++)
            {
                bool row = false;
                if (r > maxHor)
                {
                    row = true;
                    r = 0;
                }
                if (row)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
                if (i == activeTile) GUI.backgroundColor = SelectedTileColor;
                else GUI.backgroundColor = new Color(0.3f, 0.3f, 0.3f);
                GUIStyle st = new GUIStyle("Box");
                int size = 64;
                if (!editMode) size = 96;
                st.overflow = new RectOffset(3, 2, 3, 3);
                if (GUILayout.Button("", st, GUILayout.Width(size), GUILayout.Height(size)) && TileTex[i]) activeTile = i;
                Rect re = GUILayoutUtility.GetLastRect();
                if (activeTile != i) re = new Rect(re.x + 2, re.y + 2, re.width - 5, re.height - 5);
                else re = new Rect(re.x, re.y, re.width - 1, re.height - 1);
                if (TileTex[i])
                {
                    if (activeTile == i) GUI.color = (SelectedTileColor * 0.2f + Color.white * 0.8f);
                    if (TileSmartMat[i])
                    {
                        Rect rct = new Rect(0.125f, 0.25f, 0.25f, 0.5f);
                        GUI.DrawTextureWithTexCoords(re, TileTex[i], rct);
                    }
                    else
                    {
                        GUI.DrawTexture(re, TileTex[i]);
                    }
                    GUI.color = Color.white;
                    int ofst = 2;
                    if (TileCollision[i])
                    {
                        GUI.DrawTexture(new Rect(re.x + ofst, re.y + 2, 16, 16), _collisionIcon);
                        ofst += 18;
                    }
                    if (TileSmartMat[i])
                    {
                        GUI.DrawTexture(new Rect(re.x + ofst, re.y + 2, 16, 16), _smartMatIcon);
                        ofst += 18;
                    }
                    if (TileBigMat[i])
                    {
                        GUI.DrawTexture(new Rect(re.x + ofst, re.y + 2, 16, 16), _bigMatIcon);
                        ofst += 18;
                    }
                }

                GUI.backgroundColor = Color.white;
                r++;
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    void OnFocus()
    {
        // Remove delegate listener if it has previously
        // been assigned.
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        // Add (or re-add) the delegate.
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
    }

    void OnDestroy()
    {
        // When the window is destroyed, remove the delegate
        // so that it will no longer do any drawing.
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        if (layersWindow) layersWindow.Close();
    }

    void OnSceneGUI(SceneView sceneView)
    {

        //Hotkeys
        //Draw
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.D && (!editMode && !poly))
        {
            if (draw) draw = false;
            else
            {
                draw = true;
                rect = false;
            }
            this.Repaint();
        }

        //Big Brush
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.B && (!editMode && !poly))
        {
            if (bigBrush) bigBrush = false;
            else
            {
                bigBrush = true;
                horBrush = false;
                verBrush = false;
            }
            this.Repaint();
        }

        //Hor Brush
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.H && (!editMode && !poly))
        {
            if (horBrush) horBrush = false;
            else
            {
                bigBrush = false;
                horBrush = true;
                verBrush = false;
            }
            this.Repaint();
        }

        //Ver Brush
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.V && (!editMode && !poly))
        {
            if (verBrush) verBrush = false;
            else
            {
                bigBrush = false;
                horBrush = false;
                verBrush = true;
            }
            this.Repaint();
        }

        //Rect
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.R && (!editMode && !poly))
        {
            if (rect) rect = false;
            else
            {
                rect = true;
                draw = false;
            }
            this.Repaint();
        }

        if (draw || rect)
        {

            //Disable Unity tools
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            Tools.current = Tool.None;

            Vector2 mpos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
            Vector2 mgrid = new Vector2(Mathf.FloorToInt(mpos.x), Mathf.FloorToInt(mpos.y));

            if (draw)
            {
                Handles.color = SelectedTileColor;
                if (bigBrush)
                {
                    Handles.DrawLine(mgrid + new Vector2(-1, -1), mgrid + new Vector2(-1, 2));
                    Handles.DrawLine(mgrid + new Vector2(-1, -1), mgrid + new Vector2(2, -1));
                    Handles.DrawLine(mgrid + new Vector2(2, 2), mgrid + new Vector2(-1, 2));
                    Handles.DrawLine(mgrid + new Vector2(2, 2), mgrid + new Vector2(2, -1));

                }
                else if (verBrush)
                {
                    Handles.DrawLine(mgrid + new Vector2(0, -1), mgrid + new Vector2(0, 2));
                    Handles.DrawLine(mgrid + new Vector2(0, -1), mgrid + new Vector2(1, -1));
                    Handles.DrawLine(mgrid + new Vector2(1, 2), mgrid + new Vector2(0, 2));
                    Handles.DrawLine(mgrid + new Vector2(1, 2), mgrid + new Vector2(1, -1));
                }
                else if (horBrush)
                {
                    Handles.DrawLine(mgrid + new Vector2(-1, 0), mgrid + new Vector2(-1, 1));
                    Handles.DrawLine(mgrid + new Vector2(-1, 0), mgrid + new Vector2(2, 0));
                    Handles.DrawLine(mgrid + new Vector2(2, 1), mgrid + new Vector2(-1, 1));
                    Handles.DrawLine(mgrid + new Vector2(2, 1), mgrid + new Vector2(2, 0));
                }
                else
                {
                    Handles.DrawLine(mgrid, mgrid + new Vector2(0, 1));
                    Handles.DrawLine(mgrid, mgrid + new Vector2(1, 0));
                    Handles.DrawLine(mgrid + new Vector2(1, 1), mgrid + new Vector2(0, 1));
                    Handles.DrawLine(mgrid + new Vector2(1, 1), mgrid + new Vector2(1, 0));
                }
                Handles.color = Color.white;
            }

            if (rect)
            {
                Handles.color = Color.yellow;

                Handles.DrawLine(mgrid, mgrid + new Vector2(0, 1));
                Handles.DrawLine(mgrid, mgrid + new Vector2(1, 0));
                Handles.DrawLine(mgrid + new Vector2(1, 1), mgrid + new Vector2(0, 1));
                Handles.DrawLine(mgrid + new Vector2(1, 1), mgrid + new Vector2(1, 0));

                if (drawRect)
                {
                    Handles.color = SelectedTileColor;

                    Handles.DrawLine(rectStart2, new Vector2(rectStart2.x, rectEnd.y));
                    Handles.DrawLine(rectStart2, new Vector2(rectEnd.x, rectStart2.y));
                    Handles.DrawLine(rectEnd, new Vector2(rectStart2.x, rectEnd.y));
                    Handles.DrawLine(rectEnd, new Vector2(rectEnd.x, rectStart2.y));

                    Handles.color = Color.white;
                }

                if (delRect)
                {
                    Handles.color = Color.red;

                    Handles.DrawLine(rectStart2, new Vector2(rectStart2.x, rectEnd.y));
                    Handles.DrawLine(rectStart2, new Vector2(rectEnd.x, rectStart2.y));
                    Handles.DrawLine(rectEnd, new Vector2(rectStart2.x, rectEnd.y));
                    Handles.DrawLine(rectEnd, new Vector2(rectEnd.x, rectStart2.y));

                    Handles.color = Color.white;
                }
            }

            //Place tiles
            if ((Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag))
            {
                if (draw)
                {
                    if (Event.current.button == 0)
                    {
                        DrawTile(mgrid);
                        if (bigBrush)
                        {
                            DrawTile(mgrid + new Vector2(-1, -1));
                            DrawTile(mgrid + new Vector2(0, -1));
                            DrawTile(mgrid + new Vector2(1, -1));
                            DrawTile(mgrid + new Vector2(-1, 0));
                            DrawTile(mgrid + new Vector2(1, 0));
                            DrawTile(mgrid + new Vector2(-1, 1));
                            DrawTile(mgrid + new Vector2(0, 1));
                            DrawTile(mgrid + new Vector2(1, 1));
                        }

                        if (verBrush)
                        {
                            DrawTile(mgrid + new Vector2(0, -1));
                            DrawTile(mgrid + new Vector2(0, 1));
                        }

                        if (horBrush)
                        {
                            DrawTile(mgrid + new Vector2(-1, 0));
                            DrawTile(mgrid + new Vector2(1, 0));
                        }
                    }
                    //Delete Tiles
                    if (Event.current.button == 1)
                    {
                        DeleteTile(mgrid);
                        if (bigBrush)
                        {
                            DeleteTile(mgrid + new Vector2(-1, -1));
                            DeleteTile(mgrid + new Vector2(0, -1));
                            DeleteTile(mgrid + new Vector2(1, -1));
                            DeleteTile(mgrid + new Vector2(-1, 0));
                            DeleteTile(mgrid + new Vector2(1, 0));
                            DeleteTile(mgrid + new Vector2(-1, 1));
                            DeleteTile(mgrid + new Vector2(0, 1));
                            DeleteTile(mgrid + new Vector2(1, 1));
                        }

                        if (verBrush)
                        {
                            DeleteTile(mgrid + new Vector2(0, -1));
                            DeleteTile(mgrid + new Vector2(0, 1));
                        }

                        if (horBrush)
                        {
                            DeleteTile(mgrid + new Vector2(-1, 0));
                            DeleteTile(mgrid + new Vector2(1, 0));
                        }
                    }
                }

                if (rect)
                {
                    if (Event.current.button == 0 && !delRect)
                    {
                        if (!drawRect)
                        {
                            drawRect = true;
                            rectStart = mgrid;
                        }

                        int ex = 1;
                        if (mgrid.x < rectStart.x) ex = 0;
                        int ey = 1;
                        if (mgrid.y < rectStart.y) ey = 0;

                        rectEnd = mgrid + new Vector2(ex, ey);
                        rectStart2 = rectStart - new Vector2(ex - 1, ey - 1);

                    }

                    if (Event.current.button == 1 && !drawRect)
                    {
                        if (!delRect)
                        {
                            delRect = true;
                            rectStart = mgrid;
                        }

                        int ex = 1;
                        if (mgrid.x < rectStart.x) ex = 0;
                        int ey = 1;
                        if (mgrid.y < rectStart.y) ey = 0;

                        rectEnd = mgrid + new Vector2(ex, ey);
                        rectStart2 = rectStart - new Vector2(ex - 1, ey - 1);

                    }

                }
            }

            if (Event.current.type == EventType.MouseUp)
            {
                if (drawRect && Event.current.button == 0)
                {
                    Rect tr = new Rect();
                    if (rectStart.x < rectEnd.x)
                    {
                        tr.x = rectStart.x;
                        tr.width = Mathf.Abs(rectEnd.x - rectStart.x);
                    }
                    else
                    {
                        tr.x = rectEnd.x;
                        tr.width = Mathf.Abs(rectEnd.x - rectStart.x) + 1;
                    }

                    if (rectStart.y < rectEnd.y)
                    {
                        tr.y = rectStart.y;
                        tr.height = Mathf.Abs(rectEnd.y - rectStart.y);
                    }
                    else
                    {
                        tr.y = rectEnd.y;
                        tr.height = Mathf.Abs(rectEnd.y - rectStart.y) + 1;
                    }

                    for (int y = 0; y < tr.height; y++)
                    {
                        for (int x = 0; x < tr.width; x++)
                        {
                            DrawTile(new Vector2(tr.x + x, tr.y + y));
                        }
                    }


                    drawRect = false;
                }

                if (delRect && Event.current.button == 1)
                {
                    Rect tr = new Rect();
                    if (rectStart.x < rectEnd.x)
                    {
                        tr.x = rectStart.x;
                        tr.width = Mathf.Abs(rectEnd.x - rectStart.x);
                    }
                    else
                    {
                        tr.x = rectEnd.x;
                        tr.width = Mathf.Abs(rectEnd.x - rectStart.x) + 1;
                    }

                    if (rectStart.y < rectEnd.y)
                    {
                        tr.y = rectStart.y;
                        tr.height = Mathf.Abs(rectEnd.y - rectStart.y);
                    }
                    else
                    {
                        tr.y = rectEnd.y;
                        tr.height = Mathf.Abs(rectEnd.y - rectStart.y) + 1;
                    }

                    for (int y = 0; y < tr.height; y++)
                    {
                        for (int x = 0; x < tr.width; x++)
                        {
                            DeleteTile(new Vector2(tr.x + x, tr.y + y));
                        }
                    }


                    delRect = false;
                }
            }

            Selection.activeObject = null;
            if (Event.current.type != EventType.Repaint && Event.current.type != EventType.Layout) Event.current.Use();
        }
    }

    void DrawTile(Vector2 pos)
    {
        Ray ray = new Ray(new Vector3(pos.x + 0.5f, pos.y + 0.5f, -500), Vector3.forward);
        RaycastHit hit = new RaycastHit();
        if (!Physics.Raycast(ray, out hit, 1000.0f, _lmask))
        {
            if (!levelParent) LevelParent();

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            MeshFilter mf = go.GetComponent<MeshFilter>();
            Renderer rn = go.GetComponent<Renderer>();
            rn.material = TileMat[activeTile];
            go.transform.parent = layerObj[layerActive].transform;
            go.transform.position = new Vector3(pos.x + 0.5f, pos.y + 0.5f, layerDepth[layerActive]);
            if (TileSmartMat[activeTile]) mf.mesh = CreateSmartmatMesh(rn.sharedMaterial, new Vector2(pos.x + 0.5f, pos.y + 0.5f));
            if (!TileSmartMat[activeTile] && TileBigMat[activeTile])
            {
                AdjustUVS(mf, TileBigMatSize[activeTile]);
            }
            if (TileCollision[activeTile]) go.AddComponent<BoxCollider>();

            Undo.RegisterCreatedObjectUndo(go, "Place tile");
        }
    }

    void DeleteTile(Vector2 pos)
    {
        Ray ray = new Ray(new Vector3(pos.x + 0.5f, pos.y + 0.5f, -500), Vector3.forward);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, 1000.0f, _lmask) && levelParent)
        {
            if (hit.transform.parent.parent == levelParent.transform)
            {
                Material mat = hit.transform.GetComponent<Renderer>().sharedMaterial;
                Vector2 pos2 = new Vector2(hit.transform.position.x, hit.transform.position.y);
                Undo.DestroyObjectImmediate(hit.transform.gameObject);
                SmartMatAdjustSurroundingUV(mat, pos2);

            }
        }
    }

    void LoadMaterial(int tileId)
    {
        string path = EditorUtility.OpenFilePanel("Load Material", "Assets", "mat");
        if (path.Length > 0)
        {
            //Trim path
            string localPath = "Assets" + path.Remove(0, (Application.dataPath).Length);
            Debug.Log("Opening Material: " + localPath);
            TileMat[tileId] = AssetDatabase.LoadAssetAtPath(localPath, typeof(Material)) as Material;
            TileTex[tileId] = TileMat[tileId].mainTexture;
        }
        else
        {
            Debug.Log("Opening canceled.");
        }
    }

    void NewTiles()
    {
        TileTex = new Texture2D[50];
        TileMat = new Material[50];
        TileCollision = new bool[50];
        TileSmartMat = new bool[50];
        TileBigMat = new bool[50];
        TileBigMatSize = new Vector2[50];
    }

    void SaveTiles()
    {

        string path = EditorUtility.SaveFilePanel("Save Noise Profile", "Assets", "Tileset", "prefab");

        if (path.Length > 0)
        {
            //Trim path
            string localPath = "Assets" + path.Remove(0, (Application.dataPath).Length);
            Debug.Log("Saving Tileset at: " + localPath);

            GameObject obj = new GameObject("SavedTiles");
            SEStoredTiles scr = obj.AddComponent<SEStoredTiles>();
            scr.TileTex = TileTex;
            scr.TileMat = TileMat;
            scr.TileCollision = TileCollision;
            scr.TileSmartMat = TileSmartMat;
            scr.TileBigMat = TileBigMat;
            scr.TileBigMatSize = TileBigMatSize;
            obj.name = Path.GetFileName(path);
            PrefabUtility.CreatePrefab(localPath, obj);
            DestroyImmediate(obj);
            Debug.Log("Saved Tileset.");

            _settings.LastOpenedTileset = localPath;
        }
    }

    void LoadTiles()
    {
        string path = EditorUtility.OpenFilePanel("Open Tileset", "Assets", "prefab");
        if (path.Length > 0)
        {

            //Trim path
            string localPath = "Assets" + path.Remove(0, (Application.dataPath).Length);
            Debug.Log("Opening Tileset: " + localPath);

            SEStoredTiles scr = AssetDatabase.LoadAssetAtPath(localPath, typeof(SEStoredTiles)) as SEStoredTiles;
            if (scr)
            {
                TileTex = scr.TileTex;
                TileMat = scr.TileMat;
                TileCollision = scr.TileCollision;
                TileSmartMat = scr.TileSmartMat;
                TileBigMat = scr.TileBigMat;
                TileBigMatSize = scr.TileBigMatSize;
                Debug.Log("Opened Tileset.");

                _settings.LastOpenedTileset = localPath;
            }
            else Debug.Log("Didn't find Tiles file.");
        }
    }

    void LevelParent()
    {

        LoadIcons();

        layerObj = new List<GameObject>();
        layerVisible = new List<bool>();
        levelParent = GameObject.Find("LevelParent");
        if (levelParent)
        {
            levelMFilter = levelParent.GetComponent<MeshFilter>();
            poly = levelMFilter.GetComponent<PolygonCollider2D>();

            for (int i = 0; i < levelParent.transform.childCount; i++)
            {
                layerObj.Add(levelParent.transform.GetChild(i).gameObject);
                layerVisible.Add(true);
                layerDepth.Add(levelParent.transform.GetChild(i).localPosition.z);
            }
            Debug.Log("Detected " + layerObj.Count + " layers.");
        }
        else
        {
            levelParent = new GameObject("LevelParent");
            levelMFilter = levelParent.AddComponent<MeshFilter>();
            levelParent.AddComponent<MeshRenderer>();
        }

        //create layer if not
        if (layerObj.Count == 0)
        {
            GameObject l = new GameObject("Layer 1");
            l.transform.parent = levelParent.transform;
            layerObj.Add(l);
            layerVisible.Add(true);
            layerDepth.Add(0);
        }


    }

    void UnbuildLevel()
    {
        levelParent = GameObject.Find("LevelParent");
        if (levelParent)
        {
            levelMFilter = levelParent.GetComponent<MeshFilter>();
            DestroyImmediate(levelMFilter.sharedMesh);
            poly = levelMFilter.GetComponent<PolygonCollider2D>();
            if (poly) DestroyImmediate(poly);

            int childs = levelParent.transform.childCount;

            Debug.Log("Unbuilding " + childs + " Tiles");
            for (int i = 0; i < childs; i++)
            {
                GameObject gm = levelParent.transform.GetChild(i).gameObject;
                int i2 = 0;
                foreach (GameObject go in layerObj)
                {
                    if (go == gm) gm.SetActive(layerVisible[i2]);
                    i2++;
                }
            }
            layersWindow.Repaint();
        }
        else
        {
            Debug.Log("No Level Found to unbuild");
        }
    }

    void BuildLevel()
    {
        rect = false;
        draw = false;
        //Create Collisions
        BoxCollider[] mc = FindObjectsOfType<BoxCollider>();
        collisions = new List<Vector4>();
        //find edges
        Debug.Log("BUILDING COLLISIONS");
        foreach (BoxCollider m in mc)
        {
            Vector3 t = m.transform.position;
            if (!CheckForTile(t + new Vector3(0, 1, 0))) collisions.Add(new Vector4(t.x - 0.5f, t.y + 0.5f, t.x + 0.5f, t.y + 0.5f));
            if (!CheckForTile(t + new Vector3(1, 0, 0))) collisions.Add(new Vector4(t.x + 0.5f, t.y + 0.5f, t.x + 0.5f, t.y - 0.5f));
            if (!CheckForTile(t + new Vector3(0, -1, 0))) collisions.Add(new Vector4(t.x + 0.5f, t.y - 0.5f, t.x - 0.5f, t.y - 0.5f));
            if (!CheckForTile(t + new Vector3(-1, 0, 0))) collisions.Add(new Vector4(t.x - 0.5f, t.y - 0.5f, t.x - 0.5f, t.y + 0.5f));
        }
        Debug.Log("Found " + collisions.Count + " collisions.");

        //Trace edges & create polygon
        Debug.Log("BEGIN CREATING COLLIDER:");
        poly = levelParent.AddComponent<PolygonCollider2D>();
        _points = new List<Vector2>();
        int path = 0;
        int currEdge = 0;
        Vector4 firstEdge = collisions[0];
        int cnt = collisions.Count;
        for (int i = 0; i < cnt; i++)
        {

            int nextEdge = FindNextEdge(currEdge);

            if (nextEdge != -1)
            {
                if (EdgeDirection(collisions[currEdge]) != EdgeDirection(collisions[nextEdge]))
                {
                    _points.Add(new Vector2(collisions[nextEdge].x, collisions[nextEdge].y));
                    collisions.RemoveAt(currEdge);
                    if (currEdge < nextEdge) currEdge = nextEdge - 1;
                    else currEdge = nextEdge;
                }
                else
                {
                    collisions.RemoveAt(currEdge);
                    if (currEdge < nextEdge) currEdge = nextEdge - 1;
                    else currEdge = nextEdge;
                }
            }
            else
            {
                if (EdgeDirection(collisions[currEdge]) != EdgeDirection(firstEdge)) _points.Add(new Vector2(collisions[currEdge].z, collisions[currEdge].w));
                collisions.RemoveAt(currEdge);
                currEdge = 0;
                if (collisions.Count > 0) firstEdge = collisions[0];
                poly.pathCount = path + 1;
                poly.SetPath(path, _points.ToArray());
                path++;
                _points.Clear();
            }


            if (i == collisions.Count)
            {
                poly.pathCount = path + 1;
                poly.SetPath(path, _points.ToArray());
                path++;
                _points.Clear();
            }

        }
        Debug.Log("Collisions Done");


        //Create Mesh
        Debug.Log("Building Mesh");
        MeshFilter[] meshFilters = levelMFilter.GetComponentsInChildren<MeshFilter>();
        MeshRenderer[] meshRenderers = levelMFilter.GetComponentsInChildren<MeshRenderer>();
        Material[] mats = new Material[50];
        int matCount = 0;

        foreach (MeshRenderer mf in meshRenderers)
        {
            if (!MatExist(mats, mf.sharedMaterial))
            {
                mats[matCount] = mf.sharedMaterial;
                matCount++;
            }
        }

        //Assing materials ro renderer
        MeshRenderer rend = levelParent.GetComponent<MeshRenderer>();
        Material[] shMats = new Material[matCount];
        for (int i = 0; i < matCount; i++)
        {
            shMats[i] = mats[i];
        }
        rend.sharedMaterials = shMats;

        levelMFilter.sharedMesh = new Mesh();

        List<Mesh> tempMeshes = new List<Mesh>();
        for (int o = 0; o < matCount; o++)
        {
            CombineInstance[] tempCombine = new CombineInstance[meshFilters.Length - 1];

            int i = 1;
            int m = 0;
            while (i < meshFilters.Length)
            {
                if (meshRenderers[i].sharedMaterial == mats[o])
                {
                    tempCombine[i - 1].mesh = meshFilters[i].sharedMesh;
                    tempCombine[i - 1].transform = meshFilters[i].transform.localToWorldMatrix;
                    m++;
                }
                i++;
            }
            CombineInstance[] combine = new CombineInstance[m];
            int n = 0;
            foreach (CombineInstance c in tempCombine)
            {
                if (c.mesh != null)
                {
                    combine[n] = c;
                    n++;
                }

            }
            Mesh nm = new Mesh();
            Debug.Log("Combining " + combine.Length + " Sub Meshes");
            nm.CombineMeshes(combine);
            tempMeshes.Add(nm);

            if (o == matCount - 1)
            {
                CombineInstance[] comb = new CombineInstance[tempMeshes.Count];
                int id = 0;
                foreach (Mesh msh in tempMeshes)
                {
                    comb[id].mesh = msh;
                    comb[id].transform = levelParent.transform.localToWorldMatrix;
                    id++;
                }
                Debug.Log("Combining " + comb.Length + " Meshes");
                levelMFilter.sharedMesh.CombineMeshes(comb, false);
                Debug.Log("Mesh Built");
                layersWindow.Repaint();
                foreach (GameObject g in layerObj)
                {
                    g.SetActive(false);
                }
            }
        }
    }

    bool MatExist(Material[] matsArray, Material currentMat)
    {
        foreach (Material m in matsArray)
        {
            if (m == currentMat) return true;
        }
        return false;
    }

    int FindNextEdge(int curEdge)
    {

        for (int i = 0; i < collisions.Count; i++)
        {
            if (collisions[curEdge].z == collisions[i].x && collisions[curEdge].w == collisions[i].y)
            {
                return i;
            }
        }
        return -1;
    }

    Vector2 EdgeDirection(Vector4 edge)
    {

        return (new Vector2(edge.x, edge.y) - new Vector2(edge.z, edge.w)).normalized;

    }

    void DrawDebugLines()
    {
        if (debugLines.Count > 0)
            for (int i = 0; i < debugLines.Count - 1; i++)
            {
                Debug.DrawLine(new Vector2(debugLines[i].x, debugLines[i].y), new Vector2(debugLines[i].z, debugLines[i].w), Color.red);
            }
    }

    bool CompareEdges(Vector4 a, Vector4 b)
    {
        return Vector4.SqrMagnitude(a - b) < 0.0001;
    }

    bool CheckForTile(Vector2 pos)
    {
        Ray cray = new Ray();
        cray.direction = new Vector3(0, 0, 1);
        cray.origin = new Vector3(pos.x, pos.y, -500); ;
        RaycastHit[] chit;
        chit = Physics.RaycastAll(cray, 1000.0f, Physics.AllLayers);
        if (levelParent)
            foreach (RaycastHit h in chit)
            {
                if (h.transform.parent.parent == levelParent.transform && h.transform.GetComponent<BoxCollider>())
                {
                    return true;
                }
                //else return false;
            }
        return false;
    }

    void AdjustUVS(MeshFilter mf, Vector2 size)
    {
        Vector2[] uvs = new Vector2[4];
        uvs[0] = new Vector2((mf.transform.position.x - 0.5f) / size.x, (mf.transform.position.y - 0.5f) / size.y);
        uvs[1] = new Vector2((mf.transform.position.x + 0.5f) / size.x, (mf.transform.position.y + 0.5f) / size.y);
        uvs[2] = new Vector2((mf.transform.position.x + 0.5f) / size.x, (mf.transform.position.y - 0.5f) / size.y);
        uvs[3] = new Vector2((mf.transform.position.x - 0.5f) / size.x, (mf.transform.position.y + 0.5f) / size.y);
        Mesh nm = Instantiate(mf.sharedMesh);

        nm.uv = uvs;

        mf.sharedMesh = nm;


    }

    private Mesh CreateSmartmatMesh(Material mat, Vector2 pos)
    {
        Mesh m = new Mesh();
        Vector3[] verts = new Vector3[16];
        int[] tris = new int[24];
        Vector2[] uvs = new Vector2[16];

        verts[0] = new Vector3(-0.5f, 0.5f, 0);
        verts[1] = new Vector3(0, 0.5f, 0);
        verts[2] = new Vector3(0, 0, 0);
        verts[3] = new Vector3(-0.5f, 0, 0);

        verts[4] = new Vector3(0, 0.5f, 0);
        verts[5] = new Vector3(0.5f, 0.5f, 0);
        verts[6] = new Vector3(0.5f, 0, 0);
        verts[7] = new Vector3(0, 0, 0);

        verts[8] = new Vector3(-0.5f, 0, 0);
        verts[9] = new Vector3(0, 0, 0);
        verts[10] = new Vector3(0, -0.5f, 0);
        verts[11] = new Vector3(-0.5f, -0.5f, 0);

        verts[12] = new Vector3(0, 0, 0);
        verts[13] = new Vector3(0.5f, 0, 0);
        verts[14] = new Vector3(0.5f, -0.5f, 0);
        verts[15] = new Vector3(0, -0.5f, 0);

        tris[0] = 0;
        tris[1] = 1;
        tris[2] = 3;
        tris[3] = 1;
        tris[4] = 2;
        tris[5] = 3;

        tris[6] = 4;
        tris[7] = 5;
        tris[8] = 7;
        tris[9] = 5;
        tris[10] = 6;
        tris[11] = 7;

        tris[12] = 8;
        tris[13] = 9;
        tris[14] = 11;
        tris[15] = 9;
        tris[16] = 10;
        tris[17] = 11;

        tris[18] = 12;
        tris[19] = 13;
        tris[20] = 15;
        tris[21] = 13;
        tris[22] = 14;
        tris[23] = 15;

        Vector2 quad;

        quad = new Vector2(0, 0);
        uvs[0] = quad;
        uvs[1] = quad + new Vector2(0.125f, 0);
        uvs[2] = quad + new Vector2(0.125f, -0.25f);
        uvs[3] = quad + new Vector2(0, -0.25f);

        //quad = new Vector2(0.375f, 1);
        uvs[4] = quad;
        uvs[5] = quad + new Vector2(0.125f, 0);
        uvs[6] = quad + new Vector2(0.125f, -0.25f);
        uvs[7] = quad + new Vector2(0, -0.25f);

        //quad = new Vector2(0, 0.25f);
        uvs[8] = quad;
        uvs[9] = quad + new Vector2(0.125f, 0);
        uvs[10] = quad + new Vector2(0.125f, -0.25f);
        uvs[11] = quad + new Vector2(0, -0.25f);

        //quad = new Vector2(0.375f, 0.25f);
        uvs[12] = quad;
        uvs[13] = quad + new Vector2(0.125f, 0);
        uvs[14] = quad + new Vector2(0.125f, -0.25f);
        uvs[15] = quad + new Vector2(0, -0.25f);


        m.vertices = verts;
        m.triangles = tris;
        m.uv = uvs;
        m.RecalculateNormals();

        Transform[] atiles = SmartMatAdjustUV(m, mat, pos);

        //Adjust adjascent tiles
        foreach (Transform atile in atiles)
        {
            if (atile) SmartMatAdjustUV(atile.GetComponent<MeshFilter>().sharedMesh, mat, new Vector2(atile.transform.position.x, atile.transform.position.y));
        }

        return m;
    }

    void SmartMatAdjustSurroundingUV(Material mat, Vector2 pos)
    {

        Transform[] atiles = SmartMatAdjustUV(null, mat, pos);

        //Adjust adjascent tiles
        foreach (Transform atile in atiles)
        {
            if (atile) SmartMatAdjustUV(atile.GetComponent<MeshFilter>().sharedMesh, mat, new Vector2(atile.transform.position.x, atile.transform.position.y));
        }
    }

    private Transform[] SmartMatAdjustUV(Mesh msh, Material mat, Vector2 pos)
    {


        //Check around tile
        Transform[] check = new Transform[8];
        check[0] = CheckForSMTile(pos + new Vector2(-1, 0), mat);
        check[1] = CheckForSMTile(pos + new Vector2(-1, 1), mat);
        check[2] = CheckForSMTile(pos + new Vector2(0, 1), mat);
        check[3] = CheckForSMTile(pos + new Vector2(1, 1), mat);
        check[4] = CheckForSMTile(pos + new Vector2(1, 0), mat);
        check[5] = CheckForSMTile(pos + new Vector2(1, -1), mat);
        check[6] = CheckForSMTile(pos + new Vector2(0, -1), mat);
        check[7] = CheckForSMTile(pos + new Vector2(-1, -1), mat);

        if (msh == null || msh.vertexCount < 10) return check;

        Vector2 quad = new Vector2(0, 1);
        //First Quarter
        if (!check[0] && !check[2]) quad = new Vector2(0, 1);
        if (check[0] && !check[2]) quad = new Vector2(0.25f, 1);
        if (!check[0] && check[2]) quad = new Vector2(0, 0.75f);
        if (check[0] && check[2]) quad = new Vector2(0.25f, 0.5f);
        if (check[0] && !check[1] && check[2]) quad = new Vector2(0.625f, 0.75f);

        Vector2[] uvs = msh.uv;

        uvs[0] = quad;
        uvs[1] = quad + new Vector2(0.125f, 0); ;
        uvs[2] = quad + new Vector2(0.125f, -0.25f); ;
        uvs[3] = quad + new Vector2(0, -0.25f); ;

        //Second Quarter
        if (!check[2] && !check[4]) quad = new Vector2(0.375f, 1);
        if (check[2] && !check[4]) quad = new Vector2(0.375f, 0.75f);
        if (!check[2] && check[4]) quad = new Vector2(0.125f, 1);
        if (check[2] && check[4]) quad = new Vector2(0.125f, 0.5f);
        if (check[2] && !check[3] && check[4]) quad = new Vector2(0.75f, 0.75f);

        uvs[4] = quad;
        uvs[5] = quad + new Vector2(0.125f, 0); ;
        uvs[6] = quad + new Vector2(0.125f, -0.25f); ;
        uvs[7] = quad + new Vector2(0, -0.25f); ;

        //Third Quarter
        if (!check[4] && !check[6]) quad = new Vector2(0.375f, 0.25f);
        if (check[4] && !check[6]) quad = new Vector2(0.125f, 0.25f);
        if (!check[4] && check[6]) quad = new Vector2(0.375f, 0.5f);
        if (check[4] && check[6]) quad = new Vector2(0.125f, 0.75f);
        if (check[4] && !check[5] && check[6]) quad = new Vector2(0.75f, 0.5f);

        uvs[12] = quad;
        uvs[13] = quad + new Vector2(0.125f, 0); ;
        uvs[14] = quad + new Vector2(0.125f, -0.25f); ;
        uvs[15] = quad + new Vector2(0, -0.25f); ;

        //Fourth Quarter
        if (!check[6] && !check[0]) quad = new Vector2(0, 0.25f);
        if (check[6] && !check[0]) quad = new Vector2(0, 0.5f);
        if (!check[6] && check[0]) quad = new Vector2(0.25f, 0.25f);
        if (check[6] && check[0]) quad = new Vector2(0.25f, 0.75f);
        if (check[6] && !check[7] && check[0]) quad = new Vector2(0.625f, 0.5f);

        uvs[8] = quad;
        uvs[9] = quad + new Vector2(0.125f, 0); ;
        uvs[10] = quad + new Vector2(0.125f, -0.25f); ;
        uvs[11] = quad + new Vector2(0, -0.25f); ;


        msh.uv = uvs;

        return check;
    }

    Transform CheckForSMTile(Vector2 pos, Material mat)
    {
        Ray cray = new Ray();
        cray.direction = new Vector3(0, 0, 1);
        cray.origin = new Vector3(pos.x, pos.y, -500); ;
        RaycastHit chit = new RaycastHit();
        if (Physics.Raycast(cray, out chit, 1000.0f, _lmask) && levelParent)
        {
            if (chit.transform.parent.parent == levelParent.transform && chit.transform.GetComponent<Renderer>().sharedMaterial == mat)
                return chit.transform;
            else return null;
        }
        return null;
    }

    void LoadIcons()
    {
        _visibleIcon = AssetDatabase.LoadAssetAtPath("Assets/Snap Editor/Editor/Icons/visible_icon.png", typeof(Texture)) as Texture;
        _hiddenIcon = AssetDatabase.LoadAssetAtPath("Assets/Snap Editor/Editor/Icons/hidden_icon.png", typeof(Texture)) as Texture;
        _brushIcon = AssetDatabase.LoadAssetAtPath("Assets/Snap Editor/Editor/Icons/brush_icon.png", typeof(Texture)) as Texture;
        _brushActiveIcon = AssetDatabase.LoadAssetAtPath("Assets/Snap Editor/Editor/Icons/brush_active_icon.png", typeof(Texture)) as Texture;
        _collisionIcon = AssetDatabase.LoadAssetAtPath("Assets/Snap Editor/Editor/Icons/collision_icon.png", typeof(Texture)) as Texture;
        _smartMatIcon = AssetDatabase.LoadAssetAtPath("Assets/Snap Editor/Editor/Icons/smartmat_icon.png", typeof(Texture)) as Texture;
        _bigBrushIcon = AssetDatabase.LoadAssetAtPath("Assets/Snap Editor/Editor/Icons/big_brush_icon.png", typeof(Texture)) as Texture;
        _bigBrushActiveIcon = AssetDatabase.LoadAssetAtPath("Assets/Snap Editor/Editor/Icons/big_brush_active_icon.png", typeof(Texture)) as Texture;
        _verBrushIcon = AssetDatabase.LoadAssetAtPath("Assets/Snap Editor/Editor/Icons/ver_brush_icon.png", typeof(Texture)) as Texture;
        _verBrushActiveIcon = AssetDatabase.LoadAssetAtPath("Assets/Snap Editor/Editor/Icons/ver_brush_active_icon.png", typeof(Texture)) as Texture;
        _horBrushIcon = AssetDatabase.LoadAssetAtPath("Assets/Snap Editor/Editor/Icons/hor_brush_icon.png", typeof(Texture)) as Texture;
        _horBrushActiveIcon = AssetDatabase.LoadAssetAtPath("Assets/Snap Editor/Editor/Icons/hor_brush_active_icon.png", typeof(Texture)) as Texture;
        _rectIcon = AssetDatabase.LoadAssetAtPath("Assets/Snap Editor/Editor/Icons/rect_icon.png", typeof(Texture)) as Texture;
        _rectActiveIcon = AssetDatabase.LoadAssetAtPath("Assets/Snap Editor/Editor/Icons/rect_active_icon.png", typeof(Texture)) as Texture;
        _bigMatIcon = AssetDatabase.LoadAssetAtPath("Assets/Snap Editor/Editor/Icons/bigmat_icon.png", typeof(Texture)) as Texture;
    }
}