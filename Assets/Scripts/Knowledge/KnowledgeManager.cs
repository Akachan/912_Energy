using SavingSystem;
using UnityEngine;
using Utilities;

namespace Knowledge
{
    public class KnowledgeManager : BigResourceManager
    {
        private BigNumber _kps;
        private KnowledgeUI _ui;
        private SavingWrapper _saving;
        private float _currentTime;

        private void Awake()
        {
            _ui = GetComponent<KnowledgeUI>();
            CurrentResources = new BigNumber(0, 0);
            _saving = FindFirstObjectByType<SavingWrapper>();
        }

        private void Start()
        {
            Load();
            Battery.Battery battery = FindFirstObjectByType<Battery.Battery>();
            battery.OnPause += SaveRps;
            
        }

        private void Update()
        {
            UpdateResources();
            
        }

        private void UpdateResources()
        {
            _currentTime += Time.deltaTime;
            if (!(_currentTime >= 1f)) return;
            AddResources(_kps);
            _currentTime = 0f;
            
            _ui.SetKnowledgeValue(CurrentResources);
            Save();
        }
        
        public void SetKps(BigNumber kps)
        {
            if (_kps == null)
            {
                _kps = new BigNumber(0, 0);
            }

            _kps = kps;
        }



        //GUARDADO//
        public override void Save()
        {
            SaveCurrentResources();
        }
        
        private void SaveCurrentResources()
        {
            _saving.SetTemporalSave(SavingKeys.Knowledge.Current, CurrentResources.ToToken());
        }
        private void SaveRps()
        {
            _saving.SaveInFile(SavingKeys.Knowledge.Rps, _kps.ToToken());
        }


        public override void Load()
        {
            var resources= _saving.GetSavingValue(SavingKeys.Knowledge.Current);
            if (resources == null)
            {
                Debug.Log("No se ha encontrado el recurso en el archivo de guardado.");
                return;
            }
            CurrentResources = resources?.ToBigNumber();
            _ui.SetKnowledgeValue(CurrentResources);
            
        }
    }
}