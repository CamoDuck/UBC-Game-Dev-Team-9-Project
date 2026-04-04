using System.Collections.Generic;
using Godot;

/// <summary>
/// Autoload: dialog systems query IsUnlocked(unlockId). QuestManager registers ids when quests complete.
/// </summary>
public partial class DialogUnlockHub : Node
{
	private readonly HashSet<string> _unlockedIds = new();

	[Signal]
	public delegate void DialogueUnlockGrantedEventHandler(string unlockId);

	public void RegisterUnlock(string unlockId)
	{
		if (string.IsNullOrEmpty(unlockId) || !_unlockedIds.Add(unlockId))
			return;
		EmitSignal(SignalName.DialogueUnlockGranted, unlockId);
	}

	public bool IsUnlocked(string unlockId)
		=> !string.IsNullOrEmpty(unlockId) && _unlockedIds.Contains(unlockId);

	/// <summary>Optional: call from save-game load for each restored id.</summary>
	public void RestoreUnlock(string unlockId)
	{
		if (string.IsNullOrEmpty(unlockId))
			return;
		_unlockedIds.Add(unlockId);
	}

	public void ClearAllUnlocksForDebug()
	{
		_unlockedIds.Clear();
	}
}
