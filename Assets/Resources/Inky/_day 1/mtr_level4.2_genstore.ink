=== scene4_2 ===
# Location: IdaHome and Goods, General Store
    * [talk_to_roy] -> Roy_Dialogue  
    * [store_window] -> store_window
    * [merch_shirt] -> merch_shirt
    * [merch_sticker] -> merch_sticker
    * [merch_pamphlet] -> pamphlet
    
        ->DONE
= intro
    ~PlaySFX("Doors/genstore_doorBell")
    -> Roy_Dialogue.roy_intro_cutscene
= pamphlet
    ~ DiscoverClue(merch_pamphlet)
    ~ SetSpeaker(Speaker.Lupe)
    TODO Wow, it's a pamphlet w/ loads of info!
   -> DONE
= roy
    -> Roy_Dialogue
= store_window
    -> Roy_Dialogue.window
    
= merch_shirt
    -> Roy_Dialogue.merch_shirt
    
= merch_sticker
    -> Roy_Dialogue.merch_sticker

= front_door
    ~ SetSpeaker(Speaker.Roy_Rodgerson)
    Take care!
    ~PlaySFX("Doors/doorClose")
    ~ mainStreetCheck()
    ->DONE