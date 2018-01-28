using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class TPWindow : EditorWindow
{

    private bool _creatingLayer;
    private bool _deletingLayer;
    private bool _renamingLayer;

    void OnGUI()
    {
        if (!SnapEditor.levelParent)
        {
            return;
        }
        GUI.backgroundColor = Color.grey;
        EditorGUILayout.BeginVertical("Box");
        GUI.backgroundColor = Color.white;
        if (!SnapEditor.poly)
        {
            if (!_creatingLayer && !_deletingLayer)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("+", GUILayout.Height(32), GUILayout.Width(32)))
                {
                    _creatingLayer = true;
                    _renamingLayer = false;
                    SnapEditor._newLayername = "Layer " + SnapEditor.layerObj.Count;
                }

                if (GUILayout.Button("-", GUILayout.Height(32), GUILayout.Width(32)))
                {
                    _deletingLayer = true;
                    _renamingLayer = false;
                }

                if (!_renamingLayer && GUILayout.Button("Rename", GUILayout.Height(32), GUILayout.Width(64)))
                {
                    _renamingLayer = true;
                }

                if (_renamingLayer && GUILayout.Button("Done", GUILayout.Height(32), GUILayout.Width(64)))
                {
                    _renamingLayer = false;
                }

                EditorGUILayout.EndHorizontal();
            }
            
            if (_creatingLayer)
            {
                GUILayout.Label("Create new Layer:");
                SnapEditor._newLayername = GUILayout.TextField(SnapEditor._newLayername);
                if (GUILayout.Button("Create", GUILayout.Height(32)))
                {
                    GameObject l = new GameObject(SnapEditor._newLayername);
                    l.transform.parent = SnapEditor.levelParent.transform;
                    SnapEditor.layerObj.Add(l);
                    SnapEditor.layerVisible.Add(true);
                    SnapEditor.layerDepth.Add(0);
                    _creatingLayer = false;
                }
                if (GUILayout.Button("Cancel", GUILayout.Height(32))) _creatingLayer = false;
            }

            if (_deletingLayer)
            {
                GUILayout.Label("Delete layer '" + SnapEditor.layerObj[SnapEditor.layerActive].name + "' ?");
                if (GUILayout.Button("Delete", GUILayout.Height(32)))
                {
                    DestroyImmediate(SnapEditor.layerObj[SnapEditor.layerActive]);
                    SnapEditor.layerObj.RemoveAt(SnapEditor.layerActive);
                    SnapEditor.layerDepth.RemoveAt(SnapEditor.layerActive);
                    SnapEditor.layerVisible.RemoveAt(SnapEditor.layerActive);
                    _deletingLayer = false;
                }
                if (GUILayout.Button("Cancel", GUILayout.Height(32))) _deletingLayer = false;
            }

            int i = 0;

            EditorGUILayout.Separator();
            if (!_creatingLayer && !_deletingLayer)
                foreach (GameObject g in SnapEditor.layerObj)
                {
                    
                    GUIStyle st = new GUIStyle("Box");
                    st.alignment = TextAnchor.MiddleCenter;
                    st.margin = new RectOffset(0, 0, 0, 0);

                    if (SnapEditor.layerActive == i) GUI.backgroundColor = SnapEditor.SelectionColor * 0.9f;
                    else GUI.backgroundColor = Color.white*0.9f;
                    GUILayout.BeginHorizontal();
                    if (SnapEditor.layerVisible[i]) SnapEditor.layerVisible[i] = GUILayout.Toggle(SnapEditor.layerVisible[i], SnapEditor._visibleIcon, st, GUILayout.Width(32), GUILayout.Height(32));
                    else SnapEditor.layerVisible[i] = GUILayout.Toggle(SnapEditor.layerVisible[i], SnapEditor._hiddenIcon, st, GUILayout.Width(32), GUILayout.Height(32));
                    g.SetActive(SnapEditor.layerVisible[i]);
                    //layerVisible[i] = GUILayout.Toggle(layerVisible[i], g.name, "Button", GUILayout.Height(24));
                    SnapEditor.layerDepth[i] = EditorGUILayout.FloatField(SnapEditor.layerDepth[i],st, GUILayout.Width(48), GUILayout.Height(32));
                    Vector3 pos = SnapEditor.layerObj[i].transform.localPosition;
                    SnapEditor.layerObj[i].transform.localPosition = new Vector3(pos.x,pos.y, SnapEditor.layerDepth[i]);

                    if (SnapEditor.layerActive == i) GUI.backgroundColor = SnapEditor.SelectionColor;
                    else GUI.backgroundColor = Color.white;
                    st.stretchWidth = true;
                    if (!_renamingLayer || SnapEditor.layerActive != i)
                    {
                        if (GUILayout.Toggle(SnapEditor.layerActive == i, g.name, st, GUILayout.Height(32), GUILayout.MinWidth(64)))
                        {
                            SnapEditor.layerActive = i;

                            for (int i2 = 0; i2 < SnapEditor.layerObj.Count; i2++)
                            {
                                int l = 0;
                                if (i != i2) l = 2;

                                for  (int i3 = 0; i3 < SnapEditor.layerObj[i2].transform.childCount; i3++)
                                {
                                    SnapEditor.layerObj[i2].transform.GetChild(i3).gameObject.layer = l;
                                }
                            }

                            _renamingLayer = false;
                        }
                    }else
                    {
                        st.fontStyle = FontStyle.Italic;
                        SnapEditor.layerObj[SnapEditor.layerActive].name = GUILayout.TextField(SnapEditor.layerObj[SnapEditor.layerActive].name,st, GUILayout.Height(32), GUILayout.MinWidth(64));
                    }
                    GUILayout.EndHorizontal();
                    GUI.backgroundColor = Color.white;
                    i++;

                }
        }else
        {
            GUI.color = Color.black;
            GUILayout.Label("Unbuild level to edit it.", EditorStyles.centeredGreyMiniLabel);
            GUI.color = Color.white;
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Separator();
        
    }
}
