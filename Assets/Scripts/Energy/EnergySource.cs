using Knowledge;
using UnityEngine;

namespace Energy
{
    public class EnergySource: MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float timeToRecalculate = 1f;
        
        [Header("Referencias")]
        [SerializeField] private GameObject infoPanelPrefab;
        
        private EnergyManager _energy;
        private EnergySourceSo _energySource;
        private EnergySourceUI _ui;
        private EnergySourcesSpawner _spawner;
        private KnowledgeManager _knowledgeManager;
        
        private int _currentLevel = 1;
        private float _currentTime;
        private bool _isLocked = true;
        private bool _isLastLevel = false;
        
        private void Awake()
        {
            _energy = FindObjectOfType<EnergyManager>();
            _ui = GetComponent<EnergySourceUI>();
            _spawner = GetComponentInParent<EnergySourcesSpawner>();
            _knowledgeManager = FindObjectOfType<KnowledgeManager>();

        }
        private void OnDisable()
        {
            _energy.OnEnergySourceChange  -= UpdateRatio;
        }
    
        private void Update()
        {
            UpdateUi();
        }
        
        private void UpdateUi()
        {
            _currentTime += Time.deltaTime;

            if (!(_currentTime >= timeToRecalculate)) return;

            UpdateUpgradeButton();
            UpdateUnlockButton();

            _currentTime = 0f;
        }
        
        public void SetEnergySource(EnergySourceSo sourceSo)
        {
            _energySource = sourceSo;
            
            if (_energySource == null) return;
            
            _ui.SetLockedEnergySourceData(_energySource.GetCost(_currentLevel), _energySource.CostToUnlock);

            PlayerPrefs.SetInt($"{_energySource.SourceName}IsLocked", _isLocked == true ? 1 : 0);
            PlayerPrefs.SetInt($"{_energySource.SourceName}Level", _currentLevel);
        }
        
        public void UnlockEnergySource(bool isFirst = false)
        {
            if (!isFirst)
            {
                if (!_knowledgeManager.RemoveResources(_energySource.CostToUnlock)) return;
            }
            
            _isLocked = false;
            PlayerPrefs.SetInt($"{_energySource.SourceName}IsLocked", _isLocked == true ? 1 : 0);
            PlayerPrefs.SetInt($"{_energySource.SourceName}Level", _currentLevel);
            
            _energy.OnEnergySourceChange += UpdateRatio;
            _spawner.CreateNewEnergySource();
            
            _energy.UpdateSource(_energySource.SourceName, _energySource.GetRps(_currentLevel));;
        
            var difference = _energySource.GetDifferenceRps(_currentLevel);
            _ui.SetUnlockedEnergySourceData(_energySource.SourceName, 
                _currentLevel,
                _energySource.GetRps(_currentLevel),
                difference,
                _energySource.GetCost(_currentLevel),
                _energySource.Illustration);
            
            var instance = Instantiate(infoPanelPrefab, _energy.transform);
            instance.GetComponent<SetInfoPanel>().SetInfo(_energySource.SourceName, _energySource.Illustration,_energySource.Description);
            
        }
        
        public void BuyUpgrade(int levelsToBuy = 1)
        {
            var nextLevel = _currentLevel + levelsToBuy;

            if (nextLevel > _energySource.MaxLevel)
            {
                Debug.LogError($"No se puede subir de nivel ya que supera el máximo de {_energySource.MaxLevel}");
                return;
            }
            
            if (!_energy.RemoveResources(_energySource.GetCost(_currentLevel, levelsToBuy))) return;
            Debug.Log("Buy Upgrade");
            
            _currentLevel = nextLevel;
           
            if (_currentLevel < _energySource.MaxLevel)
            {
                _energy.UpdateSource(_energySource.SourceName, _energySource.GetRps(_currentLevel));;
                _ui.UpdateEnergySourceData( _currentLevel,
                                        _energySource.GetRps(_currentLevel),
                                _energySource.GetDifferenceRps(_currentLevel),
                                            _energySource.GetCost(_currentLevel));
                UpdateUpgradeButton();
            }
            else if (_currentLevel == _energySource.MaxLevel)
            {
                _energy.UpdateSource(_energySource.SourceName, _energySource.GetRps(_currentLevel));;
                _ui.UpdateLastLevelEnergySourceData( _currentLevel, _energySource.GetRps(_currentLevel));
                _isLastLevel = true;
            }
            
            PlayerPrefs.SetInt($"{_energySource.SourceName}Level", _currentLevel);

        }
        
        private void UpdateRatio( BigNumber totalEps)
        {
            var ratio =Calculator.DivideBigNumbers(_energySource.GetRps(_currentLevel), _energy.Eps);
            _ui.UpdateRatioText(ratio);
        }
        public void RestoreEnergySource(EnergySourceSo energySo)
        {
            _energySource = energySo;
            _isLocked = PlayerPrefs.GetInt($"{_energySource.SourceName}IsLocked") == 1;

            if (PlayerPrefs.HasKey($"{_energySource.SourceName}Level"))
            {
                _currentLevel = PlayerPrefs.GetInt($"{_energySource.SourceName}Level");
            }
            
            if (_isLocked)
            {
                _ui.SetLockedEnergySourceData(_energySource.GetRps(_currentLevel), _energySource.CostToUnlock);
            }
            else
            {
                _energy.OnEnergySourceChange += UpdateRatio;
               
                _energy.UpdateSource(_energySource.SourceName, _energySource.GetRps(_currentLevel));

                if (_currentLevel < _energySource.MaxLevel)
                {
                    var difference = _energySource.GetDifferenceRps(_currentLevel);
                    _ui.SetUnlockedEnergySourceData(_energySource.SourceName,
                        _currentLevel,
                        _energySource.GetRps(_currentLevel),
                        difference,
                        _energySource.GetCost(_currentLevel),
                        _energySource.Illustration);
                }
                else
                {
                    _ui.UpdateLastLevelEnergySourceData(_energySource.SourceName, _energySource.Illustration,_currentLevel, _energySource.GetRps(_currentLevel));
                }
            }
        }
        
        private void UpdateUnlockButton()
        {
            if (!_isLocked) return;
            Calculator.CompareBigNumbers(   _knowledgeManager.GetResources(), 
                                            _energySource.CostToUnlock, 
                                            out var result);
            _ui.SetUnlockButtonState(result is ComparisonResult.Bigger or ComparisonResult.Equal);
        }

        private void UpdateUpgradeButton()
        {
            if (_isLocked) return;  
            if (_isLastLevel) return;
            Calculator.CompareBigNumbers(_energy.GetResources(),
                                        _energySource.GetCost(_currentLevel), 
                                        out var result);

            _ui.SetUpgradeButtonState(result is ComparisonResult.Bigger or ComparisonResult.Equal);
        }
    }
}