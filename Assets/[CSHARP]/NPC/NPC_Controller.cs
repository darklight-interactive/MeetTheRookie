using System;
using System.Collections.Generic;
using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Utility;
using UnityEngine;
using static WalkState;

[RequireComponent(typeof(MTRCharacterAnimator))]
public class NPC_Controller : MonoBehaviour
{
    public NPC_StateMachine stateMachine;
    public MTRCharacterAnimator animator => GetComponent<MTRCharacterAnimator>();

    [SerializeField, ShowOnly]
    NPCState currentState;

    // =============== [ PUBLIC INSPECTOR VALUES ] =================== //
    public GameObject player => FindFirstObjectByType<MTRPlayerInput>().gameObject;
    public NPCState startingState = NPCState.IDLE;

    [Tooltip("State to return to after animation")]
    public NPCState stateAfterAnimation = NPCState.IDLE;

    [Tooltip("Speed for the WALK state")]
    [Range(0.1f, 1f)]
    public float npcSpeed = .2f;

    [Tooltip("Speed for the FOLLOW state")]
    [Range(0.1f, 1f)]
    public float followSpeed = .5f;

    [Tooltip("Speed for the HIDE state")]
    [Range(0.1f, 1f)]
    public float hideSpeed = .5f;

    [Tooltip("Speed for the CHASE state")]
    [Range(0.1f, 1f)]
    public float chaseSpeed = .5f;

    [Tooltip("Distance the NPC will follow the player")]
    public float followDistance = 1;

    [Tooltip("Distance to cause dialogue in the SPEAK state")]
    public float chaseSpeakDistance = .75f;

    public float walkDestinationX = 0f;
    public NPCState stateAfterWalking = NPCState.IDLE;
    private DestinationWrapper destinationWrapper;

    // ================ [ UNITY MAIN METHODS ] =================== //
    public virtual void Start()
    {
        destinationWrapper = new DestinationWrapper() { walkDestinationX = walkDestinationX };

        // Create instances of the states
        IdleState idleState = new(stateMachine, NPCState.IDLE, new object[] { stateMachine, this });
        WalkState walkState =
            new(
                stateMachine,
                NPCState.WALK,
                new object[] { stateMachine, this, npcSpeed, destinationWrapper, stateAfterWalking }
            );
        SpeakState speakState = new(stateMachine, NPCState.SPEAK, new object[] { stateMachine });
        FollowState followState =
            new(
                stateMachine,
                NPCState.FOLLOW,
                new object[] { stateMachine, this, followDistance, followSpeed }
            );
        HideState hideState =
            new(stateMachine, NPCState.HIDE, new object[] { stateMachine, this, hideSpeed });
        ChaseState chaseState =
            new(
                stateMachine,
                NPCState.CHASE,
                new object[] { stateMachine, chaseSpeakDistance, chaseSpeed }
            );
        PlayAnimationState playAnimationState =
            new(
                stateMachine,
                NPCState.PLAY_ANIMATION,
                new object[] { stateMachine, stateAfterAnimation }
            );

        // Create dictionary to hold the possible states
        Dictionary<NPCState, FiniteState<NPCState>> possibleStates =
            new()
            {
                { NPCState.IDLE, idleState },
                { NPCState.WALK, walkState },
                { NPCState.SPEAK, speakState },
                { NPCState.FOLLOW, followState },
                { NPCState.HIDE, hideState },
                { NPCState.CHASE, chaseState },
                { NPCState.PLAY_ANIMATION, playAnimationState },
            };

        // Create the NPCStateMachine
        stateMachine = new(possibleStates, startingState, this, animator);

        // Hacky solution to fix null reference bug, setting the stateMachine field for each state
        idleState._stateMachine = stateMachine;
        walkState._stateMachine = stateMachine;
        speakState._stateMachine = stateMachine;
        followState._stateMachine = stateMachine;
        hideState._stateMachine = stateMachine;
        chaseState._stateMachine = stateMachine;
        playAnimationState._stateMachine = stateMachine;
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Step();
        currentState = stateMachine.CurrentState;
        destinationWrapper.walkDestinationX = walkDestinationX;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(
            new Vector3(walkDestinationX, 0, this.GetComponent<Transform>().position.z),
            new Vector3(walkDestinationX, 5, this.GetComponent<Transform>().position.z)
        );
    }

    // This is a workaround because you cannot call FindObjectsByType on a reference to this
    public Hideable_Object[] FindHideableObjects()
    {
        return FindObjectsByType<Hideable_Object>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );
    }
}
