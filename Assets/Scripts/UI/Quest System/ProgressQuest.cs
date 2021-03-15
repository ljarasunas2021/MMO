using UnityEngine;
using UnityEngine.UI;

namespace MMO.UI.Quests
{
    public class ProgressQuest : Quest
    {
        [Header("References")]
        [SerializeField] private Slider slider = null;

        // Sets slider progress to given value
        public void SetProgress(float val)
        {
            // Clamp val between 0 and 1 and set slider
            float clampedVal = Mathf.Clamp01(val);
            slider.value = val;
        }
    }
}
