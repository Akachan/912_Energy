using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnergySource : MonoBehaviour
{
    [SerializeField] private EnergySourceSo energySource;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI eps;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private int currentLevel = 1;
    
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
