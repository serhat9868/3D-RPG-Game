using RPG.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "DelayedClickTargeting", menuName = "Ability/Targeting/New Delayed Click Targeting", order = 0)]
    public class DelayedClickTargeting : TargetingStrategy
    {
        [SerializeField] Texture2D texture2D;
        [SerializeField] Vector2 hotspot;
        [SerializeField] LayerMask layerMask;
        [SerializeField] float areaEffectRadius;
        [SerializeField] Transform summonCircle;

        Transform targetingPrefabInstance = null;
        public override void StartTargeting(AbilityData data, Action finished)
        {
            PlayerController playerController = data.GetUser().GetComponent<PlayerController>();
            playerController.StartCoroutine(Targeting(data, playerController,finished));
        }
        
        public IEnumerator Targeting(AbilityData data,PlayerController playerController, Action finished)
        {
            playerController.enabled = false;
            if(targetingPrefabInstance == null)
            {
                targetingPrefabInstance = Instantiate(summonCircle);
            }
            else
            {
                targetingPrefabInstance.gameObject.SetActive(true);
            }
        
            targetingPrefabInstance.localScale = new Vector3(areaEffectRadius * 2, 1, areaEffectRadius * 2);
            while (!data.IsCancelled())
            {
                Cursor.SetCursor(texture2D, hotspot, CursorMode.Auto);
                RaycastHit raycastHit;
                if (Physics.Raycast(PlayerController.GetMouseRay(), out raycastHit, 1000, layerMask))
                {
                    targetingPrefabInstance.position = raycastHit.point;

                    if (Input.GetMouseButtonDown(0))
                    {
                        //Absorb the whole mouse click
                        yield return new WaitWhile(() => Input.GetMouseButton(0));
                        playerController.enabled = true;
                        targetingPrefabInstance.gameObject.SetActive(false);
                        data.SetTargetedPoint(raycastHit.point);
                        data.SetTargets(GetGameObjectsInRadius(raycastHit.point));
                        finished();
                        break;
                    }
                }
                    yield return null;
            }
            targetingPrefabInstance.gameObject.SetActive(false);
            playerController.enabled = true;
            finished();
        }
        private IEnumerable<GameObject> GetGameObjectsInRadius(Vector3 point)
        {          
                RaycastHit[] hits = Physics.SphereCastAll(point, areaEffectRadius, Vector3.up, 0);
                foreach(RaycastHit hit in hits)
                {
                    yield return hit.collider.gameObject;
                }   
        }
    }
}
