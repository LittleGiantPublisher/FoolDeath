using System;
using CameraShake;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using F;

namespace F.UI
{
	public class OptionsSetter : MonoBehaviour
	{
		private void Start()
		{
			this.AM = AudioManager.Instance;
			this.Refresh();
		}

		public void Refresh()
		{
			this.SetBGM(this.AM.MusicVolume);
			this.SetSFX(this.AM.SFXVolume);
			this._resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
			bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
			this.SetFullscreen(fullscreen);
			this.SetCameraShake(PlayerPrefs.GetInt("CameraShake", 1) == 1);
		}

		public void OnClickBGMVolume()
		{
			float num = this.AM.MusicVolume;
			num += 0.1f;
			if (num > 1.01f)
			{
				num = 0f;
			}
			this.SetBGM(num);
		}

		public void OnClickSFXVolume()
		{
			float num = this.AM.SFXVolume;
			num += 0.1f;
			if (num > 1.01f)
			{
				num = 0f;
			}
			this.SetSFX(num);
		}

		public void OnClickFullscreen()
		{
			this.SetFullscreen(!Screen.fullScreen);
		}

		public void OnClickResolution()
		{
			this._resolutionIndex++;
			if (this._resolutionIndex >= this.SupportedResolutions.Length)
			{
				this._resolutionIndex = 0;
			}
			PlayerPrefs.SetInt("ResolutionIndex", this._resolutionIndex);
			this.SetResolution(this._resolutionIndex);
		}

		public void OnClickCameraShake()
		{
			this.SetCameraShake(!CameraShaker.ShakeOn);
		}

		private void SetBGM(float volume)
		{
			SetTextPairs( bgmLabel, bgmTMP);
            this.bgmResultTMP.text = string.Format(" {0:P0}.", volume);
            this.AM.MusicVolume = volume;
		}

		private void SetSFX(float volume)
		{
            SetTextPairs( sfxLabel, sfxTMP);
            this.sfxResultTMP.text = string.Format(" {0:P0}.", volume);
			this.AM.SFXVolume = volume;
		}

		private void SetFullscreen(bool isFS)
		{
            SetTextPairs( fullscreenLabel, fullscreenTMP);
			if (isFS)
			{
                SetTextPairs( onString, onOffResolutionTMP);
			}
			else
			{
                SetTextPairs( offString, onOffResolutionTMP);
			}

			if (isFS)
			{
				this.DisableResolution();
				Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
			}
			else
			{
				this.resolutionButton.interactable = true;
				this.SetResolution(this._resolutionIndex);
			}
			PlayerPrefs.SetInt("Fullscreen", isFS ? 1 : 0);
		}

		private void SetResolution(int resolutionIndex)
		{
			Vector2Int vector2Int = this.SupportedResolutions[resolutionIndex];
			Screen.SetResolution(vector2Int.x, vector2Int.y, false);
			SetTextPairs( resolutionLabel, resolutionTMP);
            string text = "";
			text = string.Concat(new object[]
			{
				text,
				vector2Int.x,
				"x",
				vector2Int.y
			});
			this.resolutionResultTMP.text = text;
		}

		private void DisableResolution()
		{
			this.resolutionButton.interactable = false;
            SetTextPairs( resolutionLabel, resolutionTMP);

			this.resolutionResultTMP.text = "-";
		}

		private void SetCameraShake(bool isOn)
		{
			if (isOn)
			{
				SetTextPairs( onString, onOffCameraTMP);
			}
			else
			{
				SetTextPairs( offString, onOffCameraTMP);
			}
			SetTextPairs( cameraShakeLabel, cameraShakeTMP);
			CameraShaker.ShakeOn = isOn;
			PlayerPrefs.SetInt("CameraShake", isOn ? 1 : 0);
		}

        private void SetTextPairs( string text, TMP_Text tmpText){
                if (tmpText != null ){
					LocalizationSystem.RegisterComponent(tmpText, () => LocalizationSystem.GetLocalizedValue(text));
                }
                else
                {
                    Debug.LogWarning("TMP_Text component is missing for a StringTMPTextPair.");
                }
            
        }

		public static bool AutoReloadEnabled;

		[SerializeField]
		private string bgmLabel;

		[SerializeField]
		private string sfxLabel;

		[SerializeField]
		private string fullscreenLabel;

		[SerializeField]
		private string resolutionLabel;

		[SerializeField]
		private string cameraShakeLabel;

		[SerializeField]
		private string onString;

		[SerializeField]
		private string offString;

		[SerializeField]
		private TMP_Text bgmTMP;

		[SerializeField]
		private TMP_Text sfxTMP;

		[SerializeField]
		private TMP_Text fullscreenTMP;

		[SerializeField]
		private TMP_Text resolutionTMP;

        [SerializeField]
		private TMP_Text resolutionResultTMP;

		[SerializeField]
		private TMP_Text cameraShakeTMP;

        [SerializeField]
        private TMP_Text onOffCameraTMP;

        [SerializeField]
        private TMP_Text onOffResolutionTMP;

        [SerializeField]
        private TMP_Text sfxResultTMP;

        [SerializeField]
        private TMP_Text bgmResultTMP;

		[SerializeField]
		private Button resolutionButton;

		// Token: 0x04000899 RID: 2201
		private Vector2Int[] SupportedResolutions = new Vector2Int[]
		{
			new Vector2Int(800, 450),
			new Vector2Int(1200, 675),
			new Vector2Int(1600, 900),
			new Vector2Int(1920, 1080)
		};

		// Token: 0x0400089A RID: 2202
		private int _resolutionIndex;

		// Token: 0x0400089B RID: 2203
		private AudioManager AM;
	}
}
