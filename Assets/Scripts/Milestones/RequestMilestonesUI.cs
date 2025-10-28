using System;
using Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Milestones
{
    public class RequestMilestonesUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI requestMilestoneText;
        [SerializeField] private Image sprite;
        
        private RequestMilestones _requestMilestone;
        
        private void Awake()
        {
            _requestMilestone = GetComponent<RequestMilestones>();  
            
        }

        private void Start()
        {
            _requestMilestone.OnRequestMilestneAdvance += UpdateUi;
            _requestMilestone.OnRequestMilestoneGoalReached += SetNewGoal;
            InitializeUi();
        }

        private void InitializeUi()
        {
            var energyTrade = StatsManager.EnergyStat.Trade;
            UpdateUi(energyTrade);
        }

        private void OnDisable()
        {
            _requestMilestone.OnRequestMilestneAdvance -= UpdateUi;
        }

        private void UpdateUi(BigNumber tradedEnergy)
        {
            var data = _requestMilestone.GetGoal();
            
            if (data == null)
            {   
                requestMilestoneText.text = "No goals";
                sprite.fillAmount = 0f;
                return;
            }
            
            BigNumber ratio = Calculator.DivideBigNumbers(tradedEnergy, data.energyGoal);
            float fRatio = Calculator.GetFloatRatio(ratio);
            fRatio = Mathf.Clamp(fRatio, 0f, 1f);
            
         
            requestMilestoneText.text = $"{BigNumberFormatter.SetSuffixFormat(tradedEnergy)} / {BigNumberFormatter.SetSuffixFormat(data.energyGoal)} => Reward: {data.milestoneReward}";
            
            if(sprite == null)return;
            sprite.fillAmount = fRatio;
            
        }

        private void SetNewGoal(BigNumber tradedEnergy)
        {
            
            //por ahora va a ser así
            UpdateUi(tradedEnergy);
        }
    }
}