using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer
{
    public class PlayerDisplay : MonoBehaviourPunCallbacks
    {
        [Header("HUD")]

        [SerializeField] private Slider healthSlider;
        [SerializeField] private Canvas playerCanvas; //The heads-up-display canvas

        [Header("Game Mode HUD")]

        [SerializeField] private TextMeshProUGUI objectiveText; //Displays the current gamemode objective
        [SerializeField] private TextMeshProUGUI timerText;

        [Header("Ability Display")]

        [SerializeField] private Slider[] abilitySliderArray;
        [SerializeField] private TextMeshProUGUI[] abilityCooldownTextArray;

        #region Properties

        public Slider HealthSlider
        {
            get { return healthSlider; }
        }

        public Canvas PlayerCanvas
        {
            get { return playerCanvas; }
        }

        #endregion

        public void UpdateEliminationDisplay(int eliminations, int maxEliminations)
        {
            objectiveText.text = "Eliminations Remaining: " + (maxEliminations - eliminations).ToString(); //Display the number of eliminations left until victory
        }

        [PunRPC]
        public void UpdateTimerDisplay(float timer) //Updates the timer in the format of MINUTES:SECONDS
        {
            string minutes = (Mathf.FloorToInt(timer / 60)).ToString("00");
            string seconds = (timer % 60).ToString("00");
            timerText.text = minutes + ":" + seconds;
        }

        public void UpdateCooldownDisplay(int abilityIndex, float maxCooldown, float cooldown) //Display the cooldown use the corresponding slider and text using the ability index
        {
            abilitySliderArray[abilityIndex].maxValue = maxCooldown;
            abilitySliderArray[abilityIndex].value = cooldown;

            if (cooldown > 0) //Display the ability cooldown text only if it is active
            {
                abilityCooldownTextArray[abilityIndex].text = cooldown.ToString("F0");
            }
            else //Once the cooldown reaches zero, 'hide' the text
            {
                abilityCooldownTextArray[abilityIndex].text = "";
            }
        }
    }
}

