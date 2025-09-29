using System;
using Unity.VisualScripting;
using UnityEngine;


//BigResourceManager es un script para controlar Recursos que tienen grandes números

public abstract class BigResourceManager : MonoBehaviour, IResource<BigNumber>, ISaveable
{
    
    protected BigNumber CurrentResources;
    
    public void AddResources(BigNumber resource)
    {
        CurrentResources = Calculator.AddBigNumbers(CurrentResources, resource);
        Save();
    }

    public bool RemoveResources(BigNumber resource)
    {
        if (resource == null)
        {
            Debug.LogError("El recurso a remover es nulo.");
            return false;       
        }
           
        Calculator.CompareBigNumbers(CurrentResources, resource, out var result);
        if (result != ComparisonResult.Bigger && result != ComparisonResult.Equal) return false;
      
        CurrentResources = Calculator.SubtractBigNumbers(CurrentResources, resource);
        Save();
        return true;
    }

    public BigNumber GetResources()
    {
        return CurrentResources;
    }
    public void SetResources(BigNumber resource)
    {
        CurrentResources = resource;
        Save();
    }

    public virtual void UpdateUI()
    {
       
    }

    public virtual void Save()
    {
       
    }

    public virtual void Load()
    {
        
    }
}
