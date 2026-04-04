using Godot;

/// <summary>
/// Injects the shared Quests button + overlay onto talent, weapon, and inventory menus.
/// </summary>
public static class SkillTreeQuestUi
{
	private const string AddonScenePath = "res://skill-tree/skill_tree_quest_addon.tscn";

	public static void AttachIfNeeded(Control host)
	{
		if (host == null || !GodotObject.IsInstanceValid(host))
			return;
		if (host.GetNodeOrNull("QuestPanel") != null)
			return;

		var packed = GD.Load<PackedScene>(AddonScenePath);
		if (packed == null)
		{
			GD.PrintErr($"SkillTreeQuestUi: could not load {AddonScenePath}");
			return;
		}

		var wrapper = packed.Instantiate<Node>();
		var list = wrapper.GetChildren();
		foreach (Node child in list)
		{
			wrapper.RemoveChild(child);
			host.AddChild(child);
		}

		wrapper.QueueFree();

		var questButton = host.GetNodeOrNull<Button>("QuestButton");
		var questPanel = host.GetNodeOrNull<Control>("QuestPanel");
		if (questButton != null && questPanel != null)
			questButton.Pressed += () => { questPanel.Visible = !questPanel.Visible; };
	}
}
