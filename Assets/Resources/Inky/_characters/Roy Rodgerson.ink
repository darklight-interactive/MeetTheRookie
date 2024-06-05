=== Roy_Dialogue ===
{ IsQuestComplete(visited_roy) == false:
    ~ CompleteQuest(visited_roy)
    ~ SetSpeaker(Speaker.Misra)
    Hey, Roy!
    ~ SetSpeaker(Speaker.Roy_Rodgerson)
    Misra, mijo! 
    It's good to see you.
    ~ SetSpeaker(Speaker.Misra)
    It's good to see you too, Uncle. 
    How's business?
    ~ SetSpeaker(Speaker.Roy_Rodgerson)
    Meh. Not very good. But I am used to it by now.
    Who's your friend?
    ~SetSpeaker(Speaker.Lupe)
    Detective Lupe.
    ~SetSpeaker(Speaker.Roy_Rodgerson)
    A Detective? What's this about, Misra?
    ~ SetSpeaker(Speaker.Misra)
    We're just asking a few questions about a disturbance last night.
    Would you mind helping us?
    ~ SetSpeaker(Speaker.Roy_Rodgerson)
    Of course, in anyway that I can.
    -> interogate
- else:
    What do you want to know?
    -> interogate
}

= interogate
    ~ SetSpeaker(Speaker.Lupe)
    * [Tell me about yourself] -> personal_info
    * [Tell me about the Town.] -> town_history
    * [Tell me about the Winery.] -> winery_closing
{IsClueFound(roy_personal_info) && IsClueFound(roy_winery_closing) && IsClueFound(roy_town_history):
        Thank you for your cooperation. 
        ~ SetSpeaker(Speaker.Roy_Rodgerson)
        Of course. 
        Feel free to look around, if it helps any.
        -> DONE
        }

= personal_info
~ SetSpeaker(Speaker.Roy_Rodgerson)
 Well, I was born and raised here in Kettle Rock.
 Lived here my whole life.
 IdaHome is my family's business.
~ SetSpeaker(Speaker.Lupe)
Are they local, as well?
~ SetSpeaker(Speaker.Roy_Rodgerson)
 No. 
 They've all passed. 
 My sister was the last, in 1940. 
 Unless, of course, you count Misra.
~ SetSpeaker(Speaker.Lupe)
You two are related?
~ SetSpeaker(Speaker.Misra)
Not biologically.
Roy's my godfather.
When my parent's moved away, he became my guardian.
~ SetSpeaker(Speaker.Roy_Rodgerson)
 I begged and begged for Misra to leave, too. 
 But they wouldn't go.
~ SetSpeaker(Speaker.Misra)
Kettle Rock is my home.
~ SetSpeaker(Speaker.Roy_Rodgerson)
 Of course. 
 But things are changing.
~ SetSpeaker(Speaker.Misra)
~ DiscoverClue(roy_personal_info)
// Add to Synthesis - The Town of KR
    ...
~ SetSpeaker(Speaker.Lupe)
-> interogate

= town_history
~ SetSpeaker(Speaker.Roy_Rodgerson)
 As I'm sure you've noticed, it's not in the best shape.
 Lots of places shutting down, going out of business.
 Lots of locals moving away.
 I think it's just unsustainable as it is.
~ SetSpeaker(Speaker.Misra)
Roy, come on...
~ SetSpeaker(Speaker.Roy_Rodgerson)
 I know you don't like to hear it, but it's the truth.
 You stepped up as Sheriff once the rest of the force retired to take care of this place
 but it seems to me like it might be time to move on.
~ SetSpeaker(Speaker.Misra)
I'm doing my best. 
Someone's got to try and turn things around.
~ SetSpeaker(Speaker.Roy_Rodgerson)
 Ah, mijo...
~ SetSpeaker(Speaker.Lupe)
    // Add to Synthesis - The Town of KR
~ DiscoverClue(roy_town_history)
-> interogate

= winery_closing
~ SetSpeaker(Speaker.Roy_Rodgerson)
 Ah, the straw that broke the camels back.
 It finally shut down, about a month ago.
 They closed it up tight.
 I have no idea what will become of it.
 There are only so many things you can do with an old Winery.
~ SetSpeaker(Speaker.Lupe)
~ DiscoverClue(roy_winery_closing)
-> interogate


= window
    ~ SetSpeaker(Speaker.Lupe)
    You have a perfect view of the Winery from here.
    ~ SetSpeaker(Speaker.Roy_Rodgerson)
     I do.
     It's not much to look at these days.
    ~ SetSpeaker(Speaker.Lupe)
    Did you...see anything out of place, last night?
    ~ SetSpeaker(Speaker.Roy_Rodgerson)
     What do you mean?
    ~ SetSpeaker(Speaker.Misra)
    We're investigating a break in that happened.
    At the Winery, last night. 
    ~ SetSpeaker(Speaker.Roy_Rodgerson)
     Ay, Misra, you should not be poking around up there.
    ~ SetSpeaker(Speaker.Misra)
    It's my job.
    ~ SetSpeaker(Speaker.Roy_Rodgerson)
     There's nothing there of interest.
    ~ SetSpeaker(Speaker.Misra)
    Did you see anything, Uncle?
    ~ SetSpeaker(Speaker.Roy_Rodgerson)
    ...
     No. 
    ~ SetSpeaker(Speaker.Lupe)
    Any ideas on who would want to break in and trash the place? 
        -> potential_suspects
        

= merch_shirt
        Really leaning into that small town appeal, I see.
        ~ SetSpeaker(Speaker.Roy_Rodgerson)
         Believe it or not, those used to fly off the shelves.
         I'd sell out of them one month, and then by the month after, I couldn't pay people to touch them.
        ~ SetSpeaker(Speaker.Lupe)
        -> DONE

= merch_sticker
        ~ SetSpeaker(Speaker.Lupe)
        ...
        People seriously buy these?
        ~ SetSpeaker(Speaker.Roy_Rodgerson)
         You'd be surprised!
        ~ SetSpeaker(Speaker.Misra)
        I have one!
        ~ SetSpeaker(Speaker.Lupe)
        I feel like you'd need witchcraft to get these things to sell.
        ~ SetSpeaker(Speaker.Roy_Rodgerson)
         Aha!
        ~ SetSpeaker(Speaker.Lupe)
        -> DONE

= pamphlet
* The Rocky Years?
    ~ SetSpeaker(Speaker.Roy_Rodgerson)
     Dios Mio; that was a hard time.
     Although I'd argue that we've fallen on harder times now.
    ~ SetSpeaker(Speaker.Misra)
    Come on Roy, don't say that!
    Have a little hope.
    We'll bounce back, I'm sure!
    We just need more time.
    ~ SetSpeaker(Speaker.Roy_Rodgerson)
     So optimistic, and I love you for that.
     But not very realistic.
     The Rocky Years was a time of inconsistency.
     We would flourish for the first part of the decade and struggle the latter.
     Disease of an unexplained nature swept through town.
     We couldn't link it to anything -
     not spoiled food or contaminated water,
     No known infection or flu.
    ~ SetSpeaker(Speaker.Lupe)
    That bad, huh?
    ~ SetSpeaker(Speaker.Roy_Rodgerson)
     It was. I do not miss that time.
    ~ SetSpeaker(Speaker.Lupe)
    ~ DiscoverClue(rocky_years)
      // Add to Synthesis - The Town of KR
    -> DONE

* The Tragedy?
    ...
     Terrible.
     Delusional people.
     Folk around here speak ill of them, but don't listen.
     Certain people even like to gossip, call them names, 
     insinuate that there was something else going on.
    Like what?
     Nothing real, and nothing worth talking about.
    ~ DiscoverClue(tragedy)
    // Add to Synthesis - The Town of KR
    -> pamphlet

* The Golden Age?
     A time of true prosperity.
     Business was, as you say, booming.
     The Winery sales were good, tourism was up.
     people recovered from whatever mysterious disease had passed over us.
     It was a good time, but it's over now.
    For now. Good times will come again!
     Sometimes that's not the case, mijo.
     Sometimes, the good times are just behind us.
     And that's okay.
    ...
    ~ DiscoverClue(golden_age)
        // Add to Synthesis - The Town of KR

    -> pamphlet
  
 //* {golden_age && tragedy && rocky_years} -> scene4_2  

= potential_suspects
{IsQuestComplete(complete_arcade):
    ~ SetSpeaker(Speaker.Roy_Rodgerson)
     If I had to place blame,
     Those delinquents at Power Up.
    ~ SetSpeaker(Speaker.Lupe)
    Ah, them.
    ~ SetSpeaker(Speaker.Misra)
    We talked with them earlier.
    They didn't let much slip.
    ~ SetSpeaker(Speaker.Lupe)
    // Add to Synthesis: Who broke into the Old Winery?
    -> DONE
    - else:
    
    ~ SetSpeaker(Speaker.Roy_Rodgerson)
     If I had to pick someone...
     I'd say those troublemakers down at the Arcade.
    ~ SetSpeaker(Speaker.Misra)
    Oh.
    <i>Those</i> guys.
    // Add to Synthesis: Who broke into the Old Winery?
    ~ DiscoverClue(roys_suspicion)
    -> DONE

}

= teenagers
~ SetSpeaker(Speaker.Lupe)
We spoke to the teens at Power Up.
~ SetSpeaker(Speaker.Roy_Rodgerson)
 Those rascals.
 Jenny still giving you a hard time?
~ SetSpeaker(Speaker.Misra)
Ugh.
Yes.
~ SetSpeaker(Speaker.Lupe)
They denied having anything to do with the Winery.
~ SetSpeaker(Speaker.Roy_Rodgerson)
 I'm sure they did.
~ SetSpeaker(Speaker.Lupe)
They did point us towards a "Jenkins Tomm".
~ SetSpeaker(Speaker.Roy_Rodgerson)
 ...
 I'm not sure he'll be any help to you.
 Really, Misra, I think you two should leave this alone.
~ SetSpeaker(Speaker.Misra)
We've got to get to the bottom of this!
~ SetSpeaker(Speaker.Lupe)
-> DONE

 


