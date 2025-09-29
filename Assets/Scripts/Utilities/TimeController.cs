using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class TimeController : MonoBehaviour
{
    [SerializeField] private bool _isDebug = false;

    [SerializeField] private BigNumber resourceToAdd;
    
    
    [Header("References")]
    private EnergyManager _energyManager;
    private KnowledgeManager _knowledgeManager;


    private void Awake()
    {
        _energyManager = FindObjectOfType<EnergyManager>();
        _knowledgeManager = FindObjectOfType<KnowledgeManager>();
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
        if (!_isDebug) return;
        ResetGame();
        SetTimeScale();
    }

    private void ResetGame()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
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
            Time.timeScale = 3f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Time.timeScale = 4f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Time.timeScale = 5f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Time.timeScale = 6f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Time.timeScale = 7f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Time.timeScale = 8f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Time.timeScale = 9f;
        }
    }
}
