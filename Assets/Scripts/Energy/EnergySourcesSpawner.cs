using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SavingSystem;
using UnityEngine;
using Utilities;

namespace Energy
{
    public class EnergySourcesSpawner : MonoBehaviour
    {
        [SerializeField] private EnergySourceSo[] energySourcesSo;

        [Header("References")] 
        [SerializeField]  private GameObject energySourcePrefab;

        [SerializeField] private Transform energySourceParent;
        
        private Dictionary<int, EnergySourceData> _energySources;
    
        private SavingWrapper _saving;


        private void Awake()
        {
            _saving = FindFirstObjectByType<SavingWrapper>();
            Load();
        }
        

        private void Start()
        {
            if (_energySources == null)
            {
                print("no hay sources guardadas");
                InitializeFirstEnergySource();
                return;
            }
            RestoreEnergySources();
            
        }

    

        private void InitializeFirstEnergySource()
        {
            var data = new EnergySourceData(0);
            
            var instance = Instantiate(energySourcePrefab, energySourceParent).GetComponent<EnergySource>(); 
            instance.SetEnergySource(data, energySourcesSo[data.SoIndex]);
            
            _energySources = new Dictionary<int, EnergySourceData>();
            _energySources[data.SoIndex] = data;
            Save(); 
            
            Debug.Log($"Se spawneo: {energySourcesSo[data.SoIndex].SourceName}");
           
            instance.UnlockEnergySource(true);  
            
        }
        private void RestoreEnergySources()
        {
            foreach (var data in _energySources)
            {
                var instance = Instantiate(energySourcePrefab, energySourceParent).GetComponent<EnergySource>();
                
                var source = energySourcesSo[data.Value.SoIndex];
                
                instance.RestoreEnergySource(data.Value, source);
                
            }
        }

        public void CreateNewEnergySource()
        {
            if(_energySources.Count >= energySourcesSo.Length) return;
            
            var data = new EnergySourceData(_energySources.Count);
            
            var instance = Instantiate(energySourcePrefab, energySourceParent).GetComponent<EnergySource>(); 
            instance.SetEnergySource(data, energySourcesSo[data.SoIndex]);
            
            _energySources[data.SoIndex] = data;
            Save(); 
            
            Debug.Log($"Se spawneo: {energySourcesSo[data.SoIndex].SourceName}");
            
        }

        public void UpdateEnergySource(EnergySourceData data)
        {
            _energySources[data.SoIndex] = data;
            TemporalSave();
            
        }

        private void Save()
        {
            _saving.SaveInFile(SavingKeys.Energy.Sources, JToken.FromObject(_energySources));
        }
        private void TemporalSave()
        {
            _saving.SetTemporalSave(SavingKeys.Energy.Sources, JToken.FromObject(_energySources));
        }
        private void Load()
        {
            var sources = _saving.GetSavingValue(SavingKeys.Energy.Sources);
            if (sources != null)
            {
                _energySources = sources.ToObject<Dictionary<int, EnergySourceData>>();
            }
        }

    }
    
    public class EnergySourceData
    {
        public readonly int SoIndex;
        public bool IsLocked;
        public int Level;

        public EnergySourceData(int soIndex)
        {
            SoIndex = soIndex;
            IsLocked = true;
            Level = 1;
            
        }
    }
}
