using Managers;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class ProjectInstallers : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IdleManager>().FromComponentOn(CreateIdleManagerPrefab()).AsSingle().NonLazy();
        }

        private static GameObject CreateIdleManagerPrefab()
        {
            var idleManager = new GameObject();
            idleManager.AddComponent<IdleManager>();
            return idleManager;
        }
    }
}