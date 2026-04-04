using Godot;

/// <summary>
/// Populates a VBox with one row per tracked quest (title, progress, complete state).
/// Attach to the root of the quest panel; expects child path QuestScroll/QuestList.
/// </summary>
public partial class QuestPanelUI : Control
{
	[Export]
	public NodePath QuestListPath { get; set; } =
		new("CenterContainer/PanelContainer/MarginContainer/VBox/QuestScroll/QuestList");

	private QuestManager _quests;
	private VBoxContainer _list;

	public override void _Ready()
	{
		_list = GetNodeOrNull<VBoxContainer>(QuestListPath);
		_quests = GetNodeOrNull<QuestManager>("/root/QuestManager");
		if (_quests != null)
			_quests.QuestStateChanged += Refresh;

		VisibilityChanged += OnVisibilityChanged;
		if (Visible)
			Refresh();
	}

	public override void _ExitTree()
	{
		if (_quests != null)
			_quests.QuestStateChanged -= Refresh;

		VisibilityChanged -= OnVisibilityChanged;
	}

	private void OnVisibilityChanged()
	{
		if (Visible)
			Refresh();
	}

	/// <summary>Bound from scene Close button.</summary>
	public void HidePanel()
	{
		Visible = false;
	}

	public void Refresh()
	{
		if (_list == null)
			return;

		foreach (Node child in _list.GetChildren())
			child.QueueFree();

		if (_quests == null)
		{
			_list.AddChild(NewLabel("QuestManager not found."));
			return;
		}

		if (_quests.TrackedQuests == null || _quests.TrackedQuests.Count == 0)
		{
			_list.AddChild(NewLabel("No quests configured.\nAssign TrackedQuests on the QuestManager autoload."));
			return;
		}

		foreach (var quest in _quests.TrackedQuests)
		{
			if (quest == null)
				continue;
			_list.AddChild(BuildRow(quest));
		}
	}

	private static Label NewLabel(string text)
	{
		var l = new Label { Text = text, AutowrapMode = TextServer.AutowrapMode.WordSmart };
		return l;
	}

	private Control BuildRow(QuestResource quest)
	{
		var box = new VBoxContainer();
		box.AddThemeConstantOverride("separation", 4);

		var title = new Label
		{
			Text = _quests.IsQuestComplete(quest.QuestId) ? $"✓ {quest.Title}" : quest.Title,
		};
		title.AddThemeFontSizeOverride("font_size", 18);
		box.AddChild(title);

		if (!string.IsNullOrEmpty(quest.Description))
			box.AddChild(NewLabel(quest.Description));

		int req = _quests.GetProgressRequired(quest);
		int cur = _quests.GetProgressCurrent(quest);
		if (quest.CompletionStatistic != PlayerStatistic.None && req > 0)
		{
			int shown = Mathf.Min(cur, req);
			var progress = new Label
			{
				Text = _quests.IsQuestComplete(quest.QuestId)
					? "Completed"
					: $"{quest.CompletionStatistic}: {shown} / {req}",
			};
			box.AddChild(progress);
		}

		var sep = new HSeparator();
		box.AddChild(sep);
		return box;
	}
}
