using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnowledgeSource : MonoBehaviour
{

    [SerializeField] private KnowledgeSo knowledgeSo;

    private int _currentKnowledgeLevel = 1;
    private Knowledge _knowledge;
    private KnowledgeSourceUI _sourceUI;
    

    private void Awake()
    {
        _knowledge = FindFirstObjectByType<Knowledge>();
        _sourceUI = GetComponent<KnowledgeSourceUI>();
    }

    private void Start()
    {
        SetInitialKnowledge();
    }

    private void SetInitialKnowledge() 
    {
        if(!GetLevelData(_currentKnowledgeLevel, out var data)) return;
        
        _knowledge.SetKps(data.KPS);
        _sourceUI.UpdateKnowledgeData(_currentKnowledgeLevel,
                                data.KPS,
                                GetDifferenceWithNextLevel(data.KPS),
                                data.Cost);

    }

    private BigNumber GetDifferenceWithNextLevel(BigNumber kps)
    {
        if (!GetLevelData(_currentKnowledgeLevel + 1, out var nextDataLevel)) return null;
        var difference = Calculator.SubtractBigNumbers(nextDataLevel.KPS, kps);
        return difference;
    }

    private bool GetLevelData(int level, out LevelKnowledgeData dataLevel)
    {
        if (knowledgeSo.TryGetKnowledgeData(level, out var data))
        {
            dataLevel = data;
            return true;
        }

        Debug.LogError("No se han encontrado los datos del energySource");
        dataLevel = default;
        return false;

    }
    
    
}
