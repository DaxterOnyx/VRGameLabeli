﻿class FireEffect : ElementEffect
{
	[UnityEngine.Tooltip("Damage each second")]
	public float Damage = 10;
	
	protected override void StartEffect()
	{
		//TODO add graphic fire effect 
	}
	protected override void Effect()
	{
		TakeHits.takeHits(Damage, false);
	}
	protected override void StopEffect()
	{
		//TODO remove graphic fire effect
	}
}
