using Knowledge;
using UnityEngine;
using Utilities;

namespace Energy
{
    public class EnergySource: MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float timeToRecalculate = 1f;
        
        [Header("Referencias")]
        [SerializeField] private GameObject infoPanelPrefab;
        
        private EnergySourceData _data;
        
        private EnergyManager _energy;
        private EnergySourceUI _ui;
        private EnergySourcesSpawner _spawner;
        private KnowledgeManager _knowledgeManager;
        private EnergySourceSo _sourceSo;
        
        
        private float _currentTime;
        private bool _isLastLevel = false;
       
        
        private void Awake()
        {
            _energy = FindFirstObjectByType<EnergyManager>();
            _ui = GetComponent<EnergySourceUI>();
            _spawner = GetComponentInParent<EnergySourcesSpawner>();
            _knowledgeManager = FindFirstObjectByType<KnowledgeManager>();

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
        
        public void SetEnergySource(EnergySourceData data, EnergySourceSo sourceSo)
        {
            _data = data;
            _sourceSo = sourceSo;
            
            if (_sourceSo == null) return;
            
            _ui.SetLockedEnergySourceData(_sourceSo.GetCost(_data.Level), _sourceSo.CostToUnlock);
            
        }
        
        public void UnlockEnergySource(bool isFirst = false)
        {
            if (!isFirst)
            {
                if (!_knowledgeManager.RemoveResources(_sourceSo.CostToUnlock)) return;
            }
            
            _data.IsLocked = false;
            _spawner.UpdateEnergySource(_data);
            
            _energy.OnEnergySourceChange += UpdateRatio;
            _spawner.CreateNewEnergySource();
            
            _energy.UpdateSource(_sourceSo.SourceName, _sourceSo.GetRps(_data.Level));;
        
            var difference = _sourceSo.GetDifferenceRps(_data.Level);
            _ui.SetUnlockedEnergySourceData(_sourceSo.SourceName, 
                _data.Level,
                _sourceSo.GetRps(_data.Level),
                difference,
                _sourceSo.GetCost(_data.Level),
                _sourceSo.Illustration);
            
            var instance = Instantiate(infoPanelPrefab, _energy.transform);
            instance.GetComponent<SetInfoPanel>().SetInfo(_sourceSo.SourceName, _sourceSo.Illustration,
                _sourceSo.Description);
            
        }
        
        public void BuyUpgrade(int levelsToBuy = 1)
        {
            var nextLevel = _data.Level + levelsToBuy;

            if (nextLevel > _sourceSo.MaxLevel)
            {
                Debug.LogError($"No se puede subir de nivel ya que supera el máximo de {_sourceSo.MaxLevel}");
                return;
            }
            
            if (!_energy.RemoveResources(_sourceSo.GetCost(_data.Level, levelsToBuy))) return;
            Debug.Log("Buy Upgrade");
            
            _data.Level = nextLevel;
            var rps = _sourceSo.GetRps(_data.Level);
            
            if (_data.Level < _sourceSo.MaxLevel)
            {
                _energy.UpdateSource(_sourceSo.SourceName, rps);
                _ui.UpdateEnergySourceData( _data.Level,
                                        rps,
                                        _sourceSo.GetDifferenceRps(_data.Level),
                                        _sourceSo.GetCost(_data.Level));
                UpdateUpgradeButton();
            }
            else if (_data.Level == _sourceSo.MaxLevel)
            {
                _energy.UpdateSource(_sourceSo.SourceName, rps);
                _ui.UpdateLastLevelEnergySourceData( _data.Level, rps);
                _isLastLevel = true;
            }
            
            PlayerPrefs.SetInt($"{_sourceSo.SourceName}Level", _data.Level);

        }
        
        private void UpdateRatio( BigNumber totalEps)
        {
            var ratio =Calculator.DivideBigNumbers(_sourceSo.GetRps(_data.Level), _energy.Eps);
            _ui.UpdateRatioText(ratio);
        }
        public void RestoreEnergySource(EnergySourceData data, EnergySourceSo sourceSo)
        {
            _data = data;
            _sourceSo = sourceSo;
            
            if (_data.IsLocked)
            {
                _ui.SetLockedEnergySourceData(_sourceSo.GetRps(_data.Level), _sourceSo.CostToUnlock);
            }
            else
            {
                _energy.OnEnergySourceChange += UpdateRatio;
               
                _energy.UpdateSource(_sourceSo.SourceName, _sourceSo.GetRps(_data.Level));

                if (_data.Level < _sourceSo.MaxLevel)
                {
                    var difference = _sourceSo.GetDifferenceRps(_data.Level);
                    _ui.SetUnlockedEnergySourceData(_sourceSo.SourceName,
                        _data.Level,
                        _sourceSo.GetRps(_data.Level),
                        difference,
                        _sourceSo.GetCost(_data.Level),
                        _sourceSo.Illustration);
                }
                else
                {
                    _ui.UpdateLastLevelEnergySourceData(_sourceSo.SourceName, _sourceSo.Illustration,_data.Level, 
                        _sourceSo.GetRps(_data.Level));
                }
            }
        }
        
        private void UpdateUnlockButton()
        {
            if (!_data.IsLocked) return;
            Calculator.CompareBigNumbers(   _knowledgeManager.GetResources(), 
                _sourceSo.CostToUnlock, 
                                            out var result);
            _ui.SetUnlockButtonState(result is ComparisonResult.Bigger or ComparisonResult.Equal);
        }

        private void UpdateUpgradeButton()
        {
            if (_data.IsLocked) return;  
            if (_data.Level == _sourceSo.MaxLevel ) return;
            Calculator.CompareBigNumbers(_energy.GetResources(),
                _sourceSo.GetCost(_data.Level), 
                                        out var result);

            _ui.SetUpgradeButtonState(result is ComparisonResult.Bigger or ComparisonResult.Equal);
        }
    }
}