using UnityEngine;
using System.Collections;

public class SoundControls : MonoBehaviour {

	public void ToggleMusic()
    {
        // toggle music/sound only when user click on the sound/music button,
        // so we wait for a time to avoid calling the toggle function when we set it manually from PlayerPrefs
        if (Time.time > .3f)     
		    SoundManager.Instance.ToggleMuteMusic();
	}

	public void ToggleSoundEffect()
    {
        if (Time.time > .3f)
		    SoundManager.Instance.ToggleMuteSoundEffects();
	}
}
