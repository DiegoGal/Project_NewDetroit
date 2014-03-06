using UnityEngine;
using System.Collections;

public class UnitHeavyArtillery : UnitArtillery
{

    public float attackPower1 = 20.0f;
    public float attackPower2 = 40.0f;

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
