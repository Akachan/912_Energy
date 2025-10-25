using System;
using SavingSystem;
using UnityEngine;
using Utilities;

namespace Cash
{
    public class CashManager : BigResourceManager
    {
        
        private SavingWrapper _saving;
        public event Action<BigNumber> OnCashChange;
        
        
        private void Awake()
        {
            _saving = FindFirstObjectByType<SavingWrapper>();
            CurrentResources = new BigNumber();
            
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
            _saving.SetTemporalSave(SavingKeys.Cash.Current, CurrentResources.ToToken());
        }
        public override void Load()
        {
            var cc= _saving.GetSavingValue(SavingKeys.Cash.Current);
            if (cc != null)
            {
                CurrentResources = cc.ToBigNumber(); 
                UpdateUI();
            }
        }
    }
}