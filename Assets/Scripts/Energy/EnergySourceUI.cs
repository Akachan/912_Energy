using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Energy
{
    public class EnergySourceUI : MonoBehaviour
    {   [Header("References")]
        [SerializeField] private Image illustration;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI epsText;
        [SerializeField] private TextMeshProUGUI ratioText;
        [SerializeField] private TextMeshProUGUI nextLevelEpsText;
        [SerializeField] private TextMeshProUGUI unlockCostText;
        [SerializeField] private TextMeshProUGUI upgradeCostText;
        [SerializeField] private Button unlockButton;
        [SerializeField] private Button upgradeButton;

      

        public void SetLockedEnergySourceData(BigNumber firstLevelEps, BigNumber costToUnlock)
        {
            titleText.text = "???";
            epsText.text = "";
            ratioText.text = "";
            nextLevelEpsText.text = $"NextEPS: +{BigNumberFormatter.SetSuffixFormat(firstLevelEps)}";
            unlockCostText.text = $"{BigNumberFormatter.SetSuffixFormat(costToUnlock)}";
            upgradeCostText.text = "";
        
            unlockButton.gameObject.SetActive(true);
            upgradeButton.gameObject.SetActive(false);
        }

        public void SetUnlockedEnergySourceData(string energySourceName, int level, BigNumber eps, BigNumber nextLevelEps, BigNumber upgradeCost, Sprite newSprite)
        {
            illustration.sprite = newSprite;
            titleText.text = energySourceName;
            UpdateEnergySourceData(level, eps, nextLevelEps, upgradeCost);
            unlockButton.gameObject.SetActive(false);
            upgradeButton.gameObject.SetActive(true);
        }

        public void UpdateEnergySourceData(int level, BigNumber eps, BigNumber nextLevelEps, BigNumber upgradeCost)
        {
            levelText.text = $"Level: {level}";
            epsText.text = $"EPS: {BigNumberFormatter.SetSuffixFormat(eps)}";
            nextLevelEpsText.text = $"NextEPS: +{BigNumberFormatter.SetSuffixFormat(nextLevelEps)}";
            upgradeCostText.text = $"{BigNumberFormatter.SetSuffixFormat(upgradeCost)}";
  
        }

        public void UpdateLastLevelEnergySourceData( int level, BigNumber eps)
        {
            levelText.text = $"Level: {level}";
            epsText.text = $"EPS: {BigNumberFormatter.SetSuffixFormat(eps)}";
            nextLevelEpsText.text = "";
            upgradeButton.gameObject.SetActive(false);
        }
        public void UpdateLastLevelEnergySourceData(string energySourceName, Sprite newSprite, int level, BigNumber eps)
        {
            UpdateLastLevelEnergySourceData(level, eps);
            titleText.text = energySourceName;
            illustration.sprite = newSprite;
            unlockButton.gameObject.SetActive(false);
        }

        public void UpdateRatioText(BigNumber ratio)
        {
            var newRatio = GetFloatRatio(ratio);
            ratioText.text = $"Ratio: {newRatio*100:0.00}%";
        }

        private float GetFloatRatio(BigNumber ratio)
        {
            if (ratio == null)
            {
                return 0f;
            }
            switch (ratio.Exponent)
            {
                case > 0:
                    Debug.LogError("error en el cálculo del ratio");
                    break;
                case < -4:
                    return 0f;
            }

            var newRatio = ratio.Base * Mathf.Pow(10, ratio.Exponent);
            return (float)newRatio;
        }

        public void SetUnlockButtonState(bool state)
        {
            unlockButton.interactable = state;
        }
        public void SetUpgradeButtonState(bool state)
        {
            upgradeButton.interactable = state;
        }
    
    
    }
}
