using System.Collections.Generic;
using UnityEngine;

namespace Generator.HeightMap
{
    class CompositeHeightMap : IHeightMap
    {
        private List<IHeightMap> maps;

        public CompositeHeightMap()
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
            return acc;
        }
    }
}
