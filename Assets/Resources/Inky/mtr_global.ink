/// ------------------------------------------
// Hi <3 -- here's the inky documentation
// ---->> https://github.com/inkle/ink/blob/master/Documentation/WritingWithInk.md << -----
//
// REMEMBER :: 
//      - LISTS are Enums(or just names) that can toggle ( boolean )
//      - VARS are *one or more* data values that store a single type of data
/////////////////////////////////////////////////////////^*

// ====== INCLUDE == >>

//DAY 1
INCLUDE mtr_mystery.ink

INCLUDE _characters/Misra.ink
INCLUDE _characters/Roy Rodgerson.ink
INCLUDE _characters/Teens.ink
INCLUDE _characters/Jenny.ink
INCLUDE _characters/Calvin.ink
INCLUDE _characters/Josh.ink
INCLUDE _characters/Jenkins.ink
INCLUDE _characters/Irene.ink

INCLUDE _day 1/mtr_level1_melOmart.ink
INCLUDE _day 1/mtr_level2_precinct_DS.ink
INCLUDE _day 1/mtr_level3_wineryMorning.ink
INCLUDE _day 1/mtr_level4_mainstreet.ink
INCLUDE _day 1/mtr_level4.2_genstore.ink
INCLUDE _day 1/mtr_level4.3_arcade.ink
INCLUDE _day 1/mtr_level4.4_bar.ink
INCLUDE _day 1/mtr_level4.5_bar_DS.ink
INCLUDE _day 1/mtr_level5.1_stakeout_DS.ink
INCLUDE _day 1/mtr_level5.2_wineryNIGHT.ink

// DAY 2 
INCLUDE _day 2/mtr_level7.1_DS.ink
INCLUDE _day 2/mtr_level6.1_melOmart.ink

// ====== EXTERNAL FUNCTIONS == >>
EXTERNAL ChangeGameScene(knotName)
//EXTERNAL AddSynthesisClue(clue)
EXTERNAL PlaySpecialAnimation(speaker)

== function ChangeGameScene(knotName)
    ~ return

== function AddSynthesisClue(clue)
    ~ return


// ====== SPEAKER HANDLING == >>
LIST Speaker = (Unknown), (Misra), (Lupe), (Chief_Thelton), (Marlowe), (Beth), (Mel), (Roy_Rodgerson), (Jenny), (Calvin), (Josh), (Irene), (Jenkins)
VAR CURRENT_SPEAKER = Speaker.Lupe
=== function SetSpeaker(value)
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
LIST GLOBAL_KNOWLEDGE = (DEFAULTCLUE)
=== function DiscoverClue(clue)
    #DiscoverClue >> {clue}
    ~ GLOBAL_KNOWLEDGE += clue
    ~ AddSynthesisClue(clue)
=== function IsClueFound(clue)
    ~ return GLOBAL_KNOWLEDGE ? clue


// -------------------- LEVEL 1 ------------------------------------
LIST Level1_Quests = (first_interact), (pay_for_gas), (look_at_tree)

// -------------------- LEVEL 3 ------------------------------------
LIST Level3_Quests = (talk_to_misra_quest), (visit_fence), (visit_window), (discover_outside_clues), (discuss_misra), (visit_footsteps), (visit_floor), (visit_barrels), (visit_backroom_door), (discover_inside_clues), (visit_inside_window)

// -------------------- LEVEL 4 ------------------------------------
LIST Level4_Quests = (entered_arcade), (visited_goop), (visited_symbol), (visited_misra), (visited_roy), (complete_gen_store), (complete_arcade), (visited_machines), (visited_jenny), (visited_calvin), (visited_josh), (irene_intro), (memorial_plaque_visited), (irene_convo_1), (irene_convo_2), (jenkins_wakes_up), (calvin_first_interact), (josh_first_interact), (KR_jenny_quest), (personal_info_jenny_quest), (winery_jenny_quest), (personal_info_josh_quest), (KR_josh_quest), (winery_josh_quest), (suspects), (jenkins_intro), (what_is_hosi), (lupe_not_a_cop), (jenny_KR_question), (jenny_local_question), (jenny_personal_question), (jenny_winery_question), (jenny_crazies_question), (calvin_KR_questions), (calvin_personal_question), (calvin_local_question), (josh_KR_question), (josh_personal_questions), (jenny_first_interact), (calvin_KR_question), (calvin_winery_question), (josh_winery_question), (josh_suspicion), (jenny_suspicion), (calvin_suspicion), (car_first_interact), (irene_gives_cue), (jenkins_first_interact), (visit_roy_window), (gooptalk)





// ----------------------------------------------------
//  LEVEL 5 : Quests and Clues
// ----------------------------------------------------

    LIST Level5_Quests = (winerygraph) , (blueprint) , (newspaper), (handwrittennote)

// LEVEL 6 
    LIST Level6_Quests = (haggle)








