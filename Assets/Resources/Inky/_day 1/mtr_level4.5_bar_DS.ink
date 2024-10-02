// SCENE 4.5: Buy Me a Drink?
// NOTES: this whole scene happens in the Dating sim style! (Cannot access this location like other locations)
//        this also appears to be a sort of cutscene so there isnt much logic to do! :)

// VARIABLES HERE:
// VAR reported_incident = false
// VAR case_file_received = false
// VAR love_points = 0
// VAR tease_level = 0
// VAR sincerity_level = 0

* [Scene 4.5 - The Bar] -> scene4_5_DS


=== scene4_5_DS ===
# name: description
<i>You sit on the hard bar stool and lean against the wooden table, trying to avoid a splinter from the old wood. </i>

<i>Irene looks at you, awaiting for you to order a drink. </i>

# name: Lupe
# emote : Lupe \| Neutral_2

"Can I get..." 
+["A whiskey, neat"] -> MisraOrder
+["A whiskey on the rocks"] -> MisraOrder 
+["An Old Fashioned"] -> MisraConfused 

= MisraConfused
# name: Misra 
# emote : Misra \| Curious_1
"Huh." 

# name: Lupe
"What?"

# name: Misra
# emote : Misra \| Nervous_1
"Nothing! Just wasn't expecting you to have a sweet tooth." 

+["There's plenty you don't know about me."] -> DontKnow
+["I can be sweet."] -> Sweet 

= Sweet
~ tease_level++
#name: Misra
# emote : Misra \| Neutral_1
"Oh really, Mx. Mysterious Detective?" 

#name: Lupe 
# emote : Lupe \| Serious_2
"I'm not dark and gloomy." 

#name: Misra  
"Sure...sure." 

#name: Lupe 
"...I'm not." 

<i>Misra lightly giggles to themselves.</i>

{ tease_level > sincerity_level: <i> And let out a light puff of air from your nose </i> | -> MisraOrder }

-> MisraOrder

=DontKnow 

#name: Misra
# emote : Misra \| Inquisiting_1
"Hmmmmm" 

#name: Lupe
# emote : Lupe \| Neutral_2
"What?" 

#name: Misra 
"I don't know about that, you seem predictable." 

#name: Lupe 
"No I'm not."

#name: Misra 
# emote : Misra \| Neutral_1
"You wan'na bet on it?" 

+["What's the bet?"] -> bet
+["No."] -> ShockedNo

=bet 
~ tease_level++
#name: Misra
"Ok, if I can guess...your favorite color! If I can guess it, you have to buy me a drink!" 
+["And if you get it wrong?"] -> wrong 
+["Ok."] -> color 
+["Nevermind."] -> ShockedNo

=wrong 
#name: Misra 
"Then I'll buy you a drink, deal?" 
+["Ok."] -> color 
+["Nevermind."] -> ShockedNo

=color 
#name: Misra 
# emote : Misra \| Inquisiting_1
"Ok!! Your favorite color is..." 

<i>They hit the table with a drum roll.</i>

"Purple!!" 

#name: Lupe
...

#name: Misra 
# emote : Misra \| Nervous_1
"I'm right, right?" 

#name: Lupe
# emote : Lupe \| Neutral_2
"Yeah." 

#name: Misra 
# emote : Misra \| Neutral_1
"Ha ha!! Gotcha!!" 

<i> How did they know? </i> 

#name: Lupe
{ tease_level > sincerity_level:  "Lucky guess." | "Alright." }

#name: Misra
{ tease_level > sincerity_level:  "It wasn't luck, you're just easy to read." | "..." }

+["Let me guess yours."] -> GuessMisras 
+["Are you going to order?"] -> MisraOrderReminder

=GuessMisras
#name: Misra
# emote : Misra \| Surprised_2
# emote : Lupe \| Inquistive_1
{ tease_level > sincerity_level:  "Oh ok! C'mon, you're not going to get it right." | "Sure!" }

+["Green."] -> CorrectColor 
+ "Blue." 
-> IncorrectColor 
+"Orange." 
-> IncorrectColor

=CorrectColor
~ tease_level++
<i>You reach into the back of your mind, and for some reason this color resonates with you. </i>

#name: Lupe
# emote : Lupe \| Inquistive_1
"Green." 

#name: Misra
# emote : Misra \| Nervous_1
"Ha! No-" 

#name: Misra 
# emote : Misra \| Surprised_2
"Wait, green?" 

#name: Lupe 
# emote : Lupe \| Neutral_2
"Yes. Green." 

#name: Misra 
# emote : Misra \| Surprised_2
"Yeah...that's right." 

"How did you...?" 

#name: Lupe 
{ tease_level > sincerity_level:  "Guess you're easy to read." | "Lucky guess." } 
-> MisraOrder

=IncorrectColor

#name: Misra 
# emote : Misra \| Nervous_1
"Ha! Nope!" 

#name: Lupe 
{ tease_level > sincerity_level:  "What is it then?" | "Damn." }

#name: Misra 
{ tease_level > sincerity_level:  "It's green!" | -> MisraOrder }

#name: Lupe 
{ tease_level > sincerity_level:  "Good to know." | "Damn." }


-> MisraOrder

=MisraOrderReminder
#name: Misra 
# emote : Misra \| Neutral_1
"Oh, yeah!" 
-> MisraOrder

=ShockedNo 
~ sincerity_level ++
#name: Misra 
"Oh...ok!" 
-> MisraOrder



=MisraOrder
~ tease_level++
<i>You see Misra prepare to order.</i>
+[Offer to pay] -> offerToPay
+[Don't offer] -> MisraOrdersForThemselves

= offerToPay 
#name: Lupe 
# emote : Lupe \| Neutral_2
"May I put it on my tab?" 

#name: Misra 
# emote : Misra \| Surprised_2
{ tease_level > sincerity_level:  "What?! No way!" | "Um...no I can't let you do that!" }

#name: Lupe
" I insist..." 

{- color: 
"A bet is a bet. You guessed my favorite color, and now I need to pay my end of the bargin."
- else: 
"You've been too kind, it's the least I can do." 
} 

<i> Irene meets eyes with you. </i>
+[Wait for Misra to order] -> MisraOrdersForThemselves
+[Attempt to guess their drink] -> GuessMisrasOrder 

=MisraOrdersForThemselves
<i>You look over at Misra. </i>

#name: Misra
# emote : Misra \| Neutral_1
"I'll have an Old Fashioned please!" 

#name: Lupe 
{-  MisraConfused: 
"Heh. funny. Seems like we have similar tastes."
- else: 
<i> You watch Irene make the Old Fashioned and place it in front of Misra. </i>
#name: Misra
"Thank you very much!" 
} 
->questions

=GuessMisrasOrder
They'll have...
+[An Old Fashioned] -> guessDrinkRight 
+[A Cosmopolitan] -> guessDrinkWrong 
+[Lemon Drop] -> guessDrinkWrong

=guessDrinkRight
# emote : Lupe \| Neutral_2
<i>You reach into the back of your mind, and for some reason this  resonates with you.</i> 

~ tease_level++
#name: Misra
# emote : Misra \| Surprised_2
"Oh! Thank you" 

#name: Lupe 
"Was that right?" 

#name: Misra 
# emote : Misra \| Nervous_1
"Yeah, actually!"

#name: Lupe

{ MisraConfused: "Heh. funny. Seems like we have similar tastes."}

#name: Lupe: 
{-  CorrectColor: 
"And you say I'm the predictable one?"

#name: Misra 
# emote : Misra \| Neutral_1
"Hey, don't get cocky now!"

- else: 
#name: Lupe
"Oh, good." 
} 

#name: Misra
# emote : Misra \| Neutral_1
"Thank you very much!" 

->questions

= guessDrinkWrong
~ sincerity_level ++
#name: Misra 
# emote : Misra \| Nervous_1
"Oh! Um, actually can I get an Old Fashioned?" 

#name: Lupe 
"Sorry." 

{-  CorrectColor: 
#name: Misra
"Not as predictable as you may think!"

#name: Lupe
"Ha ha, ok."

- else: 
#name: Misra
"It's alright!" 
} 

-> questions


==questions==
*["Roy seems nice."] -> Roy
*["Who are the 'goats'?] -> Bachitan
*[Look at phone] -> Thelton
*["Those teens..."] -> Teens
*-> WhatNow

==Roy== 
#name: Misra
# emote : Misra \| Neutral_1
"Oh yeah! He's the best!" 

#name: Lupe 
# emote : Lupe \| Neutral_2
"You said he's your uncle?" 

#name: Misra

"Yep!"

"Well..."

"Technically no. He's my godfather." 

#name: Lupe 
"Ah, I see." 
-> RoyQuestions

=RoyQuestions
*["You two seem close."] -> close 
*["Sounds like Roy's buisness isn't doing too well."] -> buisness
*["Roy sure knows alot about this town."] -> town 
*-> questions

= close 
#name: Misra 
"Most definately! He took me under his wing when my parents left." 

#name: Lupe 
"Oh...I'm so sorry to hear that." 

#name: Misra 
# emote : Misra \| Nervous_1
"No worries! They're not dead or anything like that." 

"They just moved over to Montana, that's all!"

#name: Lupe 
+["How old were you?"] -> old 
+["Seems like you're doing well on your own."] -> yourOwn 

=yourOwn 
~ sincerity_level ++
#name: Misra 
"...yeah. I guess so." 

#name: Misra 
# emote : Misra \| Neutral_1
"Mostly thanks to Roy, though." 

"I don't know where I'd be without him!" 
-> RoyQuestions

=old 
#name: Misra 
# emote : Misra \| Nervous_1
"Oh! Oh they didn't abandoned me as a child or anything like that!"
"I was 27 at the time."
"Don't worry, my backstory isn't THAT tragic!" 

#name: Lupe 
+["Why didn't you leave with them?"] -> leaveWithThem 
+["Seems like you're doing well on your own."] -> yourOwn 

= leaveWithThem
#name: Misra 
# emote : Misra \| Curious_1
"..." 

#name: Misra 
# emote : Misra \| Nervous_1
"Because I still have hope for this town." 

"I know, I know...sounds crazy!" 

"People keep leaving, thinking that this is some sort of crazy downfall." 

#name: Lupe
# emote : Lupe \| Inquistive_1
"Is it not?" 

#name: Misra
# emote : Misra \| Surprised_2
"No way! There has to be some way to get back on our feet. Kettle Rock has done it multiple times in the past, and we can do it again!"

# emote : Misra \| Nervous_1
"Even if I have to be the only person working towards that, then so be it." 

# emote : Misra \| Fright_2
"I just..." 

"I just wish more people cared about our home, you know?" 

#name: Misra 
# emote : Misra \| Fright_2
"Augh, I'm being a debby downer."

"You know what, we'll recover! I just know it!!" 

->RoyQuestions

=buisness 
#name: Misra  
# emote : Misra \| Nervous_1
"Oh yeah...yeah." 

"Things have been rather rough around here, even for him. But...I don't know." 

"We're not really the most wealthy town, at least not anymore." 

#name: Lupe 
+["But he's offering to buy out buisnesses?"] -> buyOut 
+["Sorry to hear that."] -> itsAlright 

= itsAlright
#name: Misra 
"It's alright! We'll get back on our feet eventually!" 
-> RoyQuestions

= buyOut 

# emote : Misra \| Fright_2
"Yeah...he's been...planning for a while." 

#name: Lupe 
# emote : Lupe \| Inquistive_1 
"Planning?" 

#name: Misra 
"Yeah..." 

#name: Lupe 
"So...he believes in the 'goats'?" 

#name: Misra 
# emote : Misra \| Surprised_2
"Oh no! No, no, no!" 

#name: Lupe 
"Then...why?" 

#name: Misra 
# emote : Misra \| Nervous_1
"Maybe..."

"..."

# emote : Misra \| Fright_2
"...losing hope?" 

"Or maybe...maybe he's just trying to help us." 

<i> They break eye contact for a brief moment, attemping to blink away the tears at the edge of their eyes. </i> 

# emote : Misra \| Nervous_1
"Or!"
"Maybe he's the richest man in town and we don't know it!!" 
"That would be nice, wouldn't it?" 

#name: Lupe 
# emote : Lupe \| Neutral_2
"I guess so." 
->RoyQuestions

=town 
#name: Misra
# emote : Misra \| Neutral_1
"Oh yeah!! He's been here a long time." 

"Actually, he's been here his whole life!" 

"I'm pretty sure him and his sister never traveled outside of Kettle Rock" 

+["Did you know his sister?"] -> RoySister 
+["Interesting."] -> RoyQuestions

=RoySister
#name: Misra 
"Yeah! She was a really nice lady!" 

+["What happened to her?"] -> RoySisterExplination 
+["Interesting."] -> RoyQuestions

=RoySisterExplination
#name: Misra 
# emote : Misra \| Fright_2
"She um..." 
"Well, she went missing alongside the other folks of the tragedy." 

#name: Lupe 
"Sorry to hear that..." 

+["Maybe she's out there somewhere."] -> maybe 
+["Maybe she's doing well."] -> doingWell

= maybe 
#name: Misra 
# emote : Misra \| Nervous_1
"..." 
"...maybe." 
<i> There's a slight crack in their voice, and rapidly clear it before taking another sip of their drink. </i>
"Yeah...maybe."
->RoyQuestions

=doingWell
#name: Misra
# emote : Misra \| Nervous_1
"Yeah, I hope she is." 
-> RoyQuestions

==Bachitan==
#name: Misra 
# emote : Misra \| Nervous_1
"Oh, that's just an old legend." 

#name: Lupe 
# emote : Lupe \| Inquistive_1
"Are we talking monster legend? Or some other legend?" 

#name: Misra 
"Ha! No, there's no monsters in kettle Rock." 

# emote : Misra \| Surprised_2
"This town is so small you'd think someone would notice a monster by now!" 

#name: Lupe
# emote : Lupe \| Inquistive_1
"Then what is it?" 
+["Some sort of sacrifice cult?"] -> sacrifice 
+["Some sort of club?"] -> club 
+["Some sort of prank?"] -> prank 

= sacrifice
#name: Misra 
# emote : Misra \| Neutral_1
"Nope! The whole thing about a sacrifice is just a rumor to scare the kids." 

-> legendExplination 

=club 
#name: Misra 
# emote : Misra \| Nervous_1
"Kind of? I mean in a way I suppose cults are just more dangerous clubs." 
-> legendExplination

=prank 
#name: Misra 
# emote : Misra \| Nervous_1
"I wish it was! Would be a very sad prank though." 
->legendExplination

=legendExplination
#name: Misra 
# emote : Misra \| Neutral_1
"They say there was once some sort of 'council' in this town, though I don't know much about it myself." 

#name: Lupe 
# emote : Lupe \| Inquistive_1
"Why not? Shouldn't you know more about this supposed 'council' to help the people?" 

#name: Misra 
# emote : Misra \| Curious_1
"Well, people believe they're the cause of the disappearances. I'd rather leave those people be than poke into their buisness." 

"Let them rest..." 

"Wherever they are." 

-> questions


==Thelton== 
<i>You look down at your phone. </i>

#name: Misra 
# emote : Misra \| Curious_1
"Is everything alright?" 

#name: Lupe
# emote : Lupe \| Inquistive_1
"Yeah..." 

<i> There's no new calls. </i>

#name: Lupe 
"Just...strange." 

#name: Misra 
"What?" 

#name: Lupe 
"Usually Thelton calls back by now." 

#name: Misra 
"Who's Thelton?" 

#name: Lupe 
"Just the chief at my station." 

#name: Misra 
"Ah! Gotcha!" 

"And where would that be?" 

#name: Lupe 
"What?" 

#name: Misra 
# emote : Misra \| Curious_1
"Just wondering where you're from!"
+["A couple hours away."] -> dontGiveLocation
+[Give name of precinct] -> GiveLocation 

=dontGiveLocation
~ sincerity_level ++
#name: Misra: 
# emote : Misra \| Neutral_1
"Interesting!" 
-> questions

=GiveLocation
#name: Lupe 
# emote : Lupe \| Neutral_2
"Boise Central precinct" 
-> hometown2

=hometown2
#name: Misra 
# emote : Misra \| Neutral_1
# emote : Lupe \| Neutral_2
"So you're not too far from home!" 

"That's, what, four...five hours away?" 

#name: Lupe 
"More or less." 

->questions


==Teens== 
#name: Misra 
# emote : Misra \| Curious_1
"What about them?" 

#name: Lupe 
# emote : Lupe \| Inquistive_1
"They're hiding something..." 

#name: Misra 
"How do you know?" 

+["I'm a human lie detector."] -> LieDetector 
+["I can just tell."] -> justTell 

= LieDetector
~ tease_level++
{ tease_level > sincerity_level: 
#name: Misra 
# emote : Misra \| Neutral_1
"Oh boy! I'm not in any trouble, am I?" 

#name: Lupe 
# emote : Lupe \| Neutral_2 
"Well now I'm going to keep my ears peeled to ensure you're not lying to me about anything." 

#name: Misra 
"Oh ok good! You haven't caught me yet!" 

#name: Lupe 
# emote : Lupe \| Serious_2 
"..." 

#name: Misra 
# emote : Misra \| Nervous_1 
"I'm joking! I'm joking! Relax!" 
}

{-  intro.IncorrectColor: 
#name: Misra
# emote : Misra \| Inquisiting_1
"So your mind reading powers only apply to lies and not favorite colors? 

#name: Lupe 
# emote : Lupe \| Annoyed_1
"Ha ha, very funny."

- else:
#name: Misra
# emote : Misra \| Curious_1
"That skill must be very useful for a detective, though!" 

#name: Lupe 
# emote : Lupe \| Neutral_2
"Extremely." 
} 

->questions

= justTell 
#name: Misra 
# emote : Misra \| Inquisiting_1
 "Maybe we can find proof behind this accusation!" 
 
 #name: Lupe 
 # emote : Lupe \| Inquistive_1
 "Perhaps." 
 -> questions
 
 ==WhatNow== 
  #name: Misra 
 # emote : Misra \| Curious_1
 "Alright, you've asked enough questions. My turn!" 
 
 #name: Lupe
 # emote : Lupe \| Neutral_2
 "Hmph?" 
 
 #name: Misra 
 "Why are you so invested?" 
 
 #name: Lupe 
 "What do you mean?" 
 
 #name: Misra 
 "I mean, you followed me all around town for a case you unltimately really don't have anything to gain from." 
 
 -> LupeQuestioning 
 
 =LupeQuestioning
 {LupeQuestioning > 2: -> whatNowContinue} 
 "{LupeQuestioning > 1: In that case, }What gives?" 
 
 +["Boredom."] -> boredom 
 +["A bet."] -> theBet 
 +["Company.] -> company 
 +{LupeQuestioning > 1} ["We should focus on the case."] ->whatNowContinue
 = boredom 
 #name: Misra 
 "Really?" 
 
 #name: Lupe 
 "Yeah...I mean there's not much else to do." 
 
 #name: Misra 
 # emote : Misra \| Surprised_2
 "There's plenty to do here!" 
 
 #name: Lupe 
 "Oh? And what's that?" 
 
 #name: Misra 
 # emote : Misra \| Neutral_1
 "There's the arcade..." 
 
 #name: Lupe 
 "Mhm..." 
 
 #name: Misra 
 "And...the bar..." 
 
#name: Lupe 
"Mhm..." 

#name: Misra 
"And the uhh..." 

#name: Misra 
# emote : Misra \| Nervous_1
"Uhhhh....." 

+ ["You're really selling this, huh?"] -> sellingThis 
+ ["And nothing."] -> nothing
 
#name: Misra 
=sellingThis
~tease_level++
#name: Misra 
{ tease_level > sincerity_level: "Oh shush!" | "...Yeah." } 

{ tease_level > sincerity_level: <i> They lightly hit you on the shoulder, causing a light laughter to escape. </i> } 

{ tease_level > sincerity_level: <i> They too begin to laugh, and something resonates within you.</i>} 

{ tease_level > sincerity_level: <i> You can't explain it but this feels...right. </i>} 
-> LupeQuestioning

=nothing 
#name: Misra
"Yeah...yeah I suppose there really isn't much here." 
->LupeQuestioning

 = theBet 
#name: Misra 
# emote : Misra \| Surprised_2
"A bet?!" 

# emote : Misra \| Curious_1 
"With who?" 

#name: Lupe 
"My cheif." 

#name: Misra 
"What kind of bet?" 

#name: Lupe 
# emote : Lupe \| Annoyed_1
"It's...nothing." 

#name: Misra 
# emote : Misra \| Neutral_1
"Oh come on! You can't leave me hanging like this!" 

+ ["It was about solving cases..."] -> cocky
+ ["Nevermind."] -> Nevermind

= cocky 
#name: Lupe 
# emote : Lupe \| Neutral_2 
"I made a remark that I could solve any case tossed at me..." 

{ tease_level > sincerity_level: "Which is true!"} 

"And she wanted to place a bet that I could hold up that statement." 

#name: Misra 
# emote : Misra \| Neutral_1
"Really?" 

#name: Lupe 
"Yeah, and I guess this counts." 

{ tease_level > sincerity_level: 
#name: Misra 
"Alright Sherlock, if you're so good at your job then what's the craziest case you've solved?" 
-> craziestCase 
} 

-> lose 

=craziestCase
+[The case of the shoe] -> shoe
+[The case of the toothbrush] -> toothbrush
+[The case of the dog collar] -> dogCollar 

=shoe 
#name: Lupe 
"I once found a killer with the only evidence being a single shoe print" 

#name: Misra 
# emote : Misra \| Inquisiting_1
"Easy peasy!" 

#name: Lupe
# emote : Lupe \| Serious_2
"Really? I'd like to see you try." 

{ tease_level > sincerity_level: "Mx. half-empty police report"} 

#name: Misra 
"Touche...touche."

-> lose 

= toothbrush 
#name: Lupe
"I once found a missing person because she had shared a toothbrush with her kidnapper." 

#name: Misra 
# emote : Misra \| Surprised_2
"Ewwwwwww!!!" 

#name: Lupe 
# emote : Lupe \| Neutral_2
"Yeah...sometimes this line of work can get gross." 

#name: Misra
# emote : Misra \| Nervous_1
"Oh I've learned!"

-> lose

= dogCollar 
#name: Lupe 
# emote : Lupe \| Neutral_2
"I once found a missing person based off a dog collar in the woods." 

#name: Misra 
# emote : Misra \| Curious_1
"Oh! Was it because of their dog...?" 

#name: Lupe 
# emote : Lupe \| Annoyed_1
"No...it was...their collar." 

#name: Misra 
"What?" 

#name: Lupe 
"Yeah, they're alive but it was an...odd relationship between the kidnappee and abductor." 

#name: Misra 
"..." 

#name: Misra 
# emote : Misra \| Surprised_2 
"OH!!!" 

#name: Lupe 
"Yeah." 

#name: Misra 
# emote : Misra \| Nervous_1 
"I'm curious to know more, but I don't think I want to know." 

<i> You give a thousand-yard stare into your glass. </i> 

#name: Lupe 
# emote : Lupe \| Neutral_2
"Yeah, you really don't" 

"Curiosity is good, but sometimes can put us in bad situations." 

#name: Misra 
"Oh yeah, I know a thing or two about that!" 
-> lose 


=lose 
#name: Misra 
# emote : Misra \| Curious_1 
"So, what would happen if you lose?" 

#name: Lupe 
# emote : Lupe \| Neutral_2 
"First off, I'd become a laughing stock in my department." 
"Secondly, I'd have to buy coffee for her for a whole year." 

#name: Misra 
# emote : Misra \| Surprised_2
"A whole year!?" 

#name: Lupe 
# emote : Lupe \| Annoyed_1 
"A whole...year." 

#name: Misra
# emote : Misra \| Neutral_1 
"Then we better solve this!" 

#name: Lupe 
"Yeah." 

->whatNowContinue

= Nevermind 
#name: Misra 
# emote : Misra \| Neutral_1
"...Alright." 
-> LupeQuestioning

 =company 
~tease_level ++ 
<i> The words fall out of your mouth almost naturally.</i> 
<i> Even though you had just met them, it's as though a weight is lifted off your chest letting those words out. </i>
<i> It feels...good to say. </i> 

#name: Misra 
# emote : Misra \| Curious_1
"Company?" 

#name: Lupe 
# emote : Lupe \| Fluster_1 
"Yeah..." 

"If I'm going to be stuck here...I don't know." 

"Feels...right being stuck with you." 

#name: Misra
# emote : Misra \| Nervous_1
"Oh..." 
"Yeah...I suppose it does feel right." 

-> whatNowContinue


 =whatNowContinue
 #name: Misra 
 # emote : Misra \| Curious_1
 "Well, what now?" 
 
 #name: Lupe 
 # emote : Lupe \| Inquistive_1
 "Hm?" 
 
 #name: Misra 
# emote : Misra \| Curious_1
 "Are we ready to crack this case right open!?" 
 
 +["Not even close."] -> NotEvenClose 
 +["Are you kidding me?"] -> KiddingMe 
 +["We need more evidence."] -> NotEvenClose
 
 =NotEvenClose
 #name: Misra
 # emote : Misra \| Surprised_2
 "Really?!" 
 
 #name: Lupe
 # emote : Lupe \| Serious_2
 "We have scattered evidence, but nothing to latch onto." 
 
 ->Stakeout
 
 =KiddingMe 
 #name:Misra
  # emote : Misra \| Surprised_2
 "What?!" 
 
 #name: Lupe 
  # emote : Lupe \| Serious_2
 "We only have loose ends, but nothing concrete." 
 
 ->Stakeout
 
 =Stakeout
 #name: Misra
 # emote : Misra \| Curious_1
 "But we've already asked everyone around town! What else is there to do?" 
 
 #name: Lupe 
 # emote : Lupe \| Neutral_2
"I suggest a stakeout." 

#name: Misra 
# emote : Misra \| Surprised_2
"Like those old cop movies?!" 

"I've always wanted to do that!!" 

#name: Lupe 
"Yes. Similar." 

#name: Misra 
# emote : Misra \| Nervous_1
"Small issue...I don't have a car." 

+["What about the station car?"] -> stationCars 
+["We can use mine."] -> LupeCar 

=stationCars 
#name: Misra
# emote : Misra \| Nervous_1
"I may or may not have lost the keys..." 

#name: Lupe 
# emote : Lupe \| Serious_2
"..." 

#name: Misra
"..." 

#name: Lupe 
"Fine."

"We can use mine." 

-> LupeCar 

=LupeCar 
#name: Misra 
# emote : Misra \| Surprised_2
"Really?" 

#name: Lupe 
# emote : Lupe \| Neutral_2
"Sure." 

#name: Misra
# emote : Misra \| Neutral_1
"WOOO HOOO!!!" 

#name: Lupe
{ tease_level > sincerity_level:  "Alright, alright don't get too excited. This is a one-time thing." | "Calm down. It's just this once." }

#name: Misra 
{ tease_level > sincerity_level:  "Sure, whatever you say! I call the radio station though!" | "Sorry." }

{scene4_5_DS.offerToPay: <i>You pay the tab for both your drink and Misra's, then head out.</i>} 
{scene4_5_DS.MisraOrdersForThemselves: <i>You pay your tab, Misra pays for theirs, and you both leave the bar.</i>} 
~ ChangeGameScene("scene5_1_DS")
-> DONE