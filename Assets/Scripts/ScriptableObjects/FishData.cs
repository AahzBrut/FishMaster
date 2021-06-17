using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Add new fish type", order = 0)]
    public class FishData : ScriptableObject
    {
        public int price;
        public int fishCount;
        public float minDepth;
        public float maxDepth;
        public float minSpeed;
        public float maxSpeed;
        public float size;
        public Sprite sprite;
    }
}