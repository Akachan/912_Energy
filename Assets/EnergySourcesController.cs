using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySourcesController : MonoBehaviour
{
    [SerializeField] private EnergySourceSo[] energySources;

    [Header("References")] 
    [SerializeField]  private GameObject energySourcePrefab;

    [SerializeField] private Transform energySourceParent;
    
    private int _energySourceIndex = 0;


  
    // Crear prefab de energySource y colococar
    // Asignar SO a nuevo energy source

    private void Start()
    {
        InitializeFirstEnergySource();
    }

    private void InitializeFirstEnergySource()
    {
        var instance = Instantiate(energySourcePrefab, energySourceParent).GetComponent<EnergySource>(); 
        instance.SetEnergySource(energySources[_energySourceIndex]);
        _energySourceIndex++;
        instance.SetFirstUnlocked();  //El primero está siempre desbloqueado
        
        
        
    }

    public void CreateNewEnergySource()
    {
        if(_energySourceIndex >= energySources.Length) return;
        var instance = Instantiate(energySourcePrefab, energySourceParent).GetComponent<EnergySource>(); 
        instance.SetEnergySource(energySources[_energySourceIndex]);
        _energySourceIndex++;
        
    }
    

}
