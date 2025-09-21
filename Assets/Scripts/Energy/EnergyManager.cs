using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Energy
{
   public class EnergyManager : MonoBehaviour
   {
      [SerializeField] private BigNumber debugEnergy = new BigNumber(1, 2);
   
      public event Action<BigNumber> OnEnergySourceChange; 
      private BigNumber _currentEnergy;
      private Dictionary<string, BigNumber> _energySources;
      private float _currentTime = 9f;
      private BigNumber _energyToAdd;

      private EnergyUI _energyUi;

      private void Awake()
      {
         _energyUi = GetComponent<EnergyUI>();
         _currentEnergy = new BigNumber(0, 0);
         _energyToAdd = new BigNumber(0, 0);
         
         
         
      }

      private void Start()
      {
         if (PlayerPrefs.HasKey("EnergyBase"))
         {
            _currentEnergy = new BigNumber(PlayerPrefs.GetFloat("EnergyBase"), PlayerPrefs.GetInt("EnergyExponent"));
            _energyUi.SetEnergyValue(_currentEnergy);
         }
      }

      private void Update()
      {
         ///////DEBUG
         /*
         if(Input.GetKeyDown(KeyCode.B))
         {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
         }
         */
         ///////DEBUG
         
         
         
         _currentTime += Time.deltaTime;
         if (_currentTime >= 1f)
         {
            _currentTime = 0f;
            AddNewEnergy();
            _energyUi.SetEnergyValue(_currentEnergy);
            
            //todo: Guardar energia nueva
            PlayerPrefs.SetFloat("EnergyBase", (float)_currentEnergy.Base);
            PlayerPrefs.SetInt("EnergyExponent", _currentEnergy.Exponent);
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

      public void UpdateSource(string sourceName, BigNumber eps)
      {
         if (_energySources == null)
         {
            _energySources = new Dictionary<string, BigNumber>();
         }

         _energySources[sourceName] = new BigNumber(eps.Base, eps.Exponent);

         _energyToAdd = CalculateNewEnergy();

         OnEnergySourceChange?.Invoke(_energyToAdd);
      }

   
      [ContextMenu("Add Energy")]
      public void AddEnergy()
      {
         AddEnergy(debugEnergy);
      }
      
      public void AddEnergy(BigNumber energyToAdd)
      {
         _currentEnergy = Calculator.AddBigNumbers(_currentEnergy, energyToAdd);
         print("se agregó energia: " + energyToAdd + "");
         //todo: Guardar energia nueva
         PlayerPrefs.SetFloat("EnergyBase", (float)_currentEnergy.Base);
         PlayerPrefs.SetInt("EnergyExponent", _currentEnergy.Exponent);
         
      }
   
      [ContextMenu("Remove Energy")]
      public void RemoveEnergy()
      {
         _currentEnergy = Calculator.SubtractBigNumbers(_currentEnergy, debugEnergy);
      }
      
      private void OnDestroy()
      {
         var eps = GetEps();
         PlayerPrefs.SetFloat("EpsBase", (float)eps.Base);
         PlayerPrefs.SetInt("EpsExponent", eps.Exponent);;
      }
   
   
   
   
   
   
   }
}
