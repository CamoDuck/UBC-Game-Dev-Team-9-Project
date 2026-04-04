using Godot;

namespace Team9Game.Scripts.Dialog;

/// <summary>
/// Use from dialog code when you need to branch on quest-granted unlock tokens.
/// Tokens are registered in QuestResource.DialogueUnlockIds and appear when the quest completes.
/// </summary>
public static class DialogUnlockQueries
{
	public static bool IsDialogueUnlocked(string unlockId)
	{
		if (string.IsNullOrEmpty(unlockId))
			return false;
		if (Engine.GetMainLoop() is not SceneTree tree)
			return false;
		var hub = tree.Root.GetNodeOrNull<DialogUnlockHub>("DialogUnlockHub");
		return hub != null && hub.IsUnlocked(unlockId);
	}
}
