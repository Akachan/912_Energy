using System;
using System.Collections;
using System.Globalization;
using Energy;
using Knowledge;
using Newtonsoft.Json.Linq;
using SavingSystem;
using UnityEngine;
using Utilities;

namespace Battery
{
    public class Battery : MonoBehaviour
    {
        [SerializeField] private GameObject restoreInfoPanel;
        

        private static Battery _instance;
        public SavingWrapper _saving;
        public event Action OnPause;
   

        private void Awake()
        {
            ManageSingleton();
            _saving = FindFirstObjectByType<SavingWrapper>();
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
            _saving.SaveInFile(SavingKeys.Battery.SavingTime, JToken.FromObject(date));
            print($"SaveDateInString: {date}");
            
            /*
            PlayerPrefs.SetString("SavingTime", date);
            PlayerPrefs.Save(); // Fuerza el guardado inmediato
            
            */
        }

#if UNITY_ANDROID
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

        
#endif
#if UNITY_WEBGL
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
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
#endif
        private void OnApplicationQuit()
        {
            OnPause?.Invoke();
            SaveDate();
        }
 


        IEnumerator RestoreEnergy()
        {
            
            //if (!PlayerPrefs.HasKey("SavingTime")) yield return null;
            var dateData = _saving.GetSavingValue(SavingKeys.Battery.SavingTime);
            if (dateData == null) yield return null;
            var date = dateData.ToObject<DateTime>().ToUniversalTime();
            
            
            yield return new WaitForSeconds(0.2f);

            //debug
            /*
            var previusEnergy = FindAnyObjectByType<EnergyManager>().GetResources();
            var previusKnowledge = FindAnyObjectByType<KnowledgeManager>().GetResources();
            */ 
            
            //Calculate Time
            //var saveDate = DateTime.Parse(PlayerPrefs.GetString("SavingTime"));
            
            var now = DateTime.UtcNow;
            var diff = now.Subtract(date);
            if (diff.TotalHours > 2)
            {
                diff = TimeSpan.FromHours(2);
            }
            var seconds = Mathf.FloorToInt((float)diff.TotalSeconds);
            print($"Fecha anterior = {date}, Ahora= {now}, Pasaron Seconds: {seconds}");
           
            //Calculate energy
            var energyToAdd = CalculateEnergyToAdd(seconds);

            //Calculate knowledge
            var kps = new BigNumber(PlayerPrefs.GetFloat("KpsBase"), PlayerPrefs.GetInt("KpsExponent"));
            BigNumber knowledgeToAdd = Calculator.MultiplyBigNumbers(kps, seconds);
           
            
            if (seconds < 5 * 60)
            {
                AddProgress(energyToAdd, knowledgeToAdd);
            }
            else
            {
                VerifyPreviousBattery();
                InstantiateBatteryUI(diff, energyToAdd, knowledgeToAdd);
            }
            
        }

        private BigNumber CalculateEnergyToAdd(int seconds)
        {
            //var eps = new BigNumber(PlayerPrefs.GetFloat("EpsBase"), PlayerPrefs.GetInt("EpsExponent"));
            
            var eps = _saving.GetSavingValue(SavingKeys.Energy.Rps)?.ToBigNumber();
            
            BigNumber energyToAdd = Calculator.MultiplyBigNumbers(eps, seconds);
            return energyToAdd;
        }

        private static void VerifyPreviousBattery()
        {
            var previousBattery = FindFirstObjectByType<BatteryUI>();
            if (previousBattery != null)
            {
                previousBattery.AddProgress();   
            }
        }
        private void InstantiateBatteryUI(TimeSpan diff, BigNumber energyToAdd, BigNumber knowledgeToAdd)
        {
            var instance = Instantiate(restoreInfoPanel, FindFirstObjectByType<EnergyUI>().transform);
            var batteryUI = instance.GetComponent<BatteryUI>();
            batteryUI.SetBatteryInfo(diff, energyToAdd, knowledgeToAdd);
            batteryUI.OnAddProgress += () => AddProgress(energyToAdd, knowledgeToAdd);
        }
        
        private void AddProgress(BigNumber energyToAdd, BigNumber knowledgeToAdd)
        {
            FindAnyObjectByType<EnergyManager>().AddResources(energyToAdd);
            FindAnyObjectByType<KnowledgeManager>().AddResources(knowledgeToAdd);
            
        }

    }
}
