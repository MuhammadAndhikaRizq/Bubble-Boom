using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;

public class TileSlot : MonoBehaviour
{
    private MeshRenderer meshRenderer => GetComponent<MeshRenderer>();
    private MeshFilter meshFilter => GetComponent<MeshFilter>();
    private Collider myCollider => GetComponent<Collider>();
    private NavMeshSurface myNavMesh => GetComponentInParent<NavMeshSurface>(true);
    private TileSlotHolder tileSlotHolder => GetComponentInParent<TileSlotHolder>(true);

    public void SwitchTile(GameObject referenceTile)
    {
        gameObject.name = referenceTile.name;

        TileSlot newTile = referenceTile.GetComponent<TileSlot>();

        meshFilter.mesh = newTile.GetMesh();
        meshRenderer.material = newTile.GetMaterial();

        UpdateCollider(newTile.GetCollider());
        UpdateChildren(newTile);
        UpdateLayer(referenceTile);
        UpdateNavmesh();

        TurnIntoBuilldSlot(referenceTile);
    }

    
    
    public Material GetMaterial() => meshRenderer.sharedMaterial;
    public Mesh GetMesh() => meshFilter.sharedMesh;
    public Collider GetCollider() => myCollider;

    public List<GameObject> GetAllChildren()
    {
        List<GameObject> children = new List<GameObject>();

        foreach(Transform child in transform)
        {
            children.Add(child.gameObject);
        }

        return children;
    }

    private void TurnIntoBuilldSlot(GameObject referenceTile)
    {
        BuildSlot buildSlot = GetComponent<BuildSlot>();

        if(referenceTile != tileSlotHolder.tileField)
        {
            if(buildSlot != null)
                DestroyImmediate(buildSlot);
        }else{
            if(buildSlot == null)
                gameObject.AddComponent<BuildSlot>();
        }
    }

    private void UpdateNavmesh() => myNavMesh.BuildNavMesh();
    public void UpdateCollider(Collider newCollider)
    {
        DestroyImmediate(myCollider);

        if(newCollider is BoxCollider)
        {
            BoxCollider origin = newCollider.GetComponent<BoxCollider>();
            BoxCollider myNewCollider = transform.AddComponent<BoxCollider>();

            myNewCollider.center = origin.center;
            myNewCollider.size = origin.size;   
        }

        if(newCollider is MeshCollider)
        {
            MeshCollider origin = newCollider.GetComponent<MeshCollider>();
            MeshCollider myNewCollider = transform.AddComponent<MeshCollider>();

            myNewCollider.sharedMaterial = origin.sharedMaterial;
            myNewCollider.convex = origin.convex;   
        }
    }

    private void UpdateChildren(TileSlot newTile)
    {
        foreach (GameObject obj in GetAllChildren())
        {
            DestroyImmediate(obj);
        }

        foreach (GameObject obj in newTile.GetAllChildren())
        {
            Instantiate(obj, transform);
        }
    }

    public void UpdateLayer(GameObject referenceObj) => gameObject.layer = referenceObj.layer;
    public void RotateTile(int dir)
    {
        transform.Rotate(0, 90 * dir, 0);
        UpdateNavmesh();
    } 
    public void AdjustY(int verticalDir)
    {
        transform.position += new Vector3(0, .1f * verticalDir, 0);
        UpdateNavmesh();
    } 
}
