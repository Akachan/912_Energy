using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utilities;

public class KnowledgeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI knowledgeValueText;

    public void SetKnowledgeValue(BigNumber value)
    {

        //energyValue.text = $"{value.Base:#.########}e{value.Exponent}";
        knowledgeValueText.text = BigNumberFormatter.SetSuffixFormat(value);
    }
}
