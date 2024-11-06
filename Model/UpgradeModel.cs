using _Development.Scripts.Upgrade.Data;
using _Development.Scripts.Upgrade.Initialization;
using _Development.Scripts.Upgrade.State;
using BLINK.RPGBuilder.Characters;
using UnityEngine;

namespace _Development.Scripts.Upgrade.Model
{
	public class UpgradeModel
	{
		private int _currentLevel;
		private UpgradeData _upgradeData;

		public int ID => _upgradeData.ID;
		public UpgradeModel(int currentLevel, UpgradeData upgradeData)
		{
			_currentLevel = currentLevel;
			_upgradeData  = upgradeData;
		}
		public int GetLevel()
		{
			return _currentLevel;
		}

		public bool TryUpgrade()
		{
			return _upgradeData.TryUpgrade(_currentLevel + 1);
		}
		public void UpgradeLevel()
		{
			_currentLevel++;
		}
		public UpgradeData.Upgrade GetUpgrade()
		{
			return _upgradeData.GetUpgrade(_currentLevel);
		}
		public UpgradeData.Upgrade NextUpgrade()
		{
			return _upgradeData.GetUpgrade(_currentLevel + 1);
		}
		public string GetDescription()
		{
			return _upgradeData.GetDescription(_currentLevel);
		}

		public void Load(UpgradeState state)
		{
			_currentLevel = state.level;
		}

		public UpgradeData GetData()
		{
			return _upgradeData;
		}
	}
}