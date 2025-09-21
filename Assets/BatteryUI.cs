using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utilities;

public class BatteryUI : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI infoText;
   
   
   public void SetInfo(string saveDate, string nowDate, float second, 
                        BigNumber eps, BigNumber kps, 
                        BigNumber currentEnergy, BigNumber currentKnowledge,
                        BigNumber previousEnergy, BigNumber previousKnowledge,
                        BigNumber energyToAdd, BigNumber knowledgeToAdd)
   {
       infoText.text = $"Fecha de guardado: {saveDate} \n" +
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

   public void OnQuitButton()
   {
       Destroy(gameObject);
   }
}
