public interface IResource<T>
{
    public void AddResources(T resource);
    public bool RemoveResources(T resource);
    
    T GetResources();
    public void SetResources(T resource);

    public void UpdateUI();
}