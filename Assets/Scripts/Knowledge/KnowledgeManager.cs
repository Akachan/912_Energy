using UnityEngine;

public class KnowledgeManager : BigResourceManager
{
    private BigNumber _kps;
    private KnowledgeUI _ui;
    private float _currentTime;

    private void Awake()
    {
        _ui = GetComponent<KnowledgeUI>();
        CurrentResources = new BigNumber(0, 0);
    }

    private void Start()
    {
        Load();
        FindFirstObjectByType<Battery.Battery>().OnPause += SaveKps;
            
    }

    private void Update()
    {
        UpdateResources();
        Save();
    }

    private void UpdateResources()
    {
        _currentTime += Time.deltaTime;
        if (!(_currentTime >= 1f)) return;
        AddResources(_kps);
        _currentTime = 0f;
            
        _ui.SetKnowledgeValue(CurrentResources);
    }
        
    public void SetKps(BigNumber kps)
    {
        if (_kps == null)
        {
            _kps = new BigNumber(0, 0);
        }

        _kps = kps;
    }


    private void SaveKps()
    {
        PlayerPrefs.SetFloat("KpsBase", (float)_kps.Base);
        PlayerPrefs.SetInt("KpsExponent", _kps.Exponent);
        PlayerPrefs.Save(); // Fuerza el guardado inmediato
    }

    public override void Save()
    {
        PlayerPrefs.SetFloat("KnowledgeBase", (float)CurrentResources.Base);
        PlayerPrefs.SetInt("KnowledgeExponent", CurrentResources.Exponent);
    }
    public override void Load()
    {
        if (PlayerPrefs.HasKey("KnowledgeBase"))
        {
            CurrentResources = new BigNumber(PlayerPrefs.GetFloat("KnowledgeBase"), PlayerPrefs.GetInt("KnowledgeExponent"));
            _ui.SetKnowledgeValue(CurrentResources);
        }
    }
        
        
   
}