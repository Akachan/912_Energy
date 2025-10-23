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
        private EnergyRequestSaver _energyRequestSaver;
        private int _index;
        
        
        public BigNumber EnergyToRequest => _energyToRequest;
        public event Action<BigNumber> OnFulFillRequest;
        
        
        
        private void Awake()
        {
            _energyManager = FindFirstObjectByType<EnergyManager>();
            _energyRequestManager = FindFirstObjectByType<EnergyRequestManager>();
        }
        
        public void SetEnergyToRequest(EnergyRequestSaver saver, BigNumber energyToRequest, int index)
        {
            _energyRequestSaver = saver;
            _energyToRequest = energyToRequest;
            _index = index;
            _energyRequestSaver.SaveRequest(_index, _energyToRequest);
            
        }
        


        public void FulFillRequest()
        {
            if (_energyManager.RemoveResources(_energyToRequest))
            {
                _energyRequestSaver.DeleteRequest(_index);
                
                var calculateGoldToGet = CalculateGoldToGet();
                FindFirstObjectByType<CashManager>().AddResources(calculateGoldToGet);
                
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