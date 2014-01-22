using UnityEngine;
using System.Collections;

public class UnitBasicArtillery : UnitArtillery
{

    public int attackPower1 = 10;
    public int attackPower2 = 20;

	// Use this for initialization
    public override void Start ()
	{
        base.Start();

        basicAttackPower = attackPower1;
        secondaryAttackPower = attackPower2;
	}
	
	// Update is called once per frame
    public override void Update ()
	{
		base.Update();
	}

} // class UnitBasicArtillery
