using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Requests
{
    public class EnergyRequestManager : MonoBehaviour
    {
        [SerializeField] private GameObject energyRequestPrefab;
        [SerializeField] private BigNumber energyToRequestBase = new BigNumber(1,1); //Ver a futuro que sea levemente aleatorio
        [SerializeField] private int maxRequests = 3;
        private float _currentTime = 99f;
        private float _timeToSpawn = 10f;

        private void Update()
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
            //limite de requests
            var children  = transform.childCount;
            
            print($"children {children}, maxRequests {maxRequests}");
            if(children >= maxRequests) return;
            
            //nueva request
            var instance = Instantiate(energyRequestPrefab, transform).GetComponent<EnergyRequest>();
            instance.SetEnergyToRequest(energyToRequestBase);
            
        }
    }
}
