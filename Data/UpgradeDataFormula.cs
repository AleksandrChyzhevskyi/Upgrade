using System.Collections.Generic;
using BLINK.RPGBuilder.Combat;
using BLINK.RPGBuilder.Managers;
using NaughtyAttributes;
using UnityEngine;

namespace _Development.Scripts.Upgrade.Data
{
    [CreateAssetMenu(menuName = "Development/UpgradeData")]
    public sealed class UpgradeDataFormula : UpgradeData
    {
        [field: SerializeField, BoxGroup("Stat")]
        public Upgrade BaseValue { get; private set; }

        [field: SerializeField, BoxGroup("Stat")]
        public float MultiplierStat { get; private set; }

        [field: SerializeField, BoxGroup("Stat")]
        public float MultiplierCost { get; private set; }

        public bool IsPercentage;

        [InfoBox("≈сли ћаксимум равен 0 то он будет иметь возможность прокачиватьс€ бесконечно", EInfoBoxType.Normal)]
        [SerializeField, BoxGroup("Stat")]
        private int _maxUpgrade;

        public override bool TryUpgrade(int currentLevel)
        {
            if (_maxUpgrade == 0)
            {
                return true;
            }

            return currentLevel < _maxUpgrade + 1;
        }

        public override Upgrade GetUpgrade(int currentLevel)
        {
            Upgrade newUpgrade = new Upgrade();

            newUpgrade.UpgradeStat = BaseValue.UpgradeStat + BaseValue.UpgradeStat * MultiplierStat * currentLevel;
            newUpgrade.UpgradeCost = BaseValue.UpgradeCost + BaseValue.UpgradeCost * MultiplierCost * currentLevel;

            return newUpgrade;
        }

        public override string GetDescription(int currentLevel)
        {
            Dictionary<int, CombatData.CombatEntityStat> stats = GameState.playerEntity.GetStats();

            float curValue = 0;

            if (GameDatabase.Instance.GetStats()[StatUpgrade.ID].isVitalityStat)
            {
                curValue = stats[StatUpgrade.ID].currentMaxValue;
            }
            else
            {
                curValue = stats[StatUpgrade.ID].currentValue;
            }

            float baseValueUpgradeStat = BaseValue.UpgradeStat + BaseValue.UpgradeStat * MultiplierStat * currentLevel;

            if (IsPercentage)
            {
                curValue = Mathf.Max(curValue - 100, 0f);
                baseValueUpgradeStat -= 100;

                return
                    $"{StatUpgrade.entryName}<br><size=20><color=#BEB5B6>{StatUpgrade.entryDescription} from +{(int)curValue}% to </color><color=#59c321>+{(int)baseValueUpgradeStat}%</color></size>";
            }
            else
            {
                return
                    $"{StatUpgrade.entryName}<br><size=20><color=#BEB5B6>{StatUpgrade.entryDescription} from {curValue} to </color><color=#59c321>{baseValueUpgradeStat} </color></size>";
            }

            return
                $"{StatUpgrade.entryName}<br><size=20><color=#BEB5B6>{StatUpgrade.entryDescription} from {curValue} to </color><color=#59c321>{baseValueUpgradeStat} </color></size>";
            //return $"{StatUpgrade.entryName} <size=20><color=#BEB5B6> {StatUpgrade.entryDescription} from {BaseValue.UpgradeStat + BaseValue.UpgradeStat * MultiplierStat * (currentLevel - 1)} to {BaseValue.UpgradeStat + BaseValue.UpgradeStat * MultiplierStat * currentLevel} </color></size>";
        }
    }
}