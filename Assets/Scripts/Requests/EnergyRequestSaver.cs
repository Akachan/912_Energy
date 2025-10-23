using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Requests
{
    public class EnergyRequestSaver
    {

        
        private List<int> _keys = new List<int>();
        

        public void SaveRequest(int index, BigNumber energyToRequest)
        {
            if (!PlayerPrefs.HasKey($"EnergyRequest_{index}_Base"))
            {
                PlayerPrefs.SetFloat($"EnergyRequest_{index}_Base", (float)energyToRequest.Base);
                PlayerPrefs.SetInt($"EnergyRequest_{index}_Exponent", energyToRequest.Exponent);
                
                _keys.Add(index);
                
                //Tranformar _keys en un array de strings
                string keysString = string.Join("-", _keys);
                PlayerPrefs.SetString("RequestKeys", keysString);
            }
            else
            {
                Debug.LogWarning("EnergyRequestIndex already exists. Restore request");
            }
            
        }
        public void DeleteRequest(int index)
        {
            if (!PlayerPrefs.HasKey($"EnergyRequest_{index}_Base"))
            {
                Debug.LogWarning("EnergyRequestIndex does not exists");
                return;
            }
            PlayerPrefs.DeleteKey($"EnergyRequest_{index}_Base");
            PlayerPrefs.DeleteKey($"EnergyRequest_{index}_Exponent");
            
            _keys.Remove(index);
            
            //Tranformar _keys en un array de strings
            string keysString = string.Join("-", _keys);
            PlayerPrefs.SetString("RequestKeys", keysString);
        
        }
        
        public List<RequestData> LoadRequests()
        {
            _keys.Clear();
            string keysString = PlayerPrefs.GetString("RequestKeys", "");
            
            if (string.IsNullOrEmpty(keysString)) return null;
            
            string[] keyArray = keysString.Split('-');
            
            List<RequestData> requests = new List<RequestData>();

            foreach (var key in keyArray)
            {
                if (!int.TryParse(key, out int keyInt)) continue;
                
                var request = new RequestData
                {
                    Index = keyInt,
                    EnergyToRequest = new BigNumber(PlayerPrefs.GetFloat($"EnergyRequest_{keyInt}_Base"), PlayerPrefs.GetInt($"EnergyRequest_{keyInt}_Exponent"))
                };
                
                requests.Add(request);
                _keys.Add(keyInt);
            }
            return requests;
        }
        
        public struct RequestData
        {
            public int Index;
            public BigNumber EnergyToRequest;
        }
        
    }
}