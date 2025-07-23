using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace F
{
    public class KarmaController : MonoBehaviour
    {
        [SerializeField] private Slider positiveSlider;
        [SerializeField] private Slider negativeSlider;
        [SerializeField] private TMP_Text karmaPointsText;
        [SerializeField] private TMP_Text minKarmaText;
        [SerializeField] private TMP_Text maxKarmaText;

        private float lerpSpeed = 5f;

        private void Start()
        {
            CombatStateStatus.KarmaChangedEvent += OnKarmaChanged;
            CombatStateStatus.MinKarmaChangedEvent += OnMinKarmaChanged;
            CombatStateStatus.MaxKarmaChangedEvent += OnMaxKarmaChanged;

            UpdateKarmaDisplay(CombatStateStatus.Karma);
            UpdateKarmaBoundsDisplay();
        }

        private void OnDestroy()
        {
            CombatStateStatus.KarmaChangedEvent -= OnKarmaChanged;
            CombatStateStatus.MinKarmaChangedEvent -= OnMinKarmaChanged;
            CombatStateStatus.MaxKarmaChangedEvent -= OnMaxKarmaChanged;
        }

        private void Update()
        {
            if (positiveSlider.value != GetPositiveSliderValue())
            {
                positiveSlider.value = Mathf.Lerp(positiveSlider.value, GetPositiveSliderValue(), lerpSpeed * Time.deltaTime);
            }

            if (negativeSlider.value != GetNegativeSliderValue())
            {
                negativeSlider.value = Mathf.Lerp(negativeSlider.value, GetNegativeSliderValue(), lerpSpeed * Time.deltaTime);
            }
        }

        private void OnKarmaChanged(object sender, int newKarma)
        {
            UpdateKarmaDisplay(newKarma);
        }

        private void OnMinKarmaChanged(object sender, int newMinKarma)
        {
            UpdateKarmaBoundsDisplay();
        }

        private void OnMaxKarmaChanged(object sender, int newMaxKarma)
        {
            UpdateKarmaBoundsDisplay();
        }

        private void UpdateKarmaDisplay(int karma)
        {
            karmaPointsText.text = karma.ToString();
        }

        private float GetPositiveSliderValue()
        {
            return CombatStateStatus.Karma >= 0 ? (float)CombatStateStatus.Karma / CombatStateStatus.MaxKarma : 0;
        }

        private float GetNegativeSliderValue()
        {
            return CombatStateStatus.Karma < 0 ? (float)-CombatStateStatus.Karma / -CombatStateStatus.MinKarma : 0;
        }

        private void UpdateKarmaBoundsDisplay()
        {
            minKarmaText.text = CombatStateStatus.MinKarma.ToString();
            maxKarmaText.text = CombatStateStatus.MaxKarma.ToString();
        }
    }
}
