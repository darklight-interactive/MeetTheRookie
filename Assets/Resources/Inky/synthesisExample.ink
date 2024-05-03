EXTERNAL playerAddItem(itemName)
EXTERNAL playerHasItem(itemName)
EXTERNAL playerRemoveItem(itemName)

~ playerAddItem("Test")
~ playerAddItem("Other Test")

-> END

// Args are sorted alphabetically
=== function synthesize(a, b, c) ===
{
	- a == "A new clue" && b == "Other Test" && c == "Test":
		~ playerRemoveItem("Test")
	- a == "Other Test" && b == "Test":
		~ playerAddItem("A new clue")
}