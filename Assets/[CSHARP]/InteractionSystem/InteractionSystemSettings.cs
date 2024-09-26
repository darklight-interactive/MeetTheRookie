using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Darklight/InteractionSystem/Settings")]
public class InteractionSystemSettings : ScriptableObject
{
    [Header("Interaction Layers")]
    [SerializeField, Layer] string _playerLayer = "Player";
    [SerializeField, Layer] string _npcLayer = "NPC";
    [SerializeField, Layer] string _interactableLayer = "Interactable";

    public string PlayerLayer => _playerLayer;
    public string NPCLayer => _npcLayer;
    public string InteractableLayer => _interactableLayer;

    public LayerMask GetCombinedNPCAndInteractableLayer()
    {
        return LayerMask.GetMask(_npcLayer, _interactableLayer);
    }
}

