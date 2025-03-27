using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.WSA;

namespace SaveSystem
{
    public class SaveManager : MonoBehaviour
    {
        string _fullPath;
        string _folderPath;

        readonly string FolderName = "Saves";
        readonly string FileName = "savedata.txt";

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

        [System.Serializable]
        public class SaveData
        {
            public List<InventoryItemSaveData> PlayerInventory = null;
            public List<ItemWithCount> BaseInventory = null;
            public object Turrets = null;
            public object Grid = null;

            public SaveData() { }

            public SaveData(List<InventoryItemSaveData> playerInventory, List<ItemWithCount> baseInventory, object turrets, object grid)
            {
                PlayerInventory = playerInventory;
                BaseInventory = baseInventory;
                Turrets = turrets;
                Grid = grid;
            }
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

            _folderPath = Path.Combine(UnityEngine.Application.persistentDataPath, FolderName);
            _fullPath = Path.Combine(_folderPath, FileName);

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

        /// <summary>
        /// Replaces player inventory in the current save. Is not saved to file before the WriteToSaveData function is called
        /// </summary>
        /// <param name="saveData">Save data</param>
        public void ReplacePlayerInventory(List<InventoryItemSaveData> saveData)
        {
            _currentSave.PlayerInventory = saveData;
        }

        /// <summary>
        /// Replaces base inventory in the current save. Is not saved to file before the WriteToSaveData function is called
        /// </summary>
        /// <param name="saveData">Save data which includes ID of Resource Data and Count</param>
        public void ReplaceBaseInventory(List<ItemWithCount> saveData)
        {
            _currentSave.BaseInventory = saveData;
        }
    }
}
