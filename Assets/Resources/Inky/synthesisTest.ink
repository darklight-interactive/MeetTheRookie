EXTERNAL playerAddItem(type, id, value)
EXTERNAL playerRemoveItem(id)

~ playerAddItem("MediumPaper1", "cutfence", "Cut Fence")
~ playerAddItem("MediumPaper2", "brokenwindow", "Broken Windows")
~ playerAddItem("MediumPaper2", "strangefootsteps", "Strange Footsteps")
~ playerAddItem("MediumPaper3", "clawmarks", "Claw Marks on Wine Barrels")
~ playerAddItem("MediumPaper1", "damagedequipment", "Damaged Equipment")



->END

=== nada ===
Unreachable
-> END

=== function synthesize(a, b, c) ===
{
	- a == "brokenwindow" && b == "cutfence" && c == "strangefootsteps":
		~ playerRemoveItem("cutfence")
		~ playerRemoveItem("brokenwindow")
		~ playerRemoveItem("strangefootsteps")
		~ playerAddItem("MediumPaper2", "who", "Who Broke into the Winery?")
}
	~ return