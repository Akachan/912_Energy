using System;
using System.Collections.Generic;
using Milestones;
using Newtonsoft.Json.Linq;
using SavingSystem;
using UnityEngine;
using Utilities;

namespace Requests
{
    public class RequestUpgrader : MonoBehaviour, ISaveable
    {
        [SerializeField] private List<Population> populationProgression;
        [SerializeField] private List<Industries> industriesProgression;
        [SerializeField] private List<Commerce> commerceProgression;
        
        private EnergyRequestManager _energyRequestManager;
        private MilestoneManager _milestoneManager;
        private SavingWrapper _saving;
        
        private int _populationLevel;
        private int _industriesLevel;
        private int _commerceLevel;
        
        public int PopulationLevel => _populationLevel;
        public int IndustriesLevel => _industriesLevel;
        public int CommerceLevel => _commerceLevel;     
        
        public event Action OnUpgradePopulation;
        public event Action OnUpgradeIndustries;
        public event Action OnUpgradeCommerce;


        private void Awake()
        {
            _energyRequestManager = FindFirstObjectByType<EnergyRequestManager>();
            _milestoneManager = FindFirstObjectByType<MilestoneManager>();
            _saving = FindFirstObjectByType<SavingWrapper>();
        }

        private void Start()
        {
            Load();
            InitializeEnergyRequestSettings();
        }

        private void InitializeEnergyRequestSettings()
        {
            _energyRequestManager.SetTimeToSpawn(populationProgression[_populationLevel].spawnTime);
            _energyRequestManager.SetEnergyToRequest(industriesProgression[_industriesLevel].energy);
            _energyRequestManager.SetCashRatio(commerceProgression[_commerceLevel].ratio);
        }

        //Acciones de los botones
        public void OnUpgradePopulationButton()
        {
            //verificar el costo primero
            if (_milestoneManager.RemoveResources(populationProgression[_populationLevel].cost))
            {
                _populationLevel++;
                _energyRequestManager.SetTimeToSpawn(populationProgression[_populationLevel].spawnTime);
                OnUpgradePopulation?.Invoke();
            }
            else
            {
                Debug.Log("No hay suficiente Milestone");
            }
        }

        public void OnUpgradeIndustriesButton()
        {
            //verificar el costo primero
            if (_milestoneManager.RemoveResources(industriesProgression[_industriesLevel].cost))
            {
                _industriesLevel++;
                _energyRequestManager.SetEnergyToRequest(industriesProgression[_industriesLevel].energy);
                OnUpgradeIndustries?.Invoke();
            }
            else
            {
                Debug.Log("No hay suficiente Milestone");
            }
        }

        public void OnUpgradeCommerceButton()
        {
            //verificar el costo primero
            if (_milestoneManager.RemoveResources(commerceProgression[_commerceLevel].cost))
            {

                _commerceLevel++;
                _energyRequestManager.SetCashRatio(commerceProgression[_commerceLevel].ratio);
                OnUpgradeCommerce?.Invoke();
            }
            else
            {
                Debug.Log("No hay suficiente Milestone");
            }
        }
        
        
        //Getters
        public Population GetPopulation()
        {
            return populationProgression[_populationLevel];
        }

        public Population? GetPopulation(int level)
        {
            if (level >= populationProgression.Count)
            {
                return null ;
            }
            return populationProgression[level];       
        }

        public Industries? GetIndustries()
        {
            return GetIndustries(_industriesLevel);
        }
        public Industries? GetIndustries(int level)
        {
            if (level >= industriesProgression.Count)
            {
                return null ;
            }
            return industriesProgression[level];       
        }
        public Commerce? GetCommerce()
        {
            return GetCommerce(_commerceLevel);       
        }
        public Commerce? GetCommerce(int level)
        {
            if (level >= commerceProgression.Count)
            {
                return null;
            }
            return commerceProgression[level];       
        }


        [Serializable]
        public struct Population
        {
            public int cost;
            public float spawnTime;
            
        }
        [Serializable]
        public struct Industries
        {
            public int cost;
            public BigNumber energy;
        }
        [Serializable]
        public struct Commerce
        {
            
            public int cost;
            public float ratio;
        }

        
        //Save and Load
        public void Save()
        {
            _saving.SetTemporalSave(SavingKeys.Request.PopulationLevel, JToken.FromObject(_populationLevel));
            _saving.SetTemporalSave(SavingKeys.Request.IndustriesLevel, JToken.FromObject(_industriesLevel));
            _saving.SetTemporalSave(SavingKeys.Request.CommerceLevel, JToken.FromObject(_commerceLevel));
        }

        public void Load()
        {
            var population = _saving.GetSavingValue(SavingKeys.Request.PopulationLevel);
            var industries = _saving.GetSavingValue(SavingKeys.Request.IndustriesLevel);
            var commerce = _saving.GetSavingValue(SavingKeys.Request.CommerceLevel);
            
            if (population != null)
            {
                _populationLevel = population.ToObject<int>();
            }
            if (industries != null)
            {
                _industriesLevel = industries.ToObject<int>();
            }
            if (commerce != null)
            {
                _commerceLevel = commerce.ToObject<int>();
            }
        }
    }
}