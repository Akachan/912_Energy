using System;
[Serializable]
public class BigNumber
{
    public double Base;
    public int Exponent;
        
    
    public BigNumber(double baseValue, int exponent)
    {
        Base = baseValue;
        Exponent = exponent;
    }
}