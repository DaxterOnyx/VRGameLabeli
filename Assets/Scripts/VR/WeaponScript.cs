﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

/// <summary>
/// script mère de toutes les armes du joueur VR
/// @author Brice
/// </summary>
public abstract class WeaponScript : Tool
{
    public float projectileSpeed = 100f;
	public Transform projectileSpawnPoint;
	public GameObject ActiveProjectile;

	public virtual void Shoot()
	{
		if (ActiveProjectile != null && projectileSpawnPoint != null)
		{
			GameObject clonedProjectile = Instantiate(ActiveProjectile, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
			Rigidbody projectileRigidbody = clonedProjectile.GetComponent<Rigidbody>();
			if (projectileRigidbody != null)
			{
				projectileRigidbody.AddForce(clonedProjectile.transform.forward * projectileSpeed,ForceMode.VelocityChange);
			}
		}
	}
}