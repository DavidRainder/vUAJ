using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
public class NotificationUI : MonoBehaviour
{
    public UnityEngine.UI.Image iconImage;
    public UnityEngine.UI.Image backgroundImage;
    public UnityEngine.UI.Image borderImage;

    private UnityEngine.UI.Image blackOverlay;
    private UnityEngine.UI.Image transparencyOverlay;
    private Coroutine fadeCoroutine;
    public GameObject glowPreferences;
    private Coroutine glowCoroutine;

    public TMP_Text messageText;
    public AudioSource audioSource;
    public RectTransform rectTransform;
    public enum HapticType { SoftPulse, StrongPulse, QuickBuzz }


    public float topCenterOffsetY = 5f;
    public float horizontalOffset = 5f;
    public float verticalOffset = 100f;

    private NotificationPosition position;
    private float durationSeconds;

    private NotificationManager manager;

    //REFERENCIAR AL GAMEMANAGER PROPIO PARA PAUSAR NOTIFICACIONES
    private Platformer.GameManager gameManager;  
    private void Awake()
    {
        //SI SE QUIERE QUE LAS NOTIFICACIONES (CUALQUIERA MENOS LA NORMAL) PAUSEN EL JUEGO, 
        //ES NECESARIO QUE LA LOGICA DE JUEGO TENGA UN GAMEMANAGER
        gameManager = Platformer.GameManager.Instance;
        manager = NotificationManager.Instance;
        if (manager != null)
        {
            blackOverlay = manager.blackOverlay.GetComponent<UnityEngine.UI.Image>();
            transparencyOverlay = manager.transparencyOverlay.GetComponent<UnityEngine.UI.Image>();
        }
    }
    public void SetUp(
        string text,
        Sprite icon,
        Color color,
        AudioClip sound,
        HapticFeedbackType hapticType,
        NotificationPosition position,
        NotificationSize size,
        NotificationStyle style,
        NotificationDuration duration
    )
    {
        //Texto
        if (text != null)
            messageText.text = text;

        //Icono
        // Icono
        if (icon != null)
        {
            iconImage.sprite = icon;
            iconImage.gameObject.SetActive(true);
            if (color == Color.white)
                iconImage.color = DarkenColor(color, 0.7f);

        }
        else
        {
            iconImage.gameObject.SetActive(false);

            // Expandir el texto si no hay icono
            var textRect = messageText.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0f, textRect.anchorMin.y); // expand to left
            textRect.offsetMin = new Vector2(20f, textRect.offsetMin.y); // optional padding
        }

        //Color
        backgroundImage.color = color;
        borderImage.color = DarkenColor(color, 0.7f);

        //Sonido
        if (sound != null)
        {
            audioSource.clip = sound;
            audioSource.Play();
        }

        if (hapticType != HapticFeedbackType.None)
        {
            PlayHapticFeedback(hapticType);
        }

        //Posici�n
        this.position = position;
        rectTransform.anchorMin = rectTransform.anchorMax = GetAnchorFromPosition(position);
        rectTransform.pivot = GetPivotFromPosition(position);

        //Tama�o
        rectTransform.localScale = GetScaleFromSize(size);

        // Estilo visual 
        ApplyStyle(style);

        //Duraci�n
        durationSeconds = GetDurationSeconds(duration);

        //Iniciar ciclo de vida
        StartCoroutine(PlayLifecycle(duration != NotificationDuration.Unlimited));
    }

    private void PlayHapticFeedback(HapticFeedbackType type)
    {
        if (Gamepad.current != null)
        {
            switch (type)
            {
                case HapticFeedbackType.Light:
                    Gamepad.current.SetMotorSpeeds(0.2f, 0.2f);
                    break;
                case HapticFeedbackType.Medium:
                    Gamepad.current.SetMotorSpeeds(0.5f, 0.5f);
                    break;
                case HapticFeedbackType.Heavy:
                    Gamepad.current.SetMotorSpeeds(1.0f, 1.0f);
                    break;
                case HapticFeedbackType.Pulse:
                    StartCoroutine(PulseHaptics());
                    break;
                case HapticFeedbackType.None:
                default:
                    Gamepad.current.SetMotorSpeeds(0, 0);
                    break;
            }
        }
    }

    private IEnumerator PulseHaptics()
    {
        if (Gamepad.current == null) yield break;
        for (int i = 0; i < 3; i++)
        {
            Gamepad.current.SetMotorSpeeds(1f, 1f);
            yield return new WaitForSeconds(0.1f);
            Gamepad.current.SetMotorSpeeds(0, 0);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private Vector2 GetPivotFromPosition(NotificationPosition pos)
    {
        return pos switch
        {
            NotificationPosition.TopCenter => new Vector2(0.5f, 1),
            NotificationPosition.TopLeft => new Vector2(0f, 1),
            NotificationPosition.TopRight => new Vector2(1f, 1),
            NotificationPosition.BottomLeft => new Vector2(0f, 0),
            NotificationPosition.BottomRight => new Vector2(1f, 0),
            _ => new Vector2(0.5f, 1)
        };
    }
    private Color DarkenColor(Color baseColor, float factor)
    {
        return new Color(baseColor.r * factor, baseColor.g * factor, baseColor.b * factor, baseColor.a);
    }
    private IEnumerator PlayLifecycle(bool autoDestroy)
    {
        yield return AnimateIn();

        if (autoDestroy)
        {
            yield return new WaitForSeconds(durationSeconds);
            yield return AnimateOut();
            yield return new WaitForSeconds(0.1f);
            Destroy(gameObject);
        }
        else
        {
            while (!Input.anyKeyDown) yield return null;
            yield return AnimateOut();
            Destroy(gameObject);
        }
    }

    private IEnumerator AnimateIn()
    {
        Vector2 startPos = GetInitialOffset(position);
        Vector2 endPos = GetOffset(position);
        float elapsed = 0f, duration = 0.3f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsed / duration);
            yield return null;
        }
        rectTransform.anchoredPosition = endPos;
    }

    private Vector2 GetInitialOffset(NotificationPosition pos)
    {
        float width = rectTransform.rect.width * rectTransform.localScale.x;
        float height = rectTransform.rect.height * rectTransform.localScale.y;

        float margin = 100f;

        return pos switch
        {
            NotificationPosition.TopCenter => new Vector2(0, height + margin), // entra desde arriba
            NotificationPosition.TopLeft => new Vector2(-(width + margin), -verticalOffset), // entra desde la izquierda
            NotificationPosition.TopRight => new Vector2(width + margin, -verticalOffset),   // entra desde la derecha
            NotificationPosition.BottomLeft => new Vector2(-(width + margin), verticalOffset),
            NotificationPosition.BottomRight => new Vector2(width + margin, verticalOffset),
            _ => new Vector2(0, height + margin)
        };
    }
    private IEnumerator AnimateOut()
    {
        Vector2 startPos = GetOffset(position);
        Vector2 endPos = GetInitialOffset(position);
        float elapsed = 0f, duration = 0.3f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsed / duration);
            ResetAllPreferences();
            yield return null;
        }
    }

    private Vector2 GetAnchorFromPosition(NotificationPosition pos)
    {
        return pos switch
        {
            NotificationPosition.TopCenter => new Vector2(0.5f, 1),
            NotificationPosition.TopLeft => new Vector2(0f, 1f),
            NotificationPosition.TopRight => new Vector2(1f, 1f),
            NotificationPosition.BottomLeft => new Vector2(0f, 0f),
            NotificationPosition.BottomRight => new Vector2(1f, 0f),
            _ => new Vector2(0.5f, 1f)
        };
    }
    private Vector2 GetOffset(NotificationPosition pos)
    {
        float x = horizontalOffset;
        float y = verticalOffset;

        return pos switch
        {
            NotificationPosition.TopCenter => new Vector2(0, -topCenterOffsetY),
            NotificationPosition.TopLeft => new Vector2(x, -y),
            NotificationPosition.TopRight => new Vector2(-x, -y),
            NotificationPosition.BottomLeft => new Vector2(x, y),
            NotificationPosition.BottomRight => new Vector2(-x, y),
            _ => new Vector2(0, -topCenterOffsetY)
        };
    }

    private Vector3 GetScaleFromSize(NotificationSize size)
    {
        return size switch
        {
            NotificationSize.Small => Vector3.one,
            NotificationSize.Medium => Vector3.one * 1.5f,
            NotificationSize.Large => Vector3.one * 2.0f,
            _ => Vector3.one
        };
    }

    private float GetDurationSeconds(NotificationDuration duration)
    {
        return duration switch
        {
            NotificationDuration.Seconds3 => 3f,
            NotificationDuration.Seconds5 => 5f,
            NotificationDuration.Seconds7 => 7f,
            NotificationDuration.Seconds10 => 10f,
            NotificationDuration.Seconds15 => 15f,
            NotificationDuration.Seconds20 => 20f,
            _ => 0f
        };
    }

    private void ApplyStyle(NotificationStyle style)
    {
        ResetAllPreferences();
        
        switch (style)
        {
            case NotificationStyle.Black:
                    if (blackOverlay != null)
                    {
                        gameManager.GameIsPaused = true;
                        StartCoroutine(FadeImage(blackOverlay, 0f, 1.0f, 0.3f));
                    }
                break;

            case NotificationStyle.Blur:
                    if (transparencyOverlay != null)
                    {
                        gameManager.GameIsPaused = true;
                        StartCoroutine(FadeImage(transparencyOverlay, 0f, 0.5f, 0.3f));
                    }
                break;

            case NotificationStyle.Glow:
                if (glowPreferences != null)
                {
                    glowPreferences.SetActive(true);
                    glowCoroutine = StartCoroutine(GlowPulse(glowPreferences, backgroundImage.color));
                }
                break;

            case NotificationStyle.Normal:
                break;
        }
    }


    private void ResetAllPreferences()
    {
        gameManager.GameIsPaused = false;
        if (blackOverlay != null)
            blackOverlay.gameObject.SetActive(false);

        if (transparencyOverlay != null)
            transparencyOverlay.gameObject.SetActive(false);

        if (glowPreferences != null)
            glowPreferences.SetActive(false);

        if (glowCoroutine != null)
            StopCoroutine(glowCoroutine);
    }

    private IEnumerator FadeImage(UnityEngine.UI.Image image, float fromAlpha, float toAlpha, float duration)
    {
        image.gameObject.SetActive(true);
        Color c = image.color;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsed / duration);
            image.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }
        image.color = new Color(c.r, c.g, c.b, toAlpha);
    }

    private IEnumerator GlowPulse(GameObject glowRoot, Color baseColor)
    {
        var children = glowRoot.GetComponentsInChildren<UnityEngine.UI.Image>();

        // Aumentamos saturaci�n y brillo
        Color glowColor = SaturateAndBrighten(baseColor, 1f, 1f);

        float t = 0f;

        while (true)
        {
            t += Time.deltaTime * 2.5f; // m�s r�pido
            float pulse = (Mathf.Sin(t) + 1f) / 2f; // de 0 a 1

            float alpha = Mathf.Lerp(0f, 0.8f, pulse); // m�s visible

            foreach (var img in children)
            {
                img.color = new Color(glowColor.r, glowColor.g, glowColor.b, alpha);
            }

            yield return null;
        }
    }

    private Color SaturateAndBrighten(Color color, float saturationBoost, float brightnessBoost)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        s = Mathf.Clamp01(s * saturationBoost);
        v = Mathf.Clamp01(v * brightnessBoost);
        return Color.HSVToRGB(h, s, v);
    }
}