using UnityEngine;

public abstract class Calculator
{
    private static BigNumber NormalizeBigNumber(BigNumber number)
    {
        while (number.Base >= 10)
        {
            number.Base /= 10;
            number.Exponent++;
        }
        while (number.Base < 1 && number.Base != 0)
        {
            number.Base *= 10;
            number.Exponent--;
        }
        return number;
    }

    private static void AdjustToSameExponent(ref BigNumber num1, ref BigNumber num2)
    {
        while (num1.Exponent > num2.Exponent)
        {
            num2.Base /= 10;
            num2.Exponent++;
        }
        while (num1.Exponent < num2.Exponent)
        {
            num1.Base /= 10;
            num1.Exponent++;
        }
    }

    public static BigNumber SumBigNumbers(BigNumber num1, BigNumber num2)
    {
        if (num1 == null || num2 == null) return null;

        BigNumber result = new BigNumber(0, 0);
        num1 = NormalizeBigNumber(new BigNumber((float)num1.Base, num1.Exponent));
        num2 = NormalizeBigNumber(new BigNumber((float)num2.Base, num2.Exponent));

        AdjustToSameExponent(ref num1, ref num2);

        result.Base = num1.Base + num2.Base;
        result.Exponent = num1.Exponent;

        return NormalizeBigNumber(result);
    }
        
        public static BigNumber TranformToBigNumber(string value)
        {
            
            int exponent = value.Length-1;
            if (exponent <4) return null;
            
            if (int.TryParse(value, out int intValue))
            {
                var baseValue = intValue/(Mathf.Pow(10, exponent));
                return new BigNumber(baseValue, exponent);
            }
            else
            {
                Debug.LogError("El valor ingresado no es un entero");
                return null;
            }
            
        }

        public static int TransformToInt(BigNumber value)
        {
            var newValue = (int)(value.Base*Mathf.Pow((float)10,value.Exponent));

            return newValue;
        }

        public static int TransformToInt(string value)
        {
            var newValue = value.Split("e");
            Debug.Log($"base: {newValue[0]} exponent: {newValue[1]}");
            var baseValue = float.Parse(newValue[0]);
            var exponent = int.Parse(newValue[1]);

            return TransformToInt(new BigNumber(baseValue, exponent));
        }
        
       
        
        
        
        
        
        
}