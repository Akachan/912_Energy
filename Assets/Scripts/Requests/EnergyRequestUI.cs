using System;
using System.Collections;
using Energy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Requests
{
    public class EnergyRequestUI: MonoBehaviour
    {
        
        [SerializeField] private TextMeshProUGUI energyToRequestText;
        [SerializeField] private Button requestButton;
        
        [SerializeField] private float timeToRecalculate = 1f;
        
        private EnergyRequest _energyRequest;
        private EnergyManager _energyManager;
        private float _currentTime;
        private bool _isInit= false;


        private void Awake()
        {
            _energyManager = FindFirstObjectByType<EnergyManager>();
            _energyRequest = GetComponent<EnergyRequest>();
            
        }

        private void OnEnable()
        {
            
            _energyRequest.OnFulFillRequest += DestroyRequestUi;
        }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            energyToRequestText.text = BigNumberFormatter.SetSuffixFormat((_energyRequest.EnergyToRequest));
            _isInit = true;
        }

        private void LateUpdate()
        {
            
            if(!_isInit) return;
            UpdateUI();
        }
        private void UpdateUI()
        {
            _currentTime += Time.deltaTime;
            
            if (!(_currentTime > timeToRecalculate)) return;
            
            Calculator.CompareBigNumbers(_energyManager.GetResources(), _energyRequest.EnergyToRequest, out var result);
            
            requestButton.interactable  = result is ComparisonResult.Bigger or ComparisonResult.Equal;
            _currentTime = 0f;

        }

        private void DestroyRequestUi(BigNumber gold)
        {
            Destroy(gameObject);
        }

        
        
     


    }
}