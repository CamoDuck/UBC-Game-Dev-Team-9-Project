using Godot;

[Tool]
public partial class TalentIcon : PanelContainer
{

	[Export]
	public TalentResource TalentResource
	{
		get => _talentResource;
		set
		{
			_talentResource = value;

			if (_talentResource == null)
			{
				AddThemeStyleboxOverride("panel", new StyleBoxEmpty());
				UpdateTexture(null);
			}
			else
			{
				AddThemeStyleboxOverride("panel", TalentIconStylebox);
				// Respect HideUntilUnlocked from the very first assignment
				if (_talentResource.HideUntilUnlocked && !_talentResource.IsUnlocked)
					UpdateTexture(LockedIcon);
				else
					UpdateTexture(_talentResource.TalentIcon);
			}
		}
	}

	private TalentResource _talentResource;

	[Export] public Color LockColorBorder { get; set; } = new Color(0.2f, 0.25f, 0.3f, 1f);
	[Export] public Color UnlockedColorBorder { get; set; } = new Color(0.9f, 0.8f, 0.2f, 1f);
	[Export] public Texture2D LockedIcon { get; set; }

	private TextureRect _textureRect;
	private TalentTree _parentTree;
	private bool _isProcessingClick = false;

	private static readonly StyleBox TalentIconStylebox =
		GD.Load<StyleBox>("res://skill-tree/talent_icon_style.tres");

	// === READY ===

	public override void _Ready()
	{
		_textureRect = GetNodeOrNull<TextureRect>("TextureRect");
		
		// Find the parent TalentTree by walking up the node hierarchy
		Node current = GetParent();
		while (current != null)
		{
			if (current is TalentTree tree)
			{
				_parentTree = tree;
				break;
			}
			current = current.GetParent();
		}

		if (_talentResource == null)
			return;

		AddToGroup("talents");
		AddThemeStyleboxOverride("panel", TalentIconStylebox);
		
		SetStyle();
	}

	// === METHODS ===

	private void UpdateTexture(Texture2D texture)
	{
		if (_textureRect == null)
		{
			_textureRect = GetNodeOrNull<TextureRect>("TextureRect");
		}

		if (_textureRect != null)
		{
			_textureRect.Texture = texture;
		}
	}

	public Vector2 GetCenter()
	{
		return CustomMinimumSize / 2;
	}

	private void SetStyle()
	{
		if (_talentResource == null) return;

		var baseStyle = GetThemeStylebox("panel") as StyleBoxFlat;
		if (baseStyle == null)
			return;

		var styleBox = (StyleBoxFlat)baseStyle.Duplicate();

		if (_talentResource.IsUnlocked)
		{
			styleBox.BorderColor = UnlockedColorBorder;
			UpdateTexture(_talentResource.TalentIcon);
		}
		else
		{
			styleBox.BorderColor = LockColorBorder;
			if (_talentResource.HideUntilUnlocked)
				UpdateTexture(LockedIcon);
			else
				UpdateTexture(_talentResource.TalentIcon);
		}

		AddThemeStyleboxOverride("panel", styleBox);
	}

	private void UnlockTalent()
	{
		if (_talentResource == null) return;
		
		if (_talentResource.IsUnlocked) return;

		// Guard: don't process if node is being freed (fast clicking during scene transitions)
		if (IsQueuedForDeletion()) return;

		// Check if the parent tree allows us to unlock this skill
		if (_parentTree != null && !_parentTree.CanUnlock(_talentResource))
		{
			GD.Print("Cannot unlock this skill yet! Parent skill is not unlocked.");
			return; 
		}

		_talentResource.IsUnlocked = true;
		SetStyle();
		_parentTree?.QueueRedraw();
		
		// Trigger notification
		if (IsInsideTree())
		{
			var notifSystem = GetNodeOrNull<NotificationSystem>("/root/NotificationSystem");
			notifSystem?.Notify(_talentResource.AbilityName, _talentResource.TalentIcon);
		}
	}

	private void _on_button_pressed()
	{
		// Guard against rapid double-fire clicks
		if (_isProcessingClick) return;
		_isProcessingClick = true;
		
		UnlockTalent();
		
		// Reset flag next frame
		if (IsInsideTree())
			CallDeferred(nameof(ResetClickFlag));
	}
	
	private void ResetClickFlag()
	{
		_isProcessingClick = false;
	}

	public void OnTalentUnlocked()
	{
		if (_talentResource == null) return;
		_talentResource.IsUnlocked = true;
		SetStyle();
	}
}
