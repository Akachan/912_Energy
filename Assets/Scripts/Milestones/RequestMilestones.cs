using System;
using Newtonsoft.Json.Linq;
using SavingSystem;
using Stats;
using UnityEngine;
using UnityEngine.AdaptivePerformance.Google.Android;
using Utilities;

namespace Milestones
{
    public class RequestMilestones : MonoBehaviour, ISaveable
    {
        [SerializeField] private RequestMilestoneGoals[] goals;
        private int _currentGoalToReach = 0;
        private MilestoneManager _milestoneManager;
        private SavingWrapper _saving;
        public event Action<BigNumber> OnRequestMilestoneGoalReached;
        public event Action<BigNumber> OnRequestMilestneAdvance;


        private void Awake()
        {
            _milestoneManager = GetComponent<MilestoneManager>();
            _saving = FindFirstObjectByType<SavingWrapper>();
            Load();
        }

        private void Start()
        {
            StatsManager.EnergyStat.OnEnergyTradeStatChange += VerifyGoals;
        }
        private void VerifyGoals(BigNumber tradedEnergy)
        {
            if (_currentGoalToReach >= goals.Length) return;
            Calculator.CompareBigNumbers(tradedEnergy, goals[_currentGoalToReach].energyGoal, out var result);
            if (result == ComparisonResult.Bigger || result == ComparisonResult.Equal)
            {
                ReachGoal(tradedEnergy);
            }
            else
            {
                OnRequestMilestneAdvance?.Invoke(tradedEnergy);
            }
        
        }

        private void ReachGoal(BigNumber tradedEnergy)
        {
            _milestoneManager.AddResources(goals[_currentGoalToReach].milestoneReward);
            _currentGoalToReach++;
            Save();
            OnRequestMilestoneGoalReached?.Invoke(tradedEnergy);
           
        }

        public RequestMilestoneGoals GetGoal()
        {
            //le pasa el dato de lo que necesita conseguir y lo que obtiene a futuro verificar
            //si conviene volverlo a cero.
            if (_currentGoalToReach >= goals.Length)
            {
                return null;
            }
            return goals[_currentGoalToReach];
        }
        
        public void Save()
        {
            _saving.SaveInFile(SavingKeys.Milestone.Goals, JToken.FromObject(_currentGoalToReach));
        }

        public void Load()
        {
            var current = _saving.GetSavingValue(SavingKeys.Milestone.Goals);
            if (current != null)
            {
                _currentGoalToReach = current.ToObject<int>();
            }   
        }
    }
    
    

    [Serializable]
    public class RequestMilestoneGoals
    {
        public BigNumber energyGoal;
        public int milestoneReward;
    }
    
    
   
    
}