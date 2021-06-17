using Fish;
using ScriptableObjects;
using UnityEngine;

namespace Spawners
{
    public class FishSpawner : MonoBehaviour
    {
        [SerializeField] private FishController fishPrefab;
        [SerializeField] private FishData[] fishTypes;


        private void Awake()
        {
            foreach (var fishType in fishTypes)
            {
                for (var i = 0; i < fishType.fishCount; i++)
                {
                    var fish = Instantiate(fishPrefab);
                    fish.FishData = fishType;
                    fish.ResetFish();
                }
            }
        }
    }
}