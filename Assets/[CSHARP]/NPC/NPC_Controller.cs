/*
 * Last Edited by Garrett Blake
 * 4/10/2024
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Darklight.Game.Utility;
using static Darklight.UnityExt.CustomInspectorGUI;


[RequireComponent(typeof(NPC_Animator))]
public class NPC_Controller : MonoBehaviour
{
    public NPC_StateMachine stateMachine;
    private NPC_Animator animationManager;
    [SerializeField, ShowOnly] NPCState currentState;

    // =============== [ PUBLIC INSPECTOR VALUES ] =================== //
    public GameObject player;
    [Range(0.1f, 1f)] public float npcSpeed = .2f;
    [Range(0.1f, 1f)] public float followSpeed = .5f;
    [Range(0.1f, 1f)] public float hideSpeed = .5f;
    [Range(0.1f, 1f)] public float chaseSpeed = .5f;
    public float leftBound;
    public float rightBound;
    public float followDistance = 1;
    public float chaseSpeakDistance = .75f;
    [Range(0f, 10f)] public float idleMaxDuration = 3f;
    [Range(0f, 10f)] public float walkMaxDuration = 3f;

    // ================ [ UNITY MAIN METHODS ] =================== //
    void Start()
    {

        // Create instances of the states
        IdleState idleState = new(NPCState.IDLE, new object[] { idleMaxDuration });
        WalkState walkState = new(NPCState.WALK, new object[] { walkMaxDuration });
        SpeakState speakState = new(NPCState.SPEAK, new object[] { player });
        FollowState followState = new(NPCState.FOLLOW, new object[] { player, followSpeed, followDistance });
        HideState hideState = new(NPCState.HIDE, new object[] { player, hideSpeed });
        ChaseState chaseState = new(NPCState.CHASE, new object[] { player, chaseSpeed, chaseSpeakDistance });

        // Create dictionary to hold the possible states
        Dictionary<NPCState, FiniteState<NPCState>> possibleStates = new()
        {
            { NPCState.IDLE, idleState },
            { NPCState.WALK, walkState },
            { NPCState.SPEAK, speakState },
            { NPCState.FOLLOW, followState },
            { NPCState.HIDE, hideState },
            { NPCState.CHASE, chaseState },
        };

        // Create the NPCStateMachine
        stateMachine = new(possibleStates, NPCState.IDLE, this);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Step();
        currentState = stateMachine.currentState;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(new Vector3(leftBound, this.GetComponent<Transform>().position.y, 0), new Vector3(rightBound, this.GetComponent<Transform>().position.y, 0));
    }

    // This is a workaround because you cannot call FindObjectsByType on a reference to this
    public Hideable_Object[] FindHideableObjects()
    {
        return FindObjectsByType<Hideable_Object>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    }
}