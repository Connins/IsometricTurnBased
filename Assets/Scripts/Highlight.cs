using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    //we assign all the renderers here through the inspector
    [SerializeField] private List<Renderer> renderers;
    private Color colorWhite = Color.white;
    private Dictionary<string, Color> colourModifier = new Dictionary<string, Color>();
    private Color originalColour;
    //helper list to cache all the materials ofd this object
    private List<Material> materials;
    //Gets all the materials from each renderer
    private void Awake()
    {
        materials = new List<Material>();
        foreach (var renderer in renderers)
        {
            originalColour = renderer.material.color;
            renderer.material.DisableKeyword("_EMISSION");
            //A single child-object might have mutliple materials on it
            //that is why we need to all materials with "s"
            materials.AddRange(new List<Material>(renderer.materials));
        }
        
        initializeHighlightMap();
    }
    public void highlightMaterial(string highlight)
    {
        foreach (var renderer in renderers)
        {
            renderer.material.color = colourModifier[highlight];
            if (highlight == "noHighlight")
            {
                renderer.material.DisableKeyword("_EMISSION");
            }
            if (highlight == "inMoveRangeHighlight")
            {
                renderer.material.EnableKeyword("_EMISSION");
                renderer.material.SetColor("_EmissionColor", colorWhite * 0.5f);
            }
        }
    }
    private void initializeHighlightMap()
    {
        colourModifier.Add("noHighlight", originalColour);
        colourModifier.Add("inMoveRangeHighlight", originalColour);
        colourModifier.Add("inAttackRangeHighlight", new Color(0.9433962f, 0.3070487f, 0.3985098f, 1f));
        colourModifier.Add("red", Color.red);
        colourModifier.Add("yellow", Color.yellow);
    }

    public void changeOriginalColour(string colour)
    {
        originalColour = colourModifier[colour];
        colourModifier["noHighlight"] = originalColour;
        colourModifier["inMoveRangeHighlight"] = originalColour;
        highlightMaterial("noHighlight");
    }
}
