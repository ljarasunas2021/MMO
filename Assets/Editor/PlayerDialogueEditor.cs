using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(PlayerDialogue))]
public class PlayerDialogueEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PlayerDialogue playerDialogue = (PlayerDialogue)target;

        if (playerDialogue.actionBeforeDialogue)
        {
            EditorGUILayout.LabelField("Action", EditorStyles.boldLabel);
            playerDialogue.action = (Action1)EditorGUILayout.ObjectField("Action", playerDialogue.action, typeof(Action1));
            if (playerDialogue.action != null)
            {
                playerDialogue.actionBeforeDialogue = EditorGUILayout.Toggle("Action Before Dialogue", playerDialogue.actionBeforeDialogue);
            }

            EditorGUILayout.Space();
        }

        EditorGUILayout.LabelField("Dialogue", EditorStyles.boldLabel);
        playerDialogue.options = EditorGUILayout.Toggle("Player Has Options", playerDialogue.options);
        if (playerDialogue.options)
        {
            playerDialogue.button = (Button)EditorGUILayout.ObjectField("Option Button", playerDialogue.button, typeof(Button));
        }
        else
        {
            playerDialogue.text = EditorGUILayout.TextField("Text", playerDialogue.text);
            playerDialogue.time = EditorGUILayout.FloatField("Time To Display Dialogue", playerDialogue.time);
        }

        EditorGUILayout.Space();

        if (!playerDialogue.actionBeforeDialogue)
        {
            EditorGUILayout.LabelField("Action", EditorStyles.boldLabel);
            playerDialogue.action = (Action1)EditorGUILayout.ObjectField("Action", playerDialogue.action, typeof(Action1));
            if (playerDialogue.action != null)
            {
                playerDialogue.actionBeforeDialogue = EditorGUILayout.Toggle("Action Before Dialogue", playerDialogue.actionBeforeDialogue);
            }

            EditorGUILayout.Space();
        }

        EditorGUILayout.LabelField("Next Dialogue", EditorStyles.boldLabel);

        playerDialogue.nextDialogueIsNPC = EditorGUILayout.Toggle("NPC Speaks Next", playerDialogue.nextDialogueIsNPC);

        if (playerDialogue.nextDialogueIsNPC)
        {
            string name = "null";

            if (playerDialogue.nextDialogue != null)
            {
                name = playerDialogue.nextDialogue.text;
            }

            playerDialogue.nextDialogue = (NPCDialogue)EditorGUILayout.ObjectField("Next Dialogue: " + name, playerDialogue.nextDialogue, typeof(NPCDialogue));
        }
        else
        {
            playerDialogue.nextOptions = EditorGUILayout.Toggle("Player Has Options", playerDialogue.nextOptions);

            if (playerDialogue.nextOptions)
            {
                EditorGUILayout.LabelField("Player Dialogue Options");

                int length = EditorGUILayout.IntField("Length", playerDialogue.playerDialogueOptions.Length);
                if (playerDialogue.playerDialogueOptions.Length != length)
                {
                    PlayerDialogue[] copy = playerDialogue.playerDialogueOptions;

                    playerDialogue.playerDialogueOptions = new PlayerDialogue[length];

                    for (int i = 0; i < copy.Length && i < length; i++)
                    {
                        playerDialogue.playerDialogueOptions[i] = copy[i];
                    }
                }

                for (int i = 0; i < length; i++)
                {
                    string name = " ";

                    if (playerDialogue.playerDialogueOptions[i] != null)
                    {
                        name = playerDialogue.playerDialogueOptions[i].button.GetComponentInChildren<Text>().text;
                    }

                    playerDialogue.playerDialogueOptions[i] = (PlayerDialogue)EditorGUILayout.ObjectField(name, playerDialogue.playerDialogueOptions[i], typeof(PlayerDialogue));
                    if (playerDialogue.playerDialogueOptions[i] != null) playerDialogue.playerDialogueOptions[i].nextOptions = true;
                }

                //NPCDialogue.playerDialogueOptions[0] = (PlayerDialogue[])EditorGUILayout.ObjectField("Player Dialogue Options", NPCDialogue.playerDialogueOptions, typeof(PlayerDialogue[]));
            }
            else
            {
                string name = "null";

                if (playerDialogue.playerDialogue != null)
                {
                    name = playerDialogue.playerDialogue.text;
                }

                playerDialogue.playerDialogue = (PlayerDialogue)EditorGUILayout.ObjectField("Player Dialogue: " + name, playerDialogue.playerDialogue, typeof(PlayerDialogue));
                if (playerDialogue.playerDialogue != null) playerDialogue.playerDialogue.nextOptions = false;
            }
        }

        EditorGUILayout.Space();

        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
