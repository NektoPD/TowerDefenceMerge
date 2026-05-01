using Economy;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class EconomyInstaller : MonoInstaller
    {
        [SerializeField, Min(0)] private int _startingCoins = 0;
        [SerializeField, Min(0)] private int _startingEye = 0;
        [SerializeField, Min(0)] private int _startingBook = 0;
        [SerializeField, Min(0)] private int _startingVoid = 0;
        [SerializeField, Min(0)] private int _startingMystery = 0;

        public override void InstallBindings()
        {
            Container.Bind<Wallet>().AsSingle().WithArguments(_startingCoins);
            Container.Bind<ResourceInventory>().AsSingle().WithArguments(_startingEye, _startingBook, _startingVoid, _startingMystery);
            Container.Bind<CoinCounterTarget>().AsSingle();
        }
    }
}

