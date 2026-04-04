using Godot;
using System;

public partial class InventorySlot : PanelContainer
{
	[Export]
	public TextureRect ItemIcon { get; set; }

	[Export]
	public Label StackCountLabel { get; set; }
	
	private int _slotIndex;
	private InventoryManager _inventoryManager;

	public void Init(int index)
	{
		_slotIndex = index;
		_inventoryManager = GetNodeOrNull<InventoryManager>("/root/InventoryManager");
	}

	// *Call this from our InventoryManager whenever an item is added or removed
	public void UpdateSlot(ItemResource item, int amount)
	{
		if (item == null || amount <= 0)
		{
			// Empty slot visually
			ItemIcon.Texture = null;
			StackCountLabel.Text = "";
		}
		else
		{
			ItemIcon.Texture = item.Icon;
			StackCountLabel.Text = amount > 1 ? amount.ToString() : "";
		}
	}

	public override Variant _GetDragData(Vector2 atPosition)
	{
		if (ItemIcon.Texture == null) return default;

		// Create a small preview of the icon when dragging
		var preview = new TextureRect();
		preview.Texture = ItemIcon.Texture;
		preview.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
		preview.Size = new Vector2(60, 60);
		preview.GlobalPosition = -preview.Size / 2; // Center it on mouse
		
		var previewContainer = new Control();
		previewContainer.AddChild(preview);
		SetDragPreview(previewContainer);

		// Return the slot index as the data to be dropped
		return _slotIndex;
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		// Data should be an integer index
		return data.VariantType == Variant.Type.Int;
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		int fromIndex = (int)data;

		if (_inventoryManager != null)
		{
			_inventoryManager.SwapSlots(fromIndex, _slotIndex);
		}
	}
}
