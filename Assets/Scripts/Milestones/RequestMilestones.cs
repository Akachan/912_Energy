using System;
using Stats;
using UnityEngine;
using Utilities;

namespace Milestones
{
    public class RequestMilestones : MonoBehaviour
    {
        [SerializeField] private RequestMilestoneGoals[] goals;
        private int _currentGoalToReach = 0;

        private void Start()
        {
            StatsManager.EnergyStat.OnEnergyTradeStatChange += VerifyGoals;
        }
        private void VerifyGoals(BigNumber energy)
        {
        
        }
        
        
    }
    
    

    [Serializable]
    public class RequestMilestoneGoals
    {
        public BigNumber energyGoal;
        public int milestoneReward;
    }
    
    
   
    
}