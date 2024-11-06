using System;

namespace _Development.Scripts.Upgrade.State
{
	[Serializable]
	public struct UpgradeState
	{
		public int IDState;
		public int level;
		public float CurrentValue;
	}
}