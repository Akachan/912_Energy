using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Knowledge
{
    public class KnowledgeSourceUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI knowledgeLevelText;
        [SerializeField] private TextMeshProUGUI knowledgeKps;
        [SerializeField] private TextMeshProUGUI knowledgeNextLevel;
        [SerializeField] private TextMeshProUGUI upgradeCostText;
        [SerializeField] private Button upgradeButton;
    
        public void UpdateKnowledgeData(int level, BigNumber kps, BigNumber nextLevelKps, BigNumber upgradeCost)
        {
            knowledgeLevelText.text = $"Level: {level}";
            //epsText.text = $"EPS: {eps.Base:#.########}e{eps.Exponent}";
            knowledgeKps.text = $"KPS: {BigNumberFormatter.SetSuffixFormat(kps)}";
            //nextLevelEpsText.text = $"NextEPS: +{nextLevelEps.Base:#.####}e{nextLevelEps.Exponent}";
            knowledgeNextLevel.text = $"NextKPS: +{BigNumberFormatter.SetSuffixFormat(nextLevelKps)}";
            //upgradeCostText.text = $"Upgrade: {upgradeCost.Base:#.########}e{upgradeCost.Exponent}";
            upgradeCostText.text = $"{BigNumberFormatter.SetSuffixFormat(upgradeCost)}";
  
        }

        public void UpdateLastLevelKnowledgeSourceData(int level, BigNumber kps)
        {
            knowledgeLevelText.text = $"Level: {level}";
            knowledgeKps.text = $"KPS: {BigNumberFormatter.SetSuffixFormat(kps)}";
            knowledgeNextLevel.text = $"";
            upgradeCostText.text = $"MAX";
        }

        public void SetUpgradeButtonState(bool state)
        {
            upgradeButton.interactable = state;
        }
    }
}
