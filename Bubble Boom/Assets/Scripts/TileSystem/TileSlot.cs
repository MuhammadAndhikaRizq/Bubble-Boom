using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileSlot : MonoBehaviour
{
    private MeshRenderer meshRenderer => GetComponent<MeshRenderer>();
    private MeshFilter meshFilter => GetComponent<MeshFilter>();
    private Collider myCollider => GetComponent<Collider>();

    public void SwitchTile(GameObject referenceTile)
    {
        gameObject.name = referenceTile.name;

        TileSlot newTile = referenceTile.GetComponent<TileSlot>();

        meshFilter.mesh = newTile.GetMesh();
        meshRenderer.material = newTile.GetMaterial();

        UpdateCollider(newTile.GetCollider());

        foreach(GameObject obj in GetAllChildren())
        {
            DestroyImmediate(obj);
        }

        foreach(GameObject obj in newTile.GetAllChildren())
        {
            Instantiate(obj, transform);
        }
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
}
