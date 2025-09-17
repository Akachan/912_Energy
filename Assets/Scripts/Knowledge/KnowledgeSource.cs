using System;
using System.Collections;
using System.Collections.Generic;
using Energy;
using Knowledge;
using UnityEngine;

public class KnowledgeSource : MonoBehaviour
{
    

    [SerializeField] private KnowledgeSo knowledgeSo;
    
    

    private int _currentKnowledgeLevel = 1;
    private KnowledgeManager _knowledgeManager;
    private KnowledgeSourceUI _ui;
    private EnergyManager _energy;
    private bool _isLastLevel;
    private float _currentTime;


    private void Awake()
    {
        _knowledgeManager = FindFirstObjectByType<KnowledgeManager>();
        _ui = GetComponent<KnowledgeSourceUI>();
        _energy = FindFirstObjectByType<EnergyManager>();
    }

    private void Start()
    {
        SetInitialKnowledge();
    }

    private void Update()
    {
        _currentTime += Time.deltaTime;

        if (!(_currentTime >= 0.2)) return;

        UpdateUpgradeButton();
      
        _currentTime = 0f;
    }

    private void SetInitialKnowledge() 
    {
        if(!GetLevelData(_currentKnowledgeLevel, out var data)) return;
        
        _knowledgeManager.SetKps(data.KPS);
        _ui.UpdateKnowledgeData(_currentKnowledgeLevel,
                                data.KPS,
                                GetDifferenceWithNextLevel(data.KPS),
                                data.Cost);

    }

    private BigNumber GetDifferenceWithNextLevel(BigNumber kps)
    {
        if (!GetLevelData(_currentKnowledgeLevel + 1, out var nextDataLevel)) return null;
        var difference = Calculator.SubtractBigNumbers(nextDataLevel.KPS, kps);
        return difference;
    }

    private bool GetLevelData(int level, out LevelKnowledgeData dataLevel)
    {
        if (knowledgeSo.TryGetKnowledgeData(level, out var data))
        {
            dataLevel = data;
            return true;
        }

        Debug.LogError("No se han encontrado los datos del energySource");
        dataLevel = default;
        return false;

    }
    
    //Al hacer Click en el Botón
    public void BuyUpgrade(int levelsToBuy = 1)
    {
        var nextLevel = _currentKnowledgeLevel + levelsToBuy;
        if (!_energy.RemoveEnergy(knowledgeSo.GetKnowledgeCost(nextLevel, _currentKnowledgeLevel))) return;
        Debug.Log("Buy Upgrade");
        
        _currentKnowledgeLevel = nextLevel;
        
        if (_currentKnowledgeLevel < knowledgeSo.GetMaxLevel())
        {
            if (!GetLevelData(_currentKnowledgeLevel, out var dataLevel)) return;
            
            _knowledgeManager.SetKps(dataLevel.KPS);;
            _ui.UpdateKnowledgeData( _currentKnowledgeLevel,
                dataLevel.KPS,
                GetDifferenceWithNextLevel(dataLevel.KPS),
                dataLevel.Cost);
            
            UpdateUpgradeButton();
        }
        else
        {
            if (!GetLevelData(_currentKnowledgeLevel, out var dataLevel)) return;
            _knowledgeManager.SetKps(dataLevel.KPS);;
            _ui.UpdateLastLevelKnowledgeSourceData(_currentKnowledgeLevel, dataLevel.KPS);
            _isLastLevel = true;
        }

    }

    private void UpdateUpgradeButton()
    {
        
        if (_isLastLevel) return;
        Calculator.CompareBigNumbers(_energy.GetCurrentEnergy(),
            knowledgeSo.GetKnowledgeCost(_currentKnowledgeLevel), 
            out var result);

        _ui.SetUpgradeButtonState(result is ComparisonResult.Bigger or ComparisonResult.Equal);
    }
}

