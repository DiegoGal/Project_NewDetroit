using UnityEngine;
using System.Collections;

public class UnitHeavyArtillery : UnitArtillery
{

    public int attackPower1 = 20;
    public int attackPower2 = 40;

	// Use this for initialization
    public override void Start ()
	{
        base.Start();

        basicAttackPower = attackPower1;
        secondaryAttackPower = attackPower2;
	}
	
	// Update is called once per frame
    public override void Update()
	{
		base.Update();
	}
	
} // class UnitHeavyArtillery
