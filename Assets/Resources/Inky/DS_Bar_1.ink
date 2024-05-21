// SCENE 4.5: Buy Me a Drink?
// NOTES: this whole scene happens in the Dating sim style! (Cannot access this location like other locations)
//        this also appears to be a sort of cutscene so there isnt much logic to do! :)

// VARIABLES HERE:
VAR reported_incident = false
VAR case_file_received = false
VAR love_points = 0
VAR tease_level = 0
VAR sincerity_level = 0



* [BEGIN] -> intro

=== intro ===
# name: description
You sit on the hard bar stool and lean against the wooden table, trying to avoid a splinter from the old wood. 

Irene looks at you, awaiting for you to order a drink. 

# name: Lupe

"Can I get..." 
+[A whiskey, neat.] -> MisraOrder
+[A whiskey on the rocks.] -> MisraOrder 
+[An Old Fashioned.] -> MisraConfused 

= MisraConfused
# name: Misra 
"Huh." 

# name: Lupe 
"What?"

# name: Misra 
"Nothing! Just wasn't expecting you to have a sweet tooth." 

+["There's plenty you don't know about me."] -> DontKnow
+["I can be sweet."] -> Sweet 

= Sweet
~ tease_level++
#name: Misra  
"Oh really, Mx. mysterious Detective?" 

#name: Lupe 
"I'm not dark and gloomy." 

#name: Misra  
"Sure...sure." 

#name: Lupe 
"...I'm not." 

Misra lightly giggles to themselves.

{ tease_level > sincerity_level:  And let out a light puff of air from your nose | -> MisraOrder }

-> MisraOrder

=DontKnow 

#name: Misra 
"Hmmmmm" 

#name: Lupe 
"What?" 

#name: Misra 
"I don't know about that, you seem predictable." 

#name: Lupe 
"No I'm not."

#name: Misra 
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
"Ok!! Your favorite color is..." 

They hit the table with a drum roll.

"Purple!!" 

#name: Lupe
...

#name: Misra 
"I'm right, right?" 

#name: Lupe
"Yes." 

#name: Misra 
"Ha ha!! Gotcha!!" 

#name: Lupe
{ tease_level > sincerity_level:  "Lucky guess." | "Alright." }

#name: Misra
{ tease_level > sincerity_level:  "It wasn't luck, you're just easy to read." | "..." }

+["Let me guess yours."] -> GuessMisras 
+["Are you going to order?"] -> MisraOrderReminder

=GuessMisras
{ tease_level > sincerity_level:  "Oh ok! C'mon, you're not going to get it right." | "Sure!" }

+["Green."] -> CorrectColor 
+ "Blue." 
-> IncorrectColor 
+"Orange." 
-> IncorrectColor

=CorrectColor
~ tease_level++
You reach into the back of your mind, and for some reason this color resonates with you. 

#name: Lupe 
"Green." 

#name: Misra 
"Ha! No-" 

#name: Misra 
"Wait, green?" 

#name: Lupe 
"Yes. Green." 

#name: Misra 
"Yeah...that's right." 

"How did you...?" 

#name: Lupe 
{ tease_level > sincerity_level:  "Guess you're easy to read." | "Lucky guess." } 
-> MisraOrder

=IncorrectColor

#name: Misra 
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
"Oh, yeah!" 
-> MisraOrder

=ShockedNo 
~ sincerity_level ++
#name: Misra 
"Oh...ok!" 
-> MisraOrder



=MisraOrder
~ tease_level++
You see Misra prepare to order.
+[Offer to pay] -> offerToPay
+[Don't offer] -> MisraOrdersForThemselves

= offerToPay 
#name: Lupe 
"May I put it on my tab?" 

#name: Misra 
{ tease_level > sincerity_level:  "What?! No way!" | "Um...no I can't let you do that!" }

#name: Lupe 
" I insist..." 

{- color: 
"A bet is a bet. You guessed my favorite color, and now I need to pay my end of the bargin."
- else: 
"You've been too kind, it's the least I can do." 
} 

Irene meets eyes with you. 
+[Wait for Misra to order.] -> MisraOrdersForThemselves
+[Attempt to guess their drink.] -> GuessMisrasOrder 

=MisraOrdersForThemselves
You look over at Misra. 

#name: Misra
"I'll have an Old Fashioned please!" 

#name: Lupe 
{-  MisraConfused: 
"Heh. funny. Seems like we have similar tastes."
- else: 
You watch Irene make the Old Fashioned and place it in front of Misra. 
#name: Misra
"Thank you very much!" 
} 
->questions

=GuessMisrasOrder
"They'll have...
+[...An Old Fashioned."] -> guessDrinkRight 
+[...A Cosmopolitan."] -> guessDrinkWrong 
+[..A Lemon Drop."] -> guessDrinkWrong

=guessDrinkRight
You reach into the back of your mind, and for some reason this  resonates with you. 

~ tease_level++
#name: Misra
"Oh! Thank you" 

#name: Lupe 
"Was that right?" 

#name: Misra 
"Yeah, actually!"

#name: Lupe

{ MisraConfused: "Heh. funny. Seems like we have similar tastes."}

#name: Lupe: 
{-  CorrectColor: 
"And you say I'm the predictable one?"

#name: Misra 
"Hey, don't get cocky now!"

- else: 
"Oh, good." 
} 

#name: Misra
"Thank you very much!" 

->questions

= guessDrinkWrong
~ sincerity_level ++
#name: Misra 
"Oh! Um, actually can I get an Old Fashioned?" 

#name: Lupe 
"Sorry." 

{-  CorrectColor: 
"Not as predictable as you may think!"

#name: Misra 
"Ha ha, ok."

- else: 
"It's alright!" 
} 

-> questions


==questions==
*["Roy seems nice."] -> Roy
*["Who are the 'goats'?] -> Bachitan
*[Look at phone.] -> Thelton
*["Those teens..."] -> Teens
*-> WhatNow

==Roy== 
#name: Misra
"Oh yeah! He's the best!" 

#name: Lupe 
"You said he's your uncle?" 

#name: Misra
"Yep! Well, technically no. He's my godfather." 

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
"No worries! They're not dead or anything like that." 

"They just moved over to Montana, that's all!"

#name: Lupe 
+["How old were you?"] -> old 
+["Seems like you're doing well on your own."] -> yourOwn 

=yourOwn 
~ sincerity_level ++
#name: Misra 
"...yeah. I guess so." 

"Mostly thanks to Roy, though." 

"I don't know where I'd be without him!" 
-> RoyQuestions

=old 
#name: Misra 
"19." 

#name: Lupe 
"Wow." 
+["Why didn't you leave with them?"] -> leaveWithThem 
+["Seems like you're doing well on your own."] -> yourOwn 

= leaveWithThem
#name: Misra 
"..." 

"Because I still have hope for this town." 

"I know, I know...sounds crazy!" 

"People keep leaving, thinking that this is some sort of crazy downfall." 

#name: Lupe  
"Is it not?" 

#name: Misra 
"No way! There has to be some way to get back on our feet. Kettle Rock has done it multiple times in the past, and we can do it again!"

"Even if I have to be the only person working towards that, then so be it." 

"I just..." 

"I just wish more people cared about our home, you know?" 

#name: Misra 
"Augh, I'm being a debby downer."

"You know what, we'll recover! I just know it!!" 

->RoyQuestions

=buisness 
#name: Misra  
"Oh yeah...but that's pretty normal around these parts." 

"We're not really the most wealthy town, at least not anymore." 

#name: Lupe 
+["But he's offering to buy out buisnesses?"] -> buyOut 
+["Sorry to hear that."] -> itsAlright 

= itsAlright
#name: Misra 
"It's alright! We'll get back on our feet eventually!" 
-> RoyQuestions

= buyOut 
#name: Misra 
"Yeah...he's been saving up money for quite some time." 
"Maybe he's the richest man in town and we don't know it!!" 
"That would be nice, wouldn't it?" 

#name: Lupe 
"I guess so." 
->RoyQuestions

=town 
#name: Misra 
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
"She um..." 
"Well, she went missing alongside the other folks of the tragedy." 

#name: Lupe 
"Sorry to hear that..." 

+["Maybe she's out there somewhere."] -> maybe 
+["Maybe she's doing well."] -> doingWell

= maybe 
#name: Misra 
"..." 
"...maybe." 
->RoyQuestions

=doingWell
#name: Misra 
"Yeah, I hope she is." 
-> RoyQuestions

==Bachitan==
#name: Misra 
"Oh, that's just an old legend." 

#name: Lupe 
"Are we talking monster legend? Or some other legend?" 

#name: Misra 
"Ha! No, there's no monsters in kettle Rock." 

"This town is so small you'd think someone would notice a monster by now!" 

#name: Lupe 
"Then what is it?" 
+["Some sort of sacrifice cult?"] -> sacrifice 
+["Some sort of club?"] -> club 
+["Some sort of prank?"] -> prank 

= sacrifice
#name: Misra 
"Nope! The whole thing about a sacrifice is just a rumor to scare the kids." 

-> legendExplination 

=club 
#name: Misra 
"Kind of?? I mean in a way I suppose cults are just more dangerous clubs." 
-> legendExplination

=prank 
#name: Misra 
"I wish it was! Would be a very sad prank though." 
->legendExplination

=legendExplination
#name: Misra 
"They say there was once a cult that called themselves 'The Council of Bachitan', though I don't know much about it myself." 

#name: Lupe 
"Why not?" 

#name: Misra 
"People believe they're the cause of the disappearances. I'd rather leave those people be than poke into their buisness." 

"Let them rest..." 

"Wherever they are." 

-> questions


==Thelton== 
You look down at your phone. 

#name: Misra 
"Is everything alright?" 

#name: Lupe 
"Yeah..." 

There's no new calls. 

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
{ tease_level > sincerity_level:  "Boise Central Precinct" ->hometown2 | "Sorry?" -> hometown1 }

= hometown1
#name: Misra 
"Just wondering where you're from!"
+["A couple hours away."] -> dontGiveLocation
+[Give name of precinct.] -> GiveLocation 

=dontGiveLocation
~ sincerity_level ++
#name: Misra: 
"Interesting!" 
-> questions

=GiveLocation
#name: Lupe 
"Boise Central precinct." 
-> hometown2

=hometown2
#name: Misra 
"So you're not too far from home!" 

"That's, what, four...five hours away?" 

#name: Lupe 
"More or less." 

->questions


==Teens== 
#name: Misra 
"What about them?" 

#name: Lupe 
"They're hiding something..." 

#name: Misra 
"How do you know?" 

+["I'm a human lie detector."] -> LieDetector 
+["I can just tell."] -> justTell 

= LieDetector
~ tease_level++
{-  intro.IncorrectColor: 
#name: Misra
"Oh ok, so your mind reading powers only apply to lies and not favorite colors? 

#name: Lupe 
"Ha ha, very funny."

- else:
#name: Misra
"That must be very useful for a detective!" 

#name: Lupe 
"Extremely." 
} 

->questions

= justTell 
#name: Misra 
 "Maybe we can find proof behind this accusation!" 
 
 #name: Lupe 
 "Perhaps." 
 -> questions
 
 ==WhatNow== 
 #name: Misra 
 "Well, what now?" 
 
 #name: Lupe 
 "Hm?" 
 
 #name: Misra 
 "Are we ready to crack this case right open!?" 
 
 +["Not even close."] -> NotEvenClose 
 +["Are you kidding me?"] -> KiddingMe 
 +["We need more evidence."] -> NotEvenClose
 
 =NotEvenClose
 #name: Misra 
 "Really?!" 
 
 #name: Lupe 
 "We have scattered evidence, but nothing to latch onto." 
 
 ->Stakeout
 
 =KiddingMe 
 #name:Misra
 "What?!" 
 
 #name: Lupe 
 "We only have loose ends, but nothing concrete." 
 
 ->Stakeout
 
 =Stakeout
 #name: Misra 
 "But we've already asked everyone around town! What else is there to do?" 
 
 #name: Lupe 
"I suggest a stakeout." 

#name: Misra 
"Like those old cop movies?!" 

"I've always wanted to do that!!" 

#name: Lupe 
"Yes. Similar." 

#name: Misra 
"Small issue...I don't have a car." 

+["What about the station car?] -> stationCars 
+["We can use mine."] -> LupeCar 

=stationCars 
#name: Misra 
"I may or may not have lost the keys..." 

#name: Lupe 
"..." 

#name: Misra
"..." 

#name: Lupe. 
"Fine."

"We can use mine." 

-> LupeCar 

=LupeCar 
#name: Misra 
"Really?" 

#name: Lupe 
"Sure." 

#name: Misra 
"WOOO HOOO!!!" 

#name: Lupe
{ tease_level > sincerity_level:  "Alright, alright don't get too excited. This is a one-time thing." | "Calm down. It's just this once." }

#name: Misra 
{ tease_level > sincerity_level:  "Sure, whatever you say! I call the radio station though!" | "Sorry." }

{intro.offerToPay: You pay the tab for both your drink and Misra's, then head out.} 
{intro.MisraOrdersForThemselves: You pay your tab, Misra pays for theirs, and you both leave the bar.} 

-> DONE