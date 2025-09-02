using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergySourceUI : MonoBehaviour
{   [Header("References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI epsText;
    [SerializeField] private TextMeshProUGUI ratioText;
    [SerializeField] private TextMeshProUGUI nextLevelEpsText;
    [SerializeField] private TextMeshProUGUI unlockCostText;
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private Button unlockButton;
    [SerializeField] private Button upgradeButton;

    private Energy _energy;
    private void Awake()
    {
        _energy = FindObjectOfType<Energy>();
    }


    public void SetLockedEnergySourceData(BigNumber firstLevelEps, BigNumber costToUnlock)
    {
        titleText.text = "???";
        epsText.text = "";
        ratioText.text = "";
        nextLevelEpsText.text = $"NextEPS: +{firstLevelEps.Base:#.####}e{firstLevelEps.Exponent}";
        unlockCostText.text = $"Unlock: {costToUnlock.Base}e{costToUnlock.Exponent}";
        upgradeCostText.text = "";
        
        unlockButton.gameObject.SetActive(true);
        upgradeButton.gameObject.SetActive(false);
    }

    public void SetUnlockedEnergySourceData(string energySourceName, int level, BigNumber eps, BigNumber ratio, BigNumber nextLevelEps, BigNumber upgradeCost)
    {
        
        titleText.text = energySourceName;
        UpdateEnergySourceData(level, eps, ratio, nextLevelEps, upgradeCost);
        unlockButton.gameObject.SetActive(false);
        upgradeButton.gameObject.SetActive(true);
    }

    public void UpdateEnergySourceData(int level, BigNumber eps, BigNumber ratio, BigNumber nextLevelEps, BigNumber upgradeCost)
    {
        levelText.text = $"Level: {level}";
        epsText.text = $"EPS: {eps.Base:#.########}e{eps.Exponent}";
        
        var newRatio = GetFloatRatio(ratio);
        ratioText.text = $"Ratio: {newRatio*100:0.00}%";
        nextLevelEpsText.text = $"NextEPS: +{nextLevelEps.Base:#.####}e{nextLevelEps.Exponent}";
        upgradeCostText.text = $"Upgrade: {upgradeCost.Base:#.########}e{upgradeCost.Exponent}";
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
