using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnergyManager : BigResourceManager
{
       private BigNumber _eps;
       private Dictionary <string, BigNumber> _energySources;
       private EnergyUI _ui;
       private float _currentTime;
       public event Action<BigNumber> OnEnergySourceChange; 

       public BigNumber Eps => _eps;
       private void Awake()
       {
              _ui = GetComponent<EnergyUI>();
              CurrentResources = new BigNumber(0, 0);
              _eps = new BigNumber(0, 0);
       }

       void Start()
       {
              Load();
              FindFirstObjectByType<Battery.Battery>().OnPause += SaveEps;
       }

       private void Update()
       {
              UpdateResources();
       }

       private void UpdateResources()
       {
              _currentTime += Time.deltaTime;
              if (_currentTime >= 1f)
              {
                     _currentTime = 0f;
                     AddResources(_eps);
                     
                     _ui.SetEnergyValue(CurrentResources);
            
                     Save();
              }
       }
       public void UpdateSource(string sourceName, BigNumber eps)
       {
              if (_energySources == null)
              {
                     _energySources = new Dictionary<string, BigNumber>();
              }

              _energySources[sourceName] = new BigNumber(eps.Base, eps.Exponent);

              _eps = CalculateNewEnergy();

              OnEnergySourceChange?.Invoke(_eps);
       }
       private BigNumber CalculateNewEnergy()
       { 
              if(_energySources.Count == 0) return new BigNumber(0, 0);
      
              var newEnergy = new BigNumber(0, 0);
              foreach (var source in _energySources)
              {
                     newEnergy = Calculator.AddBigNumbers(newEnergy, source.Value);
              }
      
              _ui.SetEpsValue(newEnergy);
              return newEnergy;
       }

  

       //GUARDADO//
       public override void Save()
       {
              PlayerPrefs.SetFloat("EnergyBase", (float)CurrentResources.Base);
              PlayerPrefs.SetInt("EnergyExponent", CurrentResources.Exponent);
       }
       public override void Load()
       {
              if (!PlayerPrefs.HasKey("EnergyBase")) return;
              CurrentResources = new BigNumber(PlayerPrefs.GetFloat("EnergyBase"), PlayerPrefs.GetInt("EnergyExponent"));
              _ui.SetEnergyValue(CurrentResources);
       }
       private void SaveEps() //para la battery
       {
              PlayerPrefs.SetFloat("EpsBase", (float)_eps.Base);
              PlayerPrefs.SetInt("EpsExponent", _eps.Exponent);
              PlayerPrefs.Save(); // Fuerza el guardado inmediato
       }
}