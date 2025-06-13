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
EXTERNAL ChangeGameScene(knotName, spawnIndex)
EXTERNAL SetSpeaker(speaker)
EXTERNAL RequestSpecialUI(special_ui_tag)
EXTERNAL PlaySpecialAnimation(speaker)
EXTERNAL PlaySFX(sfx)


// ====== SCENE HANDLING == >>
=== function ChangeGameScene(knotName, spawnIndex)
    ~ SetSpeaker(Speaker.Lupe)
    ~ return

// ====== SPEAKER HANDLING == >>
LIST Speaker = (Unknown), (Misra), (Lupe), (Chief_Thelton), (Marlowe), (Beth), (Mel), (Roy_Rodgerson), (Jenny), (Calvin), (Josh), (Irene), (Jenkins)
VAR CURRENT_SPEAKER = Speaker.Lupe
=== function SetSpeaker(speaker)
    # SetSpeaker >> {speaker}
    ~ CURRENT_SPEAKER = speaker
    ~ return

// ====== SPECIAL_UI == >>
LIST SPECIAL_UI = (su_pamphlet), (su_plaque), (su_pinpad), (su_note), (su_blueprint)
=== function RequestSpecialUI(special_ui_tag)
    ~ return

// ====== QUEST HANDLING == >>
VAR MAIN_QUEST = () // <- highest priority quest
LIST ACTIVE_QUESTS = DEFAULT // <- overwrite this list
LIST COMPLETED_QUESTS = DEFAULT // <- all completed quests
=== function StartQuest(quest)
    #StartQuest >> {quest}
    ~ ACTIVE_QUESTS += quest
=== function CompleteQuest(quest)
    #CompleteQuest >> {quest}
    ~ ACTIVE_QUESTS -= quest
    ~ COMPLETED_QUESTS += quest
=== function IsQuestComplete(quest)
    #IsQuestComplete >> {quest}
    ~ return COMPLETED_QUESTS ? quest
=== function IsQuestActive(quest)
    #IsQuestActive >> {quest}
    ~ return ACTIVE_QUESTS ? quest


=== function openDoor()
    ~PlaySFX("Doors/doorOpen")
    ~ return
=== function closeDoor()
    ~PlaySFX("Doors/doorClose")
    ~ return

// -------------------- LEVEL 1 ------------------------------------
LIST Level1_Quests = (first_interact), (pay_for_gas), (look_at_tree)

// -------------------- LEVEL 3 ------------------------------------
LIST Level3_Quests = (talk_to_misra_quest), (discover_outside_clues), (discuss_misra), (discover_inside_clues)

// -------------------- LEVEL 4 ------------------------------------
LIST Level4_1_Quests =  (visited_goop), (visited_symbol), (visited_misra),   (calvin_first_interact),  (suspects),  (car_first_interact)

LIST Level4_2_Quests = (visited_roy), (roy_personal_info), (roy_winery_closing), (roy_town_history), (roy_window), (roy_rocky_years), (roy_tragedy), (roy_golden_age), (complete_gen_store)

LIST Level4_3_Quests = (entered_arcade), (visited_machines), (what_is_hosi), (hosi_highscore), (lupe_not_a_cop), (complete_arcade), (visited_jenny), (jenny_first_interact), (jenny_KR), (jenny_personal_info), (jenny_winery), (jenny_local), (jenny_crazies), (visited_josh), (josh_first_interact), (josh_personal_info), (josh_KR), (josh_winery), (visited_calvin), (calvin_hosi), (calvin_KR), (calvin_personal_info), (calvin_local), (calvin_winery)

LIST Level4_4_Quests = (memorial_plaque_visited),(irene_intro), (irene_convo_1), (irene_convo_2), (irene_mention_goats), (irene_sacrifice), (irene_gives_cue), (irene_KR), (irene_closed_shops),  (jenkins_first_interact), (jenkins_intro), (jenkins_wakes_up), (jenkins_winery), (jenkins_sacrifice), (jenkins_council), (jenkins_sarah), (gooptalk)

LIST Level5_Quests = (winerygraph) , (blueprint) , (newspaper), (handwrittennote), (discover_pinpad)

// LEVEL 6 
LIST Level6_Quests = (haggle)



=== SFX
= playOpenDoor
~ openDoor()
-> DONE
= playCloseDoor
~ closeDoor()
-> DONE


=== scene_default ===
= interaction_default
~ SetSpeaker(Speaker.Lupe)
Hey, it's Lupe here.
It seems like you're hanging out in the default scene.
Just so you're aware, this is for bug testing.
Sky really likes little things like to help figure things out
They tend to break stuff.
-> DONE

