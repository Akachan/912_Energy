using System;
using Newtonsoft.Json.Linq;
using SavingSystem;
using UnityEngine;
using Utilities;

namespace Stats
{
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
            EnergyStat.Load();
            EventStatBus.Instance.OnEnergyProduced += EnergyStat.AddEnergyProduced;
            EventStatBus.Instance.OnEnergyConsumed += EnergyStat.AddEnergyConsumed;
        
            //Knowledge
            EventStatBus.Instance.OnKnowledgeProduced += KnowledgeStat.AddResourceProduced;
            EventStatBus.Instance.OnKnowledgeConsumed += KnowledgeStat.AddResourcesConsumed;
        
            //Cash
            EventStatBus.Instance.OnCashProduced += CashStat.AddResourceProduced;
            EventStatBus.Instance.OnCashConsumed += CashStat.AddResourcesConsumed;
        
            //Resquest
            EventStatBus.Instance.OnFulFillRequest += EnergyStat.AddEnergyTrade;
            
            //Milestones
            EventStatBus.Instance.OnMilestoneProduced += MilestoneStat.AddResourceProduced;
            EventStatBus.Instance.OnMilestoneConsumed += MilestoneStat.AddResourcesConsumed;
        }
        private void OnDisable()
        {
            EventStatBus.Instance.OnEnergyProduced -= EnergyStat.AddEnergyProduced;
            EventStatBus.Instance.OnEnergyConsumed -= EnergyStat.AddEnergyConsumed;
        
            EventStatBus.Instance.OnKnowledgeProduced -= KnowledgeStat.AddResourceProduced;
            EventStatBus.Instance.OnKnowledgeConsumed -= KnowledgeStat.AddResourcesConsumed;
            
            EventStatBus.Instance.OnCashProduced -= CashStat.AddResourceProduced;
            EventStatBus.Instance.OnCashConsumed -= CashStat.AddResourcesConsumed;
        
            EventStatBus.Instance.OnFulFillRequest -= EnergyStat.AddEnergyTrade;
            
            EventStatBus.Instance.OnMilestoneProduced -= MilestoneStat.AddResourceProduced;
            EventStatBus.Instance.OnMilestoneConsumed -= MilestoneStat.AddResourcesConsumed;
        
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

        public void LoadStats()
        {
            EnergyStat.Load();
            KnowledgeStat.Load();
            CashStat.Load();
            MilestoneStat.Load();
        }
    
        public static class EnergyStat
        {
            private static StatData _statsData;
            public static BigNumber Produced => _statsData.Produced;
            public static BigNumber Consumed => _statsData.Consumed;
            public static BigNumber Trade => _statsData.Trade;

            public static event Action<BigNumber> OnProducedStatChange;
            public static event Action<BigNumber> OnEnergyConsumedStatChange;
            public static event Action<BigNumber> OnEnergyTradeStatChange;

            public static void AddEnergyProduced(BigNumber energyToAdd)
            {
                if (_statsData == null && energyToAdd == null) return;
                
                _statsData.Produced = Calculator.AddBigNumbers(_statsData.Produced, energyToAdd);
                OnProducedStatChange?.Invoke(_statsData.Produced);
                Save();


            }
            public static void AddEnergyConsumed(BigNumber energyToAdd)
            {
                if (energyToAdd == null) return;
                _statsData.Consumed = Calculator.AddBigNumbers(_statsData.Consumed, energyToAdd);
                OnEnergyConsumedStatChange?.Invoke(_statsData.Consumed);
                Save();
            }
            public static void AddEnergyTrade(BigNumber energyToAdd)
            {
                _statsData.Trade = Calculator.AddBigNumbers(_statsData.Trade, energyToAdd);
                OnEnergyTradeStatChange?.Invoke(_statsData.Trade);
                Save();
            }

            private static void Save()
            {
                Instance._saving.SetTemporalSave(SavingKeys.Energy.Stats, JToken.FromObject(_statsData));
            }

            public static void Load()
            {
                var stats = Instance._saving.GetSavingValue(SavingKeys.Energy.Stats);
                if (stats != null)
                {
                    _statsData = stats.ToObject<StatData>();
                }
                else
                {
                    _statsData = new StatData();
                }
            }

            private class StatData
            {
                public BigNumber Produced = new(0, 0);
                public BigNumber Consumed = new(0, 0);
                public BigNumber Trade = new(0, 0);
                
                
            }
        
        
        
        }
    
        public static class KnowledgeStat
        {
            private static StatData _stats;

            public static event Action<BigNumber> OnProducedStatChange;
            public static event Action<BigNumber> OnConsumedStatChange;


            public static void AddResourceProduced(BigNumber resourceToAdd)
            {
                if (_stats == null || resourceToAdd == null) return;
                _stats.Produced = Calculator.AddBigNumbers(_stats.Produced, resourceToAdd);
                OnProducedStatChange?.Invoke(_stats.Produced);
                Save();

            }
            public static void AddResourcesConsumed(BigNumber energyToAdd)
            {
                if (_stats == null || energyToAdd == null) return;
                _stats.Consumed = Calculator.AddBigNumbers(_stats.Consumed, energyToAdd);
                OnConsumedStatChange?.Invoke(_stats.Consumed);
                Save();
            }
        

            private static void Save()
            {
                Instance._saving.SetTemporalSave(SavingKeys.Knowledge.Stats, JToken.FromObject(_stats));
            }

            public static void Load()
            {
                var stats = Instance._saving.GetSavingValue(SavingKeys.Knowledge.Stats);
                if (stats != null)
                {
                    _stats = stats.ToObject<StatData>();
                }
                else
                {
                    _stats = new StatData();
                }
            }

            private class StatData
            {
                public BigNumber Produced = new(0, 0);
                public BigNumber Consumed = new(0, 0);
            }
        
        
        
        }
        
        public static class CashStat
        {
            private static StatData _stats;

            public static event Action<BigNumber> OnProducedStatChange;
            public static event Action<BigNumber> OnConsumedStatChange;
            public static event Action<BigNumber> OnChange;


            public static void AddResourceProduced(BigNumber resourceToAdd)
            {
                if (_stats == null || resourceToAdd == null) return;
                _stats.Produced = Calculator.AddBigNumbers(_stats.Produced, resourceToAdd);
                OnProducedStatChange?.Invoke(_stats.Produced);
                OnChange?.Invoke(_stats.Produced);
                Save();

            }
            public static void AddResourcesConsumed(BigNumber energyToAdd)
            {
                if (_stats == null || energyToAdd == null) return;
                _stats.Consumed = Calculator.AddBigNumbers(_stats.Consumed, energyToAdd);
                OnConsumedStatChange?.Invoke(_stats.Consumed);
                OnChange?.Invoke(_stats.Consumed);
                
                Save();
            }
        

            private static void Save()
            {
                Instance._saving.SetTemporalSave(SavingKeys.Cash.Stats, JToken.FromObject(_stats));
            }

            public static void Load()
            {
                var stats = Instance._saving.GetSavingValue(SavingKeys.Cash.Stats);
                if (stats != null)
                {
                    _stats = stats.ToObject<StatData>();
                }
                else
                {
                    _stats = new StatData();
                }
            }

            private class StatData
            {
                public BigNumber Produced = new(0, 0);
                public BigNumber Consumed = new(0, 0);
            }
        
        
        
        }
        
        public static class MilestoneStat
        {
            private static StatData _stats;

            public static event Action<int> OnProducedStatChange;
            public static event Action<int> OnConsumedStatChange;
            


            public static void AddResourceProduced(int resourceToAdd)
            {
                
                
                if (_stats == null ) return;
                _stats.Produced += resourceToAdd;
                print($"Se produjeron {resourceToAdd}");
                OnProducedStatChange?.Invoke(_stats.Produced);
                
                Save();

            }
            public static void AddResourcesConsumed(int resourceToAdd)
            {
                if (_stats == null) return;
                _stats.Consumed += resourceToAdd;
                OnConsumedStatChange?.Invoke(_stats.Consumed);
                Save();
            }
        

            private static void Save()
            {
                Instance._saving.SetTemporalSave(SavingKeys.Milestone.Stats, JToken.FromObject(_stats));
            }

            public static void Load()
            {
                var stats = Instance._saving.GetSavingValue(SavingKeys.Milestone.Stats);
                if (stats != null)
                {
                    _stats = stats.ToObject<StatData>();
                }
                else
                {
                    _stats = new StatData();
                }
            }

            private class StatData
            {
                public int Produced = 0;
                public int Consumed = 0;
            }
        
        
        
        }
    }
}