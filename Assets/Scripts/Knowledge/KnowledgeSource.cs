using Energy;
using UnityEngine;

namespace Knowledge
{
    public class KnowledgeSource : MonoBehaviour
    {
        [SerializeField] private KnowledgeSourceSo knowledgeSourceSo;
        
        private int _currentLevel = 1;
        private KnowledgeManager _knowledgeManager;
        private KnowledgeSourceUI _ui;
        private EnergyManager _energy;
        private bool _isLastLevel;
        private float _currentTime;


        private void Awake()
        {
            _knowledgeManager = FindFirstObjectByType<KnowledgeManager>();
            _ui = GetComponent<KnowledgeSourceUI>();
            _energy = FindFirstObjectByType<EnergyManager>();
        }

        private void Start()
        {
            SetInitialKnowledge();
        }

        private void Update()
        {
            _currentTime += Time.deltaTime;

            if (!(_currentTime >= 0.2)) return;

            UpdateUpgradeButton();
      
            _currentTime = 0f;
        }

        private void SetInitialKnowledge() 
        {
            if (PlayerPrefs.HasKey("KnowledgeLevel"))
            {
                _currentLevel = PlayerPrefs.GetInt("KnowledgeLevel");
            }
            
            _knowledgeManager.SetKps(knowledgeSourceSo.GetRps(_currentLevel));

            if (_currentLevel < knowledgeSourceSo.MaxLevel)
            {
                
                _ui.UpdateKnowledgeData(_currentLevel,
                    knowledgeSourceSo.GetRps(_currentLevel),
                    knowledgeSourceSo.GetDifferenceRps(_currentLevel),
                    knowledgeSourceSo.GetCost(_currentLevel));
            }
            else
            {
                _ui.UpdateLastLevelKnowledgeSourceData(_currentLevel,knowledgeSourceSo.GetRps(_currentLevel));
                _isLastLevel = true;
            }
        }

     
       
    
        //Al hacer Click en el Botón
        public void BuyUpgrade(int levelsToBuy = 1)
        {
            var nextLevel = _currentLevel + levelsToBuy;
            if (!_energy.RemoveResources(knowledgeSourceSo.GetCost(_currentLevel))) return;
            Debug.Log("Buy Upgrade");
        
            _currentLevel = nextLevel;
        
        
            if (_currentLevel < knowledgeSourceSo.MaxLevel)
            {
                _knowledgeManager.SetKps(knowledgeSourceSo.GetRps(_currentLevel));;
                _ui.UpdateKnowledgeData( _currentLevel,
                    knowledgeSourceSo.GetRps(_currentLevel),
                    knowledgeSourceSo.GetDifferenceRps(_currentLevel),
                    knowledgeSourceSo.GetCost(_currentLevel));
            
                UpdateUpgradeButton();
            }
            else
            {
                
                _knowledgeManager.SetKps(knowledgeSourceSo.GetRps(_currentLevel));;
                _ui.UpdateLastLevelKnowledgeSourceData(_currentLevel, knowledgeSourceSo.GetRps(_currentLevel));
                _isLastLevel = true;
            }

            PlayerPrefs.SetInt("KnowledgeLevel", _currentLevel);
        }

        private void UpdateUpgradeButton()
        {
        
            if (_isLastLevel) return;
            Calculator.CompareBigNumbers(_energy.GetResources(),
                knowledgeSourceSo.GetCost(_currentLevel), 
                out var result);

            _ui.SetUpgradeButtonState(result is ComparisonResult.Bigger or ComparisonResult.Equal);
        }
    }
}

