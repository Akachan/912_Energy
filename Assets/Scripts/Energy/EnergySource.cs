using Knowledge;
using UnityEngine;

namespace Energy
{
    public class EnergySource : MonoBehaviour
    {



        [Header("Settings")] [SerializeField] private int currentLevel = 1;
        [SerializeField] private float timeToRecalculate = 3;
        
        [Header("Referencias")]
        [SerializeField] private GameObject infoPanelPrefab;
        





        private float _currentTime;
        private EnergyManager _energy;
        private bool _isLocked = true;
        private bool _isLastLevel = false;
        private EnergySourceSo _energySource;
        private EnergySourceUI _ui;
        private EnergySourcesSpawner _spawner;
        private KnowledgeManager _knowledgeManager;

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
            
            PlayerPrefs.SetInt($"{_energySource.EnergySourceName}IsLocked", _isLocked == true ? 1 : 0);
            
            
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
                if (!_knowledgeManager.RemoveResources(_energySource.GetCostToUnlock())) return;
            }
            _isLocked = false;
            PlayerPrefs.SetInt($"{_energySource.EnergySourceName}IsLocked", _isLocked == true ? 1 : 0);
            PlayerPrefs.SetInt($"{_energySource.EnergySourceName}Level", currentLevel);
            
            _energy.OnEnergySourceChange += UpdateRatio;
            _spawner.CreateNewEnergySource();
        
            if (!GetLevelData(currentLevel, out var dataLevel)) return;
            _energy.UpdateSource(_energySource.EnergySourceName, dataLevel.EPS);;
        
            var difference = GetDifferenceWithNextLevel(dataLevel.EPS);
            _ui.SetUnlockedEnergySourceData(_energySource.EnergySourceName, 
                currentLevel,
                dataLevel.EPS,
                difference,
                dataLevel.Cost,
                _energySource.Illustration);
            
            var instance = Instantiate(infoPanelPrefab, _energy.transform);
            instance.GetComponent<SetInfoPanel>().SetInfo(_energySource.EnergySourceName, _energySource.Illustration,_energySource.Description);
            
        }

        private BigNumber GetDifferenceWithNextLevel(BigNumber eps)
        {
            if (!GetLevelData(currentLevel + 1, out var nextDataLevel)) return null;
            var difference = Calculator.SubtractBigNumbers( nextDataLevel.EPS, eps);
            
        
            return  Calculator.NormalizeBigNumber(difference);
        }


        private void UpdateUnlockButton()
        {
            if (!_isLocked) return;
            Calculator.CompareBigNumbers(_knowledgeManager.GetResources(), 
                _energySource.GetCostToUnlock(), 
                out var result);
            _ui.SetUnlockButtonState(result is ComparisonResult.Bigger or ComparisonResult.Equal);
        }

        private void UpdateUpgradeButton()
        {
            if (_isLocked) return;
            if (_isLastLevel) return;
            Calculator.CompareBigNumbers(_energy.GetResources(),
                _energySource.GetCost(currentLevel), 
                out var result);

            _ui.SetUpgradeButtonState(result is ComparisonResult.Bigger or ComparisonResult.Equal);
        }




        //Al hacer Click en el Botón
        public void BuyUpgrade(int levelsToBuy = 1)
        {
            var nextLevel = currentLevel + levelsToBuy;

            if (nextLevel > _energySource.GetMaxLevel())
            {
                Debug.LogError($"No se puede subir de nivel ya que supera el máximo de {_energySource.GetMaxLevel()}");
                return;
            }
            
            if (!_energy.RemoveResources(_energySource.GetCost(nextLevel, currentLevel))) return;
            Debug.Log("Buy Upgrade");
            
            currentLevel = nextLevel;
           
            if (currentLevel < _energySource.GetMaxLevel())
            {
                if (!GetLevelData(currentLevel, out var dataLevel)) return;
            
                _energy.UpdateSource(_energySource.EnergySourceName, dataLevel.EPS);;
                _ui.UpdateEnergySourceData( currentLevel,
                    dataLevel.EPS,
                    GetDifferenceWithNextLevel(dataLevel.EPS),
                    dataLevel.Cost);
                UpdateUpgradeButton();
            }
            else
            {
                if (!GetLevelData(currentLevel, out var dataLevel)) return;
                _energy.UpdateSource(_energySource.EnergySourceName, dataLevel.EPS);;
                _ui.UpdateLastLevelEnergySourceData( currentLevel, dataLevel.EPS);
                _isLastLevel = true;
            }
            
            PlayerPrefs.SetInt($"{_energySource.EnergySourceName}Level", currentLevel);

        }

        private void UpdateRatio( BigNumber totalEps)
        {
            if(!GetLevelData(currentLevel, out var dataLevel)) return;
            var ratio =Calculator.DivideBigNumbers(dataLevel.EPS, _energy.Eps);
            _ui.UpdateRatioText(ratio);
        }



        public void RestoreEnergySource(EnergySourceSo energySo)
        {
            _energySource = energySo;
            _isLocked = PlayerPrefs.GetInt($"{_energySource.EnergySourceName}IsLocked") == 1;

            if (PlayerPrefs.HasKey($"{_energySource.EnergySourceName}Level"))
            {
                currentLevel = PlayerPrefs.GetInt($"{_energySource.EnergySourceName}Level");
            }
            
            

            if (_isLocked)
            {
                if (!GetLevelData(currentLevel, out var dataLevel)) return;
                _ui.SetLockedEnergySourceData(dataLevel.EPS, _energySource.GetCostToUnlock());
            }
            else
            {
            
                if (!GetLevelData(currentLevel, out var dataLevel)) return;
                
                _energy.OnEnergySourceChange += UpdateRatio;
               
                _energy.UpdateSource(_energySource.EnergySourceName, dataLevel.EPS);

                if (currentLevel < _energySource.GetMaxLevel())
                {
                    var difference = GetDifferenceWithNextLevel(dataLevel.EPS);
                    _ui.SetUnlockedEnergySourceData(_energySource.EnergySourceName,
                        currentLevel,
                        dataLevel.EPS,
                        difference,
                        dataLevel.Cost,
                        _energySource.Illustration);
                }
                else
                {
                    
                    _ui.UpdateLastLevelEnergySourceData(_energySource.EnergySourceName, _energySource.Illustration,currentLevel, dataLevel.EPS);
                }

            }

        }

    
    }
}
