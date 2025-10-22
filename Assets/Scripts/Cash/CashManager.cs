using System;
using UnityEngine;

namespace Cash
{
    public class CashManager : BigResourceManager
    {
        private CashManagerUI _ui;
        public event Action<BigNumber> OnCashChange;
        
        
        private void Awake()
        {
            _ui = GetComponent<CashManagerUI>();
            CurrentResources = new BigNumber(0, 0);
        }

        private void Start()
        {
            UpdateUI();
            Load();
        }

        public override void AddResources(BigNumber resourcesToAdd)
        {
            base.AddResources(resourcesToAdd);
            UpdateUI();
            
            
        }

        public override void UpdateUI()
        {
            OnCashChange?.Invoke(CurrentResources);
        }
    
        
        public override void Save()
        {
            PlayerPrefs.SetFloat("CashBase", (float)CurrentResources.Base);
            PlayerPrefs.SetInt("CashExponent", CurrentResources.Exponent);
        }
        public override void Load()
        {
            if (!PlayerPrefs.HasKey("CashBase")) return;
            CurrentResources = new BigNumber(PlayerPrefs.GetFloat("CashBase"), PlayerPrefs.GetInt("CashExponent"));
            _ui.SetCashValue(CurrentResources);
        }
    }
}