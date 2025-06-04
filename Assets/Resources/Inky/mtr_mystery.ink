

EXTERNAL DiscoverMystery(mystery_index)


// ====== CLUE HANDLING == >>
LIST MYSTERY_KNOWLEDGE = DEFAULT
=== function DiscoverMystery(mystery_index)
    ~ return

LIST GLOBAL_KNOWLEDGE = DEFAULT
=== function DiscoverClue(clue)
    ~ GLOBAL_KNOWLEDGE += clue
=== function IsClueFound(clue)
    ~ return GLOBAL_KNOWLEDGE ? clue

LIST Mystery0 = (evidence_broken_gas_pump), (evidence_broken_cash_reg), (evidence_cashreg_fix)

LIST Mystery1 = (evidence_broken_window) , (evidence_footsteps), (evidence_fence), (evidence_claw_marks), (evidence_damages), (evidence_handprint)

LIST Mystery2 = (merch_pamphlet), (roys_suspicion), (roy_personal_info), (roy_winery_closing), (golden_age), (tragedy), (rocky_years), (roy_town_history), (HOSI_mentioned), (jenny_crazies), (HOSI_calvin), (josh_suspects), (calvin_suspects), (goats_mentioned), (sacrifice_mentioned), (KR_irene), (closed_shops_irene), (jenkins_winery), (sarah_mentioned), (council_mentioned), (sacrifice_mentioned_jenkins), (symbol_evidence), (goop_evidence), (HOSI_highscore), (personal_info_jenny), (personal_info_josh), (winery_josh), (KR_josh), (calvin_questions_intro), (josh_questions_intro)
