using Godot;
using System;

// A small data class to hold what's inside a single slot
public partial class InventorySlotData : RefCounted
{
	public ItemResource Item;
	public int Quantity;

	public InventorySlotData(ItemResource item, int quantity)
	{
		Item = item;
		Quantity = quantity;
	}
}

[GlobalClass]
public partial class InventoryManager : Node
{
	// UI will listen to this to know when to redraw
	[Signal]
	public delegate void InventoryUpdatedEventHandler();

	[Export]
	public int MaxSlots { get; private set; } = 24; // e.g. a 6x4 grid

	// The actual inventory array containing our items
	public Godot.Collections.Array<InventorySlotData> Slots = new Godot.Collections.Array<InventorySlotData>();

	public override void _Ready()
	{
		// Initialize the inventory with empty slots
		for (int i = 0; i < MaxSlots; i++)
		{
			Slots.Add(null);
		}
	}

	// Returns true if fully added, false if inventory was full
	public bool AddItem(ItemResource item, int amount = 1)
	{
		if (item == null) return false;

		// 1. Try to add to existing non-full stacks of the same item
		for (int i = 0; i < Slots.Count; i++)
		{
			if (Slots[i] != null && Slots[i].Item == item && Slots[i].Quantity < item.MaxStackSize)
			{
				int spaceLeft = item.MaxStackSize - Slots[i].Quantity;
				int amountToAdd = Mathf.Min(amount, spaceLeft);
				
				Slots[i].Quantity += amountToAdd;
				amount -= amountToAdd;

				if (amount <= 0)
				{
					EmitSignal(SignalName.InventoryUpdated);
					return true;
				}
			}
		}

		// 2. If we still have an amount left to add, find empty slots
		for (int i = 0; i < Slots.Count; i++)
		{
			if (Slots[i] == null)
			{
				int amountToAdd = Mathf.Min(amount, item.MaxStackSize);
				Slots[i] = new InventorySlotData(item, amountToAdd);
				amount -= amountToAdd;

				if (amount <= 0)
				{
					EmitSignal(SignalName.InventoryUpdated);
					return true; // Successfully added everything
				}
			}
		}

		// 3. If we reach here, inventory is full and we couldn't add everything
		EmitSignal(SignalName.InventoryUpdated);
		return false; 
	}

	public void SwapSlots(int indexA, int indexB)
	{
		if (indexA < 0 || indexA >= Slots.Count || indexB < 0 || indexB >= Slots.Count)
			return;

		InventorySlotData temp = Slots[indexA];
		Slots[indexA] = Slots[indexB];
		Slots[indexB] = temp;

		EmitSignal(SignalName.InventoryUpdated);
	}

	public void ClearInventory()
	{
		for (int i = 0; i < Slots.Count; i++)
		{
			Slots[i] = null;
		}
		EmitSignal(SignalName.InventoryUpdated);
	}

	// ============================================================
	// COOKING / CRAFTING UTILITY API
	// These methods are designed to be called by external systems
	// (e.g. a cooking station) via the InventoryManager autoload.
	// ============================================================

	/// Returns the total quantity of a specific item across all stacks.
	/// e.g. inventoryManager.GetTotalCount(tomatoResource) → 7
	public int GetTotalCount(ItemResource item)
	{
		if (item == null) return 0;
		int total = 0;
		foreach (var slot in Slots)
		{
			if (slot != null && slot.Item == item)
				total += slot.Quantity;
		}
		return total;
	}

	/// Returns true if the inventory contains at least the required amount of an item.
	/// e.g. if (inventoryManager.HasRequiredItems(eggResource, 2)) { startRecipe(); }
	public bool HasRequiredItems(ItemResource item, int amount)
	{
		return GetTotalCount(item) >= amount;
	}

	/// Removes the specified amount of an item from the inventory,
	/// automatically draining across multiple stacks if needed.
	/// Returns true if the full amount was consumed, false if there wasn't enough
	/// (in which case NO items are removed — it's an all-or-nothing operation).
	/// e.g. inventoryManager.ConsumeItems(flourResource, 3);
	public bool ConsumeItems(ItemResource item, int amount)
	{
		if (item == null || amount <= 0) return false;

		// Pre-check: ensure we have enough before touching any slots
		if (!HasRequiredItems(item, amount)) return false;

		int remaining = amount;
		for (int i = 0; i < Slots.Count && remaining > 0; i++)
		{
			if (Slots[i] == null || Slots[i].Item != item) continue;

			int take = Mathf.Min(remaining, Slots[i].Quantity);
			Slots[i].Quantity -= take;
			remaining -= take;

			// If the slot is now empty, clear it
			if (Slots[i].Quantity <= 0)
				Slots[i] = null;
		}

		EmitSignal(SignalName.InventoryUpdated);
		return true;
	}

	/// Returns all occupied slots whose item has a matching tag.
	/// NOTE: This requires ItemResource to have a Tags property (Array&lt;string&gt;).
	/// When your team decides on a tag schema, add the following to ItemResource.cs:
	///     [Export] public Godot.Collections.Array&lt;string&gt; Tags { get; set; } = new();
	/// Then uncomment the body of this method.
	/// e.g. var liquids = inventoryManager.GetItemsByTag("Liquid");
	public Godot.Collections.Array<InventorySlotData> GetItemsByTag(string tag)
	{
		var results = new Godot.Collections.Array<InventorySlotData>();

		// TODO: Uncomment when ItemResource.Tags is added:
		// foreach (var slot in Slots)
		// {
		//     if (slot != null && slot.Item != null && slot.Item.Tags.Contains(tag))
		//         results.Add(slot);
		// }

		GD.PrintErr($"GetItemsByTag: ItemResource does not have a Tags property yet. Add '[Export] public Godot.Collections.Array<string> Tags {{ get; set; }}' to ItemResource.cs to enable this.");
		return results;
	}
}
