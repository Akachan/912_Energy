using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : MonoBehaviour
{
   [SerializeField] private BigNumber debugEnergy = new BigNumber(1, 2);
   
   private BigNumber _currentEnergy;
   private Dictionary<string, BigNumber> _energySources;
   private float _currentTime = 0f;
   private BigNumber _energyToAdd;

   private EnergyUI _energyUi;

   private void Awake()
   {
      _energyUi = GetComponent<EnergyUI>();
      _currentEnergy = new BigNumber(0, 0);
      _energyToAdd = new BigNumber(0, 0);
   }
   
   private void Update()
   {
      _currentTime += Time.deltaTime;
      if (_currentTime >= 1f)
      {
         _currentTime = 0f;
         AddNewEnergy();
         _energyUi.SetEnergyValue(_currentEnergy);
      }
   }

   public BigNumber GetCurrentEnergy()
   {
      return _currentEnergy;
   }

   public BigNumber GetEps()
   {
      return _energyToAdd;
   }
   private void AddNewEnergy()
   {
      _currentEnergy = Calculator.AddBigNumbers(_currentEnergy, _energyToAdd);
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
      
      _energyUi.SetEpsValue(newEnergy);
      return newEnergy;
   }

   public BigNumber UpdateSourceAndReturnRatio(string sourceName, BigNumber eps)
   {
      if (_energySources == null)
      {
         _energySources = new Dictionary<string, BigNumber>();
      }

      _energySources[sourceName] = new BigNumber(eps.Base, eps.Exponent);

      _energyToAdd = CalculateNewEnergy();
      
      var ratio = Calculator.DivideBigNumbers(eps, _energyToAdd);
      return ratio;
   }

   public BigNumber GetRatio(string sourceName)
   {
      if (_energySources == null) return null;
      if (!_energySources.TryGetValue(sourceName, out var eps)) return null;
      var ratio = Calculator.DivideBigNumbers(eps, _energyToAdd);
      return ratio;
   }
   
   

   
   [ContextMenu("Add Energy")]
   public void AddEnergy()
   {
      _currentEnergy = Calculator.AddBigNumbers(_currentEnergy, debugEnergy);
   }
   
   [ContextMenu("Remove Energy")]
   public void RemoveEnergy()
   {
      _currentEnergy = Calculator.SubtractBigNumbers(_currentEnergy, debugEnergy);
   }
   
   
   
   
   
   
}
