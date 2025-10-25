using UnityEngine;

public abstract class SmallResourceManager : MonoBehaviour, IResource<int>, ISaveable
{
    protected int CurrentResources;
    
    public void AddResources(int resource)
    {
        CurrentResources += resource;
        Save();
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

    public void UpdateUI()
    {
        throw new System.NotImplementedException();
    }

    public void Save()
    {
            
    }

    public void Load()
    {
            
    }
}