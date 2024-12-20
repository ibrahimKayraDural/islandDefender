using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

        GUILayout.Label($"Select a CSV file from the project window and press 'CONVERT' button to convert the contents into seperate scriptable objects. The objects will be saved to {ASSET_PATH}.", EditorStyles.helpBox);
        GUI.contentColor = Color.yellow;
        GUILayout.Label($" ALL FILES IN {ASSET_PATH} WILL BE DELETED AND REPLACED", EditorStyles.boldLabel);
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
        #region Writing to Wildcard Info
        string space = " ---------------------------- ";
        string wildcardIds = "                    -- DIFFICULTY --\n\n";
        foreach (var pair in GLOBAL.EnemyDifficultyIDs)
        {
            string pairValue = pair.Value.ToString();
            pairValue = space.Length > pairValue.Length ? pairValue + space.Remove(0, pairValue.Length) : pairValue + "  ";
            wildcardIds += $"{pairValue}''{pair.Key}''\n";
        }
        wildcardIds += "\n\n                    -- RANGE --\n\n";
        foreach (var pair in GLOBAL.EnemyRangeIDs)
        {
            string pairValue = pair.Value.ToString();
            pairValue = space.Length > pairValue.Length ? pairValue + space.Remove(0, pairValue.Length) : pairValue + "  ";
            wildcardIds += $"{pairValue}''{pair.Key}''\n";
        }
        wildcardIds += "\n\n                    -- ENEMY TYPES --\n\n";
        foreach (var pair in GLOBAL.EnemyTypeIDs)
        {
            string pairValue = pair.Value.ToString();
            pairValue = space.Length > pairValue.Length ? pairValue + space.Remove(0, pairValue.Length) : pairValue + "  ";
            wildcardIds += $"{pairValue}''{pair.Key}''\n";
        }

        File.WriteAllText(@"Assets\Data\WaveData\info\WildcardInfo.txt", wildcardIds);
        #endregion

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

        List<float> defaultEnemyCooldowns = GLOBAL.FailsafeEnemyCooldowns;
        List<int> defaultWaveCooldowns = GLOBAL.FailsafeWaveCooldowns;

        List<float> targetEnemyCooldowns = new List<float>();
        List<int> targetWaveCooldowns = new List<int>();

        swarmDB.DataListAccess = new List<Data<SwarmData>>();

        int enemyCount;
        string sdName = GLOBAL.UnassignedString;

        foreach (var NameSeperation in WaveDataList)
        {
            SwarmDataValueContainer swarmDataVC = new SwarmDataValueContainer();
            swarmDataVC.Waves = new List<S_Wave>();

            targetEnemyCooldowns = defaultEnemyCooldowns;
            targetWaveCooldowns = defaultWaveCooldowns;

            bool isInfo = NameSeperation[0][0].ToLower(new CultureInfo("en-US")) == INFO_IDENTIFIER;

            foreach (var RowSeperation in NameSeperation)
            {
                if (RowSeperation[0].Contains("["))
                {
                    HandleParameter(RowSeperation);
                    continue;
                }

                if (isInfo) continue;

                S_Wave currWave = new S_Wave();
                currWave.Lanes = new List<S_LaneGroup>();

                foreach (var CommaSeperation in RowSeperation)
                {
                    enemyCount = 1;
                    string enemyID = HandleOption(CommaSeperation);
                    EnemyData eData = null;

                    bool isWildCard = enemyID.StartsWith("wc-");

                    if (isWildCard == false)
                    {
                        eData = enemyDB.GetDataByID(enemyID);
                        if (eData == null)
                        {
                            Debug.LogError(CommaSeperation + " is not an enemy");
                            continue;
                        }
                    }

                    S_LaneGroup lGroup = new S_LaneGroup();
                    if (isWildCard)
                    {
                        string[] messages = enemyID.Split("-");
                        S_EnemyWithCount ewc = new S_EnemyWithCount(messages[1], messages[2], enemyCount);

                        lGroup.Enemies = new List<S_EnemyWithCount>() { ewc };
                    }
                    else
                    {
                        S_EnemyWithCount ewc = new S_EnemyWithCount(eData, enemyCount);

                        lGroup.Enemies = new List<S_EnemyWithCount>() { ewc };
                    }

                    currWave.Lanes.Add(lGroup);
                }

                swarmDataVC.Waves.Add(currWave);
            }

            if (isInfo) continue;

            string fullPath = ASSET_PATH + sdName + SCOBJ_ASSET_EXTENTION;

            SwarmData swarmData = ScriptableObject.CreateInstance<SwarmData>();
            EditorUtility.SetDirty(swarmData);
            swarmData.SetSwarmValues(swarmDataVC, false);
            swarmData.SetNameAndID(sdName);
            swarmData.DefaultEnemyCooldowns = targetEnemyCooldowns;
            swarmData.DefaultWaveCooldowns = targetWaveCooldowns;
            AssetDatabase.CreateAsset(swarmData, fullPath);
            EditorUtility.SetDirty(AssetDatabase.LoadAssetAtPath<SwarmData>(fullPath));

            EditorUtility.SetDirty(swarmDB);//changes will not be saved if this is not set dirty
            swarmDB.DataListAccess.Add(AssetDatabase.LoadAssetAtPath<SwarmData>(fullPath));


        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        //local methods
        void HandleParameter(List<string> rowSeperation)
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

        //handles options given with "<...>" and "=" symbol.
        //returns cleaned enemyId.
        string HandleOption(string enemyId)
        {
            enemyId = enemyId.Trim();
            int startIndex = enemyId.IndexOf("<");
            int endIndex = enemyId.IndexOf(">");
            if (startIndex == -1 || endIndex == -1) return enemyId;

            string message = enemyId.Substring(startIndex + 1, endIndex - startIndex - 1);
            message = message.ToLower(new CultureInfo("en-US"));
            enemyId = enemyId.Remove(startIndex, endIndex - startIndex + 1).Trim();

            int equalsIndex = message.IndexOf("=");
            string valueStr = GLOBAL.UnassignedString;
            if (equalsIndex > 0 && equalsIndex < message.Length)
            {
                valueStr = message.Substring(equalsIndex + 1, message.Length - (equalsIndex + 1));
                message = message.Substring(0, equalsIndex);
            }

            switch (message)
            {
                case "count":
                    if (int.TryParse(valueStr, out int temp))
                    { enemyCount = Mathf.Clamp(temp, 1, 99); }
                    else
                    { Debug.Log(valueStr + " is not a valid number"); }
                    break;

                case "difficulty":
                case "range":
                case "type":
                    return $"wc-{message}-{valueStr}";

                default:
                    Debug.Log(message + " is not defined");
                    break;
            }

            return enemyId;
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
