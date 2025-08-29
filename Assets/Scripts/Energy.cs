using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : MonoBehaviour
{
   private BigNumber _currentEnergy;
   private Dictionary<string, BigNumber> _energySources;
   private float _currentTime = 0f;


   private void Start()
   {
      _energySources = new Dictionary<string, BigNumber>();
      _currentEnergy = new BigNumber(0, 0);
   }

   private void Update()
   {
      _currentTime += Time.deltaTime;
      if (_currentTime >= 1f)
      {
         _currentTime = 0f;
         AddNewEnergy();
      }
      
   }

   public BigNumber GetCurrentEnergy()
   {
      return _currentEnergy;
   }
   private void AddNewEnergy()
   {
      var newEnergy = CalculateNewEnergy();
      _currentEnergy = Calculator.AddBigNumbers(_currentEnergy, newEnergy);
   }

   public bool RemoveEnergy(BigNumber value)
   {
      Calculator.CompareBigNumbers(_currentEnergy, value, out var result);

      if (result != ComparisonResult.Bigger && result != ComparisonResult.Equal) return false;
      
      _currentEnergy = Calculator.SubtractBigNumbers(_currentEnergy, value);
      return true;
   }

   private BigNumber CalculateNewEnergy()
   { 
      if(_energySources.Count == 0) return new BigNumber(0, 0);
      
      var newEnergy = new BigNumber(0, 0);
      foreach (var source in _energySources)
      {
         newEnergy = Calculator.AddBigNumbers(newEnergy, source.Value);
      }
      return newEnergy;
   }
   
   public void UpdateSource(string sourceName, BigNumber Eps)
   {
      
      _energySources[sourceName] = new BigNumber(Eps.Base, Eps.Exponent);
      
      Debug.Log($"Energy: {_energySources.ContainsKey(sourceName)}");
   }
   
   private void RemoveSource(string name)
   {
      
   }
   
   
   
   
   
   
}
