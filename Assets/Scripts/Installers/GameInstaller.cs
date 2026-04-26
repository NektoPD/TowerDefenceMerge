using Projectiles;
using Zenject;
using UnityEngine;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private ProjectilePool _projectilePool;

        public override void InstallBindings()
        {
            if (_projectilePool != null)
                Container.Bind<ProjectilePool>().FromInstance(_projectilePool).AsSingle();
        }
    }
}

