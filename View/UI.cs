using Cysharp.Threading.Tasks;
using System.ComponentModel;
using BLINK.RPGBuilder.Characters;
using UnityEngine;
using BLINK.RPGBuilder.Combat;
using BLINK.RPGBuilder.Managers;

namespace _Development.Scripts.Upgrade.View
{
	public class UI : MonoBehaviour
	{
		private UpgradeViewContainer _container;

		private void Start()
		{
			FindContainer();
		}

		private async UniTaskVoid FindContainer()
		{
			await UniTask.WaitUntil(() =>
			{

				_container = Object.FindObjectOfType<UpgradeViewContainer>(true);
				if (_container == null)
				{
					Debug.Log("Find Container");
					return false;
				}

				return true;

			});
		}

		public void ShowShop()
		{
			_container.gameObject.SetActive(true);
		}
	}
}