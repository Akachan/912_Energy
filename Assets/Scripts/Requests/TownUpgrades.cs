using System;
using System.Collections.Generic;
using Milestones;
using SavingSystem;
using UnityEngine;
using Utilities;

namespace Requests
{
    public class TownUpgrades : MonoBehaviour, ISaveable
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
        
        public event Action<int> OnUpgradePopulation;
        public event Action<int> OnUpgradeIndustries;
        public event Action<int> OnUpgradeCommerce;


        private void Awake()
        {
            _energyRequestManager = FindFirstObjectByType<EnergyRequestManager>();
            _saving = FindFirstObjectByType<SavingWrapper>();
        }

        public void OnUpgradePopulationButton()
        {
            //verificar el costo primero
            if (_milestoneManager.RemoveResources(populationProgression[_populationLevel].cost))
            {
                _populationLevel++;
                _energyRequestManager.SetTimeToSpawn(populationProgression[_populationLevel].spawnTime);
                OnUpgradePopulation?.Invoke(populationProgression[_populationLevel].cost);
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
                OnUpgradeIndustries?.Invoke(industriesProgression[_industriesLevel].cost);
            }
            else
            {
                Debug.Log("No hay suficiente Milestone");
            }

            
            
            
        }

        public void OnUpgradeCommerceButton()
        {
            //verificar el costo primero
            if (!_milestoneManager.RemoveResources(commerceProgression[_commerceLevel].cost))
            {

                _commerceLevel++;
                _energyRequestManager.SetCashRatio(commerceProgression[_commerceLevel].ratio);
                OnUpgradeCommerce?.Invoke(commerceProgression[_commerceLevel].cost);
            }
            else
            {
                Debug.Log("No hay suficiente Milestone");
            }
        }
        
        
        
        public Population GetPopulation()
        {
            return populationProgression[_populationLevel];
        }

        public Industries GetIndustries()
        {
            return industriesProgression[_industriesLevel];
        }
        public Commerce GetCommerce()
        {
            return commerceProgression[_commerceLevel];
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

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Load()
        {
            throw new NotImplementedException();
        }
    }
}