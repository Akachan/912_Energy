using System;
using UnityEngine;

public abstract class Calculator
{
    private const int Threshold = 16;
    private static double Pow10(int e)
    {
        return Math.Pow(10.0, e);
    }

    public static BigNumber NormalizeBigNumber(BigNumber value)
    {
        if (value == null) return new BigNumber(0d, 0);

        double b = value.Base;
        int e = value.Exponent;

        // Caso cero: 0 × 10^0
        if (b == 0d || double.IsNaN(b) || double.IsInfinity(b))
            return new BigNumber(0d, 0);

        const double eps = 1e-12;

        // Signo y magnitud
        double sign = Math.Sign(b);
        double a = Math.Abs(b);

        // Desplazamiento usando log10
        // k dice cuántas potencias de 10 hay que mover
        int k = (int)Math.Floor(Math.Log10(a) + eps);

        // Mantisa preliminar y exponente acumulado
        double m = a / Math.Pow(10d, k);
        int newExp = e + k;

        // Ajuste único por arrastre numérico en los bordes
        if (m >= 10d - eps)
        {
            m /= 10d;
            newExp++;
        }
        else if (m < 1d - eps)
        {
            m *= 10d;
            newExp--;
        }

        m *= sign;

        // Evitar -0 y normalizar cero
        if (Math.Abs(m) < eps)
            return new BigNumber(0d, 0);

        return new BigNumber(m, newExp);
    }

    private static void AdjustToSameExponent(ref BigNumber num1, ref BigNumber num2)
    {
        
        //elimino el while
        int delta = num1.Exponent - num2.Exponent;
        if (delta > 0)
        {
            num2.Base /= Pow10(delta);
            num2.Exponent += delta;
        }
        else if (delta < 0)
        {
            int d = -delta;
            num1.Base /= Pow10(d);
            num1.Exponent += d;
        }
    }

    public static BigNumber AddBigNumbers(BigNumber num1, BigNumber num2)
    {
        if (num1 == null || num2 == null) return null;

        num1 = NormalizeBigNumber(new BigNumber(num1.Base, num1.Exponent));
        num2 = NormalizeBigNumber(new BigNumber(num2.Base, num2.Exponent));

        if (EvaluateExponentProximity(num1, num2, out var bigNumber)) return bigNumber;

        AdjustToSameExponent(ref num1, ref num2);

        var result = new BigNumber(num1.Base + num2.Base, num1.Exponent);
        
        return NormalizeBigNumber(result);
    }

    private static bool EvaluateExponentProximity(BigNumber num1, BigNumber num2, out BigNumber bigNumber)
    {
        //Si los exponentes son muy lejanos NO SUMA
        var delta = num1.Exponent - num2.Exponent;
        if (delta > Threshold)
        {
            bigNumber = num1;
            return true;
        } 
        if (delta < -Threshold)
        {
            bigNumber = num2;
            return true;
        }

        bigNumber = null;
        return false;
    }

    public static BigNumber SubtractBigNumbers(BigNumber num1, BigNumber num2)
    {
        if (num1 == null || num2 == null) return null;
        
        num1 = NormalizeBigNumber(new BigNumber(num1.Base, num1.Exponent));
        num2 = NormalizeBigNumber(new BigNumber(num2.Base, num2.Exponent));
        
        if (EvaluateExponentProximity(num1, num2, out var bigNumber)) return bigNumber;
        
        AdjustToSameExponent(ref num1, ref num2);
        
        var result = new BigNumber(num1.Base - num2.Base, num1.Exponent);
        return NormalizeBigNumber(result);
        
    }

    public static BigNumber MultiplyBigNumbers(BigNumber num1, BigNumber num2)
    {
        if (num1 == null || num2 == null) return null;
        
        num1 = NormalizeBigNumber(new BigNumber(num1.Base, num1.Exponent));
        num2 = NormalizeBigNumber(new BigNumber(num2.Base, num2.Exponent));
        
        var result = new BigNumber(num1.Base * num2.Base, num1.Exponent + num2.Exponent);
        return NormalizeBigNumber(result); 
    }

    public static BigNumber MultiplyBigNumbers(BigNumber num1, float num2)
    {
        if (num1 == null) return null;
        
        var result = new BigNumber(num1.Base * num2, num1.Exponent);
        return NormalizeBigNumber(result);
    }
    

    public static BigNumber DivideBigNumbers(BigNumber num1, BigNumber num2)
    {
        if (num1 == null || num2 == null) return null;
        if (num2.Base == 0.0) return null;

        num1 = NormalizeBigNumber(new BigNumber(num1.Base, num1.Exponent));
        num2 = NormalizeBigNumber(new BigNumber(num2.Base, num2.Exponent));

        var result = new BigNumber(num1.Base / num2.Base, num1.Exponent - num2.Exponent);
        return NormalizeBigNumber(result);
    }

    public static BigNumber TransformToBigNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Debug.LogError("El valor ingresado está vacío o es nulo");
            return null;
        }

        value = value.Trim();

        //primero se fija si es un double
        // Soporta enteros, decimales y notación científica (e/E) con punto decimal invariante
        if (double.TryParse(value, System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out var parsed))
        {
            if (double.IsNaN(parsed) || double.IsInfinity(parsed))
            {
                Debug.LogError("El valor ingresado no es un número finito");
                return null;
            }

            return NormalizeBigNumber(new BigNumber(parsed, 0));
        }

        //Si no es un double supone que es notación científica
        // Fallback: notación científica con separación manual (admite espacios y signos)
        int ePos = value.IndexOfAny(new[] { 'e', 'E' });
        if (ePos <= 0 || ePos >= value.Length - 1)
        {
            Debug.LogError("El valor ingresado no tiene un formato numérico válido (ni simple ni en notación científica).");
            return null;
        }

        var basePart = value.Substring(0, ePos).Trim();
        var expPart = value.Substring(ePos + 1).Trim();

        bool okBase = double.TryParse(basePart, System.Globalization.NumberStyles.Float,
                                  System.Globalization.CultureInfo.InvariantCulture, out var baseValue);
        bool okExp = int.TryParse(expPart, System.Globalization.NumberStyles.Integer,
                              System.Globalization.CultureInfo.InvariantCulture, out var exponent);

        if (!okBase || !okExp)
        {
            Debug.LogError("El valor ingresado no tiene notación científica válida");
            return null;
        }

        return NormalizeBigNumber(new BigNumber(baseValue, exponent));
    }

    public static void CompareBigNumbers(BigNumber num1, BigNumber num2, out ComparisonResult result)
    {
        var delta = SubtractBigNumbers(num1, num2);

        result = delta.Base switch
        {
            > 0 => ComparisonResult.Bigger,
            < 0 => ComparisonResult.Smaller,
            _ => ComparisonResult.Equal
        };
    }
    
    //Cosas que probablemente no use
    
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
public enum ComparisonResult
{
    Bigger,
    Smaller,
    Equal
}