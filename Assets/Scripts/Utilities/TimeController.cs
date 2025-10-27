using System.Collections;
using System.Collections.Generic;
using Energy;
using Knowledge;
using SavingSystem;
using Stats;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Utilities
{
    public class TimeController : MonoBehaviour
    {
        
        [SerializeField] private bool isDebug = false;

        [SerializeField] private BigNumber resourceToAdd;
    
    
        [Header("References")]
        private EnergyManager _energyManager;
        private KnowledgeManager _knowledgeManager;


        private void Awake()
        {
            StatsManager.Instance.LoadStats();
            _energyManager = FindFirstObjectByType<EnergyManager>();
            _knowledgeManager = FindFirstObjectByType<KnowledgeManager>();
        }

        [ContextMenu("Add Energy")]
        private void AddEnergy()
        {
            _energyManager.AddResources(resourceToAdd);
        }
        [ContextMenu("Add Knowledge")]
        private void AddKnowledge()
        {
            _knowledgeManager.AddResources(resourceToAdd);
        }

        [ContextMenu("Remove All Resources")]
        private void RemoveAllResources()
        {
            _energyManager.SetResources(new BigNumber(0, 0));
            _knowledgeManager.SetResources(new BigNumber(0, 0));
        }
    
    

        void Update()
        {
            if (!isDebug) return;
            ResetGame();
            SetTimeScale();
        }

        private void ResetGame()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                FindFirstObjectByType<SavingWrapper>().DeleteSaveFile();
                PlayerPrefs.DeleteAll();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        private void SetTimeScale()
        {
        
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Time.timeScale = 1f;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Time.timeScale = 2f;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Time.timeScale = 4f;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Time.timeScale = 8f;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                Time.timeScale = 16f;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                Time.timeScale = 32f;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                Time.timeScale = 64f;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                Time.timeScale = 80f;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                Time.timeScale = 99f;
            }
        }
    }
}
