=== scene4_2 ===
# Location: IdaHome and Goods, General Store
    * [talk_to_roy] -> Roy_Dialogue  
    * [store_window] -> store_window
    * [merch_shirt] -> merch_shirt
    * [merch_sticker] -> merch_sticker
    * [merch_pamphlet] -> pamphlet
    
        ->DONE
= intro
    -> Roy_Dialogue.roy_intro_cutscene
    
= pamphlet
    ~ DiscoverClue(evidence_pamphlet)
    ~ RequestSpecialUI(su_pamphlet)
   -> DONE
   
= roy
    -> Roy_Dialogue
    
= misra
    ~ SetSpeaker(Speaker.Misra)
    Sup?
    -> DONE
    
= store_window
    -> Roy_Dialogue.window
    
= merch_shirt
    -> Roy_Dialogue.merch_shirt
    
= merch_sticker
    -> Roy_Dialogue.merch_sticker

= handwritten_note
    ~ RequestSpecialUI(su_note)
    -> DONE

= front_door
    ~ SetSpeaker(Speaker.Roy_Rodgerson)
    Take care!
    ~ closeDoor()
    ~ ChangeGameScene("scene4_1", 1)
    ~ SetSpeaker(Speaker.Lupe)
    ->DONE