using System;

namespace Utilities
{
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
        public BigNumber()
        {
            Base = 0f;
            Exponent = 0;
        }
    }
}