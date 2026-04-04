using System.Collections.Generic;
using Godot;

/// <summary>
/// Autoload singleton: call AddStatistic from combat, pickups, crafting, etc.
/// </summary>
public partial class StatisticsManager : Node
{
	private readonly Dictionary<PlayerStatistic, int> _values = new();

	[Signal]
	public delegate void StatisticChangedEventHandler(int statistic, int newValue);

	public void AddStatistic(PlayerStatistic statistic, int count = 1)
	{
		if (statistic == PlayerStatistic.None || count == 0)
			return;

		int prev = GetStatistic(statistic);
		int next = prev + count;
		if (next < 0)
			next = 0;
		_values[statistic] = next;
		EmitSignal(SignalName.StatisticChanged, (int)statistic, next);
	}

	public int GetStatistic(PlayerStatistic statistic)
	{
		if (statistic == PlayerStatistic.None)
			return 0;
		return _values.GetValueOrDefault(statistic, 0);
	}

	/// <summary>For saves / debug only.</summary>
	public void SetStatistic(PlayerStatistic statistic, int value)
	{
		if (statistic == PlayerStatistic.None)
			return;
		if (value < 0)
			value = 0;
		_values[statistic] = value;
		EmitSignal(SignalName.StatisticChanged, (int)statistic, value);
	}
}
