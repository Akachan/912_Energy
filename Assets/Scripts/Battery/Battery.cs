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
            yield return new WaitForSeconds(1f);

            //debug
            var previusEnergy = FindAnyObjectByType<EnergyManager>().GetCurrentEnergy();
            var previusKnowledge = FindAnyObjectByType<KnowledgeManager>().GetCurrentEnergy();
        
            var saveDate = DateTime.Parse(PlayerPrefs.GetString("SavingTime"));
            var now = DateTime.Now;
            var diff = now.Subtract(saveDate);
            if (diff.TotalHours > 2)
            {
                diff = TimeSpan.FromHours(2);

            }
            var seconds = Mathf.FloorToInt((float)diff.TotalSeconds);
           
            //energy
            var eps = new BigNumber(PlayerPrefs.GetFloat("EpsBase"), PlayerPrefs.GetInt("EpsExponent"));
            BigNumber energyToAdd = Calculator.MultiplyBigNumbers(eps, seconds);
            

            //knowledge
            var kps = new BigNumber(PlayerPrefs.GetFloat("KpsBase"), PlayerPrefs.GetInt("KpsExponent"));
            BigNumber knowledgeToAdd = Calculator.MultiplyBigNumbers(kps, seconds);
           
            
            //Restaurar UI
            


            if (seconds < 5 * 60)
            {
                AddProgress(energyToAdd, knowledgeToAdd);
               
            }
            else
            {
                var instance = Instantiate(restoreInfoPanel, FindFirstObjectByType<EnergyUI>().transform);
                var batteryUI = instance.GetComponent<BatteryUI>();
                batteryUI.SetBatteryInfo(diff, energyToAdd, knowledgeToAdd);
                batteryUI.OnAddProgress += () => AddProgress(energyToAdd, knowledgeToAdd);;
                
                /*
                instance.GetComponent<BatteryUI>().SetDebugInfo(PlayerPrefs.GetString("SavingTime"), 
                    now.ToString("dd/MM/yyyy HH:mm:ss"), 
                    seconds, eps, kps, 
                    FindAnyObjectByType<EnergyManager>().GetCurrentEnergy(),
                    FindAnyObjectByType<KnowledgeManager>().GetCurrentEnergy(),
                    previusEnergy, previusKnowledge,
                    energyToAdd, knowledgeToAdd );
                */
            }
            
        }

        private void AddProgress(BigNumber energyToAdd, BigNumber knowledgeToAdd)
        {
            FindAnyObjectByType<EnergyManager>().AddEnergy(energyToAdd);
            FindAnyObjectByType<KnowledgeManager>().AddKnowledge(knowledgeToAdd);
            
        }

    }
}
