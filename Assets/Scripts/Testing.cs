using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;
    // Start is called before the first frame update
    private void Start()
    {
        EntityManager entityManager = World.Active.EntityManager;

        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(LevelComponent),
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(MoveSpeedComponent)
        );

        NativeArray<Entity> entityArray = new NativeArray<Entity>(10000, Allocator.Temp);
        entityManager.CreateEntity(entityArchetype, entityArray);

        for (int i = 0; i < entityArray.Length; i++) {
            Entity entity = entityArray[i];
            entityManager.SetComponentData(entity, 
                new LevelComponent 
                {
                    level = UnityEngine.Random.Range(10, 20)
                });
            entityManager.SetComponentData(entity, 
                new MoveSpeedComponent 
                {
                    moveSpeed = UnityEngine.Random.Range(1f, 5f)
                });
            entityManager.SetComponentData(entity, 
                new Translation 
                {
                    Value = new float3(UnityEngine.Random.Range(-12f, 12f), UnityEngine.Random.Range(-5f, 5f), 0)
                });

            entityManager.SetSharedComponentData(entity, new RenderMesh { mesh = mesh, material = material });
        }

        entityArray.Dispose();
         
    }

}
