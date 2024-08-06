using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TowerDefence;
using UnityEditor;
using UnityEngine;

public class ExcelSwarmConverter : EditorWindow
{
    TextAsset _ExcelFile;

    const string INFO_IDENTIFIER = "[info]";
    const string SWARM_START_IDENTIFIER = "[title]";
    const string ASSET_PATH = "Assets/Data/WaveData/ScriptableObjects/";
    const string SCOBJ_ASSET_EXTENTION = ".asset";

void OnGUI()
    {
        ExcelSwarmConverter window = this;
        window.maxSize = new Vector2(500, 250);
        window.minSize = window.maxSize;

        GUILayout.Label($"Select a CSV file from the project window and press 'CONVERT' button to convert the contents into seperate scriptable objects. The objects will be saved to {ASSET_PATH}.",EditorStyles.helpBox);
        GUI.contentColor = Color.yellow;
        GUILayout.Label($" ALL FILES IN {ASSET_PATH} WILL BE DELETED AND REPLACED",EditorStyles.boldLabel);
        GUI.contentColor = Color.white;
        Object[] selection = Selection.GetFiltered(typeof(TextAsset), SelectionMode.Assets);
        GUILayout.Space(10);

        if (selection.Length > 0)
        {
            TextAsset temp = selection[0] as TextAsset;
            if (temp != null && AssetDatabase.GetAssetPath(temp).EndsWith(".csv"))
            {
                _ExcelFile = temp;
            }
        }

        if (_ExcelFile == null)
        { GUILayout.Label("Please select a csv file...", EditorStyles.boldLabel); }
        else
        {
            GUILayout.Label("''" + _ExcelFile.name + "''  is selected", EditorStyles.boldLabel);

            GUILayout.Space(10);
            GUI.contentColor = Color.yellow;

            if (GUILayout.Button("CONVERT"))
            {
                RefreshExcel();
                Unselect();

                RefreshSwarmDatas();
            }

            GUILayout.Space(5);

            GUI.contentColor = new Color(.8f, 0, 0);
            if (GUILayout.Button("UNSELECT"))
            { Unselect(); }
        }

        /*
        GUILayout.Space(10);
        GUI.contentColor = Color.white;
        if (GUILayout.Button("REFRESH"))
        {
            RefreshSwarmDatas();
        }
        */
    }

    [MenuItem("Window/Excel to Swarm Converter")]
    public static void ShowWindow()
    {
        GetWindow<ExcelSwarmConverter>("Excel to Swarm Converter");
    }

    void Unselect()
    {
        Selection.objects = null;
        _ExcelFile = null;
    }

    public void RefreshExcel()
    {
        foreach (var asset in AssetDatabase.FindAssets("", new string[] { ASSET_PATH }))
        {
            string path = AssetDatabase.GUIDToAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
        }

        List<List<List<string>>> WaveDataList = new List<List<List<string>>>();
        List<List<string>> tempWaveData = new List<List<string>>();

        foreach (var item in _ExcelFile.text.Split('\n'))
        {
            tempWaveData.Add(new List<string>());
            string[] currArr = item.Split(',');
            int lastIndex = tempWaveData.Count - 1;

            for (int i = 0; i < currArr.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(currArr[i])) continue;
                tempWaveData[lastIndex].Add(currArr[i]);
            }

            if (tempWaveData[lastIndex].Count == 0)
            { tempWaveData.RemoveAt(lastIndex); }
        }

        List<int> WaveNameIndexes = new List<int>() { 0 };
        int currentIndex = -1;
        do
        {
            currentIndex = tempWaveData.FindIndex(currentIndex + 1, x => x[0].ToLower(new CultureInfo("en-US")) == SWARM_START_IDENTIFIER || x[0].ToLower(new CultureInfo("en-US")) == INFO_IDENTIFIER);
            if (currentIndex >= 0 && WaveNameIndexes.Contains(currentIndex) == false) WaveNameIndexes.Add(currentIndex);
        }
        while (currentIndex >= 0);

        currentIndex = 0;
        int nextIndex = 0;
        int IndextCount = WaveNameIndexes.Count;

        for (int i = 0; i < IndextCount; i++)
        {
            currentIndex = WaveNameIndexes[i];

            if (i + 1 >= IndextCount)
            {
                WaveDataList.Add(tempWaveData.GetRange(currentIndex, tempWaveData.Count - currentIndex));
                break;
            }

            nextIndex = WaveNameIndexes[i + 1];

            WaveDataList.Add(tempWaveData.GetRange(currentIndex, nextIndex - currentIndex));
        }

        EnemyDatabase enemyDB = GLOBAL.GetEnemyDatabase();
        SwarmDatabase swarmDB = GLOBAL.GetSwarmDatabase();
        string sdName = GLOBAL.UnassignedString;

        List<float> defaultEnemyCooldowns = GLOBAL.FailsafeEnemyCooldowns;
        List<int> defaultWaveCooldowns = GLOBAL.FailsafeWaveCooldowns;

        List<float> targetEnemyCooldowns = new List<float>();
        List<int> targetWaveCooldowns = new List<int>();

        swarmDB.DataListAccess = new List<Data<SwarmData>>();

        foreach (var NameSeperation in WaveDataList)
        {
            SwarmData swarmData = ScriptableObject.CreateInstance<SwarmData>();
            swarmData.Waves = new List<S_Wave>();

            targetEnemyCooldowns = defaultEnemyCooldowns;
            targetWaveCooldowns = defaultWaveCooldowns;

            bool isInfo = NameSeperation[0][0].ToLower(new CultureInfo("en-US")) == INFO_IDENTIFIER;

            foreach (var RowSeperation in NameSeperation)
            {
                if (RowSeperation[0].Contains("["))
                {
                    HandleIdentifier(RowSeperation);
                    continue;
                }

                if (isInfo) continue;

                S_Wave currWave = new S_Wave();
                currWave.Lanes = new List<S_LaneGroup>();

                foreach (var CommaSeperation in RowSeperation)
                {
                    EnemyData eData = enemyDB.GetDataByID(CommaSeperation.Trim());
                    if (eData == null)
                    {
                        Debug.LogError(CommaSeperation + " is not an enemy");
                        continue;
                    }

                    S_LaneGroup lGroup = new S_LaneGroup();

                    S_EnemyWithCount ewc = new S_EnemyWithCount();
                    ewc.Count = 1;
                    ewc.Enemy = eData;

                    lGroup.Enemies = new List<S_EnemyWithCount>() { ewc };

                    currWave.Lanes.Add(lGroup);
                }

                swarmData.Waves.Add(currWave);
            }

            if (isInfo) continue;

            string fullPath = ASSET_PATH + sdName + SCOBJ_ASSET_EXTENTION;

            swarmData.SetNameAndID(sdName);
            swarmData.DefaultEnemyCooldowns = targetEnemyCooldowns;
            swarmData.DefaultUntillNextWave = targetWaveCooldowns;
            AssetDatabase.CreateAsset(swarmData, fullPath);
            EditorUtility.SetDirty(AssetDatabase.LoadAssetAtPath<SwarmData>(fullPath));

            EditorUtility.SetDirty(swarmDB);//changes will not be saved if this is not set dirty
            swarmDB.DataListAccess.Add(AssetDatabase.LoadAssetAtPath<SwarmData>(fullPath));
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        //local method
        void HandleIdentifier(List<string> rowSeperation)
        {
            if (rowSeperation.Count <= 1) return;

            string identifier = rowSeperation[0];

            int startIndex = identifier.IndexOf("[");
            int endIndex = identifier.IndexOf("]");
            if (startIndex == -1 || endIndex == -1) return;
            string message = identifier.Substring(startIndex + 1, endIndex - startIndex - 1);
            message = message.ToLower(new CultureInfo("en-US"));

            string firstCollumn = rowSeperation[1];
            switch (message)
            {
                case "title":
                    sdName = firstCollumn;
                    break;

                case "wave-cooldown":

                    List<int> ints = new List<int>();
                    foreach (var item in firstCollumn.Split('/'))
                    {
                        string target = item.Replace(".", ",");

                        float currFloat = 0;
                        if (float.TryParse(target, out currFloat)) ints.Add(Mathf.RoundToInt(currFloat));
                    }

                    if (ints != null && ints.Count > 0) targetWaveCooldowns = ints;

                    break;

                case "enemy-cooldown":

                    List<float> floats = new List<float>();
                    foreach (var item in firstCollumn.Split('/'))
                    {
                        string target = item.Replace(".", ",");

                        float currFloat = 0;
                        if (float.TryParse(target, out currFloat)) floats.Add(currFloat);
                    }

                    if (floats != null && floats.Count > 0) targetEnemyCooldowns = floats;

                    break;

                case "default-wave-cooldown":

                    ints = new List<int>();
                    foreach (var item in firstCollumn.Split('/'))
                    {
                        string target = item.Replace(".", ",");

                        float currFloat = 0;
                        if (float.TryParse(target, out currFloat)) ints.Add(Mathf.RoundToInt(currFloat));
                    }

                    if (ints != null && ints.Count > 0) defaultWaveCooldowns = ints;

                    break;

                case "default-enemy-cooldown":

                    floats = new List<float>();
                    foreach (var item in firstCollumn.Split('/'))
                    {
                        string target = item.Replace(".", ",");

                        float currFloat = 0;
                        if (float.TryParse(target, out currFloat)) floats.Add(currFloat);
                    }

                    if (floats != null && floats.Count > 0) defaultEnemyCooldowns = floats;

                    break;
            }
        }
    }

    void RefreshSwarmDatas()
    {
        foreach (var asset in AssetDatabase.FindAssets("", new string[] { ASSET_PATH }))
        {
            string path = AssetDatabase.GUIDToAssetPath(asset);
            SwarmData data = AssetDatabase.LoadAssetAtPath<SwarmData>(path);
            if (data == null) continue;
            EditorUtility.SetDirty(data);
            data.Refresh();
        }
    }
}
