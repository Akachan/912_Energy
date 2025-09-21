using System;
using UnityEngine;

namespace Knowledge
{
    public class KnowledgeManager : MonoBehaviour
    {
        [SerializeField] private BigNumber debugKnowledge = new BigNumber(1, 3);

        private BigNumber _currentKnowledge;
        private float _currentTime = 0f;
        private BigNumber _knowledgeToAdd;
        private KnowledgeUI _ui;

        private void Awake()
        {
            _ui = GetComponent<KnowledgeUI>();
            _currentKnowledge = new BigNumber(0, 0);
        }

        private void Start()
        {
            if (PlayerPrefs.HasKey("KnowledgeBase"))
            {
                _currentKnowledge = new BigNumber(PlayerPrefs.GetFloat("KnowledgeBase"), PlayerPrefs.GetInt("KnowledgeExponent"));
                _ui.SetKnowledgeValue(_currentKnowledge);
            }
        }

        private void Update()
        {
            _currentTime += Time.deltaTime;
            if (!(_currentTime >= 1f)) return;
            AddKnowledge();
            _currentTime = 0f;
            
            PlayerPrefs.SetFloat("KnowledgeBase", (float)_currentKnowledge.Base);
            PlayerPrefs.SetInt("KnowledgeExponent", _currentKnowledge.Exponent);


        }

        private void AddKnowledge()
        {
            AddKnowledge(_knowledgeToAdd);
            _ui.SetKnowledgeValue(_currentKnowledge);
        }

        public void SetKps(BigNumber kps)
        {
            if (_knowledgeToAdd == null)
            {
                _knowledgeToAdd = new BigNumber(0, 0);
            }

            _knowledgeToAdd = kps;
        }

        public bool RemoveKnowledge(BigNumber value)
        {
            Calculator.CompareBigNumbers(_currentKnowledge, value, out var result);

            if (result != ComparisonResult.Bigger && result != ComparisonResult.Equal) return false;

            _currentKnowledge = Calculator.SubtractBigNumbers(_currentKnowledge, value);
            _ui.SetKnowledgeValue(_currentKnowledge);

            return true;
        }

        [ContextMenu("Add Knowledge")]
        public void AddDebugKnowledge()
        {
            AddKnowledge(debugKnowledge);
        }
        public void AddKnowledge(BigNumber knowledgeToAdd)
        {
            _currentKnowledge = Calculator.AddBigNumbers(_currentKnowledge, knowledgeToAdd);
            _ui.SetKnowledgeValue(_currentKnowledge);
        }

        [ContextMenu("Remove Knowledge")]
        public void RemoveEnergy()
        {
            _currentKnowledge = Calculator.SubtractBigNumbers(_currentKnowledge, debugKnowledge);
        }

        public BigNumber GetCurrentEnergy()
        {
            return _currentKnowledge;
        }

        private void OnDestroy()
        {
            
            PlayerPrefs.SetFloat("KpsBase", (float)_knowledgeToAdd.Base);
            PlayerPrefs.SetInt("KpsExponent", _knowledgeToAdd.Exponent);;
        }
    }
}