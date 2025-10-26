using System;
using Newtonsoft.Json.Linq;
using SavingSystem;
using UnityEngine;
using Utilities;

public class StatsManager : MonoBehaviour
{
    private static StatsManager _instance;
    public static StatsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<StatsManager>();
            }
            return _instance;
        }
    }
    
    private SavingWrapper _saving;
   
    
    private void Awake()
    {
        InitializeSingleton();
        
        _saving = FindFirstObjectByType<SavingWrapper>();
        
        
        //Energy
        Energy.Load();
        EventStatBus.Instance.OnEnergyProduced += Energy.AddEnergyProduced;
        EventStatBus.Instance.OnEnergyConsumed += Energy.AddEnergyConsumed;
        
        //Knowledge
        
        //Cash
        
        //Resquest
        EventStatBus.Instance.OnFulFillRequest += Energy.AddEnergyTrade;
    }
    private void OnDisable()
    {
        EventStatBus.Instance.OnEnergyProduced -= Energy.AddEnergyProduced;
        EventStatBus.Instance.OnEnergyConsumed -= Energy.AddEnergyConsumed;
        
        
        EventStatBus.Instance.OnFulFillRequest -= Energy.AddEnergyTrade;
        
    }
    private void InitializeSingleton()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private StatsManager() { }


    public static class Energy
    {
        private static EnergyStats _stats;

        public static event Action<BigNumber> OnEnergyProducedStatChange;
        public static event Action<BigNumber> OnEnergyConsumedStatChange;
        public static event Action<BigNumber> OnEnergyTradeStatChange;

        public static void AddEnergyProduced(BigNumber energyToAdd)
        {
            if (energyToAdd == null)
            {
                print("energy to add es null");
            }

            if (_stats == null)
            {
                print("stats es null");
            }

            if (_stats != null && _stats.Produced == null)
            {
                print("produced es null");
            }
            
            print($"Energy produced: {_stats.Produced} + {energyToAdd} =");
            _stats.Produced = Calculator.AddBigNumbers(_stats.Produced, energyToAdd);
            OnEnergyProducedStatChange?.Invoke(_stats.Produced);
            Save();
        }
        public static void AddEnergyConsumed(BigNumber energyToAdd)
        {
            if (energyToAdd == null) return;
            _stats.Consumed = Calculator.AddBigNumbers(_stats.Consumed, energyToAdd);
            OnEnergyConsumedStatChange?.Invoke(_stats.Consumed);
            Save();
        }
        public static void AddEnergyTrade(BigNumber energyToAdd)
        {
            _stats.Trade = Calculator.AddBigNumbers(_stats.Trade, energyToAdd);
            OnEnergyTradeStatChange?.Invoke(_stats.Trade);
            Save();
        }

        private static void Save()
        {
            Instance._saving.SetTemporalSave(SavingKeys.Energy.Stats, JToken.FromObject(_stats));
        }

        public static void Load()
        {
            var stats = Instance._saving.GetSavingValue(SavingKeys.Energy.Stats);
            if (stats != null)
            {
                _stats = stats.ToObject<EnergyStats>();
            }
            else
            {
                _stats = new EnergyStats();
            }
        }

        private class EnergyStats
        {
            public BigNumber Produced = new(0, 0);
            public BigNumber Consumed = new(0, 0);
            public BigNumber Trade = new(0, 0);
        }
        
        
        
    }
}