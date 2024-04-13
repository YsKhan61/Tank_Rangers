using UnityEngine;


namespace BTG.Actions.UltimateAction
{
    [CreateAssetMenu(fileName = "Invisibility Factory", menuName = "ScriptableObjects/Factory/UltimateActionFactory/InvisibilityFactorySO")]
    public class InvisibilityFactorySO : UltimateActionFactorySO
    {
        [SerializeField]
        InvisibilityDataSO m_InvisibilityData;

        public override IUltimateAction CreateUltimateAction(IUltimateActor actor)
            => new Invisibility(actor, m_InvisibilityData);
    }

}