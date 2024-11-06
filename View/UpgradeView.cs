using System;
using BLINK.RPGBuilder.LogicMono;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Development.Scripts.Upgrade.View
{
    public class UpgradeView : MonoBehaviour
    {
        [field: SerializeField] public Button UpgradeButton { get; private set; }
        [field: SerializeField] public Button WatchAdButton { get; private set; }
        [field: SerializeField] public Image Icon { get; private set; }
        [field: SerializeField] public TMP_Text NameT { get; private set; }
        [field: SerializeField] public TMP_Text StatT { get; private set; }
        [field: SerializeField] public TMP_Text PriceT { get; private set; }
        [field: SerializeField] private Color _colorCanBuy, _colorCantBuy;

        private Action _action;
        private Action _showAction;
        private bool _isState;
        private RPGCurrency _currency;

        public void RegisterAction(Action action) => 
            _action = action;

        public void RegisterShowAction(Action action) => 
            _showAction = action;

        public void CanBuy(bool state, RPGCurrency currency)
        {
            if (_currency == null)
                _currency = currency;

            _isState = state;
            PriceT.color = state ? _colorCanBuy : _colorCantBuy;
            WatchAdButton.gameObject.SetActive(false);
            
            if (state || Advertising.instance.IsCanShowRewardedAd() == false) 
                return;
            
            if (_currency.ID != (int)CurrencyID.Crystal)
                Invoke(nameof(ShowWatchAdButton), 2f);
        }

        public void ShowWatchAdButton() => 
            WatchAdButton.gameObject.SetActive(true);

        public void Lock() => 
            UpgradeButton.interactable = false;

        private void OnEnable()
        {
            UpgradeButton.onClick.AddListener(Click);
            WatchAdButton.onClick.AddListener(WatchAd);
            GeneralEvents.ClosedPanelShop += UpdateUpgradeView;

            _showAction?.Invoke();
        }

        private void OnDisable()
        {
            UpgradeButton.onClick.RemoveListener(Click);
            WatchAdButton.onClick.RemoveListener(WatchAd);
            GeneralEvents.ClosedPanelShop -= UpdateUpgradeView;
            CancelInvoke(nameof(ShowWatchAdButton));
        }

        private void Click()
        {
            if (_isState == false) 
                RPGBuilderEssentials.Instance.Shop.gameObject.SetActive(true);

            _action?.Invoke();
        }

        private void WatchAd()
        {
            WatchAdButton.gameObject.SetActive(false);
            Advertising.instance.TryWatchRewardedAd();
            _isState = true;
            Click();
        }

        private void UpdateUpgradeView() => 
            _showAction?.Invoke();
    }
}