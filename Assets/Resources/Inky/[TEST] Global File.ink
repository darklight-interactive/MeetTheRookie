// ====== DAY 1 VARIABLES ======
// --- scene 1 ---
LIST Clues_1_1 = (GAS_PUMP_BROKEN), (CASHREG_BROKEN), (CASHREG_FIX)
LIST QuestChain_1_1 = FIRST_INTERACT, PAY_FOR_GAS
VAR knowledgeState_1 = ()
VAR activeQuests = ()
VAR completedQuests = ()


// ====== SPEAKER FUNCTION ======
LIST Speaker = Lupe, Misra
VAR curSpeaker = Speaker.Lupe
== function set_speaker(value)
    ~ curSpeaker = value

// ====== QUEST CHAIN FUNCTION =======
=== function StartQuest(active, quest)
    ~ active += quest
=== function CompleteQuest(active, complete, quest)
    ~ active -= quest
    ~ complete += quest
=== function IsQuestComplete(quest)
    ~ return completedQuests ? quest
    
// ====== CLUES FUNCTION =======

=== function DiscoverClue(state, clue)
    ~ state += clue
    
=== function IsClueFound(clues, state, clue)
    { LIST_ALL(clues) ? clue:
        // clues are either found, or not
        ~ return state ? clue
    }


