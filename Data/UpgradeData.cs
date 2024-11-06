using System;
using NaughtyAttributes;
using UnityEngine;

namespace _Development.Scripts.Upgrade.Data
{
	public abstract class UpgradeData : ScriptableObject
	{
		[Serializable]
		public struct Upgrade
		{
			public float UpgradeStat;
			public float UpgradeCost;
		}
		
		public bool IsNotShow;
		public int ID => StatUpgrade.ID;
		[field: SerializeField, ShowAssetPreview] public Sprite IconUpgrade { get; private set; }
		[field: SerializeField, BoxGroup("Stat")] public RPGStat StatUpgrade { get; private set; }
		[field: SerializeField, BoxGroup("Stat")] public RPGCurrency Currency { get; private set; }

		public abstract bool TryUpgrade(int currentLevel);
		public abstract Upgrade GetUpgrade(int currentLevel);

		public abstract string GetDescription(int currentLevel);
	}
}