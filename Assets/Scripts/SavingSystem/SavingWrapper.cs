using Newtonsoft.Json.Linq;
using UnityEngine;

namespace SavingSystem
{
    public class SavingWrapper : MonoBehaviour
    {
        private global::SavingSystem.SavingSystem _savingSystem;
        private const string DefaultSaveFile = "save";

        private static SavingWrapper Instance { get; set; }
        
        private void Awake()
        {
            ManageSingleton();
            _savingSystem = GetComponent<global::SavingSystem.SavingSystem>();
            LoadInFile();
        }

        private void ManageSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            
        }
        
        

        //Temporal save//
        public void SetTemporalSave(string key, JToken value)
        {
            _savingSystem.UpdateDictionary(key, value);
        }

        public JToken GetSavingValue(string key)
        {
            return _savingSystem.GetDictionaryValue(key);
        }
        
        
        
        //Save file//
        public void SaveInFile()
        {
            _savingSystem.Save(DefaultSaveFile);
        }
        public void SaveInFile(string key, JToken value)
        {
            _savingSystem.UpdateDictionary(key, value);
            _savingSystem.Save(DefaultSaveFile);
        }
        
        public void LoadInFile()
        {
            _savingSystem.Load(DefaultSaveFile);
        }
        
        public void DeleteSaveFile()
        {
            _savingSystem.Delete(DefaultSaveFile);
           
        }
        
        
    }
}