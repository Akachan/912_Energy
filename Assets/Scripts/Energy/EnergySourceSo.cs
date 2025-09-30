using UnityEngine;

namespace Energy
{
    [CreateAssetMenu(fileName = "New EnergySource", menuName = "EnergySource")]
    public class EnergySourceSo: GeometricSourceSo
    {
        [Header("Energy Source Settings")]
        [SerializeField] private BigNumber costToUnlock;
        [SerializeField] private Sprite illustration;
        [SerializeField] [TextArea(4,50)] private string description;
        
        public BigNumber CostToUnlock => costToUnlock;
        public Sprite Illustration => illustration;
        public string Description => description;
        
    }
}