using Godot;
using System;

public partial class InventoryUI : Control
{
	[Export]
	public PackedScene InventorySlotScene { get; set; }

	[Export]
	public GridContainer InventoryGrid { get; set; }

	[ExportGroup("Debug Options")]
	[Export]
	public Godot.Collections.Array<ItemResource> DebugItems { get; set; }

	private InventoryManager _inventoryManager;

	public override void _Ready()
	{
		SkillTreeQuestUi.AttachIfNeeded(this);
		_inventoryManager = GetNodeOrNull<InventoryManager>("/root/InventoryManager");
		if (_inventoryManager != null)
		{
			_inventoryManager.InventoryUpdated += RefreshUI;
			RefreshUI();
		}
	}

	public override void _ExitTree()
	{
		if (_inventoryManager != null)
			_inventoryManager.InventoryUpdated -= RefreshUI;
	}

	public void RefreshUI()
	{
		if (_inventoryManager == null || InventorySlotScene == null || InventoryGrid == null) 
			return;

		foreach (Node child in InventoryGrid.GetChildren())
			child.QueueFree();

		for (int i = 0; i < _inventoryManager.MaxSlots; i++)
		{
			InventorySlot newSlot = InventorySlotScene.Instantiate<InventorySlot>();
			InventoryGrid.AddChild(newSlot);
			newSlot.Init(i);
			
			if (i < _inventoryManager.Slots.Count)
			{
				InventorySlotData slotData = _inventoryManager.Slots[i];
				if (slotData != null) newSlot.UpdateSlot(slotData.Item, slotData.Quantity);
				else newSlot.UpdateSlot(null, 0);
			}
		}
	}

	public void _on_add_random_item_pressed()
	{
		if (_inventoryManager == null || DebugItems == null || DebugItems.Count == 0) return;
		Random random = new Random();
		ItemResource randomItem = DebugItems[random.Next(DebugItems.Count)];
		_inventoryManager.AddItem(randomItem, random.Next(1, 6));
	}

	public void _on_clear_inventory_pressed()
	{
		if (_inventoryManager != null) _inventoryManager.ClearInventory();
	}

	// Standardized navigation
	public void _on_talent_button_pressed() => GetTree().ChangeSceneToFile("res://skill-tree/talent_tree.tscn");
	public void _on_weapon_button_pressed() => GetTree().ChangeSceneToFile("res://skill-tree/weapon_ui.tscn");
	public void _on_inventory_button_pressed() => GetTree().ChangeSceneToFile("res://skill-tree/inventory_ui.tscn");
	public void _on_close_button_pressed() => QueueFree();
}
