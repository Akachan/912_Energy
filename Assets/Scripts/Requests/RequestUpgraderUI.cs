using System;
using Milestones;
using Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;


namespace Requests
{
    public class RequestUpgraderUI : MonoBehaviour
    {

        [Header("Population")] [SerializeField]
        private TextMeshProUGUI populationText;

        [SerializeField] private Button populationButton;
        [SerializeField] private TextMeshProUGUI populationButtonText;

        [Header("Industries")] [SerializeField]
        private TextMeshProUGUI industriesText;

        [SerializeField] private Button industriesButton;
        [SerializeField] private TextMeshProUGUI industriesButtonText;

        [Header("Commerce")] [SerializeField] private TextMeshProUGUI commerceText;
        [SerializeField] private Button commerceButton;
        [SerializeField] private TextMeshProUGUI commerceButtonText;

        private RequestUpgrader _requestUpgrader;
        private MilestoneManager _milestoneManager;

        private void Awake()
        {
            _requestUpgrader = GetComponent<RequestUpgrader>();
            _milestoneManager = FindFirstObjectByType<MilestoneManager>();
        }

        private void Start()
        {
            //UpdateButtons(_milestoneManager.GetResources());
            
            _milestoneManager.OnMilestoneChange += UpdateButtons;
            _requestUpgrader.OnUpgradePopulation += UpdatePopulation;
            _requestUpgrader.OnUpgradeIndustries += UpdateIndustries;
            _requestUpgrader.OnUpgradeCommerce += UpdateCommerce;
            UpdatePopulation();
            UpdateIndustries();
            UpdateCommerce();   
        }

        private void OnDisable()
        {
           _milestoneManager.OnMilestoneChange -= UpdateButtons;
           _requestUpgrader.OnUpgradePopulation -= UpdatePopulation;
           _requestUpgrader.OnUpgradeIndustries -= UpdateIndustries;
           _requestUpgrader.OnUpgradeCommerce -= UpdateCommerce;
        }

        private void UpdateButtons(int resource)
        {
           
            populationButton.interactable = resource >= _requestUpgrader.GetPopulation().cost;
            industriesButton.interactable = resource >= _requestUpgrader.GetIndustries().cost;
            commerceButton.interactable = resource >= _requestUpgrader.GetCommerce().cost;
            
        }
        
        
        private void UpdatePopulation()
        {

            var lvl = _requestUpgrader.PopulationLevel;
            var currentLevelData = _requestUpgrader.GetPopulation();
            var nextLevelData = _requestUpgrader.GetPopulation(lvl + 1);
            
            var nextLevelText = nextLevelData == null ? "Max Level" : $"NextLevel: {nextLevelData.Value.spawnTime}s";
            var text = $"Level: {lvl} \n SpawnTime: {currentLevelData.spawnTime}s \n {nextLevelText}";


            populationText.text = text;
            populationButtonText.text = currentLevelData.cost.ToString();
            
            UpdateButtons(_milestoneManager.GetResources());
        }

        private void UpdateIndustries()
        {
            var lvl = _requestUpgrader.IndustriesLevel;
            var currentLevelData = _requestUpgrader.GetIndustries();
            var nextLevelData = _requestUpgrader.GetIndustries(lvl + 1);

            var nextLevelText = nextLevelData == null ? "Max Level" : $"NextLevel: {BigNumberFormatter.SetSuffixFormat(nextLevelData.Value.energy)}";
            var text = $"Level: {lvl} \n EnergyRequest: {BigNumberFormatter.SetSuffixFormat(currentLevelData.energy)} \n {nextLevelText}";
            
            industriesText.text = text;
            industriesButtonText.text = currentLevelData.cost.ToString();
            
            UpdateButtons(_milestoneManager.GetResources());
        }
        
        private void UpdateCommerce()
        {
            var lvl = _requestUpgrader.CommerceLevel;
            var currentLevelData = _requestUpgrader.GetCommerce();
            var nextLevelData = _requestUpgrader.GetCommerce(lvl + 1);
            
            var nextLevelText = nextLevelData == null ? "Max Level" : $"NextLevel: {nextLevelData.Value.ratio}cash/energy";
            var text = $"Level: {lvl} \n cash/energy: {currentLevelData.ratio}s \n {nextLevelText}";
            
            commerceText.text = text;
            commerceButtonText.text = currentLevelData.cost.ToString();
            
            UpdateButtons(_milestoneManager.GetResources());
        }




    }

}

