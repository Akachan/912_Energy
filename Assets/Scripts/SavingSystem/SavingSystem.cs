using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace SavingSystem
{
    public class SavingSystem : MonoBehaviour
    {
        private const string Extension = ".json";
        private IDictionary<string, JToken> _savingDictionary = new Dictionary<string, JToken>();




        public void UpdateDictionary(string key, JToken value)
        {
            _savingDictionary[key] = value;
        }

        public JToken GetDictionaryValue(string key)
        {
            _savingDictionary.TryGetValue(key, out JToken value);
            return value;
        }

        private void ClearDictionary()
        {
            _savingDictionary.Clear();
        }
        
        
        
        
        
        //SAVE AND LOAD
        public void Save(string saveFile)
        {
            //JObject state = LoadJSonFromFile(saveFile);
            JObject state = JObject.FromObject(_savingDictionary);
            SaveFileAsJSon(saveFile, state);

        }
        
        public void Load(string saveFile)
        {   
            var state = LoadJSonFromFile(saveFile);
            _savingDictionary = state;
            print($"Loaded {saveFile}");
            
        }
        public void Delete(string saveFile)
        {
            ClearDictionary();
            File.Delete(GetPathFromSaveFile(saveFile));
            print($"Deleted: {saveFile}");
        }


        private void SaveFileAsJSon(string saveFile, JObject state)
        {
            var path = GetPathFromSaveFile(saveFile);
            print($"Saving to {path}");

            using (var textWriter = File.CreateText(path))              //Creo el texto
            {
                using (var writer = new JsonTextWriter(textWriter))     //Creo un escritor
                {
                    writer.Formatting = Formatting.Indented;            //determino el formato de escritura
                    state.WriteTo(writer);                              //Escribo en writer el dictionary
                }
            }
        }
        private JObject LoadJSonFromFile(string saveFile)
        {
            var path = GetPathFromSaveFile(saveFile);

            if (!File.Exists(path))
            {
                return new JObject();
            }

            using (var textReader =File.OpenText(path))
            {
                using (var reader = new JsonTextReader(textReader))
                {
                    reader.FloatParseHandling = FloatParseHandling.Double;
                    
                    return JObject.Load(reader);
                }
            }
        }
        
        
        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + Extension);
        }
        
        
        
        
        
    }
    
    
    
    
    
    
}
