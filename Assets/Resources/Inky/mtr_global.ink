/// ------------------------------------------
// Hi <3 -- here's the inky documentation
// ---->> https://github.com/inkle/ink/blob/master/Documentation/WritingWithInk.md << -----
//
// REMEMBER :: 
//      - LISTS are Enums(or just names) that can toggle ( boolean )
//      - VARS are *one or more* data values that store a single type of data
/////////////////////////////////////////////////////////^*

// ====== INCLUDE == >>
INCLUDE mtr_level1_melOmart.ink
//INCLUDE mtr_level3_wineryMorning.ink
//INCLUDE mtr_level4_mainstreet.ink


// ====== SCENE HANDLING == >>
-> global_main
=== global_main ===
#Meet The Rookie - Global Main
* [Level 1] -> level1


// ====== SPEAKER HANDLING == >>
LIST Speaker = Unkown, Misra, Lupe, Chief_Thelton, Marlowe, Beth, Mel, Roy_Rodgerson, Jenny, Calvin, Josh, Irene, Jenkins
VAR curSpeaker = Speaker.Lupe
== function SetSpeaker(value)
    # SetSpeaker >> {value}
    ~ curSpeaker = value


// ====== QUEST HANDLING == >>
VAR mainQuest = () // <- highest priority quest
LIST ACTIVE_QUEST_CHAIN = DEFAULTQUEST // <- overwrite this list
LIST COMPLETED_QUESTS = DEFAULTQUEST // <- all completed quests
=== function SetActiveQuestChain(chain)
    #SetActiveQuestChain >> {chain}
    ~ ACTIVE_QUEST_CHAIN = chain
=== function StartQuest(quest)
    #StartQuest >> {quest}
    ~ ACTIVE_QUEST_CHAIN += quest
=== function CompleteQuest(quest)
    #CompleteQuest >> {quest}
    ~ ACTIVE_QUEST_CHAIN -= quest
    ~ COMPLETED_QUESTS += quest
=== function IsQuestComplete(quest)
    #IsQuestComplete >> {quest}
    ~ return COMPLETED_QUESTS ? quest

// ====== CLUE HANDLING == >>
LIST GLOBAL_KNOWLEDGE = DEFAULTCLUE
=== function DiscoverClue(clue)
    #DiscoverClue >> {clue}
    ~ GLOBAL_KNOWLEDGE += clue
=== function IsClueFound(clue)
    { LIST_ALL(GLOBAL_KNOWLEDGE) ? clue:
        // clues are either found, or not
        ~ return GLOBAL_KNOWLEDGE ? clue
    }



