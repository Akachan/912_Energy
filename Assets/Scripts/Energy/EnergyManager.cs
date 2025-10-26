using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SavingSystem;
using UnityEngine;
using Utilities;

namespace Energy
{
       public class EnergyManager : BigResourceManager
       {
              private BigNumber _eps;
              private Dictionary <string, BigNumber> _energySources;
              private EnergyUI _ui;
              private SavingWrapper _saving;
              private float _currentTime;
              public event Action<BigNumber> OnEnergySourceChange; 

              public BigNumber Eps => _eps;
              private void Awake()
              {
                     _saving = FindFirstObjectByType<SavingWrapper>();
                     _ui = GetComponent<EnergyUI>();
                     CurrentResources = new BigNumber(0, 0);
                     _eps = new BigNumber(0, 0);
              }

              void Start()
              {
                     Load();
                     FindFirstObjectByType<Battery.Battery>().OnPause += Save;
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

              //STATS
              protected override void UpdateResourcesProducedStats(BigNumber resource)
              {
                     EventStatBus.Instance.OnEnergyProducedEvent(resource);
              }

              protected override void UpdateResourcesConsumedStats(BigNumber resource)
              {
                     EventStatBus.Instance.OnEnergyConsumedEvent(resource);
              }


              //GUARDADO//
              public override void Save()
              {
                     if (CurrentResources == null)
                     {
                            print("current resources is null");
                            CurrentResources = new BigNumber(0, 0);
                     }
                     _saving.SetTemporalSave(SavingKeys.Energy.Current, CurrentResources.ToToken());
                     _saving.SetTemporalSave(SavingKeys.Energy.Rps, _eps.ToToken());
                     
                  
              }
              public override void Load()
              {
                     var ce= _saving.GetSavingValue(SavingKeys.Energy.Current);
                     if (ce != null)
                     {
                         CurrentResources = ce.ToBigNumber();  
                         _ui.SetEnergyValue(CurrentResources);
                     }
                     
                     var eps = _saving.GetSavingValue(SavingKeys.Energy.Rps);
                     if (eps != null)
                     {
                            _eps = eps.ToBigNumber();
                            _ui.SetEpsValue(_eps);
                     }
             
              }
        
       }
}