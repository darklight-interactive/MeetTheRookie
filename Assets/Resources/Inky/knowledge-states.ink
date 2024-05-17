// Using ink to manage knowledge states
// ---------------
// https://heavens-vault-game.tumblr.com/post/160306503785/what-dyknow

// We’ve said that Heaven’s Vault is really a detective game, with the player-as-archaeologist uncovering what happened where, when and to who using evidence. Which means from a system point of view, we’re tracking what the player knows, what they think they know, what they know they know…
// We’re doing all of our knowledge tracking in ink using a combination of “simple facts” and “chained facts”. 
// A simple fact is one you either know, or you don’t - “There’s water in the well here”, “There’s a door in the hillside”. Once learned, you can’t forget them. 
// A chained fact is one that can be developed in more detail. Perhaps what starts off as “There’s a rock on the hilltop” becomes “There’s a door in the hilltop” becomes “There’s some kind of tomb under the hilltop” becomes “There’s an ancient ship burial under the hilltop.”
// And quite often we have a fact which starts life simple, and becomes a chain as the writing process continues. So we’ve built a system which allows the author to “upgrade” a fact without fuss.
// The “knowledgeState” list is a bucket into which we drop any facts we “learn”. Then when we test if we “know” a fact, we decide which type of test is appropriate based on what list the fact came from. 
// Upgrading a simple fact is a simple as taking it out of the simple facts list, and putting it into its own chained list. Simple!
// (Note that keeping the game-side informed of these changes is a little fiddlier, and uses a combination of external functions and variable observers. Another post, perhaps.)

// ---- root scene

// ---- example data ---

LIST TempleChain = SOMETHING_UP_THERE, STONES_UP_THERE, TOMB_ON_HILL, SHIP_BURIAL_UNDER_HILL

LIST SimpleFacts = WATER_IN_WELL, SHIPS_ARE_ANCIENT, JAM_IS_TASTY

// ---- system ---

VAR knowledgeState = ()

=== function learn(fact)
    ~ knowledgeState += fact
    
=== function know(fact)
    { LIST_ALL(SimpleFacts) ? fact:
        // simple facts are either known, or not
        ~ return knowledgeState ? fact
    - else:
        // chained facts are more complicated: do we know a fact in the chain of equal or higher value than this one?
        ~ temp factsInThisChain = knowledgeState ^ LIST_ALL(fact)
        ~ return (LIST_MAX(factsInThisChain) >= fact)
    }

// ---- example ---
