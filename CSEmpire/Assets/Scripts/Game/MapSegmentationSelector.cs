using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class MapSegmentationSelector : MonoBehaviour
    {
        [HideInInspector] public MapSegmentationSelector Instance;
            
        private readonly Queue<int> indices = new();
        public GameObject[] segmentations;
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                return;
            
            System.Random random = new();
            while (indices.Count < 8)
            {
                int index = random.Next(segmentations.Length);
                if (!ContainsTwice(indices, index))
                    indices.Enqueue(index);
            }
            
            indices.Enqueue(random.Next(segmentations.Length));
        }

        private static bool ContainsTwice<T>(IEnumerable<T> values, T value)
        {
            bool contains = false;

            foreach (T _ in values.Where(v => v.Equals(value)))
            {
                if (contains)
                    return true;
                contains = true;
            }

            return false;
        }

        private void ChooseNextSegmentation()
        {
            int index = indices.Dequeue();

            for (int i = 0; i < segmentations.Length; i++)
                segmentations[i].SetActive(index == i);
        }
    }
}