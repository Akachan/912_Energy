using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnergyController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI value;

    [Header("Operaciones")]
    [SerializeField]  private BigNumber num1;
    [SerializeField] private BigNumber num2;
    [SerializeField] private float num3;

    



    public void ToBigNumber()
    {
        BigNumber numero = Calculator.TranformToBigNumber(value.text);

        if (numero == null) return;
        
        value.text = $"{numero.Base:#.##}e{numero.Exponent}";
    }

    public void ToDecimal()
    {
        int numero = Calculator.TransformToInt(value.text);
        
        value.text = $"{numero}";
    }

    public void SumarNumeros()
    {
        var bigNumber = Calculator.AddBigNumbers(num1, num2);
        value.text = $"{bigNumber.Base:#.########}e{bigNumber.Exponent}";
    }

    public void RestarNumeros()
    {
        var bigNumber = Calculator.SubtractBigNumbers(num1, num2);
        value.text = $"{bigNumber.Base:#.########}e{bigNumber.Exponent}";
    }

    public void MultiplicarNumeros()
    {
        var bigNumbers = Calculator.MultiplyBigNumbers(num1, num2);
        value.text = $"{bigNumbers.Base:#.########}e{bigNumbers.Exponent}";
    }

    public void MultiplicarPorFactor()
    {
        var bigNumbers = Calculator.MultiplyBigNumbers(num1, num3);
        value.text = $"{bigNumbers.Base:#.########}e{bigNumbers.Exponent}";
    }
    
    
    
    
}
