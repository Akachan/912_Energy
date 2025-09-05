using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knowledge : MonoBehaviour
{
    private BigNumber _currentKnowledge;
    private float _currentTime = 0f;
    private BigNumber _knowledgeToAdd;
    private KnowledgeUI _ui;

    private void Awake()
    {
        _ui = GetComponent<KnowledgeUI>();
       
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
  
        _knowledgeToAdd = kps;
    }
}
