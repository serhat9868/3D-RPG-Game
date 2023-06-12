using RPG.Abilities;
using RPG.Attributes;
using RPG.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Projectile Effect", menuName = "Ability/Effects/Spawn Projectile Effect", order = 0)]
public class SpawnProjectileEffect : EffectStrategy
{
    [SerializeField] Projectile projectileToSpawn;
    [SerializeField] float damage = 5f;
    [SerializeField] bool isRightHand = true;
    [SerializeField] bool useTargetPoint = true;
    public override void StartEffect(AbilityData data, Action finished)
    {
        Fighter fighter = data.GetUser().GetComponent<Fighter>();
        Vector3 spawnPosition = fighter.GetHandTransform(isRightHand).position;
        if (useTargetPoint)
        {
            SpawnProjectileForTargetPoint(data, spawnPosition);
        }
        else
        {
            SpawnProjectilesForTargets(data,spawnPosition);
        }
            finished();
    }

    private void SpawnProjectileForTargetPoint(AbilityData data, Vector3 spawnPosition)
    {
        Projectile projectile = Instantiate(projectileToSpawn);
        projectile.transform.position = spawnPosition;
        projectile.SetTarget(data.GetTargetedPoint(), data.GetUser(), damage);
    }

    private void SpawnProjectilesForTargets(AbilityData data,Vector3 spawnPosition)
    {
        foreach (var target in data.GetTargets())
        {
            Health health = target.GetComponent<Health>();
            if (health)
            {
                Projectile projectile = Instantiate(projectileToSpawn);
                projectile.transform.position = spawnPosition;
                projectile.SetTarget(health, data.GetUser(), damage);
            }
            }
    }
}
