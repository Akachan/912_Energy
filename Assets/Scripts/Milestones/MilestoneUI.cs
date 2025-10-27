using TMPro;
using UnityEngine;

namespace Milestones
{
    public class MilestoneUI : MonoBehaviour
    {
        [Header("TopPanel")]
        [SerializeField] private TextMeshProUGUI titleText;

        private MilestoneManager _manager;

        private void Awake()
        {
            _manager = GetComponent<MilestoneManager>();
        
        }

        private void Start()
        {
            _manager.OnMilestoneChange += SetMilestoneValue;
        }

        private void OnDisable()
        {
            _manager.OnMilestoneChange -= SetMilestoneValue;
        }

        private void SetMilestoneValue(int value)
        {
            titleText.text = value.ToString();
        }
    }
}
