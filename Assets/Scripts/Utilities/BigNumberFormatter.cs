using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public static class BigNumberFormatter
    {
        private static readonly List<string> Suffixes = new List<string>()
        {
            "",
            "K",
            "M",
            "B",
            "T",
            "Qa",
            "Qi",
            "Sx",
            "Sp",
            "Oc"
        };
        public static string SetSuffixFormat(BigNumber bigNumber)
        {
            var suf = Mathf.FloorToInt(bigNumber.Exponent / 3f);
            var exp = bigNumber.Exponent - suf*3;
            
            if (suf >= Suffixes.Count) return $"{bigNumber.Base:#.##}e{bigNumber.Exponent}";
            
            var suffix = Suffixes[suf];
            var baseValue = bigNumber.Base* Mathf.Pow(10,exp);

            string numberFormatted;
            if (baseValue == 0)
            {
                 numberFormatted = $"0{suffix}";
            }
            else
            {
                 numberFormatted = $"{baseValue:#.##}{suffix}";
            }
            
            return numberFormatted;
        }
    }
}