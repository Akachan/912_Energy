using System;
using System.Collections.Generic;
using UnityEngine;



public abstract class GeometricSourceSo : ScriptableObject
{
    [Header("Settings")]
    [SerializeField] private string sourceName;
    [SerializeField] private float priceGrowthRate;
    [SerializeField] private int maxLevel;
    [SerializeField] private  BigNumber baseCost;
    [SerializeField] private  BigNumber baseRps;
    [SerializeField] private List<Multipliers>  multipliersList;

    private Dictionary<int, float> _multipliers;
    private float _multiplier;

    [Serializable]
    public struct Multipliers
    {
        public int level;
        public float multiplier;
    }

    private void Init()
    {
        var mult = 1f;
        _multipliers = new Dictionary<int, float>();
        foreach (var multiplier in multipliersList)
        {
            mult *= multiplier.multiplier;
            _multipliers.Add(multiplier.level, mult);
        }
    }
    

    public string SourceName => sourceName;
    public BigNumber BaseCost => baseCost;
    public BigNumber BaseRps => baseRps;
    public float PriceGrowthRate => priceGrowthRate;
    public int MaxLevel => maxLevel;
    
    public BigNumber GetCost(int level)
    {
        var cost = Calculator.MultiplyBigNumbers(baseCost, Mathf.Pow(priceGrowthRate, level));
        return Calculator.TruncateDecimalBigNumber(cost);
    }

    public BigNumber GetCost(int level, int levelsToBuy)
    {
        if(level + levelsToBuy - 1 > maxLevel) return null;
        
        var factor = Mathf.Pow(priceGrowthRate, level) * (Mathf.Pow(priceGrowthRate, levelsToBuy) - 1) / (priceGrowthRate - 1);
        var cost = Calculator.MultiplyBigNumbers(baseCost, factor);
        
        return Calculator.TruncateDecimalBigNumber(cost);
    }
    
    public BigNumber GetRps(int level) 
    {
        var rps = Calculator.MultiplyBigNumbers(baseRps, level * GetMultiplier(level));
        return rps;
    }
        

    public BigNumber GetDifferenceRps(int level, int levelToCompare)
    {
        if(levelToCompare > maxLevel) return null;
        
        //lo dejo momentaneamente así porque a futuro puede tener multiplicadores que cambien las cosas y no sea tan lineal
        var currentRps = GetRps(level);
        var nextRps = GetRps(levelToCompare);
        var difference = Calculator.SubtractBigNumbers(nextRps, currentRps);
        return difference;
    }

    public BigNumber GetDifferenceRps(int level)
    {
        return GetDifferenceRps(level, level + 1);
    }

    public float GetMultiplier(int level)
    {
        if(_multipliers == null)
        {
            Init();
        }
        
        // Si existe exactamente, devolverlo
        if (_multipliers.TryGetValue(level, out var exact))
        {
            return exact;
        }

        // Buscar el nivel inferior más cercano
            var bestLevel = -1;
            foreach (var key in _multipliers.Keys)
            {
                if (key <= level && key > bestLevel)
                {
                    bestLevel = key;
                }
            }

        return bestLevel >= 0 ? _multipliers[bestLevel] : 1f;
    }



}