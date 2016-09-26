
namespace Generator.HeightMap
{
    class CompositeHeightMap : IHeightMap
    {
        private IHeightMap[] maps;

        public CompositeHeightMap(params IHeightMap[] maps)
        {
            this.maps = maps;
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
