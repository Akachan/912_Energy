using UnityEngine;

public abstract class SmallResourceManager : MonoBehaviour, IResource<int>, ISaveable
{
    protected int CurrentResources;
    
    public void AddResources(int resource)
    {
        CurrentResources += resource;
        UpdateResourcesProducedStats(resource);
        Save();
        UpdateUI();
    }

    public bool RemoveResources(int resource)
    {
        if (resource <= 0)
        {
            Debug.LogError("El recurso a remover es menor o igual a cero.");
            return false;
        }
        if (resource > CurrentResources)
        {
            Debug.LogError($"No hay suficientes recursos. \n" +
                           $"CurrentResources {CurrentResources}, ResourceToRemove {resource}");
            
        }
        CurrentResources -= resource;
        UpdateResourcesConsumedStats(resource);
        Save();
        return true;
    }

    public int GetResources()
    {
        return CurrentResources;
    }

    public void SetResources(int resource)
    {
        CurrentResources = resource;
        Save();
    }

    public virtual void UpdateUI()
    {
        
    }

    //STATS

    protected virtual void UpdateResourcesProducedStats(int resource)
    {
        
    }
    protected virtual void UpdateResourcesConsumedStats(int resource)
    {
        
    }
    
    
    //SAVING SYSTEM
    public virtual void Save()
    {
            
    }

    public virtual void Load()
    {
            
    }
}