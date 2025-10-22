using System;
using Cash;
using Energy;
using UnityEngine;
using Utilities;

namespace Requests
{
    public class EnergyRequest : MonoBehaviour
    {
      
        private BigNumber _energyToRequest;
        private EnergyManager _energyManager;
        
        
        public BigNumber EnergyToRequest => _energyToRequest;
        public event Action<BigNumber> OnFulFillRequest;
        
        
        
        private void Awake()
        {
            _energyManager = FindFirstObjectByType<EnergyManager>();
        }
        
        public void SetEnergyToRequest(BigNumber energyToRequest)
        {
            _energyToRequest = energyToRequest;
            
        }
        
        private BigNumber CalculateGoldToGet(BigNumber ratioGoldEnergy)
        {
            BigNumber goldToGet = Calculator.MultiplyBigNumbers(_energyToRequest, ratioGoldEnergy);   
            return goldToGet;
        }

        public void FulFillRequest()
        {
            if (_energyManager.RemoveResources(_energyToRequest))
            {
                var calculateGoldToGet = CalculateGoldToGet(new BigNumber(1, 1));
                FindFirstObjectByType<CashManager>().AddResources(calculateGoldToGet);
                
                OnFulFillRequest?.Invoke(calculateGoldToGet); //para pasarle el texto para despues mostrarlo en el UI
                
                //Add gold to player
                //Destroy this object
            }
        }
        
        
        
    }
}