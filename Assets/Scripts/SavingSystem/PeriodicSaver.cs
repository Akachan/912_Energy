using System;
using UnityEngine;

namespace SavingSystem
{
    public class PeriodicSaver : MonoBehaviour
    {
        [SerializeField] private float timeBetweenSaves = 60f;
        private float _timeSinceLastSave = 0f;
        private SavingWrapper _savingWrapper;

        private void Awake()
        {
            _savingWrapper = FindFirstObjectByType<SavingWrapper>();
        }

        private void LateUpdate()
        {
            HandlePeriodicSave();
        }

        private void HandlePeriodicSave()
        {
            _timeSinceLastSave += Time.deltaTime;

            if (!(_timeSinceLastSave >= timeBetweenSaves)) return;
            _savingWrapper.SaveInFile();
            //Debug.Log("Periodic Saving");
            _timeSinceLastSave = 0f;
        }
    }
}