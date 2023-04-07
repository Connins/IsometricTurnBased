using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    //we assign all the renderers here through the inspector
    [SerializeField]
    private List<Renderer> renderers;
    [SerializeField]
    private Color color = Color.white;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Material highlightMaterial;

    //helper list to cache all the materials ofd this object
    private List<Material> materials;
    private Material oldMaterial;
    //Gets all the materials from each renderer
    private void Awake()
    {
        materials = new List<Material>();
        foreach (var renderer in renderers)
        {
            oldMaterial = renderer.material;
            //A single child-object might have mutliple materials on it
            //that is why we need to all materials with "s"
            materials.AddRange(new List<Material>(renderer.materials));
        }
    }

    public void ToggleHighlight(bool val)
    {
        if (val)
        {
            //print("here");
            //foreach (var material in materials)
            //{
            //    //We need to enable the EMISSION
            //    material.EnableKeyword("_EMISSION");
            //    //before we can set the color
            //    material.SetColor("_EmissionColor", color);
            //}
            foreach (var renderer in renderers) 
            { 
                renderer.material = highlightMaterial;
            }
        }
        else
        {
            //foreach (var material in materials)
            //{
            //    //we can just disable the EMISSION
            //    //if we don't use emission color anywhere else
            //    material.DisableKeyword("_EMISSION");
            //}
            foreach (var renderer in renderers)
            {
                renderer.material = oldMaterial;
            }
        }
    }

    private GameObject GetMouseGameObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Builds a ray from camera point of view to the mouse position 
        RaycastHit hit;
        // Casts the ray and get the first game object hit 
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            //Instantiate(clickMarker, hit.point, Quaternion.identity); //places clickMarker at hit.point. This isn't needed, just there for visualisation. 
            return hit.transform.gameObject;
        }
        else
        {
            return null;
        }

    }
    void FixedUpdate()
    {
        GameObject hit = GetMouseGameObject();
        print(this.gameObject.transform);
        if (hit == this.gameObject) 
        {
            ToggleHighlight(true);
        }
        else 
        {
            ToggleHighlight(false);
        }

    }
}
