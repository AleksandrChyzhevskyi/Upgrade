using _Development.Scripts.Observer;
using _Development.Scripts.Shop.Interface;
using _Development.Scripts.Upgrade.Data;
using BLINK.RPGBuilder.Characters;
using UnityEngine;
using UnityEngine.UI;

namespace _Development.Scripts.Upgrade.View
{
    public class UpgradeViewContainer : MonoBehaviour
    {
        [field: SerializeField] public Transform Container { get; private set; }
        public Button ExitButton;

        private int _countApp = 0;
        private bool _isPlayerKilledBoss;

        private IPurchaseService _purchaseService;

        private void OnEnable()
        {
            ObserverUI.Show("Upgrade Shop");
            ExitButton.onClick.AddListener(ExitPanel);
            Container.transform.position = new Vector3(Container.transform.position.x, 0, Container.transform.position.z);
            
            if (_countApp != 0) 
                return;
            
            GeneralEvents.PlayerKilledBoss += OnPlayerKilledBoss;
            _countApp = PlayerPrefsAppCount.LoadAppCount();
        }

        private void OnDisable() => 
            ExitButton.onClick.RemoveListener(ExitPanel);

        private void ExitPanel()
        {
            if (Character.Instance.CharacterData.Skins.DataFiles.Count > 1
                && _isPlayerKilledBoss
                && _countApp < 1)
            {
                AppRating.Instance.RateAndReview();
                _countApp++;
                PlayerPrefsAppCount.SaveAppCount(_countApp);
                GeneralEvents.PlayerKilledBoss -= OnPlayerKilledBoss;
            }

            gameObject.SetActive(false);
        }

        private void OnPlayerKilledBoss() =>
            _isPlayerKilledBoss = true;
    }
}