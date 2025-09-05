using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knowledge : MonoBehaviour
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

    private void Update()
    {
        _currentTime += Time.deltaTime;
        if (!(_currentTime >= 1f)) return;
        AddKnowledge();
        _currentTime = 0f;


    }

    private void AddKnowledge()
    {
        _currentKnowledge = Calculator.AddBigNumbers(_currentKnowledge, _knowledgeToAdd);
        print($"New Knowledge: {_currentKnowledge.Base}e{_currentKnowledge.Exponent}");
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
    public void AddEnergy()
    {
        _currentKnowledge = Calculator.AddBigNumbers(_currentKnowledge, debugKnowledge);
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
}