// ScriptableObject para ajustar el "tuning" desde el Inspector
// (Assets ▸ Create ▸ Boomerang Fighters ▸ Tuning) y arrastrarlo al MatchRunner.
using UnityEngine;

namespace BoomerangFighters.UnityView
{
    [CreateAssetMenu(menuName = "Boomerang Fighters/Tuning", fileName = "Tuning")]
    public sealed class TuningAsset : ScriptableObject
    {
        public BoomerangFighters.Core.Tuning Values = BoomerangFighters.Core.Tuning.Default();
    }
}
