using System;
using System.Collections;
using System.Collections.Generic;
using Energy;
using Knowledge;
using UnityEngine;

public class Battery : MonoBehaviour
{
    
    private Battery _instance;
    

    private void Awake()
    {
        Application.runInBackground = true;
        ManageSingleton();
        
    }

    private void Start()
    {
        StartCoroutine(RestoreEnergy());
    }

    private void OnDestroy()
    {
        SaveDate();
    }
    private void ManageSingleton()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    private void SaveDate()
    {
        var date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        PlayerPrefs.SetString("SavingTime", date);
        print(DateTime.Now.ToString($"SaveDate: {date}"));
        
        
        
        
    }
    
    IEnumerator  RestoreEnergy()
    {
        if (!PlayerPrefs.HasKey("SavingTime")) yield return null;
        yield return new WaitForSeconds(3f);
        
        var saveDate = DateTime.Parse(PlayerPrefs.GetString("SavingTime"));
        var now = DateTime.Now;
        var diff = now.Subtract(saveDate);
        var hours = Mathf.Clamp(diff.Seconds, 0f, 2 * 3600f);
        
        //energy
        var eps = new BigNumber(PlayerPrefs.GetFloat("EpsBase"), PlayerPrefs.GetInt("EpsExponent"));
        BigNumber energyToAdd = Calculator.MultiplyBigNumbers(eps, hours);
        FindAnyObjectByType<EnergyManager>().AddEnergy(energyToAdd);
        
        
        
        //knowledge
        var kps = new BigNumber(PlayerPrefs.GetFloat("KpsBase"), PlayerPrefs.GetInt("KpsExponent"));
        BigNumber knowledgeToAdd = Calculator.MultiplyBigNumbers(kps, hours);
        FindAnyObjectByType<KnowledgeManager>().AddKnowledge(knowledgeToAdd);
        
        Debug.Log($"RestoreEnergy: {energyToAdd.Base}e{energyToAdd.Exponent}\n RestoreKnowledg {knowledgeToAdd.Base}e{knowledgeToAdd.Exponent}");
        
    }
}
