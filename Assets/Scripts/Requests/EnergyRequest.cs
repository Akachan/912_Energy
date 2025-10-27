using System;
using Cash;
using Energy;
using Stats;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;

namespace Requests
{
    public class EnergyRequest : MonoBehaviour
    {
      
        private BigNumber _energyToRequest;
        private EnergyManager _energyManager;
        private EnergyRequestManager _energyRequestManager;
        private int _index;
        
        
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
                _energyRequestManager.RemoveRequest(_energyToRequest);
                
                var calculateGoldToGet = CalculateGoldToGet();
                FindFirstObjectByType<CashManager>().AddResources(calculateGoldToGet);
                EventStatBus.Instance.OnFulFillRequestEvent(_energyToRequest);
                
                OnFulFillRequest?.Invoke(calculateGoldToGet); //para pasarle el texto para despues mostrarlo en el UI
                
                
            }
        }
        
        private BigNumber CalculateGoldToGet()
        {
            BigNumber goldToGet = Calculator.MultiplyBigNumbers(_energyToRequest, _energyRequestManager.CashRatio);   
            return goldToGet;
        }
        
        
        
    }
}