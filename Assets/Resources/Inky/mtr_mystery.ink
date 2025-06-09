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

LIST Mystery2 = (evidence_roy), (evidence_pamphlet), (evidence_josh), (evidence_calvin), (evidence_jenny), (evidence_irene), (evidence_jenkins)
LIST Mystery3 = (evidence_winerygraph) , (evidence_blueprint) , (evidence_newspaper), (evidence_handwrittennote), (evidence_pinpad)