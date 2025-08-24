using System;
[Serializable]
public class BigNumber
{
    public double Base;
    public int Exponent;
        
    
    public BigNumber(float baseValue, int exponent)
    {
        Base = baseValue;
        Exponent = exponent;
    }
}