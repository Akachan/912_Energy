using System;
using UnityEngine;
using Utilities;

namespace Stats
{
    public class EventStatBus: MonoBehaviour
    {
        private static EventStatBus _instance;
        public static EventStatBus Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<EventStatBus>();
                }
                return _instance;
            }
        }
    
        private void Awake()
        {
            InitializeSingleton();
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

        //Energy
        public event Action<BigNumber> OnEnergyProduced;
        public event Action<BigNumber> OnEnergyConsumed;
    
    
        //Knowledge
        public event Action<BigNumber> OnKnowledgeProduced;
        public event Action<BigNumber> OnKnowledgeConsumed;
    
        //Cash
        public event Action<BigNumber> OnCashProduce;
        public event Action<BigNumber> OnCashConsume;
    
        //Resquest
        public event Action<BigNumber> OnFulFillRequest;
    
    
        //Milestones


        //EVENTS
        //Energy
        public void OnEnergyProducedEvent(BigNumber energy) => OnEnergyProduced?.Invoke(energy);
        public void OnEnergyConsumedEvent(BigNumber energy) => OnEnergyConsumed?.Invoke(energy);
    
 
        //Knowledge
        public void OnKnowledgeProducedEvent(BigNumber knowledge) => OnKnowledgeProduced?.Invoke(knowledge);
        public void OnKnowledgeConsumedEvent(BigNumber knowledge) => OnKnowledgeConsumed?.Invoke(knowledge);
    
        //Cash
        public void OnCashProduceEvent(BigNumber cash) => OnCashProduce?.Invoke(cash);
        public void OnCashConsumeEvent(BigNumber cash) => OnCashConsume?.Invoke(cash);
    
        //Resquest
        public void OnFulFillRequestEvent(BigNumber energy) => OnFulFillRequest?.Invoke(energy);
    }
}