using TMPro;
using UnityEngine;

public class SettingFunctions : MonoBehaviour
{
    public void setTTSMode()
    {
        TTSManager.Instance.TTSActivation();
    }

    public void setShowColliders(System.Boolean show)
    {
        VolumePerceptionManager.Instance.setShowColliders(show);
    }

    public void setDislexiaMode(System.Boolean dislexiaMode)
    {
        TextAccesibilityManager.Instance.setDislexiaMode(dislexiaMode);
    }

    public void onFontSizeChanged(System.Single value)
    {
        TextAccesibilityManager.Instance.onFontSizeChanged(value);
    }

    public void onFontChanged(TMP_FontAsset font)
    {
        TextAccesibilityManager.Instance.onFontChanged(font);
    }
}
