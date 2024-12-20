using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FBXMeshExporter : EditorWindow
{
    GameObject _selectedObject;
    List<KeyValuePair<string, Mesh>> _namedMeshes = new List<KeyValuePair<string, Mesh>>();
    string folder = "";
    bool areNamesListed;

    const string SAVEPATH = "Assets/Tilemap/";
    const string PREFABPATH = "Assets/Tilemap/ForEditor/TileFather.prefab";

    [MenuItem("Window/FBX Mesh Exporter")]
    public static void ShowWindow()
    {
        GetWindow<FBXMeshExporter>("FBX Mesh Exporter");
    }

    void OnGUI()
    {
        FBXMeshExporter window = this;
        window.maxSize = new Vector2(500, 250);
        window.minSize = window.maxSize;

        GUILayout.Label($"Select an FBX to filter its meshes", EditorStyles.helpBox);
        Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets);
        GUILayout.Space(10);

        if (selection.Length > 0)
        {
            GameObject temp = selection[0] as GameObject;
            if (temp != null && AssetDatabase.GetAssetPath(temp).EndsWith(".fbx"))
            {
                _selectedObject = temp;
                _namedMeshes = new List<KeyValuePair<string, Mesh>>();
                var meshFilters = _selectedObject.GetComponentsInChildren<MeshFilter>();
                foreach (var mf in meshFilters)
                {
                    _namedMeshes.Add(new KeyValuePair<string, Mesh>(mf.gameObject.name, mf.sharedMesh));
                }
            }
        }

        if (_selectedObject == null)
        { GUILayout.Label("Please select an FBX file...", EditorStyles.boldLabel); }
        else
        {
            GUILayout.Label("''" + _selectedObject.name + "''  is selected", EditorStyles.boldLabel);
            GUI.contentColor = Color.yellow;
            if (_namedMeshes != null && _namedMeshes.Count > 0) GUILayout.Label(_namedMeshes.Count + " meshes were found.", EditorStyles.boldLabel);
            else GUILayout.Label("No meshes were found.", EditorStyles.boldLabel);

            GUILayout.Space(10);
            GUI.contentColor = Color.yellow;

            if (_namedMeshes.Count > 0)
            {
                areNamesListed = EditorGUILayout.Toggle("List names? ", areNamesListed);

                if (areNamesListed)
                {
                    GUI.contentColor = Color.gray;
                    foreach (var m in _namedMeshes)
                    {
                        GUILayout.Label(m.Key);
                    }
                    GUILayout.Space(10);
                }

                GUI.contentColor = Color.white;
                folder = EditorGUILayout.TextField("Folder name: ", folder);

                string fullPath = SAVEPATH + folder;

                if (IsFolderValid() == false)
                {
                    GUI.contentColor = new Color(.8f, 0, 0);
                    GUILayout.Label("No valid folder was found at ''" + fullPath + "''", EditorStyles.boldLabel);
                    goto Unselect;
                }

                if (GUILayout.Button("Make Prefabs"))
                {
                    Object originalPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(PREFABPATH, typeof(GameObject));

                    string tileFatherPath = AssetDatabase.GenerateUniqueAssetPath(fullPath + $"/_TileFather_{_selectedObject.name}.prefab");

                    GameObject objSource = PrefabUtility.InstantiatePrefab(originalPrefab) as GameObject;
                    PrefabUtility.UnpackPrefabInstance(objSource, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                    GameObject prefabFather = PrefabUtility.SaveAsPrefabAsset(objSource, tileFatherPath);
                    DestroyImmediate(objSource);
                    AssetDatabase.SaveAssets();

                    foreach (var m in _namedMeshes)
                    {
                        GameObject fatherGO = PrefabUtility.InstantiatePrefab(prefabFather) as GameObject;

                        if (fatherGO.TryGetComponent(out MeshFilter mf)) mf.sharedMesh = m.Value;

                        string newPathName = AssetDatabase.GenerateUniqueAssetPath(fullPath + $"/{m.Key}.prefab");
                        PrefabUtility.SaveAsPrefabAsset(fatherGO, newPathName);
                        DestroyImmediate(fatherGO);
                        AssetDatabase.SaveAssets();
                    }
                }
            }

            Unselect:;
            GUILayout.Space(5);

            GUI.contentColor = new Color(.8f, 0, 0);
            if (GUILayout.Button("UNSELECT"))
            { Unselect(); }
        }
    }

    void Unselect()
    {
        Selection.objects = null;
        _selectedObject = null;
        _namedMeshes = new List<KeyValuePair<string, Mesh>>();
        folder = "";
    }

    bool IsFolderValid() => string.IsNullOrWhiteSpace(folder) == false && AssetDatabase.IsValidFolder(SAVEPATH + folder);
}
