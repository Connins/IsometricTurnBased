using System;
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
    [SerializeField] private Material inMoveRangeHighlight;
    [SerializeField] private Material inAttackRangeHighlight;
    private Dictionary<string, Material> highlights = new Dictionary<string, Material>();

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

        initializeHighlightMap();
    }

    

    public void ToggleHighlight(string highlight)
    {   
        foreach (var renderer in renderers)
        {
            print("here");
            print(highlight);
            renderer.material = highlights[highlight];
        }
    }
    private void initializeHighlightMap()
    {
        highlights.Add("noHighlight", oldMaterial);
        highlights.Add("inMoveRangeHighlight", inMoveRangeHighlight);
        highlights.Add("inAttackRangeHighlight", inAttackRangeHighlight);
    }
}
