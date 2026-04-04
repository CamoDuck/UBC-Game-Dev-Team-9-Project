using Godot;

/// <summary>
/// Designer-authored quest definition (.tres). Runtime completion is tracked by QuestManager.
/// </summary>
[Tool]
[GlobalClass]
public partial class QuestResource : Resource
{
	[Export] public string QuestId { get; set; } = "";

	[Export] public string Title { get; set; } = "Quest";

	[Export(PropertyHint.MultilineText)]
	public string Description { get; set; } = "";

	/// <summary>Statistic that must reach at least CompletionRequiredCount.</summary>
	[Export] public PlayerStatistic CompletionStatistic { get; set; } = PlayerStatistic.None;

	[Export] public int CompletionRequiredCount { get; set; } = 1;

	/// <summary>Tokens the dialog team can check via DialogUnlockHub.IsUnlocked after this quest completes.</summary>
	[Export]
	public Godot.Collections.Array<string> DialogueUnlockIds { get; set; } = new();
}
