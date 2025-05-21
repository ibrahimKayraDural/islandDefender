using Overworld;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using TowerDefence;
using UnityEngine;
using UpgradeSystem;

namespace SaveSystem
{
    public class SaveManager : MonoBehaviour
    {
        static string _fullPath => Path.Combine(_folderPath, FILE_NAME);
        static string _folderPath => Path.Combine(UnityEngine.Application.persistentDataPath, FOLDER_NAME);

        const string FOLDER_NAME = "Saves";
        const string FILE_NAME = "savedata.txt";

        public static SaveManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _isCreatingInstance = true;
                    _instance = new GameObject("Save Manager Instance", typeof(SaveManager)).GetComponent<SaveManager>();
                    _instance.Initialize();
                    _isCreatingInstance = false;
                }

                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }
        static SaveManager _instance;
        static bool _isCreatingInstance = false;

        public SaveData CurrentSave => _currentSave;

        [SerializeField] SaveData _currentSave = new();

        bool _isInitialized = false;

        void Awake()
        {
            if (_isCreatingInstance) return;

            if (Instance == null)
            {
                Instance = this;
                Initialize();
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        void Initialize()
        {
            DontDestroyOnLoad(gameObject);

            ReadSaveData();

            _isInitialized = true;
        }

        void OnDestroy()//Is actually called when quitting the app
        {
            if (_isInitialized == false) return;

            WriteToSaveData();
        }

        [ContextMenu("Read")]
        void ReadSaveData()
        {
            if (File.Exists(_fullPath) == false) return;

            using (FileStream stream = new(_fullPath, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new();
                _currentSave = (SaveData)binaryFormatter.Deserialize(stream);
            }
        }

        [ContextMenu("Write")]
        void WriteToSaveData()
        {
            if (Directory.Exists(_folderPath) == false)
            { Directory.CreateDirectory(_folderPath); }

            using (FileStream stream = new(_fullPath, FileMode.Create))
            {
                BinaryFormatter binaryFormatter = new();
                binaryFormatter.Serialize(stream, _currentSave);
            }
        }

        [ContextMenu("Delete Save File")]

        [UnityEditor.MenuItem("Save System/Delete Save File")]
        public static void DeleteSaveFile()
        {
            File.Delete(_fullPath);
            Debug.Log($"EDITORDEBUG -> Save file at {_fullPath} is deleted.");
        }

        /// <summary>
        /// Replaces player inventory in the current save. 
        /// Is not saved to file before the WriteToSaveData function is called
        /// </summary>
        /// <param name="saveData">Save data</param>
        public void ReplacePlayerInventory(List<InventoryItemSaveData> saveData)
        {
            _currentSave.PlayerInventory = saveData;
        }

        /// <summary>
        /// Replaces base inventory in the current save. 
        /// Is not saved to file before the WriteToSaveData function is called
        /// </summary>
        /// <param name="saveData">Save data which includes ID of Resource Data and Count</param>
        public void ReplaceBaseInventory(List<ItemWithCount> saveData)
        {
            _currentSave.BaseInventory = saveData;
        }

        /// <summary>
        /// Replaces Placed Turrets in the current save. 
        /// Is not saved to file before the WriteToSaveData function is called
        /// </summary>
        /// <param name="saveData">Save data which includes Turret data</param>
        public void ReplaceTurrets(List<TurretSaveData> saveData)
        {
            _currentSave.Turrets = saveData;
        }

        /// <summary>
        /// Replaces Unlocked Turrets in the current save. 
        /// Is not saved to file before the WriteToSaveData function is called
        /// </summary>
        /// <param name="turrets">Turrets to save</param>
        public void ReplaceUnlockedTurrets(List<TurretData> turrets)
        {
            _currentSave.UnlockedTurretIDs = turrets.Select(x => x.ID).ToList();
        }

        /// <summary>
        /// Replaces Unlocked Tools in the current save. 
        /// Is not saved to file before the WriteToSaveData function is called
        /// </summary>
        /// <param name="tools">Tools to save</param>
        public void ReplaceUnlockedTools(List<ToolData> tools)
        {
            _currentSave.UnlockedToolIDs = tools.Select(x => x.ID).ToList();
        }

        /// <summary>
        /// Replaces Unlocked Enemies in the current save. 
        /// Is not saved to file before the WriteToSaveData function is called
        /// </summary>
        /// <param name="enemies">Enemies to save</param>
        public void ReplaceUnlockedEnemies(List<EnemyData> enemies)
        {
            _currentSave.UnlockedEnemyIDs = enemies.Select(x => x.ID).ToList();
        }

        /// <summary>
        /// Replaces Active Tools in the current save. 
        /// Is not saved to file before the WriteToSaveData function is called
        /// </summary>
        /// <param name="tools">active tools to save</param>
        public void ReplaceActiveTools(List<Tool> tools)
        {
            _currentSave.ActiveToolIDs = tools.Select(x => x?.Data?.ID).Where(x => x != null).ToList();
        }

        /// <summary>
        /// Adds a new grid or replaces the existing one. 
        /// Is not saved to file before the WriteToSaveData function is called
        /// </summary>
        /// <param name="tileManagerID">ID of the TileManager which holds and controls the TileMaps</param>
        /// <param name="deletedTileDatas">Actual data to save</param>
        public void AddOrReplaceGrid(string tileManagerID, List<DeletedTileData> deletedTileDatas)
        {
            var grids = _currentSave.Grids;
            var gridIdx = grids.FindIndex(x => x.TileManagerID == tileManagerID);
            var newGrid = new GridSaveData(tileManagerID, deletedTileDatas);

            if (gridIdx == -1) grids.Add(newGrid);
            else grids[gridIdx] = newGrid;
        }

        /// <summary>
        /// Adds a new upgradeable or replaces the existing one. 
        /// Is not saved to file before the WriteToSaveData function is called
        /// </summary>
        /// <param name="guid">Unique id of the upradeable</param>
        /// <param name="data">Actual data to save</param>
        public void AddOrReplaceUpgradeable(string guid, List<UpgradeData> datas)
        {
            if (guid == null) return;

            var i = _currentSave.Upgrades.FindIndex(x => x.GUID == guid);
            var upgrades = new UpgradeableData(guid, datas);

            if (i == -1) _currentSave.Upgrades.Add(upgrades);
            else _currentSave.Upgrades[i] = upgrades;
        }

        /// <summary>
        /// Adds or replaces an integer to the save file
        /// Is not saved to file before the WriteToSaveData function is called
        /// </summary>
        /// <param name="id">An ID to hande the integer value</param>
        /// <param name="integer">The actual value to save</param>
        public void AddOrReplaceSavedInteger(string id, int integer)
        {
            if (id == null) return;

            int index = _currentSave.SavedIntegers.FindIndex(x => x.ID == id);

            if (index == -1) _currentSave.SavedIntegers.Add(new(id, integer));
            else _currentSave.SavedIntegers[index] = new(id, integer);
        }

        /// <summary>
        /// Tries to remove an integer with the id
        /// Is not saved to file before the WriteToSaveData function is called
        /// </summary>
        /// <param name="id">ID that the integer was saved with</param>
        public bool TryRemoveSavedInteger(string id)
        {
            if (id == null) return false;

            int index = _currentSave.SavedIntegers.FindIndex(x => x.ID == id);
            if (index == -1) return false;

            _currentSave.SavedIntegers.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Adds or replaces an object in the save file
        /// Is not saved to file before the WriteToSaveData function is called
        /// NOTE THAT OBJECT MUST BE SERIALIZABLE TO BE SAVED
        /// </summary>
        /// <param name="id">An ID to hande the integer value</param>
        /// <param name="object">The actual value to save</param>
        public void AddOrReplaceSavedObject(string id, object @object)
        {
            if (id == null) return;

            int index = _currentSave.SavedObjects.FindIndex(x => x.ID == id);

            if (index == -1) _currentSave.SavedObjects.Add(new(id, @object));
            else _currentSave.SavedObjects[index] = new(id, @object);
        }

        /// <summary>
        /// Tries to remove an object with the id
        /// Is not saved to file before the WriteToSaveData function is called
        /// </summary>
        /// <param name="id">ID that the object was saved with</param>
        public bool TryRemoveSavedObject(string id)
        {
            if (id == null) return false;

            int index = _currentSave.SavedObjects.FindIndex(x => x.ID == id);
            if (index == -1) return false;

            _currentSave.SavedObjects.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Sets core manager state. Is not saved to file before the WriteToSaveData function is called
        /// </summary>
        /// <param name="cores">All the cores in the manager</param>
        /// <param name="selectedCore">ID of the selected core of the manager. Can be null.</param>
        public void SetCoreState(List<string> cores, string selectedCore)
        {
            _currentSave.CoreState = new(cores, selectedCore);
        }

        /// <summary>
        /// Adds or replaces a crack reward index to the save file
        /// Is not saved to file before the WriteToSaveData function is called
        /// </summary>
        /// <param name="id">An ID to determine crack instance</param>
        /// <param name="integer">The actual value to save</param>
        public void AddOrReplaceCrackIndex(string id, int integer)
        {
            if (id == null)
            {
                Debug.LogError("Crack id can not be null");
                return;
            }

            int index = _currentSave.CrackIndexes.FindIndex(x => x.ID == id);

            if (index == -1) _currentSave.CrackIndexes.Add(new(id, integer));
            else _currentSave.CrackIndexes[index] = new(id, integer);
        }

        /// <summary>
        /// Deletes all crack indexes so they can reroll the reward
        /// </summary>
        public void DeleteCrackIndexes()
        {
            _currentSave.CrackIndexes = new();
        }
    }

    [System.Serializable]
    public class SaveData
    {
        public List<InventoryItemSaveData> PlayerInventory = null;
        public List<ItemWithCount> BaseInventory = null;
        public List<TurretSaveData> Turrets = null;
        public List<string> UnlockedEnemyIDs = null;
        public List<string> UnlockedToolIDs = null;
        public List<string> UnlockedTurretIDs = null;
        public List<string> ActiveToolIDs = null;
        public List<GridSaveData> Grids = new();
        public List<IntWithID> SavedIntegers = new();
        public List<ObjectWithID> SavedObjects = new();
        public List<IntWithID> CrackIndexes = new();
        public CoreManagerState CoreState = null;
        public List<UpgradeableData> Upgrades
        {
            get
            {
                if (_upgrades == null) _upgrades = new();
                return _upgrades;
            }
            set { _upgrades = value; }
        }

        List<UpgradeableData> _upgrades = new();

        public SaveData() { }

        public UpgradeableData GetUpgradesOfGUID(string guid) => Upgrades.Find(x => x.GUID == guid);
    }

    [System.Serializable]
    public class InventoryItemSaveData
    {
        public string TypeID = null;
        public string ID = null;
        public int Count = -1;

        public InventoryItemSaveData(string typeID, string id, int count)
        {
            TypeID = typeID;
            ID = id;
            Count = count;
        }
    }

    [System.Serializable]
    public class ItemWithCount
    {
        public string ID = null;
        public int Count = -1;

        public ItemWithCount(string id, int count)
        {
            ID = id;
            Count = count;
        }
    }

    [System.Serializable]
    public class TurretSaveData
    {
        public string ID = null;
        public int PosX = -1;
        public int PosY = -1;

        public TurretSaveData(string id, int posX, int posY)
        {
            ID = id;
            PosX = posX;
            PosY = posY;
        }
    }
    [System.Serializable]
    public class GridSaveData
    {
        public string TileManagerID = null;
        public List<DeletedTileData> DeletedTiles = null;

        public GridSaveData(string tileManagerID, List<DeletedTileData> deletedTiles)
        {
            TileManagerID = tileManagerID;
            DeletedTiles = deletedTiles;
        }
    }
    [System.Serializable]
    public class DeletedTileData
    {
        public Vector3Int TileCellPosition => new Vector3Int(TilePosX, TilePosY, TilePosZ);

        public string TilemapID = null;
        public int TilePosX, TilePosY, TilePosZ;

        public DeletedTileData(string tilemapID, Vector3Int tileCellPosition)
        {
            TilemapID = tilemapID;
            TilePosX = tileCellPosition.x;
            TilePosY = tileCellPosition.y;
            TilePosZ = tileCellPosition.z;
        }
    }
    [System.Serializable]
    public class UpgradeableData
    {
        public string GUID = null;
        public List<string> UpgradeIDs = null;

        public UpgradeableData(string guid, List<string> upgradeIDs)
        {
            GUID = guid;
            UpgradeIDs = upgradeIDs;
        }
        public UpgradeableData(string guid, List<UpgradeData> upgrades)
        {
            GUID = guid;
            UpgradeIDs = new();
            foreach (var upgrade in upgrades)
            {
                if (upgrade == null) continue;
                UpgradeIDs.Add(upgrade.ID);
            }
        }
    }
    [System.Serializable]
    public class IntWithID
    {
        public string ID;
        public int Value;

        public IntWithID(string id, int value)
        {
            ID = id;
            Value = value;
        }
    }
    [System.Serializable]
    public class ObjectWithID
    {
        public string ID;
        public object Value;

        public ObjectWithID(string id, object value)
        {
            ID = id;
            Value = value;
        }
    }
    [System.Serializable]
    public class CoreManagerState
    {
        public List<string> CoreIDs = new();
        public string SelectedCore = null;

        public CoreManagerState(List<string> coreIDs, string selectedCore)
        {
            CoreIDs = coreIDs;
            SelectedCore = selectedCore;
        }
    }
}
