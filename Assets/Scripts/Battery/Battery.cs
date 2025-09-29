using System;
using System.Collections;
using Energy;
using Knowledge;
using UnityEngine;

namespace Battery
{
    public class Battery : MonoBehaviour
    {
        [SerializeField] private GameObject restoreInfoPanel;
        private static Battery _instance;
        public event Action OnPause;
   

        private void Awake()
        {
            ManageSingleton();

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

        private void OnApplicationQuit()
        {
            OnPause?.Invoke();
            SaveDate();
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
        
 


        IEnumerator RestoreEnergy()
        {
            if (!PlayerPrefs.HasKey("SavingTime")) yield return null;
            yield return new WaitForSeconds(0.2f);

            //debug
            var previusEnergy = FindAnyObjectByType<EnergyManager>().GetResources();
            var previusKnowledge = FindAnyObjectByType<KnowledgeManager>().GetResources();
        
            //Calculate Time
            var saveDate = DateTime.Parse(PlayerPrefs.GetString("SavingTime"));
            var now = DateTime.Now;
            var diff = now.Subtract(saveDate);
            if (diff.TotalHours > 2)
            {
                diff = TimeSpan.FromHours(2);
            }
            var seconds = Mathf.FloorToInt((float)diff.TotalSeconds);
           
            //Calculate energy
            var eps = new BigNumber(PlayerPrefs.GetFloat("EpsBase"), PlayerPrefs.GetInt("EpsExponent"));
            BigNumber energyToAdd = Calculator.MultiplyBigNumbers(eps, seconds);
            
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
