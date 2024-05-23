/// ------------------------------------------
// Hi <3 -- here's the inky documentation
// ---->> https://github.com/inkle/ink/blob/master/Documentation/WritingWithInk.md << -----
//
// REMEMBER :: 
//      - LISTS are Enums(or just names) that can toggle ( boolean )
//      - VARS are *one or more* data values that store a single type of data
/////////////////////////////////////////////////////////^*

// ====== INCLUDE == >>
INCLUDE _characters/Misra.ink
INCLUDE _characters/Roy Rodgerson.ink
INCLUDE _characters/Jenny.ink
INCLUDE _characters/Calvin.ink
INCLUDE _characters/Josh.ink
INCLUDE _characters/Jenkins.ink
INCLUDE _characters/Irene.ink

//INCLUDE _day 1/mtr_level1_melOmart.ink

INCLUDE _extra/mtr_scene2_precinctDEMO.ink
//INCLUDE _day 1/mtr_level2_precinct.ink

INCLUDE _day 1/mtr_level4.1_mainstreet.ink
INCLUDE _day 1/mtr_level4.2_genstore.ink
INCLUDE _day 1/mtr_level4.3_arcade.ink
INCLUDE _day 1/mtr_level4.4_bar.ink

// ====== SPEAKER HANDLING == >>
LIST Speaker = (Unkown), (Misra), (Lupe), (Chief_Thelton), (Marlowe), (Beth), (Mel), (Roy_Rodgerson), (Jenny), (Calvin), (Josh), (Irene), (Jenkins)

VAR CURRENT_SPEAKER = Speaker.Lupe
== function SetSpeaker(value)
    # SetSpeaker >> {value}
    ~ CURRENT_SPEAKER = value

// ====== QUEST HANDLING == >>
VAR MAIN_QUEST = () // <- highest priority quest
LIST ACTIVE_QUEST_CHAIN = DEFAULTQUEST // <- overwrite this list
LIST COMPLETED_QUESTS = DEFAULTQUEST // <- all completed quests
=== function SetActiveQuestChain(chain)
    # SetActiveQuestChain >> {chain}
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
=== function IsQuestActive(quest)
    #IsQuestActive >> {quest}
    ~ return ACTIVE_QUEST_CHAIN ? quest

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

// ----------------------------------------------------
//  LEVEL 1 : Quests and Clues
// ----------------------------------------------------
LIST Level1_Clues = (GAS_PUMP_BROKEN), (CASHREG_BROKEN), (CASHREG_FIX)
LIST Level1_Quests = (FIRST_INTERACT), (PAY_FOR_GAS)

// ----------------------------------------------------
//  LEVEL 3 : Quests and Clues
// ----------------------------------------------------
LIST Level3_Clues = broken_window , footsteps, fence, claw_marks, damages
LIST Level3_Quests = FIRST_INTERACT_2, VISIT_FENCE, VISIT_WINDOW, DISCUSS_MISRA, VISIT_FOOTSTEPS, VISIT_FLOOR, VISIT_BARRELS, VISIT_BACKROOM_DOOR

// ----------------------------------------------------
//  LEVEL 4 : Quests and Clues
// ----------------------------------------------------
LIST Level4_Quests = visited_goop, visited_symbol, visited_misra, visited_roy, complete_gen_store, complete_arcade, visited_machines, visited_jenny, visited_calvin, visited_josh, irene_intro, memorial_plaque_visited, irene_convo_1, irene_convo_2, jenkins_wakes_up, calvin_first_interact, josh_first_interact, KR_jenny_quest, personal_info_jenny_quest, winery_jenny_quest, personal_info_josh_quest, KR_josh_quest, winery_josh_quest, suspects, jenkins_intro


LIST Level4_Clues = merch_pamphlet, roys_suspicion, roy_personal_info, roy_winery_closing, golden_age, tragedy, rocky_years, roy_town_history, HOSI_mentioned, jenny_crazies, HOSI_calvin, jenny_suspects, josh_suspects, calvin_suspects, goats_mentioned, sacrifice_mentioned, KR_irene, closed_shops_irene, jenkins_winery, sarah_mentioned, council_mentioned, sacrifice_mentioned_jenkins, symbol_evidence, goop_evidence, HOSI_highscore, personal_info_jenny, winery_jenny, KR_Jenny, personal_info_josh, winery_josh, KR_josh
















