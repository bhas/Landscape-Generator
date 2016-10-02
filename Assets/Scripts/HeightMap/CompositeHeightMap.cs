using System.Collections.Generic;
using UnityEngine;

namespace Generator.HeightMap
{
    [DisallowMultipleComponent]
    class CompositeHeightMap : MonoBehaviour, IHeightMap
    {
        private List<IHeightMap> maps;
        public AnimationCurve curve;

        void Awake()
        {
            maps = new List<IHeightMap>();
        }

        public void AddHeightMap(IHeightMap hm)
        {
            maps.Add(hm);
        }

        public float Sample(float x, float z)
        {
            // accumulate height from other heigh maps
            float acc = 0;
            foreach (IHeightMap map in maps)
            {
                acc += map.Sample(x, z);
            }
            return curve.Evaluate(acc);
        }
    }
}
