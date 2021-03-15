using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MMO.Player;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace MMO.NPC
{
    [CustomEditor(typeof(NPCDialogue))]
    public class NPCDialogueEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            NPCDialogue NPCDialogue = (NPCDialogue)target;

            if (NPCDialogue.actionBeforeDialogue)
            {
                EditorGUILayout.LabelField("Action", EditorStyles.boldLabel);
                NPCDialogue.action = (Action1)EditorGUILayout.ObjectField("Action", NPCDialogue.action, typeof(Action1), true);
                if (NPCDialogue.action != null)
                {
                    NPCDialogue.actionBeforeDialogue = EditorGUILayout.Toggle("Action Before Dialogue", NPCDialogue.actionBeforeDialogue);
                }

                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField("Dialogue", EditorStyles.boldLabel);

            NPCDialogue.text = EditorGUILayout.TextField("Text", NPCDialogue.text);
            NPCDialogue.audioClip = (AudioClip)EditorGUILayout.ObjectField("Audio", NPCDialogue.audioClip, typeof(AudioClip), true);

            EditorGUILayout.Space();

            if (!NPCDialogue.actionBeforeDialogue)
            {
                EditorGUILayout.LabelField("Action", EditorStyles.boldLabel);
                NPCDialogue.action = (Action1)EditorGUILayout.ObjectField("Action", NPCDialogue.action, typeof(Action1), true);
                if (NPCDialogue.action != null)
                {
                    NPCDialogue.actionBeforeDialogue = EditorGUILayout.Toggle("Action Before Dialogue", NPCDialogue.actionBeforeDialogue);
                }

                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField("Next Dialogue", EditorStyles.boldLabel);

            NPCDialogue.nextDialogueIsNPC = EditorGUILayout.Toggle("NPC Speaks Next", NPCDialogue.nextDialogueIsNPC);

            if (NPCDialogue.nextDialogueIsNPC)
            {
                string name = "null";

                if (NPCDialogue.nextDialogue != null)
                {
                    name = NPCDialogue.nextDialogue.text;
                }

                NPCDialogue.nextDialogue = (NPCDialogue)EditorGUILayout.ObjectField("Next Dialogue: " + name, NPCDialogue.nextDialogue, typeof(NPCDialogue), true);
            }
            else
            {
                NPCDialogue.options = EditorGUILayout.Toggle("Player Has Options", NPCDialogue.options);

                if (NPCDialogue.options)
                {
                    int length = EditorGUILayout.IntField("Player Dialogue Options Length", NPCDialogue.playerDialogueOptions.Length);
                    if (NPCDialogue.playerDialogueOptions.Length != length)
                    {
                        PlayerDialogue[] copy = NPCDialogue.playerDialogueOptions;

                        NPCDialogue.playerDialogueOptions = new PlayerDialogue[length];

                        for (int i = 0; i < copy.Length && i < length; i++)
                        {
                            NPCDialogue.playerDialogueOptions[i] = copy[i];
                        }
                    }

                    for (int i = 0; i < length; i++)
                    {
                        string name = " ";

                        if (NPCDialogue.playerDialogueOptions[i] != null && NPCDialogue.playerDialogueOptions[i].button != null)
                        {
                            name = NPCDialogue.playerDialogueOptions[i].button.GetComponentInChildren<TextMeshProUGUI>().text;
                        }

                        NPCDialogue.playerDialogueOptions[i] = (PlayerDialogue)EditorGUILayout.ObjectField("      " + name, NPCDialogue.playerDialogueOptions[i], typeof(PlayerDialogue), true);
                        if (NPCDialogue.playerDialogueOptions[i] != null) NPCDialogue.playerDialogueOptions[i].options = true;
                    }

                    //NPCDialogue.playerDialogueOptions[0] = (PlayerDialogue[])EditorGUILayout.ObjectField("Player Dialogue Options", NPCDialogue.playerDialogueOptions, typeof(PlayerDialogue[]));
                }
                else
                {
                    string name = "null";

                    if (NPCDialogue.playerDialogue != null)
                    {
                        name = NPCDialogue.playerDialogue.text;
                    }

                    NPCDialogue.playerDialogue = (PlayerDialogue)EditorGUILayout.ObjectField("Player Dialogue: " + name, NPCDialogue.playerDialogue, typeof(PlayerDialogue), true);
                    if (NPCDialogue.playerDialogue != null) NPCDialogue.playerDialogue.options = false;
                }
            }

            EditorGUILayout.Space();

            // This makes the editor gui re-draw the inspector if values have changed
            if (GUI.changed) EditorUtility.SetDirty(target);
        }
    }
}
