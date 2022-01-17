using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Character;
using NetworkedPlayer;
using Photon.Pun;
using UnityEngine;

namespace Controls.Channeling
{
    public class Slenderman : MonoBehaviourPunCallbacks, IPunObservable
    {
        public GameObject innerChannelingParticleSystem;

        #region Private Fields

        private const float RecoveryTime = 10f;
        
        private bool hasBeenAcquired;

        private bool isVisible;

        private SkinnedMeshRenderer skinnedMeshRenderer;

        #endregion

        #region Public Fields

        public int NetworkID { get; set; }

        #endregion

        public void Start()
        {
            GameObject o = gameObject;
            NetworkID = o.GetInstanceID();
            hasBeenAcquired = false;
            isVisible = true;
            skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }

        public void Update()
        {
            if (isVisible == !hasBeenAcquired)
            {
                return;
            }

            if (hasBeenAcquired)
            {
                isVisible = false;
                skinnedMeshRenderer.enabled = false;
            }
            else
            {
                isVisible = true;
                skinnedMeshRenderer.enabled = true;
            }
        }
        
        public void OnMouseDown()
        {
            if (hasBeenAcquired)
            {
                return;
            }

            PlayerController channeler = PlayerController.LocalPlayerController;

            if (!channeler.HasPage)
            {
                return;
            }

            Debug.Log("Slenderman has been clicked by player from team " + channeler.Team);
            
            if (Vector3.Distance(transform.position, channeler.Position) > PlayerValues.SlendermanChannelRange)
            {
                return;
            }

            if (channeler.IsChannelingObjective)
            {
                return;
            }

            channeler.OnChannelObjective(transform.position);

            StartCoroutine(Channel(channeler));
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // Local player, send data
                stream.SendNext(this.NetworkID);
                stream.SendNext(this.hasBeenAcquired);
            }
            else
            {
                // Network player, receive data
                this.NetworkID = (int)stream.ReceiveNext();
                this.hasBeenAcquired = (bool)stream.ReceiveNext();
            }
        }

        private IEnumerator Channel(PlayerController channeler)
        {
            float progress = 0;
            float maxProgress = 100;
            float secondsToChannel = PlayerValues.SecondsToChannelSlenderman;

            while (progress <= maxProgress)
            {
                if (!channeler.IsChannelingObjective ||
                    Vector3.Distance(transform.position, channeler.Position) > PlayerValues.SlendermanChannelRange)
                {
                    // Disable channeling effects if player moves
                    innerChannelingParticleSystem.SetActive(false);
                    channeler.InterruptChanneling();
                    yield break;
                }

                progress += maxProgress / secondsToChannel;
                Debug.Log($"Slenderman being channeled, {progress} / {maxProgress}");

                // Enable channeling effects when channeling Slenderman on the channeler.
                // The channeling effect on Slenderman will be activated in the OnCollision.cs script
                // when the particles from the channeler hit Slenderman.
                channeler.transform.Find("InnerChannelingParticleSystem").gameObject.SetActive(true);
                ParticleSystem ps = channeler.transform.Find("InnerChannelingParticleSystem").gameObject.GetComponent<ParticleSystem>();


                // Set particles color
                float hSliderValueR = 209.0F;
                float hSliderValueG = 25.0F;
                float hSliderValueB = 191.0F;
                float hSliderValueA = 255.0F;

                ps.startColor = new Color(hSliderValueR / 255, hSliderValueG / 255, hSliderValueB / 255, hSliderValueA / 255);

                //var main = ps.main;
                //main.startColor = new Color(hSliderValueR, hSliderValueG, hSliderValueB, hSliderValueA);

                // Set the force that will change the particles direcion
                var fo = ps.forceOverLifetime;
                fo.enabled = true;

                fo.x = new ParticleSystem.MinMaxCurve(transform.position.x - channeler.transform.position.x);
                fo.y = new ParticleSystem.MinMaxCurve(-transform.position.y + channeler.transform.position.y);
                fo.z = new ParticleSystem.MinMaxCurve(transform.position.z - channeler.transform.position.z);

                yield return new WaitForSeconds(1);
            }

            if (!channeler.IsChannelingObjective)
            {
                yield break;
            }

            if (hasBeenAcquired || Vector3.Distance(transform.position, channeler.Position) > PlayerValues.SlendermanChannelRange)
            {
                // Disable channeling effects if player moves
                innerChannelingParticleSystem.SetActive(false);
                channeler.InterruptChanneling();                
                yield break;
            }

            hasBeenAcquired = true;
            channeler.SacrifisePage();
            channeler.OnReceiveSlendermanBuff();

            // Disable channeling effects after hiring Slenderman
            innerChannelingParticleSystem.SetActive(false);
            channeler.transform.Find("InnerChannelingParticleSystem").gameObject.SetActive(false);

            StartCoroutine(Recover());
        }

        private IEnumerator Recover()
        {
            Debug.Log($"Slenderman recovering");           
            yield return new WaitForSeconds(RecoveryTime);
            hasBeenAcquired = false;
            Debug.Log($"Slenderman has recovered");
        }
    }
}