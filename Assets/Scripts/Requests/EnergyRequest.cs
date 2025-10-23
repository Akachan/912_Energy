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
        private EnergyRequestManager _energyRequestManager;
        
        
        public BigNumber EnergyToRequest => _energyToRequest;
        public event Action<BigNumber> OnFulFillRequest;
        
        
        
        private void Awake()
        {
            _energyManager = FindFirstObjectByType<EnergyManager>();
            _energyRequestManager = FindFirstObjectByType<EnergyRequestManager>();
        }
        
        public void SetEnergyToRequest(BigNumber energyToRequest)
        {
            _energyToRequest = energyToRequest;
            
        }
        


        public void FulFillRequest()
        {
            if (_energyManager.RemoveResources(_energyToRequest))
            {
                var calculateGoldToGet = CalculateGoldToGet();
                FindFirstObjectByType<CashManager>().AddResources(calculateGoldToGet);
                
                OnFulFillRequest?.Invoke(calculateGoldToGet); //para pasarle el texto para despues mostrarlo en el UI
                
                //Add gold to player
                //Destroy this object
            }
        }
        
        private BigNumber CalculateGoldToGet()
        {
            BigNumber goldToGet = Calculator.MultiplyBigNumbers(_energyToRequest, _energyRequestManager.CashRatio);   
            return goldToGet;
        }
        
        
        
    }
}