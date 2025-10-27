using System;
using Newtonsoft.Json.Linq;
using SavingSystem;
using Stats;
using Utilities;

namespace Milestones
{
    public class MilestoneManager : SmallResourceManager
    {
        private SavingWrapper _savingWrapper;
        public event Action<int> OnMilestoneChange;


        private void Awake()
        {
            _savingWrapper = FindFirstObjectByType<SavingWrapper>();
            CurrentResources = 0;
        }

        private void Start()
        {
            UpdateUI();
            Load();
        }
        

        public override void UpdateUI()
        {
            OnMilestoneChange?.Invoke(CurrentResources); //para la Ui
        }

        //STATS
        protected override void UpdateResourcesProducedStats(int resource)
        {
            EventStatBus.Instance.OnMilestoneProducedEvent(resource);
        }
        protected override void UpdateResourcesConsumedStats(int resource)
        {
            EventStatBus.Instance.OnMilestoneConsumedEvent(resource);
        }   
        
        //SAVING SYSTEM
        public override void Save()
        {
           _savingWrapper.SaveInFile(SavingKeys.Milestone.Current, JToken.FromObject(CurrentResources)); 
        }

        public override void Load()
        {
            var cm = _savingWrapper.GetSavingValue(SavingKeys.Milestone.Current);
            if (cm != null)
            {
                CurrentResources = cm.ToObject<int>();
                UpdateUI();
            }
        }

    }
}