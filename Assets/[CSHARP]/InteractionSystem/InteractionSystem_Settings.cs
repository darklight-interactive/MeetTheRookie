using NaughtyAttributes;
using UnityEngine;

public partial class InteractionSystem
{
    [CreateAssetMenu(menuName = "Darklight/InteractionSystem/Settings")]
    public class SystemSettings : ScriptableObject
    {
        [Header("Interaction Layers")]
        [SerializeField, Layer] string _playerLayer = "Player";
        [SerializeField, Layer] string _npcLayer = "NPC";
        [SerializeField, Layer] string _interactableLayer = "Interactable";

        [Header("Interaction Handler Prefabs")]
        [SerializeField] GameObject _iconInteractionHandlerPrefab;
        [SerializeField] GameObject _dialogueInteractionHandlerPrefab;
        [SerializeField] GameObject _choiceInteractionHandlerPrefab;

        public void AssignLayer(IInteractable interactable)
        {
            interactable.Layer = _interactableLayer;
        }
    }
}

