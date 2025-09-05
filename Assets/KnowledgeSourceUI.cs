using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utilities;

public class KnowledgeSourceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI knowledgeLevelText;
    [SerializeField] private TextMeshProUGUI knowledgeKps;
    [SerializeField] private TextMeshProUGUI knowledgeNextLevel;
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    
    
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
}
