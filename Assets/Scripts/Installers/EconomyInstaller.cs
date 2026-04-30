using Economy;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class EconomyInstaller : MonoInstaller
    {
        [SerializeField, Min(0)] private int _startingCoins = 0;
        [SerializeField, Min(0)] private int _startingWood = 0;
        [SerializeField, Min(0)] private int _startingMetal = 0;
        [SerializeField, Min(0)] private int _startingCrystal = 0;
        [SerializeField, Min(0)] private int _startingMystery = 0;

        public override void InstallBindings()
        {
            Container.Bind<Wallet>().AsSingle().WithArguments(_startingCoins);
            Container.Bind<ResourceInventory>().AsSingle().WithArguments(_startingWood, _startingMetal, _startingCrystal, _startingMystery);
            Container.Bind<CoinCounterTarget>().AsSingle();
        }
    }
}

