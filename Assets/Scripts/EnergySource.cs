using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergySource : MonoBehaviour
{
    
    
    
    [Header("Settings")]
    [SerializeField] private int currentLevel = 1; 
    [SerializeField] private float timeToRecalculate = 3;
        
    [Header("References")]
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI eps;
    [SerializeField] private TextMeshProUGUI unlockCost;
    [SerializeField] private TextMeshProUGUI upgradeCost;
    [SerializeField] private Button unlockButton;
    [SerializeField] private Button upgradeButton;
    
    private float _currentTime;
    private Energy _energy;
    private bool _isLocked = true;
    private EnergySourceSo _energySource;

    private void Awake()
    {
        _energy = FindObjectOfType<Energy>();
    }


    private void Update()
    {
        _currentTime += Time.deltaTime;
        
        if (!(_currentTime >= timeToRecalculate)) return;
        
        UpdateUpgradeButton();
        UpdateUnlockButton();

        _currentTime = 0f;
        
    }

    public void SetEnergySource(EnergySourceSo energySource)
    {
        _energySource = energySource;
        SetTitle();
        //_energy.UpdateSource(_energySource.EnergySourceName, _energySource.GetEps(currentLevel));
    }

    public void SetFirstUnlocked()
    {
        print("NewEnergySource Unlocked");
        _isLocked = false;
        SetTitle();
        //Ocultar UnlockButton
        unlockButton.gameObject.SetActive(false);
        //aparecer UpgradeButton
        upgradeButton.gameObject.SetActive(true);

        FindFirstObjectByType<EnergySourcesController>().CreateNewEnergySource();
        _energy.UpdateSource(_energySource.EnergySourceName, _energySource.GetEps(currentLevel));
    }
    private void UpdateUpgradeButton()
    {
        if(_isLocked) return;
        Calculator.CompareBigNumbers(_energy.GetCurrentEnergy(), _energySource.GetCost(currentLevel), out var result);
        upgradeButton.interactable = result is ComparisonResult.Bigger or ComparisonResult.Equal;
        
        
    }

    private void UpdateUnlockButton()
    {
        if(!_isLocked) return;
        Calculator.CompareBigNumbers(_energy.GetCurrentEnergy(), _energySource.GetCostToUnlock(), out var result);
        unlockButton.interactable = result is ComparisonResult.Bigger or ComparisonResult.Equal;
    }

   

    public void UnlockEnergySource()
    {
        if (!_energy.RemoveEnergy(_energySource.GetCostToUnlock())) return;
        
        print("NewEnergySource Unlocked");
        _isLocked = false;
        SetTitle();
        
        //Ocultar UnlockButton
        unlockButton.gameObject.SetActive(false);
        //aparecer UpgradeButton
        upgradeButton.gameObject.SetActive(true);

        GetComponentInParent<EnergySourcesController>().CreateNewEnergySource();
        _energy.UpdateSource(_energySource.EnergySourceName, _energySource.GetEps(currentLevel));

    }
    //Al hacer Click en el Botón
    public void BuyUpgrade()
    {
        if (_energy.RemoveEnergy(_energySource.GetCost(currentLevel)))
        {
            Debug.Log("Buy Upgrade");
            currentLevel++;
            
            SetTitle();
            _energy.UpdateSource(_energySource.EnergySourceName, _energySource.GetEps(currentLevel));
            
            UpdateUpgradeButton();
        }
        else
        {
              Debug.Log("Don't buy Upgrade");
        }
      
    
    }
    private void SetTitle()
    {

        if(!_energySource.TryGetLevelData(currentLevel, out var dataLevel)) return;

        title.text = _isLocked ? $"???" : $"{_energySource.EnergySourceName}";

        level.text = $"Level: {currentLevel}";
        eps.text = $"EPS: {dataLevel.EPS.Base:#.########}e{dataLevel.EPS.Exponent}";
        if (_isLocked)
        {
            unlockCost.text = $"Unlock: {_energySource.GetCostToUnlock().Base}e{_energySource.GetCostToUnlock().Exponent}";
        }
        else
        {
            upgradeCost.text = $"Upgrade: {dataLevel.Cost.Base:#.########}e{dataLevel.Cost.Exponent}";
        }
        
        
        
    }
    
    
    
    /*
    private void OnValidate()
    {
        
        SetTitle();
    }
    */
    
}
