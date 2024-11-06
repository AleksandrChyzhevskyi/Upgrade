using System;
using System.Collections;
using System.Collections.Generic;
using _Development.Scripts.Data;
using System.Linq;
using _Development.Scripts.PlayerBase.State;
using _Development.Scripts.State;
using _Development.Scripts.Upgrade.Data;
using _Development.Scripts.Upgrade.Model;
using _Development.Scripts.Upgrade.State;
using _Development.Scripts.Upgrade.View;
using BLINK.RPGBuilder.Characters;
using BLINK.RPGBuilder.Logic;
using BLINK.RPGBuilder.Managers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static _Development.Scripts.Upgrade.Data.UpgradeData;
using static RequirementsData;
using Object = UnityEngine.Object;

namespace _Development.Scripts.Upgrade.Initialization
{
    public class UpgradeInitialization
    {
        private StaticData _staticData;
        private UpgradeViewContainer _container;

        private PlayerState _state = new PlayerState();
        private const string KEY_SAVE = "PLAYER_UPGRADE";

        private Dictionary<int, UpgradeModel> _upgradeStat = new Dictionary<int, UpgradeModel>();
        private Dictionary<int, UpgradeView> _upgradeView = new Dictionary<int, UpgradeView>();

        public static UpgradeInitialization Instance;

        public static event Action<UpgradeModel> OnUpgradeStat;

        public UpgradeInitialization(StaticData staticData, UpgradeViewContainer upgradeViewContainer)
        {
            _staticData = staticData;
            _container = upgradeViewContainer;

            Instance = this;
        }

        public void Save()
        {
            _state.States = new UpgradeState[_upgradeStat.Count];

            int count = 0;
            foreach (var keyValuePair in _upgradeStat)
            {
                _state.States[count].IDState = keyValuePair.Key;
                _state.States[count].level = keyValuePair.Value.GetLevel();
                count++;
            }

            var json = JsonUtility.ToJson(_state);
            PlayerPrefs.SetString(KEY_SAVE, json);
        }

        public UpgradeState[] Load()
        {
            var json = PlayerPrefs.GetString(KEY_SAVE);

            var playerState = JsonUtility.FromJson<PlayerState>(json);

            if (playerState == null)
            {
                return null;
            }

            return playerState.States;
        }

        public void Init()
        {
            foreach (var upgradeData in _staticData.UpgradesDate)
            {
                if(upgradeData.IsNotShow)
                    continue;
                
                CreateModel(upgradeData, -1);
            }

            var upgrade = Load();

            if (upgrade != null)
            {
                foreach (var state in upgrade)
                {
                    _upgradeStat[state.IDState].Load(state);
                }
            }

            LoadUpgradePlayer();

            foreach (var upgradeStatValue in _upgradeStat.Values)
            {
                CreateUpgrade(upgradeStatValue.GetData(), upgradeStatValue);
            }
        }

        public List<UpgradeModel> GetUpgradeModel() =>
            _upgradeStat.Values.ToList();

        private void LoadUpgradePlayer()
        {
            var stats = GameState.playerEntity.GetStats();

            foreach (var upgradeModel in _upgradeStat.Values)
            {
                var currentLevel = upgradeModel.GetLevel();
                if (currentLevel == -1) continue;

                var combatEntityStat = stats[upgradeModel.ID];
                var upgrade = upgradeModel.GetUpgrade();

                combatEntityStat.currentValue = upgrade.UpgradeStat;
                combatEntityStat.currentMaxValue = upgrade.UpgradeStat;

                CombatEvents.Instance.OnStatValueChanged(GameState.playerEntity, combatEntityStat.stat,
                    combatEntityStat.currentValue, combatEntityStat.currentMaxValue);
            }
        }

        private void CreateModel(UpgradeData upgradeData, int i)
        {
            UpgradeModel upgradeModel = new UpgradeModel(i, upgradeData);
            _upgradeStat.Add(upgradeData.ID, upgradeModel);
        }

        private void CreateUpgrade(UpgradeData upgradeData, UpgradeModel upgradeModel)
        {
            var instanceUpgradeView = Object.Instantiate(_staticData.BaseUpgradeView, _container.Container);

            var upgradeLevel = upgradeModel.GetLevel() + 1;

            instanceUpgradeView.Icon.sprite = upgradeData.IconUpgrade;
            instanceUpgradeView.NameT.text = upgradeData.GetDescription(upgradeLevel);
            instanceUpgradeView.PriceT.text = $"<sprite name={upgradeData.Currency.entryName}> " +
                                              $"{StaticVariables.FormattedCurrencyString(Mathf.CeilToInt(upgradeData.GetUpgrade(upgradeLevel).UpgradeCost)):##,###,###}"
                ;

            Action upgradeAction = () =>
            {
                var nextUpgrade = upgradeModel.NextUpgrade();
                if (upgradeModel.TryUpgrade() && (Advertising.instance.IsCanGetReward() ||
                                                  InventoryManager.Instance.CanRemoveCurrency(upgradeData.Currency.ID,
                                                      nextUpgrade.UpgradeCost)))
                {
                    var stats = GameState.playerEntity.GetStats();

                    var combatEntityStat = stats[upgradeModel.ID];

                    combatEntityStat.currentValue = nextUpgrade.UpgradeStat;
                    combatEntityStat.currentMaxValue = nextUpgrade.UpgradeStat;

                    CombatEvents.Instance.OnStatValueChanged(GameState.playerEntity, combatEntityStat.stat,
                        combatEntityStat.currentValue, combatEntityStat.currentMaxValue);

                    OnUpgradeStat?.Invoke(upgradeModel);

                    upgradeModel.UpgradeLevel();

                    var newUpgrade = upgradeModel.NextUpgrade();

                    instanceUpgradeView.PriceT.text = $"<sprite name={upgradeData.Currency.entryName}> " +
                                                      $"{StaticVariables.FormattedCurrencyString(Mathf.CeilToInt(newUpgrade.UpgradeCost)):##,###,###}";


                    var nextLevel = upgradeModel.GetLevel() + 1;
                    instanceUpgradeView.NameT.text = upgradeData.GetDescription(nextLevel);

                    if (!upgradeModel.TryUpgrade())
                    {
                        instanceUpgradeView.Lock();
                    }

                    foreach (var model in _upgradeStat.Values)
                    {
                        var view = _upgradeView[model.ID];

                        var modelUpgrade = model.NextUpgrade();

                        view.CanBuy(InventoryManager.Instance.TryBuy(model.GetData().Currency.ID,
                            modelUpgrade.UpgradeCost), model.GetData().Currency);
                    }
                }
            };

            instanceUpgradeView.RegisterAction(upgradeAction);


            Action showAction = () =>
            {
                var upgrade = upgradeModel.NextUpgrade();
                instanceUpgradeView.CanBuy(InventoryManager.Instance.TryBuy(upgradeData.Currency.ID,
                    upgrade.UpgradeCost), upgradeData.Currency);
            };

            instanceUpgradeView.RegisterShowAction(showAction);

            _upgradeView.Add(upgradeModel.ID, instanceUpgradeView);
        }


        public Dictionary<int, UpgradeModel> GetUpgrades()
        {
            return _upgradeStat;
        }
    }
}