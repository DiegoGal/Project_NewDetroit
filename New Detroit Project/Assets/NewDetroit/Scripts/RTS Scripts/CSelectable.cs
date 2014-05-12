using UnityEngine;
using System.Collections;

public class CSelectable : MonoBehaviour
{
	
	private Color teamColor;
    private Color origColor;
    
    private float outlineWidth;
    private Color outlineColor;

    private Transform model;
    // 0 = Unit model, 1 = building with DiffuseColor, 2 = renderer.color, 3 = AlphaColor
    private int selectType;
    

    // indicates the number of materials in its model
    private int numberOfMaterials;

	private bool selected;

    private UnitController unitReference = null;

	// Use this for initialization
	void Awake ()
    {

        model = transform.FindChild("Model");
        if (model)
        {
            selectType = 0;
            // if there is a "model" children in the object it is a unit
            numberOfMaterials = model.renderer.materials.Length;

            outlineWidth = model.renderer.material.GetFloat("_OutlineWidth");

            for (int i = 0; i < numberOfMaterials; i++)
                model.renderer.materials[i].SetFloat("_OutlineWidth", 0.0f);

            model.renderer.material.SetFloat("_OutlineWidth", 0.0f);

            //teamColor = outlineColor = model.renderer.material.GetColor("_OutlineColor");
        }
        else
        {
            // if not, it is a building
            if (renderer.material.HasProperty("_DiffuseColor"))
            {
                selectType = 1;
                origColor = renderer.material.GetColor("_DiffuseColor");
            }
            else if (renderer.material.HasProperty("_AlphaColor"))
            {
                // is a buildable (construible) building
                selectType = 1;
                origColor = Color.white;
            }
            else
            {
                selectType = 2;
                origColor = renderer.material.color;
            }
        }
        teamColor = TeamsColors.colors[GetComponent<CTeam>().teamColorIndex];

		selected = false;

        unitReference = GetComponent<UnitController>();
	}

    public void ResetTeamColor ()
    {
        teamColor = outlineColor = TeamsColors.colors[GetComponent<CTeam>().teamColorIndex];
        for (int i = 0; i < numberOfMaterials; i++)
            model.renderer.materials[i].SetColor("_OutlineColor", teamColor);
    }

	public void SetSelected ()
	{
		selected = true;
        switch (selectType)
        {
            case 0:
                for (int i = 0; i < numberOfMaterials; i++)
                    model.renderer.materials[i].SetFloat("_OutlineWidth", outlineWidth);
                break;
            case 1:
                this.renderer.material.SetColor("_DiffuseColor", teamColor);
                break;
            case 2:
                this.renderer.material.color = teamColor;
                break;
        }

        if (unitReference)
            unitReference.isSelected = true;
	}
	
	public void SetDeselect ()
	{
		selected = false;
        switch (selectType)
        {
            case 0:
                for (int i = 0; i < numberOfMaterials; i++)
                    model.renderer.materials[i].SetFloat("_OutlineWidth", 0.0f);
                break;
            case 1:
                this.renderer.material.SetColor("_DiffuseColor", origColor);
                break;
            case 2:
                this.renderer.material.color = origColor;
                break;
        }

        if (unitReference)
            unitReference.isSelected = false;
	}
	
	public void SetSelected (bool selected)
	{
		if (selected)
			SetSelected();
		else
			SetDeselect();
	}
	
	public bool IsSelected ()
	{
		return selected;
	}
	
	public void ResetColor ()
	{
		this.renderer.material.color = origColor;
	}

    public void SetOutlineColor (Color color)
    {
        outlineColor = color;
        for (int i = 0; i < numberOfMaterials; i++)
            model.renderer.materials[i].SetColor("_OutlineColor", outlineColor);
    }

}
