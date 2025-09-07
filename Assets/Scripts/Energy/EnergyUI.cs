using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utilities;

public class EnergyUI : MonoBehaviour
{
    [Header("TopPanel")]
    [SerializeField] private TextMeshProUGUI energyValue;
    [SerializeField] private TextMeshProUGUI epsValue;

    public void SetEnergyValue(BigNumber value)
    {
        energyValue.text = BigNumberFormatter.SetSuffixFormat(value);
    }
    public void SetEpsValue(BigNumber value)
    {
        epsValue.text = $"EPS: {BigNumberFormatter.SetSuffixFormat(value)}";
    }
}
