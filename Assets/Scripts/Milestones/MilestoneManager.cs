using System;
using Newtonsoft.Json.Linq;
using SavingSystem;

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
            OnMilestoneChange?.Invoke(CurrentResources);
        }

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