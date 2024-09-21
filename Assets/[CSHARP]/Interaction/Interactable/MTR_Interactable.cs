using System.Collections;
using System.Collections.Generic;

using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;

using Ink.Runtime;

using NaughtyAttributes;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif



[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class MTR_Interactable : Interactable
{
    // ====== [[ FIELDS ]] ====================================== >>>>
    const string Prefix = "<INTERACTABLE>";

    // -- (( DESTINATION POINTS )) -------- >>
    GameObject Lupe;
    GameObject Misra;


    [Header("Interactable")]
    [SerializeField, ShowAssetPreview] Sprite _sprite;
    [SerializeField] bool onStart;
    public bool isSpawn;



    [Header("State Flags")]
    [ShowOnly, SerializeField] bool _isTarget;
    [ShowOnly, SerializeField] bool _isActive; [ShowOnly, SerializeField] bool _isComplete;



    [Header("Outline")]
    [SerializeField] Material _outlineMaterial;

    [Header("Destination Points")]
    [SerializeField] List<float> destinationPointsRelativeX;
    private List<GameObject> _destinationPoints = new List<GameObject>();


    #region ======== [[ PROPERTIES ]] ================================== >>>>


    SpriteRenderer _spriteRenderer => GetComponent<SpriteRenderer>();



    public bool IsTarget { get => _isTarget; set => _isTarget = value; }
    public bool IsActive { get => _isActive; set => _isActive = value; }
    public bool IsComplete { get => _isComplete; set => _isComplete = value; }
    #endregion

    public override void Initialize()
    {
        base.Initialize();

        // << DESTINATION POINTS >>
        var tempLupe = FindFirstObjectByType<PlayerController>();
        if (tempLupe != null)
        {
            Lupe = tempLupe.gameObject;
        }

        var tempMisra = FindFirstObjectByType<MTR_Misra_Controller>();
        if (tempMisra != null)
        {
            Misra = tempMisra.gameObject;
        }

        _destinationPoints.Clear();
        DestinationPoint[] childrenDestinationPoints = GetComponentsInChildren<DestinationPoint>();
        foreach (var destinationPoint in childrenDestinationPoints)
        {
            _destinationPoints.Add(destinationPoint.gameObject);
        }

        if (destinationPointsRelativeX == null || destinationPointsRelativeX.Count == 0)
        {
            destinationPointsRelativeX = new List<float> { -1, 1 };
        }

        base.Initialize();
    }

    void OnStart()
    {
        if (onStart)
        {
            PlayerInteractor playerInteractor = FindFirstObjectByType<PlayerInteractor>();
            playerInteractor.InteractWith(this, true);
        }
    }

    private void EnableOutline(bool enable)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.material = enable ? _outlineMaterial : null;
        }
    }

    private IEnumerator FlashOutlineRoutine()
    {
        EnableOutline(true);
        yield return new WaitForSeconds(0.25f);
        EnableOutline(false);
    }

    // ====== [[ Destination Points ]] ======================================

    private void OnDrawGizmosSelected()
    {
        if (destinationPointsRelativeX == null) { return; }

        foreach (var destinationPoint in destinationPointsRelativeX)
        {
            Gizmos.DrawLine(new Vector3(transform.position.x + destinationPoint, -5, transform.position.z), new Vector3(transform.position.x + destinationPoint, 5, transform.position.z));
        }
    }

    public void SpawnDestinationPoints()
    {

        var tempLupe = FindFirstObjectByType<PlayerController>();
        if (tempLupe != null)
        {
            Lupe = tempLupe.gameObject;
        }

        float lupeY = gameObject.transform.position.y;
        if (Lupe != null)
        {
            lupeY = Lupe.transform.position.y;
        }

        foreach (var point in _destinationPoints)
        {
            DestroyImmediate(point);
        }
        _destinationPoints.Clear();

        // Create new destination points
        for (int i = 0; i < destinationPointsRelativeX.Count; i++)
        {
            GameObject destinationPoint = new GameObject("Destination Point");
            destinationPoint.AddComponent<DestinationPoint>();
            destinationPoint.transform.position = new Vector3(gameObject.transform.position.x + destinationPointsRelativeX[i], lupeY, gameObject.transform.position.z);
            destinationPoint.transform.SetParent(this.transform);
            _destinationPoints.Add(destinationPoint);
        }
    }

    public void FindDestinationPoints()
    {
        _destinationPoints.Clear();


        DestinationPoint[] childrenDestinationPoints = GetComponentsInChildren<DestinationPoint>();
        foreach (var destinationPoint in childrenDestinationPoints)
        {
            _destinationPoints.Add(destinationPoint.gameObject);
        }
    }

    public List<GameObject> GetDestinationPoints()
    {
        return _destinationPoints;
    }

    private void ChangeSpawnPoints()
    {
        SpawnHandler spawnHandler = SpawnHandler.Instance;
        List<GameObject> interactables = spawnHandler.GetAllInteractables();

        foreach (var currentInteractable in interactables)
        {
            //currentInteractable.GetComponent<Interactable>().isSpawn = false;
        }

        isSpawn = true;

        spawnHandler.SetSpawnPoints(interactables);
    }



}