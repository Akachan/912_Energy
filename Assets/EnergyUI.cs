using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnergyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI energyValue;
    [SerializeField] private TextMeshProUGUI epsValue;

    public void SetEnergyValue(BigNumber value)
    {
        energyValue.text = $"{value.Base:#.########}e{value.Exponent}";
    }
    public void SetEpsValue(BigNumber value)
    {
        epsValue.text = $"{value.Base:#.########}e{value.Exponent}";
    }
}
