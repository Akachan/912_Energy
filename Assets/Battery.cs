using System;
using System.Collections;
using System.Collections.Generic;
using Energy;
using Knowledge;
using UnityEngine;
using UnityEngine.Serialization;

public class Battery : MonoBehaviour
{
    [SerializeField] private GameObject saveInfoPanel;
    private Battery _instance;
    public event Action OnPause;
   

    private void Awake()
    {
        ManageSingleton();
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
        else if (_instance != this)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    private void SaveDate()
    {
        var date = DateTime.UtcNow.ToString("o"); // ISO 8601 UTC format
        PlayerPrefs.SetString("SavingTime", date);
        PlayerPrefs.Save(); // Fuerza el guardado inmediato
        print($"SaveDate: {date}");
    }


    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            OnPause?.Invoke();
            SaveDate();
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(RestoreEnergy());
        }
    }

    private void OnApplicationQuit()
    {
        OnPause?.Invoke();
        SaveDate();
    }
 


    IEnumerator RestoreEnergy()
    {
        if (!PlayerPrefs.HasKey("SavingTime")) yield return null;
        yield return new WaitForSeconds(2f);

        var previusEnergy = FindAnyObjectByType<EnergyManager>().GetCurrentEnergy();
        var previusKnowledge = FindAnyObjectByType<KnowledgeManager>().GetCurrentEnergy();
        
        var saveDate = DateTime.Parse(PlayerPrefs.GetString("SavingTime"));
        var now = DateTime.Now;
        var diff = now.Subtract(saveDate);
        var seconds = Mathf.Clamp((float)diff.TotalSeconds, 0f, 2 * 3600f);
        seconds = Mathf.FloorToInt(seconds);
        //energy
        var eps = new BigNumber(PlayerPrefs.GetFloat("EpsBase"), PlayerPrefs.GetInt("EpsExponent"));
        BigNumber energyToAdd = Calculator.MultiplyBigNumbers(eps, seconds);
        FindAnyObjectByType<EnergyManager>().AddEnergy(energyToAdd);



        //knowledge
        var kps = new BigNumber(PlayerPrefs.GetFloat("KpsBase"), PlayerPrefs.GetInt("KpsExponent"));
        BigNumber knowledgeToAdd = Calculator.MultiplyBigNumbers(kps, seconds);
        FindAnyObjectByType<KnowledgeManager>().AddKnowledge(knowledgeToAdd);

        Debug.Log($"RestoreEnergy: {energyToAdd.Base}e{energyToAdd.Exponent}\n RestoreKnowledg {knowledgeToAdd.Base}e{knowledgeToAdd.Exponent}");

        var instance = Instantiate(saveInfoPanel, FindFirstObjectByType<EnergyUI>().transform);
        instance.GetComponent<BatteryUI>().SetInfo(PlayerPrefs.GetString("SavingTime"), 
                                                    now.ToString("dd/MM/yyyy HH:mm:ss"), 
                                                    seconds, eps, kps, 
                                                    FindAnyObjectByType<EnergyManager>().GetCurrentEnergy(),
                                                    FindAnyObjectByType<KnowledgeManager>().GetCurrentEnergy(),
                                                    previusEnergy, previusKnowledge,
                                                    energyToAdd, knowledgeToAdd );
    }

}
