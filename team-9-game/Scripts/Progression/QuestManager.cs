using System.Collections.Generic;
using Godot;

/// <summary>
/// Autoload: assign TrackedQuests in the inspector (on the autoload node). Evaluates completion using StatisticsManager.
/// </summary>
public partial class QuestManager : Node
{
	[Export]
	public Godot.Collections.Array<QuestResource> TrackedQuests { get; set; } = new();

	private readonly HashSet<string> _completedQuestIds = new();

	private StatisticsManager _statistics;
	private DialogUnlockHub _dialogUnlocks;

	[Signal]
	public delegate void QuestCompletedEventHandler(string questId);

	[Signal]
	public delegate void QuestStateChangedEventHandler();

	public override void _Ready()
	{
		_statistics = GetNodeOrNull<StatisticsManager>("/root/StatisticsManager");
		_dialogUnlocks = GetNodeOrNull<DialogUnlockHub>("/root/DialogUnlockHub");
		if (_statistics != null)
			_statistics.StatisticChanged += OnStatisticChanged;
		CheckAllQuests();
	}

	public override void _ExitTree()
	{
		if (_statistics != null)
			_statistics.StatisticChanged -= OnStatisticChanged;
	}

	private void OnStatisticChanged(int _, int __) => CheckAllQuests();

	/// <summary>Call after changing TrackedQuests at runtime or for tests.</summary>
	public void CheckAllQuests()
	{
		if (_statistics == null)
			return;

		foreach (var quest in TrackedQuests)
		{
			if (quest == null || string.IsNullOrEmpty(quest.QuestId))
				continue;
			if (_completedQuestIds.Contains(quest.QuestId))
				continue;
			if (quest.CompletionStatistic == PlayerStatistic.None || quest.CompletionRequiredCount <= 0)
				continue;

			if (_statistics.GetStatistic(quest.CompletionStatistic) >= quest.CompletionRequiredCount)
				CompleteQuest(quest);
		}
	}

	private void CompleteQuest(QuestResource quest)
	{
		_completedQuestIds.Add(quest.QuestId);
		EmitSignal(SignalName.QuestCompleted, quest.QuestId);
		EmitSignal(SignalName.QuestStateChanged);

		if (_dialogUnlocks == null)
			return;
		foreach (var token in quest.DialogueUnlockIds)
		{
			if (!string.IsNullOrEmpty(token))
				_dialogUnlocks.RegisterUnlock(token);
		}
	}

	public bool IsQuestComplete(string questId)
		=> !string.IsNullOrEmpty(questId) && _completedQuestIds.Contains(questId);

	public int GetProgressCurrent(QuestResource quest)
	{
		if (quest == null || _statistics == null || quest.CompletionStatistic == PlayerStatistic.None)
			return 0;
		return _statistics.GetStatistic(quest.CompletionStatistic);
	}

	public int GetProgressRequired(QuestResource quest)
		=> quest == null ? 0 : Mathf.Max(0, quest.CompletionRequiredCount);

	/// <summary>For save games: persist completed ids and pass back via LoadCompletedQuests.</summary>
	public void LoadCompletedQuests(IEnumerable<string> questIds)
	{
		_completedQuestIds.Clear();
		foreach (var id in questIds)
		{
			if (string.IsNullOrEmpty(id))
				continue;
			_completedQuestIds.Add(id);
		}

		if (_dialogUnlocks == null)
			return;
		foreach (var quest in TrackedQuests)
		{
			if (quest == null || !_completedQuestIds.Contains(quest.QuestId))
				continue;
			foreach (var token in quest.DialogueUnlockIds)
			{
				if (!string.IsNullOrEmpty(token))
					_dialogUnlocks.RestoreUnlock(token);
			}
		}

		EmitSignal(SignalName.QuestStateChanged);
	}

	public IReadOnlyCollection<string> GetCompletedQuestIds() => _completedQuestIds;
}
