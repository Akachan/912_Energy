using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergySource : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private EnergySourceSo energySource;
    
    [Header("Settings")]
    [SerializeField] private int currentLevel = 1; 
    [SerializeField] private float timeToRecalculate = 3;
        
    [Header("References")]
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI eps;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private Button upgradeButton;
    
    private float _currentTime;
    private Energy _energy;

    private void Awake()
    {
        _energy = FindObjectOfType<Energy>();
    }

    private void Start()
    {
        SetTitle();
        _energy.UpdateSource(energySource.EnergySourceName, energySource.GetEps(currentLevel));
    }


    private void Update()
    {
        _currentTime += Time.deltaTime;
        
        if (!(_currentTime >= timeToRecalculate)) return;
        
        UpdateUpgradeButton();

        _currentTime = 0f;
        
    }

    private void UpdateUpgradeButton()
    {
        Calculator.CompareBigNumbers(_energy.GetCurrentEnergy(), energySource.GetCost(currentLevel), out var result);
        upgradeButton.interactable = result is ComparisonResult.Bigger or ComparisonResult.Equal;
        
        
    }

    private void SetTitle()
    {
        if(!energySource.TryGetLevelData(currentLevel, out var dataLevel)) return;
        
        title.text = $"{energySource.EnergySourceName}";
        eps.text = $"EPS: {dataLevel.EPS.Base:#.########}e{dataLevel.EPS.Exponent}";
        cost.text = $"Cost: {dataLevel.Cost.Base:#.########}e{dataLevel.Cost.Exponent}";
    }

    //Al hacer Click en el Botón
    public void BuyUpgrade()
    {
        if (_energy.RemoveEnergy(energySource.GetCost(currentLevel)))
        {
            Debug.Log("Buy Upgrade");
            currentLevel++;
            SetTitle();
            _energy.UpdateSource(energySource.EnergySourceName, energySource.GetEps(currentLevel));
            
            UpdateUpgradeButton();
        }
        else
        {
              Debug.Log("Don't buy Upgrade");
        }
      
    
    }
    
    

    private void OnValidate()
    {
        SetTitle();
    }
    
}
