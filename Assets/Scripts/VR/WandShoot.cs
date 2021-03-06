﻿using System;
using UnityEngine;
using VRTK;

enum Element
{
	Electricity,
	Fire,
	Ice,
	Wind
}

public class WandShoot : WeaponScript
{
	[HideInInspector]
	internal new GameObject ActiveProjectile
	{
		get
		{
			switch (m_element)
			{
				case Element.Electricity:
					return ElectricityProjectile;
				case Element.Fire:
					return FireProjectile;
				case Element.Ice:
					return IceProjectile;
				case Element.Wind:
					return WindProjectile;
				default:
					Debug.LogError("Element ERROR");
					return FireProjectile;
			}
		}
	}

	public GameObject FireProjectile;
	public GameObject ElectricityProjectile;
	public GameObject IceProjectile;
	public GameObject WindProjectile;
	[SerializeField]
	private Element m_element = Element.Fire;
	public AudioSource AudioSource;
	public AudioClip Charged;

	private float power = 1;
	public float Power
	{
		get { return power; }
		set {
			if (value > PowerEnd) power = PowerEnd;
			else if (value < PowerStart) power = PowerStart;
			else power = value;
		}
	}
	public float PowerChargingSpeed = 3;
	public float PowerStart = 1;
	public float PowerEnd = 10;
	private GameObject ChargingProjectile;
	private bool Explosion = false;
	private VRTK_ControllerReference Controller;
	public float DurationHaptic = 0.1f;

	public override void Use()
	{
		base.Use();
		Power = PowerStart;
		Explosion = false;

		if (projectileSpawnPoint == null)
		{
			Debug.LogError("projectile Spawn is null");
			gameObject.SetActive(false);
		}

		ChargingProjectile = Instantiate(ActiveProjectile, projectileSpawnPoint.position, projectileSpawnPoint.rotation, transform);
	}

	public override void EndUse()
	{
		base.EndUse();
		ChargingProjectile.GetComponent<ElementProjectile>().Power = Power;
		ChargingProjectile.GetComponent<ElementProjectile>().Launch(projectileSpeed);
		ChargingProjectile = null;
	}

	private void FixedUpdate()
	{
		if (inUse)
		{
			ElementProjectile ep = ChargingProjectile.GetComponent<ElementProjectile>();

			if (Power<PowerEnd)
			{
				Power += PowerChargingSpeed * Time.fixedDeltaTime;
				if(Power >= ep.ExplosionPowerValue && !Explosion)
				{
					Explosion = true;
					AudioSource.clip = Charged;
					AudioSource.Play();
				}
				if(Power == PowerEnd)
				{
					ep.FullCharged();
				}
			}

			if (!VRTK_ControllerReference.IsValid(Controller))
				Controller = VRTK_ControllerReference.GetControllerReference(GetComponentInParent<VRTK_TrackedController>().gameObject);
			VRTK.VRTK_ControllerHaptics.TriggerHapticPulse(Controller, Power/50f, DurationHaptic, 0.01f);
			Debug.Log("haptci on " + Power / 10f);

			var ps = ChargingProjectile.GetComponentInChildren<ParticleSystem>();
			if (ep == null || ps == null) Debug.Break();
			float coef = (ep.FXMax - ep.FXMin) / (PowerEnd-PowerStart);
			float delta = ep.FXMin - (coef * PowerStart);
			float value = Power * coef + delta;
			switch (m_element)
			{
				case Element.Electricity:
					Debug.Log("value = " + value);
					var psShape = ps.shape;
					psShape.arcSpeed = value;
					break;
				case Element.Fire:
					var psEmission = ps.emission;
					psEmission.rateOverTime = value;
					break;
				case Element.Ice:
					var psEmissionIce = ps.emission;
					psEmissionIce.rateOverTime = value;
					break;
				case Element.Wind:
					//TODO triple tornado
					foreach (var psWind in ChargingProjectile.GetComponentsInChildren<ParticleSystem>())
					{
						var psShapeWind = psWind.shape;
						psShapeWind.arcSpeed = value;
					}
					break;
			}
		}
	}

	internal void ChangeElement(Element p_element)
	{
		m_element = p_element;
		Debug.Log("Element = " + m_element);
	}
}