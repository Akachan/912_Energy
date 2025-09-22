using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;

public class BatteryUI : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI debugText;
   [Header("References")]
   [SerializeField] private TextMeshProUGUI timeText;
   [SerializeField] private TextMeshProUGUI energyText;
   [SerializeField] private TextMeshProUGUI knowledgeText;
   
   public event Action OnAddProgress;
   
   public void SetDebugInfo(string saveDate, string nowDate, float second, 
                        BigNumber eps, BigNumber kps, 
                        BigNumber currentEnergy, BigNumber currentKnowledge,
                        BigNumber previousEnergy, BigNumber previousKnowledge,
                        BigNumber energyToAdd, BigNumber knowledgeToAdd)
   {
       debugText.text = $"Fecha de guardado: {saveDate} \n" +
                       $"Fecha actual: {nowDate} \n" +
                       $"Segundos: {second} \n" +
                       $"ENERGY \n" +
                       $"EPS: {BigNumberFormatter.SetSuffixFormat(eps)} \n" +
                       $"PreviousEnergy: {BigNumberFormatter.SetSuffixFormat(previousEnergy)} \n" +
                       $"EnergyToAdd: {BigNumberFormatter.SetSuffixFormat(energyToAdd)} \n" +
                       $"currentEnergy: {BigNumberFormatter.SetSuffixFormat(currentEnergy)} \n" +
                       $"KNOWLEDGE \n" +
                       $"KPS: {BigNumberFormatter.SetSuffixFormat(kps)} \n" +
                       $"PreviousKnowledge: {BigNumberFormatter.SetSuffixFormat(previousKnowledge)} \n" +
                       $"KnowledgeToAdd: {BigNumberFormatter.SetSuffixFormat(knowledgeToAdd)} \n" +
                       $"currentKnowledge: {BigNumberFormatter.SetSuffixFormat(currentKnowledge)}";
   }

   public void SetBatteryInfo(TimeSpan time, BigNumber energy, BigNumber knowledge)
   {
       
       
       timeText.text = $"{time.Hours:00}:{time.Minutes:00}:{time.Seconds:00}";
       energyText.text = BigNumberFormatter.SetSuffixFormat(energy);
       knowledgeText.text = BigNumberFormatter.SetSuffixFormat(knowledge);
   }

   public void OnAddProgressButton()
   {
       OnAddProgress?.Invoke();
       OnAddProgress = null;
       
       Destroy(gameObject);
   }
   

   public void OnQuitButton()
   {
       Destroy(gameObject);
   }
}
