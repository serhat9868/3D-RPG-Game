//using Cinemachine;
using RPG.Attributes;
using RPG.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control
{
    public class Respawner : MonoBehaviour
    {
        [SerializeField] Transform respawnLocation;
        [SerializeField] float timeToWaitAfterDie = 1f;
        [SerializeField] float fadeInTime = 1f;
        [SerializeField] float fadeOutTime = 4f;
        [SerializeField] float enemyReGenHealthPercentage = 20f;
        void Awake()
        {
            GetComponent<Health>().onDie.AddListener(RespawnFunction);
        }

        void Start()
        {
            if (GetComponent<Health>().IsDead())
            {
                RespawnFunction();
            }
        }
        private void ResetEnemies()
        {
            foreach(AIController enemyController in FindObjectsOfType<AIController>())
            {
                Health enemyHealth = enemyController.GetComponent<Health>();
               
                if (enemyHealth && !enemyHealth.IsDead())
                {
                    enemyController.Reset();
                    enemyHealth.Heal(enemyHealth.GetMaxHealthPoints() * (enemyReGenHealthPercentage / 100));
                }
            }
        }

        private IEnumerator Respawn()
        {
            //Save file after dying
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();

            //Transition
            Fader fader = FindObjectOfType<Fader>();
            yield return new WaitForSeconds(timeToWaitAfterDie);
            yield return fader.FadeOut(fadeOutTime);
            ResetEnemies();
            Health health = GameObject.FindWithTag("Player").GetComponent<Health>();
            health.Heal(30);
            GetComponent<Animator>().Rebind();
            GetComponent<NavMeshAgent>().Warp(respawnLocation.position);
            health.SetIsDead(false);

            // Camera Configurations
            Vector3 positionDelta = respawnLocation.transform.position - transform.position;
            // ICinemachineCamera activeVirtualCamera = FindObjectOfType<CinemachineBrain>().ActiveVirtualCamera;
            // if(activeVirtualCamera.Follow == transform)
            // {
            //     activeVirtualCamera.OnTargetObjectWarped(transform, positionDelta);
            // }

            //Game starts in respawn position
            yield return new WaitForSeconds(3f); 
            savingWrapper.Save();
            yield return fader.FadeIn(fadeInTime);
       
        }
        void RespawnFunction()
        {
            StartCoroutine(Respawn());
        }
    }
}