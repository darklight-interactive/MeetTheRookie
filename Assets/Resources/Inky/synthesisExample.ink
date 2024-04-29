EXTERNAL playerAddItem(type, itemID, arg1)
EXTERNAL playerHasItem(itemID)
EXTERNAL playerRemoveItem(itemID)

~ playerAddItem("Scrap", "testItem", "This is a test item.")
~ playerAddItem("Scrap", "otherTestItem", "This is another test item.")

-> END

// Args are sorted alphabetically
=== function synthesize(a, b, c) ===
{
	- a == "newClue" && b == "otherTestItem" && c == "testItem":
		~ playerRemoveItem("testItem")
	- a == "otherTestItem" && b == "testItem":
		~ playerAddItem("Scrap", "newClue", "This is a new clue.")
}