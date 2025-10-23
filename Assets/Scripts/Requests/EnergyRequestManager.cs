using System;
using UnityEngine;
using Utilities;


namespace Requests
{
    public class EnergyRequestManager : MonoBehaviour
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
        
        private float _currentTime = 99f;
        
        public float CashRatio => _cashRatio;


        private void Start()
        {
            SetEnergyToRequest();
            SetTimeToSpawn();
            SetCashRatio();
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
                _currentTime = 0f;
            }
        }

        private void SpawnRequest()
        {
            //request limit
            if(transform.childCount >= maxRequestsBase) return;
            
            //GetEnergyToRequest
            var request = CalculateEnergyToRequest();

            //New request
            var instance = Instantiate(energyRequestPrefab, transform).GetComponent<EnergyRequest>();
            instance.SetEnergyToRequest(request);
            
        }

        private BigNumber CalculateEnergyToRequest()
        {
            var randomizeFactor = UnityEngine.Random.Range(-randomizeFactorBase, randomizeFactorBase);
            var energyToRequest = Calculator.MultiplyBigNumbers(_energyToRequest, 1 + randomizeFactor);
            print("energyToRequest: " + BigNumberFormatter.SetSuffixFormat(energyToRequest) + "");
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

        private void OnValidate()
        {
            SetEnergyToRequest();
            SetTimeToSpawn();
            SetCashRatio();
        }
    }
}
