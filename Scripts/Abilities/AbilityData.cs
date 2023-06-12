using RPG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class AbilityData : IAction
{
    GameObject user;
    Vector3 targetedPoint;
    IEnumerable<GameObject> targets;
    IEnumerable<Animation> animations;
    bool cancelled = false;

    public AbilityData(GameObject user)
    {
        this.user = user;
    }
    public Vector3 GetTargetedPoint()
    {
        return targetedPoint;
    }
    public void SetTargetedPoint(Vector3 newTargetedPoint)
    {
        targetedPoint = newTargetedPoint;
    }
    public void SetTargets(IEnumerable<GameObject> target)
    {
        this.targets = target.ToList();
    }

    public IEnumerable<GameObject> GetTargets()
    {
        return targets;
    }
    public IEnumerable<Animation> GetAnimations()
    {
        return animations;
    }

    public GameObject GetUser()
    {
        return user;
    }
    public void StartCoroutine(IEnumerator coroutine)
    {
        user.GetComponent<MonoBehaviour>().StartCoroutine(coroutine);
    }

    public void Cancel()
    {
        cancelled = true;
    }
    public bool IsCancelled()
    {
        return cancelled;
    }
}
