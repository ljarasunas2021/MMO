using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MMO.Player;
using MMO.UI;
using UnityEngine.Events;

namespace MMO.NPC
{
    public class DialogueScene : MonoBehaviour
    {
        public NPCInteract npcInteract;
        public Transform init;
        public Camera camera;

        public AnimInputAndTime[] startAnims;
        public AnimInputAndTime[] endAnims;

        private List<GameObject> players = new List<GameObject>();
        private Camera mainCam;

        private void Start()
        {
            mainCam = Camera.main;
        }

        public void StartDialogue(GameObject player)
        {
            Movement m = player.GetComponent<Movement>();
            if (m != null && m.isLocalPlayer && !players.Contains(player))
            {
                StartCoroutine(StartInteraction(player, m));
            }                
        }

        public IEnumerator StartInteraction(GameObject player, Movement m)
        {            
            players.Add(player);

            UIManager.instance.canMove = false;
            m.ResetMovement();

            camera.gameObject.SetActive(true);
            mainCam.gameObject.SetActive(false);

            CharacterController cc = player.GetComponent<CharacterController>();

            cc.enabled = false;
            player.transform.position = init.position;
            player.transform.rotation = init.rotation;
            cc.enabled = true;

            Animator anim = player.GetComponent<Animator>();

            foreach (AnimInputAndTime att in startAnims)
            {
                float time = 0;
                while (time < att.time)
                {
                    yield return new WaitForEndOfFrame();

                    m.Move(att.input, true);
                    time += Time.deltaTime;

                    yield return 0;
                }                
            }

            yield return new WaitForEndOfFrame();

            yield return StartCoroutine(npcInteract.Interact());

            foreach (AnimInputAndTime att in endAnims)
            {
                float time = 0;
                while (time < att.time)
                {
                    yield return new WaitForEndOfFrame();

                    m.Move(att.input, true);
                    time += Time.deltaTime;

                    yield return 0;
                }
            }

            mainCam.gameObject.SetActive(true);
            camera.gameObject.SetActive(false);

            UIManager.instance.canMove = true;
            players.Remove(player);
        }
    }

    [System.Serializable]
    public class AnimInputAndTime
    {
        public Vector2 input;
        public float time;

        public AnimInputAndTime(Vector2 input, float time)
        {
            this.input = input;
            this.time = time;
        }
    }
}

