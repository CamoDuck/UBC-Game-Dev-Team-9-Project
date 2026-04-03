using Godot;

[Tool]
public partial class TalentTree : Control
{
	private PackedScene _tooltipScene;
	protected WeaponTooltip _activeTooltip;
	private bool _isChangingScene = false;

	public override void _Ready()
	{
		_tooltipScene = GD.Load<PackedScene>("res://weapon_tooltip.tscn");
		CallDeferred(nameof(InitTooltips));
	}

	private void InitTooltips()
	{
		if (!IsInsideTree()) return;
		var talents = GetTree().GetNodesInGroup("talents");
		foreach (Node node in talents)
		{
			if (node is TalentIcon icon && (this.IsAncestorOf(icon) || node == this))
			{
				var button = icon.GetNodeOrNull<Button>("Button");
				if (button != null)
				{
					button.MouseEntered += () => OnTalentHovered(icon);
					button.MouseExited += OnTalentUnhovered;
				}
				else
				{
					icon.MouseEntered += () => OnTalentHovered(icon);
					icon.MouseExited += OnTalentUnhovered;
				}
			}
		}
	}

	protected virtual void OnTalentHovered(TalentIcon icon)
	{
		// Don't show tooltip if we're mid-transition or icon is being freed
		if (_isChangingScene || !IsInsideTree()) return;
		if (icon == null || !IsInstanceValid(icon) || icon.TalentResource == null) return;
		
		if (_activeTooltip == null && _tooltipScene != null)
		{
			_activeTooltip = _tooltipScene.Instantiate<WeaponTooltip>();
			AddChild(_activeTooltip);
		}

		if (_activeTooltip == null) return;

		if (icon.TalentResource.IsUnlocked || !icon.TalentResource.HideUntilUnlocked)
		{
			_activeTooltip.Setup(icon.TalentResource.AbilityName, icon.TalentResource.TalentIcon, icon.TalentResource.WeaponDescription);
		}
		else
		{
			_activeTooltip.Setup("?", icon.LockedIcon, "???");
		}

		_activeTooltip.Show();
		_activeTooltip.GlobalPosition = GetGlobalMousePosition() + new Vector2(15, 15);
	}

	protected virtual void OnTalentUnhovered()
	{
		if (_activeTooltip != null && IsInstanceValid(_activeTooltip))
		{
			_activeTooltip.Hide();
		}
	}

	public override void _Process(double delta)
	{
		QueueRedraw();
		if (_activeTooltip != null && IsInstanceValid(_activeTooltip) && _activeTooltip.Visible)
		{
			_activeTooltip.GlobalPosition = GetGlobalMousePosition() + new Vector2(15, 15);
		}
	}

	public override void _Draw()
	{
		// Guard: skip drawing if the scene tree is unavailable (mid-transition)
		if (!IsInsideTree() || _isChangingScene) return;

		var talents = GetTree().GetNodesInGroup("talents");
		foreach (Node node in talents)
		{
			if (node is not TalentIcon talentNode || talentNode.TalentResource is not TalentResource talentResource)
				continue;

			foreach (TalentResource resource in talentResource.UnlockTalents)
			{
				var targetNode = GetNodeWithResource(resource);
				if (targetNode == null || !IsInstanceValid(targetNode)) continue;

				Vector2 sourcePosition = talentNode.GlobalPosition - GlobalPosition + talentNode.GetCenter();
				Vector2 targetPosition = targetNode.GlobalPosition - GlobalPosition + targetNode.GetCenter();
				Color color = talentResource.IsUnlocked ? Colors.Yellow : Colors.Gray;
				DrawLine(sourcePosition, targetPosition, color, 7.0f);
			}
		}
	}

	private TalentIcon GetNodeWithResource(TalentResource resource)
	{
		if (!IsInsideTree()) return null;
		var talents = GetTree().GetNodesInGroup("talents");
		foreach (Node node in talents)
		{
			if (node is TalentIcon talentNode && talentNode.TalentResource == resource)
				return talentNode;
		}
		return null;
	}

	public bool CanUnlock(TalentResource targetResource)
	{
		if (!IsInsideTree()) return false;
		bool hasParent = false;
		bool parentIsUnlocked = false;
		var talents = GetTree().GetNodesInGroup("talents");
		foreach (Node node in talents)
		{
			if (node is TalentIcon talentNode && talentNode.TalentResource != null)
			{
				if (talentNode.TalentResource.UnlockTalents.Contains(targetResource))
				{
					hasParent = true;
					if (talentNode.TalentResource.IsUnlocked) parentIsUnlocked = true;
				}
			}
		}
		return !hasParent || parentIsUnlocked;
	}

	// NAVIGATION METHODS — guarded against rapid double-clicks
	// Replace close button script with function that switches to normal gameplay scene
	public virtual void _on_close_button_pressed() => QueueFree();

	public virtual void _on_talent_button_pressed()
	{
		if (_isChangingScene) return;
		_isChangingScene = true;
		GetTree().ChangeSceneToFile("res://talent_tree.tscn");
	}

	public virtual void _on_weapon_button_pressed()
	{
		if (_isChangingScene) return;
		_isChangingScene = true;
		GetTree().ChangeSceneToFile("res://weapon_ui.tscn");
	}

	public virtual void _on_inventory_button_pressed()
	{
		if (_isChangingScene) return;
		_isChangingScene = true;
		GetTree().ChangeSceneToFile("res://inventory_ui.tscn");
	}
}
