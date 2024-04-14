VAR npc = "Unkown"
EXTERNAL playerAddItem(itemName)

-> Mel
=== Mel ===
{npc} I swallowed a bug.

[Mel:] You ever think about swallowing bugs?

~ playerAddItem("Look at this stupid buge")

* No[.], Bugs are my friends. -> DONE
* Yes[.], I love munching on bugs nom nom nom nom nom nom.
-> DONE

=== function combine(a, b) ===
	{
		- a == "Look at this stupid buge" && b == "Test":
			~ return "A big boy"
		- a == "Test" && b == "OtherTest":
			~ return "WAH"
		- a == "Look at this stupid buge" && b == "WAH":
			~ return "The final insult"
		- else:
			~ return false
	}