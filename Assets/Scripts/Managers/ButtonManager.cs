using UnityEngine;
using Zenject;

namespace Managers
{
    public class ButtonManager: MonoBehaviour
    {

        private IdleManager _idleManager;

        [Inject]
        public void Construct(IdleManager idleManager)
        {
            _idleManager = idleManager;
        }

        public void BuyLength()
        {
            _idleManager.BuyLength();
        }
        
        public void BuyStrength()
        {
            _idleManager.BuyStrength();
        }

        public void BuyOfflineEarnings()
        {
            _idleManager.BuyOfflineEarnings();
        }

        public void Collect()
        {
            _idleManager.Collect();
        }

        public void CollectDouble()
        {
            _idleManager.CollectDouble();
        }

        public void CastHook()
        {
            _idleManager.CastHook();
        }
    }
}