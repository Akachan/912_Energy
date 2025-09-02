using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EnergySource : MonoBehaviour
{



    [Header("Settings")] [SerializeField] private int currentLevel = 1;
    [SerializeField] private float timeToRecalculate = 3;





    private float _currentTime;
    private Energy _energy;
    private bool _isLocked = true;
    private EnergySourceSo _energySource;
    private EnergySourceUI _ui;
    private EnergySourcesController _controller;

    private void Awake()
    {
        _energy = FindObjectOfType<Energy>();
        _ui = GetComponent<EnergySourceUI>();
        _controller = GetComponentInParent<EnergySourcesController>();

    }

    private void OnEnable()
    {
        _controller.OnUpgradeEnergySource += UpdateRatio;
    }
    private void OnDisable()
    {
        _controller.OnUpgradeEnergySource -= UpdateRatio;
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

        if (!GetLevelData(currentLevel, out var dataLevel)) return;

        _ui.SetLockedEnergySourceData(dataLevel.EPS, _energySource.GetCostToUnlock());
        ;

    }

    private bool GetLevelData(int level, out LevelData dataLevel)
    {
        if (_energySource.TryGetLevelData(level, out dataLevel)) return true;
        Debug.LogError("No se han encontrado los datos del energySource");
        return false;

    }


    //la acción del botón
    public void UnlockEnergySource(bool isFirst = false)
    {
        if (!isFirst)
        {
            if (!_energy.RemoveEnergy(_energySource.GetCostToUnlock())) return;
        }

        print($"NewEnergySource: {_energySource.EnergySourceName} Unlocked");
        _isLocked = false;

        //primero seteo el nuevo desbloqueo para que pueda calcular la primera vez con un valor de eps válido
        _controller.CreateNewEnergySource();
        var ratio = _energy.UpdateSourceAndReturnRatio(_energySource.EnergySourceName,
            _energySource.GetEps(currentLevel));


        if (!GetLevelData(currentLevel, out var dataLevel)) return;
        var difference = GetDifferenceWithNextLevel(dataLevel.EPS);
        _ui.SetUnlockedEnergySourceData(_energySource.EnergySourceName, currentLevel, dataLevel.EPS, ratio, difference,
            dataLevel.Cost);

        if (isFirst) return;
        _controller.UpgradeAllRatios();


    }

    private BigNumber GetDifferenceWithNextLevel(BigNumber eps)
    {
        if (!GetLevelData(currentLevel + 1, out var nextDataLevel)) return null;
        var difference = Calculator.SubtractBigNumbers( nextDataLevel.EPS,eps);
        return difference;
    }


    private void UpdateUnlockButton()
    {
        if (!_isLocked) return;
        Calculator.CompareBigNumbers(_energy.GetCurrentEnergy(), _energySource.GetCostToUnlock(), out var result);
        _ui.SetUnlockButtonState(result is ComparisonResult.Bigger or ComparisonResult.Equal);
    }

    private void UpdateUpgradeButton()
    {
        if (_isLocked) return;
        Calculator.CompareBigNumbers(_energy.GetCurrentEnergy(), _energySource.GetCost(currentLevel), out var result);

        _ui.SetUpgradeButtonState(result is ComparisonResult.Bigger or ComparisonResult.Equal);
    }




    //Al hacer Click en el Botón
    public void BuyUpgrade()
    {
        if (_energy.RemoveEnergy(_energySource.GetCost(currentLevel)))
        {
            Debug.Log("Buy Upgrade");
            currentLevel++;

            var ratio = _energy.UpdateSourceAndReturnRatio(_energySource.EnergySourceName,
                _energySource.GetEps(currentLevel));

            if (!GetLevelData(currentLevel, out var dataLevel)) return;
            _ui.UpdateEnergySourceData(currentLevel, dataLevel.EPS, ratio, GetDifferenceWithNextLevel(dataLevel.EPS),
                dataLevel.Cost);

            UpdateUpgradeButton();
            _controller.UpgradeAllRatios();
        }
        else
        {
            Debug.Log("Don't buy Upgrade");
        }


    }

    public void UpdateRatio()
    {
        var ratio =_energy.GetRatio(_energySource.EnergySourceName);
        _ui.UpdateRatioText(ratio);
    }





    
}
