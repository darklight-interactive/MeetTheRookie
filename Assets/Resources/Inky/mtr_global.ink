/// ------------------------------------------
// Hi <3 -- here's the inky documentation
// ---->> https://github.com/inkle/ink/blob/master/Documentation/WritingWithInk.md << -----
//
// REMEMBER :: 
//      - LISTS are Enums(or just names) that can toggle ( boolean )
//      - VARS are *one or more* data values that store a single type of data
/////////////////////////////////////////////////////////^*

// ====== SCENE HANDLING >>


// ====== SPEAKER HANDLING >>
LIST Speaker = Unkown, Lupe, Misra, Employee, Mel
VAR curSpeaker = Speaker.Lupe
== function NewSpeaker(value)
    ~ curSpeaker = value

// ====== QUEST HANDLING >>
VAR mainQuest = ()
VAR activeQuests = ()
VAR completedQuests = ()

=== function StartQuest(quest)
    ~ activeQuests += quest
=== function CompleteQuest(quest)
    ~ activeQuests -= quest
    ~ completedQuests += quest
=== function IsQuestComplete(quest)
    ~ return completedQuests ? quest
    
// ====== CLUE HANDLING >>
VAR ClueKnowledge = ()
=== function DiscoverClue(clue)
    ~ ClueKnowledge += clue
=== function IsClueFound(clue)
    { LIST_ALL(ClueKnowledge) ? clue:
        // clues are either found, or not
        ~ return ClueKnowledge ? clue
    }

// ====== DAY 1 VARIABLES ======
// --- scene 1 ---
LIST Clues_1_1 = (GAS_PUMP_BROKEN), (CASHREG_BROKEN), (CASHREG_FIX)
LIST QuestChain_1_1 = FIRST_INTERACT, PAY_FOR_GAS
VAR knowledgeState_1 = ()



