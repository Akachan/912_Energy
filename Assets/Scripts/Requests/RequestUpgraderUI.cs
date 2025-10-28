using System;
using Milestones;
using Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


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
            UpdateButtons(_milestoneManager.GetResources());
            StatsManager.MilestoneStat.OnProducedStatChange += UpdateButtons;
            StatsManager.MilestoneStat.OnConsumedStatChange += UpdateButtons;
            _requestUpgrader.OnUpgradePopulation += UpdatePopulation;
            _requestUpgrader.OnUpgradeIndustries += UpdateIndustries;
            _requestUpgrader.OnUpgradeCommerce += UpdateCommerce;
        }

        private void OnDisable()
        {
            StatsManager.MilestoneStat.OnProducedStatChange -= UpdateButtons;
            StatsManager.MilestoneStat.OnConsumedStatChange -= UpdateButtons;
        }

        private void UpdateButtons(int resource)
        {
            print("se actualizo los botones");
            var milestone = _milestoneManager.GetResources();

            var popCost = _requestUpgrader.GetPopulation().cost;
            populationButton.interactable = milestone >= _requestUpgrader.GetPopulation().cost;



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

            if (currentLevelData == null) return;

            var nextLevelText = nextLevelData == null ? "Max Level" : $"NextLevel: {nextLevelData.Value.energy}";
            var text = $"Level: {lvl} \n EnergyRequest: {currentLevelData.Value.energy}s \n {nextLevelText}";


            industriesText.text = text;
            industriesButtonText.text = currentLevelData.Value.cost.ToString();
            
            UpdateButtons(_milestoneManager.GetResources());
        }
        
        private void UpdateCommerce()
        {
            var lvl = _requestUpgrader.CommerceLevel;
            var currentLevelData = _requestUpgrader.GetCommerce();
            var nextLevelData = _requestUpgrader.GetCommerce(lvl + 1);
            
            if (currentLevelData ==null) return;
            
            var nextLevelText = nextLevelData == null ? "Max Level" : $"NextLevel: {nextLevelData.Value.ratio}cash/energy";
            var text = $"Level: {lvl} \n cash/energy: {currentLevelData.Value.ratio}s \n {nextLevelText}";


            commerceText.text = text;
            commerceButtonText.text = currentLevelData.Value.cost.ToString();
            
            UpdateButtons(_milestoneManager.GetResources());
        }




    }

}

