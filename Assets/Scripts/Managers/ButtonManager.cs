using UnityEngine;
using Zenject;

namespace Managers
{
    public class ButtonManager: MonoBehaviour
    {

        private IdleManager idleManager;

        [Inject]
        public void Construct(IdleManager idleManagerZ)
        {
            idleManager = idleManagerZ;
        }

        public void BuyLength()
        {
            idleManager.BuyLength();
        }
        
        public void BuyStrength()
        {
            idleManager.BuyStrength();
        }

        public void BuyOfflineEarnings()
        {
            idleManager.BuyOfflineEarnings();
        }

        public void Collect()
        {
            idleManager.Collect();
        }

        public void CollectDouble()
        {
            idleManager.CollectDouble();
        }
    }
}