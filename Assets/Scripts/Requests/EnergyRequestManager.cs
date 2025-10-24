using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SavingSystem;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using Utilities;


namespace Requests
{
    public class EnergyRequestManager : MonoBehaviour, ISaveable
    {
        
        [Header("Base Settings")]
        [SerializeField] private BigNumber energyToRequestBase = new BigNumber(1,1); //Ver a futuro que sea levemente aleatorio
        [SerializeField ]private float timeToSpawnBase = 10f;           //population
        [SerializeField] private int maxRequestsBase = 3;
        [SerializeField] private float cashRatioBase = 1f;              //Commerce
        [SerializeField] private float randomizeFactorBase = 0.02f;
        
        
        [Header("Multipliers")]
        [SerializeField] private float energyToRequestMultiplier = 0.1f;  //este debe ir aumentando
        [SerializeField] private float timeToSpawnMultiplier = 0.5f;      //este debe ir disminuyendo
        [SerializeField] private float cashRatioMultiplier = 0.1f;        //este debe ir aumentando
        [SerializeField] private int maxRequestsAmplifier = 1;          //este debe ir aumentando

        [Header("Factores temporales")]
        [SerializeField] private int populationPurchased = 1;
        [SerializeField] private int industriesPurchased = 1;
        [SerializeField] private int commercesPurchased = 1;
        
        [Header("References")]
        [SerializeField] private GameObject energyRequestPrefab;
        
        private BigNumber _energyToRequest;
        private float _timeToSpawn;
        private float _cashRatio;
        private float _currentTime = 0f;
     
        //Saving
        private List<BigNumber> _requests = new List<BigNumber>();
        private SavingWrapper _saving;
        
        public float CashRatio => _cashRatio;

        private void Awake()
        {
            _saving = FindFirstObjectByType<SavingWrapper>();
        }

        private void Start()
        {
            SetEnergyToRequest();
            SetTimeToSpawn();
            SetCashRatio();
            Load();
        }

        private void Update()
        {
            ManageRequestTiming();
        }

        private void ManageRequestTiming()
        {
            _currentTime += Time.deltaTime;

            if (_currentTime > _timeToSpawn)
            {
                SpawnRequest();
                
                Save();
                
                _currentTime = 0f;
            }
        }

        private void SpawnRequest(BigNumber energyValue = null)
        {
            //request limit
            if(transform.childCount >= maxRequestsBase) return;
            
            //GetEnergyToRequest
            var request = energyValue;
            if (energyValue == null)
            {
                request = CalculateEnergyToRequest();
                _requests.Add(request);
                Save();
            }
            
            //New request
            var instance = Instantiate(energyRequestPrefab, transform).GetComponent<EnergyRequest>();
            instance.SetEnergyToRequest(request);
        }

        public void RemoveRequest(BigNumber energyValue)
        {
            _requests.Remove(energyValue);
            Save();
        }

        private BigNumber CalculateEnergyToRequest()
        {
            var randomizeFactor = UnityEngine.Random.Range(-randomizeFactorBase, randomizeFactorBase);
            var energyToRequest = Calculator.MultiplyBigNumbers(_energyToRequest, 1 + randomizeFactor);
            energyToRequest = Calculator.RoundDecimalBigNumber(energyToRequest);
            return energyToRequest;
        }

        private void SetEnergyToRequest()
        {
            _energyToRequest = Calculator.MultiplyBigNumbers(energyToRequestBase,
                                1 + energyToRequestMultiplier * industriesPurchased); 
        }

        private void SetTimeToSpawn()
        {
            _timeToSpawn = timeToSpawnBase * (1 - timeToSpawnMultiplier * populationPurchased);
        }

        private void SetCashRatio()
        {
            _cashRatio = cashRatioBase * (1 + cashRatioMultiplier * commercesPurchased);
        }

        public void NewRequestsOnDisconnection(int seconds)
        {
            var newRequests = Mathf.Floor(seconds / _timeToSpawn);
            var requestSlots = maxRequestsBase - _requests.Count;
            newRequests = Mathf.Clamp(newRequests, 0, requestSlots);

            if (!(newRequests > 0)) return;
            for (var i = 0; i < newRequests; i++)
            {
                SpawnRequest();
            }
            Save();


        }
        
        //SAVING SYSTEM
        public void Save()
        {
            var list = JToken.FromObject(_requests);
            _saving.SetTemporalSave(SavingKeys.Request.Current, list);
        }
        
        public void Load()
        {
            var list = _saving.GetSavingValue(SavingKeys.Request.Current);
            if (list != null)
            {
                _requests = list.ToObject<List<BigNumber>>();
            }
            
            if (_requests == null) return;
            
            foreach (var request in _requests)
            {
                var instance = Instantiate(energyRequestPrefab, transform).GetComponent<EnergyRequest>();
                instance.SetEnergyToRequest(request);
            }
            
        }
        
        private void OnValidate()
        {
            SetEnergyToRequest();
            SetTimeToSpawn();
            SetCashRatio();
        }

    }
}
