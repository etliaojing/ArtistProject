using ProceduralToolkit.Examples.UI;
using UnityEngine;

namespace ProceduralToolkit.Examples
{
    public class LowPolyTerrainGeneratorConfigurator : ConfiguratorBase
    {
        public MeshFilter terrainMeshFilter;
        public RectTransform leftPanel;
        public bool constantSeed = false;
        public LowPolyTerrainGenerator.Config config = new LowPolyTerrainGenerator.Config();

        private const int minXSize = 10;
        private const int maxXSize = 30;
        private const int minYSize = 1;
        private const int maxYSize = 5;
        private const int minZSize = 10;
        private const int maxZSize = 30;
        private const float minCellSize = 0.3f;
        private const float maxCellSize = 2;
        private const int minNoiseScale = 1;
        private const int maxNoiseScale = 20;

        private Mesh terrainMesh;

        private void Awake()
        {
            // Generate();
        }

        public void Generate(bool randomizeConfig = true)
        {
            if (constantSeed)
            {
                Random.InitState(0);
            }

            var draft = LowPolyTerrainGenerator.TerrainDraft(config);
            draft.Move(Vector3.left*config.terrainSize.x/2 + Vector3.back*config.terrainSize.z/2);
            AssignDraftToMeshFilter(draft, terrainMeshFilter, ref terrainMesh);
            terrainMeshFilter.GetComponent<MeshCollider>().sharedMesh = null;
            terrainMeshFilter.GetComponent<MeshCollider>().sharedMesh = terrainMeshFilter.mesh;
        }
    }
}