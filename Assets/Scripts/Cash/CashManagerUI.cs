using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utilities;

namespace Cash
{
    public class CashManagerUI : MonoBehaviour
    {  
        [Header("TopPanel")]
        [SerializeField] private TextMeshProUGUI cashValue;

        private CashManager _cashManager;

        private void Awake()
        {
            _cashManager = GetComponent<CashManager>();
        }

        private void Start()
        {
            _cashManager.OnCashChange += SetCashValue;
            print("Se agregó SetCashValue en  OnCashChange");
            
            
        }

        private void OnDisable()
        {
            _cashManager.OnCashChange -= SetCashValue;
        }

        public void SetCashValue(BigNumber value)
        {
            cashValue.text = BigNumberFormatter.SetSuffixFormat(value);
        }
    }

   
}