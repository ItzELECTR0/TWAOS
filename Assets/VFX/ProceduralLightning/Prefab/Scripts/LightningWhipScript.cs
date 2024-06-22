//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;
using System.Collections;

namespace DigitalRuby.ThunderAndLightning
{
    /// <summary>
    /// Lightning whip script
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class LightningWhipScript : MonoBehaviour
    {
        /// <summary>
        /// Whip crack sound
        /// </summary>
        public AudioClip WhipCrack;

        /// <summary>
        /// Whip crack thunder sound
        /// </summary>
        public AudioClip WhipCrackThunder;

        private AudioSource audioSource;
        private GameObject whipStart;
        private GameObject whipEndStrike;
        private GameObject whipHandle;
        private GameObject whipSpring;
        private Vector2 prevDrag;
        private bool dragging;
        private bool canWhip = true;

        private IEnumerator WhipForward()
        {
            if (canWhip)
            {
                // first turn off whip, like a cooldown
                canWhip = false;

                // remove the drag from all objects so they can move rapidly without decay
                for (int i = 0; i < whipStart.transform.childCount; i++)
                {
                    GameObject obj = whipStart.transform.GetChild(i).gameObject;
                    Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.drag = 0.0f;
                    }
                }

                // play the whip whoosh and crack sound
                audioSource.PlayOneShot(WhipCrack);

                // enable the spring and put it behind the whip to yank it back
                whipSpring.GetComponent<SpringJoint2D>().enabled = true;
                whipSpring.GetComponent<Rigidbody2D>().position = whipHandle.GetComponent<Rigidbody2D>().position + new Vector2(-15.0f, 5.0f);

                // wait a bit
                yield return WaitForSecondsLightning.WaitForSecondsLightningPooled(0.2f);

                // now put the spring in front of the whip to pull it forward
                whipSpring.GetComponent<Rigidbody2D>().position = whipHandle.GetComponent<Rigidbody2D>().position + new Vector2(15.0f, 2.5f);
                yield return WaitForSecondsLightning.WaitForSecondsLightningPooled(0.15f);
                audioSource.PlayOneShot(WhipCrackThunder, 0.5f);

                // wait a bit
                yield return WaitForSecondsLightning.WaitForSecondsLightningPooled(0.15f);

                // show the strike paticle system
                whipEndStrike.GetComponent<ParticleSystem>().Play();

                // turn off the spring
                whipSpring.GetComponent<SpringJoint2D>().enabled = false;

                // wait a bit longer for the whip to recoil
                yield return WaitForSecondsLightning.WaitForSecondsLightningPooled(0.65f);

                // put the drag back on
                for (int i = 0; i < whipStart.transform.childCount; i++)
                {
                    GameObject obj = whipStart.transform.GetChild(i).gameObject;
                    Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.velocity = Vector2.zero;
                        rb.drag = 0.5f;
                    }
                }

                // now they can whip again
                canWhip = true;
            }
        }

        private void Start()
        {
            whipStart = GameObject.Find("WhipStart");
            whipEndStrike = GameObject.Find("WhipEndStrike");
            whipHandle = GameObject.Find("WhipHandle");
            whipSpring = GameObject.Find("WhipSpring");
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (!dragging && Input.GetMouseButtonDown(0))
            {
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D hit = Physics2D.OverlapPoint(worldPos);
                if (hit != null && hit.gameObject == whipHandle)
                {
                    dragging = true;
                    prevDrag = worldPos;
                }
            }
            else if (dragging && Input.GetMouseButton(0))
            {
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 offset = worldPos - prevDrag;
                Rigidbody2D rb = whipHandle.GetComponent<Rigidbody2D>();
                rb.MovePosition(rb.position + offset);
                prevDrag = worldPos;
            }
            else
            {
                dragging = false;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(WhipForward());
            }
        }
    }
}