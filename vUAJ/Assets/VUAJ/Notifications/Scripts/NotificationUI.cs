using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

/// <summary>
/// Controlador visual de una notificación individual.
/// 
/// Se encarga de mostrar el mensaje, ícono, color, sonido, vibración y estilo visual asociado (blur, black, glow),
/// así como su posición, duración y animaciones de entrada/salida.
/// 
/// La notificación puede influir en el estado del juego (ej. pausarlo) dependiendo del estilo configurado.
/// Para ello el desarrollador debe usar el Script base de GameManager proporcionado y tener en cuenta que 
/// cualquier script asociado a una entidad en escena debe comprobar si el GameManager no esta pausado antes
/// de continuar con el Update() o FixedUpdate()
/// 
/// También puede usar haptics si se dispone de un Gamepad.
/// </summary>
[RequireComponent(typeof(TTSNotifications))]
public class NotificationUI : MonoBehaviour
{
    // Referencias de UI
    public UnityEngine.UI.Image iconImage;
    public UnityEngine.UI.Image backgroundImage;
    public UnityEngine.UI.Image borderImage;
    public TMP_Text messageText;
    public AudioSource audioSource;
    public RectTransform rectTransform;

    // Overlays visuales 
    private UnityEngine.UI.Image blackOverlay;
    private UnityEngine.UI.Image transparencyOverlay;

    // Estilo Glow
    public GameObject glowPreferences;
    private Coroutine glowCoroutine;

    // Desplazamiento configurables 
    public float topCenterOffsetY = 5f;
    public float horizontalOffset = 5f;
    public float verticalOffset = 100f;

    // Posición y duración calculada
    private NotificationPosition position;
    private float durationSeconds;

    // Referencias del Manager
    private NotificationManager manager;
    private vUAJBaseGameManager gameManager;
    private TTSNotifications ttsNotifications;
    private void Awake()
    {
        gameManager = vUAJBaseGameManager.Instance;
        manager = NotificationManager.Instance;
        ttsNotifications = GetComponent<TTSNotifications>();

        if (manager != null)
        {
            blackOverlay = manager.blackOverlay.GetComponent<UnityEngine.UI.Image>();
            transparencyOverlay = manager.transparencyOverlay.GetComponent<UnityEngine.UI.Image>();
        }
    }

    /// <summary>
    /// Configura y despliega la notificación según los parámetros recibidos.
    /// </summary>
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
        // Texto
        if (text != null)
        {
            messageText.text = text;
            ttsNotifications.SayNotification(messageText.text);
        }

        // Icono
        if (icon != null)
        {
            iconImage.sprite = icon;
            iconImage.gameObject.SetActive(true);

            // Oscurecer ligeramente si el ícono es blanco
            if (color == Color.white)
                iconImage.color = DarkenColor(color, 0.7f);

        }
        else
        {
            iconImage.gameObject.SetActive(false);

            // Expandir texto horizontalmente si no hay ícono
            var textRect = messageText.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0f, textRect.anchorMin.y); 
            textRect.offsetMin = new Vector2(20f, textRect.offsetMin.y); 
        }

        // Color y borde
        backgroundImage.color = color;
        borderImage.color = DarkenColor(color, 0.7f);

        // Sonido
        if (sound != null)
        {
            audioSource.clip = sound;
            audioSource.Play();
        }

        // Haptics
        if (hapticType != HapticFeedbackType.None)
        {
            PlayHapticFeedback(hapticType);
        }

        // Posición y escala
        this.position = position;
        rectTransform.anchorMin = rectTransform.anchorMax = GetAnchorFromPosition(position);
        rectTransform.pivot = GetPivotFromPosition(position);
        rectTransform.localScale = GetScaleFromSize(size);

        // Estilo visual 
        ApplyStyle(style);

        // Duración y ciclo de vida
        durationSeconds = GetDurationSeconds(duration);
        StartCoroutine(PlayLifecycle(duration != NotificationDuration.Unlimited));
    }

    /// <summary>
    /// Ejecuta retroalimentación háptica (vibración) en el gamepad actual según el tipo especificado.
    /// Diferentes tipos de vibración permiten distintos niveles de intensidad o patrones personalizados.
    /// </summary>
    private void PlayHapticFeedback(HapticFeedbackType type)
    {
        if (Gamepad.current != null)
        {
            switch (type)
            {
                case HapticFeedbackType.Light:
                    Gamepad.current.SetMotorSpeeds(0.2f, 0.2f); // vibración suave
                    break;
                case HapticFeedbackType.Medium:
                    Gamepad.current.SetMotorSpeeds(0.5f, 0.5f); // vibración media
                    break;
                case HapticFeedbackType.Heavy:
                    Gamepad.current.SetMotorSpeeds(1.0f, 1.0f); // vibración fuerte
                    break;
                case HapticFeedbackType.Pulse:
                    StartCoroutine(PulseHaptics()); // vibración pulsante
                    break;
                case HapticFeedbackType.None:
                default:
                    Gamepad.current.SetMotorSpeeds(0, 0); // detener vibración
                    break;
            }
        }
    }

    /// <summary>
    /// Ejecuta una vibración pulsante en el gamepad actual.
    /// Consiste en tres pulsos cortos de vibración fuerte con pausas breves entre ellos.
    /// </summary>
    private IEnumerator PulseHaptics()
    {
        // Si no hay gamepad conectado, salir de la corrutina.
        if (Gamepad.current == null) yield break;

        // Repetir tres pulsos de vibración.
        for (int i = 0; i < 3; i++)
        {
            Gamepad.current.SetMotorSpeeds(1f, 1f); // Activar vibración fuerte
            yield return new WaitForSeconds(0.1f);  // Esperar 0.1 segundos
            Gamepad.current.SetMotorSpeeds(0, 0);   // Detener vibración
            yield return new WaitForSeconds(0.1f);  // Esperar antes del siguiente pulso
        }
    }

    /// <summary>
    /// Devuelve el punto de pivote adecuado para la notificación, 
    /// según su posición en pantalla.
    /// </summary>
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

    /// <summary>
    /// Oscurece un color base multiplicando sus componentes RGB por un factor.
    /// El valor alfa se mantiene sin cambios.
    /// </summary>
    private Color DarkenColor(Color baseColor, float factor)
    {
        return new Color(baseColor.r * factor, baseColor.g * factor, baseColor.b * factor, baseColor.a);
    }

    /// <summary>
    /// Maneja el ciclo de vida de la notificación, incluyendo su animación de entrada, 
    /// tiempo de espera y animación de salida. Se destruye al finalizar.
    /// </summary>
    private IEnumerator PlayLifecycle(bool autoDestroy)
    {
        yield return AnimateIn();

        if (autoDestroy)
        {
            yield return new WaitForSeconds(durationSeconds); // Espera tiempo definido
            yield return AnimateOut();                        // Animación de salida
            yield return new WaitForSeconds(0.1f);            // Breve pausa
            Destroy(gameObject);                              // Eliminar la notificación
        }
        else
        {
            // Espera hasta que el usuario presione una tecla
            while (!Keyboard.current.escapeKey.wasPressedThisFrame) yield return null;
            yield return AnimateOut();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Ejecuta la animación de entrada de la notificación.
    /// La notificación se desplaza desde una posición inicial (fuera de pantalla) hasta su posición final.
    /// </summary>
    private IEnumerator AnimateIn()
    {
        // Posición inicial fuera de pantalla según la posición configurada
        Vector2 startPos = GetInitialOffset(position);

        // Posición final visible
        Vector2 endPos = GetOffset(position);

        float elapsed = 0f;
        float duration = 0.3f; // Duración de la animación

        // Lerp lineal entre la posición inicial y final 
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsed / duration);
            yield return null; // Esperar al siguiente frame
        }

        // Asegura que quede exactamente en la posición final
        rectTransform.anchoredPosition = endPos;
    }

    /// <summary>
    /// Calcula la posición inicial de entrada de la notificación (fuera de la pantalla),
    /// en función de su posición final configurada.
    /// Se usa para la animación de entrada.
    /// </summary>
    private Vector2 GetInitialOffset(NotificationPosition pos)
    {
        // Dimensiones de la notificación en pantalla
        float width = rectTransform.rect.width * rectTransform.localScale.x;
        float height = rectTransform.rect.height * rectTransform.localScale.y;

        float margin = 100f; // Margen adicional fuera de pantalla

        // Devuelve el punto de inicio fuera de la pantalla según la posición de anclaje
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

    /// <summary>
    /// Ejecuta la animación de salida de la notificación.
    /// La notificación se desplaza desde su posición visible hacia fuera de la pantalla.
    /// </summary>
    private IEnumerator AnimateOut()
    {
        // Posición actual en pantalla
        Vector2 startPos = GetOffset(position);

        // Posición de salida fuera de la pantalla (igual que al entrar pero en sentido inverso)
        Vector2 endPos = GetInitialOffset(position);

        float elapsed = 0f;
        float duration = 0.3f; // Duración de la animación de salida

        // Animación de interpolación (desplazamiento suave) entre posición visible y oculta
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsed / duration);

            // Restablece estilos visuales como blur, black o glow mientras se va ocultando
            ResetAllPreferences();
            yield return null;
        }
    }

    /// <summary>
    /// Devuelve el punto de anclaje del RectTransform en función de la posición de la notificación.
    /// Esto define desde qué parte de la pantalla se posicionará (esquina, centro, etc).
    /// </summary>
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

    /// <summary>
    /// Calcula el desplazamiento en píxeles desde el ancla según la posición de la notificación.
    /// Este offset define la separación visual respecto al borde o centro de la pantalla.
    /// </summary>
    private Vector2 GetOffset(NotificationPosition pos)
    {
        float x = horizontalOffset; // Separación horizontal configurable
        float y = verticalOffset;   // Separación vertical configurable

        return pos switch
        {
            NotificationPosition.TopCenter => new Vector2(0, -topCenterOffsetY), // Centrado arriba, pequeño desplazamiento vertical
            NotificationPosition.TopLeft => new Vector2(x, -y), // Arriba izquierda
            NotificationPosition.TopRight => new Vector2(-x, -y),  // Arriba derecha
            NotificationPosition.BottomLeft => new Vector2(x, y),  // Abajo izquierda
            NotificationPosition.BottomRight => new Vector2(-x, y), // Abajo derecha
            _ => new Vector2(0, -topCenterOffsetY)  // Valor por defecto (TopCenter)
        };
    }

    /// <summary>
    /// Devuelve la escala del RectTransform en función del tamaño de la notificación.
    /// Permite aumentar o reducir visualmente el tamaño de la UI sin cambiar su contenido.
    /// </summary>
    private Vector3 GetScaleFromSize(NotificationSize size)
    {
        return size switch
        {
            NotificationSize.Small => Vector3.one, // Escala normal (1x)
            NotificationSize.Medium => Vector3.one * 1.5f,  // 1.5x más grande
            NotificationSize.Large => Vector3.one * 2.0f, // 2x más grande
            _ => Vector3.one // Por defecto, escala 1x
        };
    }

    /// <summary>
    /// Devuelve la duración en segundos correspondiente al valor enumerado de duración de la notificación.
    /// </summary>
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

    /// <summary>
    /// Aplica el estilo visual seleccionado a la notificación.
    /// Esto puede implicar pausar el juego, mostrar overlays, o activar efectos de brillo.
    /// </summary>
    private void ApplyStyle(NotificationStyle style)
    {
        // Reinicia cualquier estado visual anterior antes de aplicar uno nuevo
        ResetAllPreferences();
        
        switch (style)
        {
            case NotificationStyle.Black:
                // Muestra una superposición negra con opacidad total y pausa el juego
                if (blackOverlay != null)
                {
                    gameManager.GameIsPaused = true;
                    StartCoroutine(FadeImage(blackOverlay, 0f, 1.0f, 0.3f));
                }
                break;

            case NotificationStyle.Translucent:
                // Muestra una superposición translúcida para simular desenfoque y pausa el juego
                if (transparencyOverlay != null)
                {
                    gameManager.GameIsPaused = true;
                    StartCoroutine(FadeImage(transparencyOverlay, 0f, 0.5f, 0.3f));
                }
                break;

            case NotificationStyle.Glow:
                // Activa un efecto de resplandor alrededor de la notificación
                if (glowPreferences != null)
                {
                    glowPreferences.SetActive(true);
                    glowCoroutine = StartCoroutine(GlowPulse(glowPreferences, backgroundImage.color));
                }
                break;

            case NotificationStyle.Normal:
                // No aplica ningún estilo especial
                break;
        }
    }

    /// <summary>
    /// Restablece todos los efectos visuales aplicados y reanuda el juego si estaba pausado.
    /// Este método desactiva overlays y efectos visuales, y detiene cualquier animación de pulso activa.
    /// </summary>
    private void ResetAllPreferences()
    {
        // Reanuda el juego después de que la notificación se haya cerrado
        gameManager.GameIsPaused = false;

        // Desactiva la superposición negra si está activa
        if (blackOverlay != null)
            blackOverlay.gameObject.SetActive(false);

        // Desactiva la superposición de transparencia si está activa
        if (transparencyOverlay != null)
            transparencyOverlay.gameObject.SetActive(false);

        // Desactiva las preferencias de resplandor si están activadas
        if (glowPreferences != null)
            glowPreferences.SetActive(false);

        // Detiene cualquier coroutine relacionada con el resplandor activo
        if (glowCoroutine != null)
            StopCoroutine(glowCoroutine);
    }

    /// <summary>
    /// Realiza una animación de desvanecimiento en la imagen especificada, interpolando entre dos valores de alfa durante un periodo de tiempo.
    /// </summary>
    private IEnumerator FadeImage(UnityEngine.UI.Image image, float fromAlpha, float toAlpha, float duration)
    {
        // Asegura que la imagen se haga visible
        image.gameObject.SetActive(true);

        // Guarda el color original de la imagen
        Color c = image.color;

        // Variable de tiempo para controlar la animación
        float elapsed = 0f;

        // Interpola el valor de alfa entre los dos valores especificados durante la duración
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsed / duration); // Interpolación de alfa
            image.color = new Color(c.r, c.g, c.b, alpha); // Aplica el nuevo valor de alfa
            yield return null; // Espera un frame antes de continuar
        }

        // Asegura que el valor final de alfa se ajuste correctamente
        image.color = new Color(c.r, c.g, c.b, toAlpha);
    }

    /// <summary>
    /// Crea un efecto de pulso de resplandor para un objeto, ajustando la saturación y el brillo de su color base.
    /// El objeto `glowRoot` tiene varias imágenes rectagulares con distintos alpha cada una
    /// como hijos que simulan el efecto de brillo al parpadear con un pulso suave.
    /// </summary>
    private IEnumerator GlowPulse(GameObject glowRoot, Color baseColor)
    {
        // Obtiene todos los componentes Image dentro del objeto glowRoot
        var children = glowRoot.GetComponentsInChildren<UnityEngine.UI.Image>();

        // Aumenta la saturación y el brillo del color base
        Color glowColor = SaturateAndBrighten(baseColor, 1f, 1f);

        // Tiempo que controla la velocidad del pulso
        float t = 0f;

        // Empieza un bucle infinito que continuará hasta que se detenga manualmente
        while (true)
        {
            t += Time.deltaTime * 2.5f; // Aumenta la velocidad del pulso

            // Calcula un valor de pulso que oscile entre 0 y 1 usando la función seno
            float pulse = (Mathf.Sin(t) + 1f) / 2f; // de 0 a 1

            // Ajusta el valor de alfa (transparencia) para el pulso, de 0 a 0.8
            float alpha = Mathf.Lerp(0f, 0.8f, pulse); // Hace que el resplandor sea más visible

            // Aplica el color con la transparencia modificada a todas las imágenes hijas
            foreach (var img in children)
            {
                img.color = new Color(glowColor.r, glowColor.g, glowColor.b, alpha);
            }

            // Espera un frame antes de continuar con el siguiente pulso
            yield return null;
        }
    }

    /// <summary>
    /// Ajusta la saturación y el brillo de un color mediante la conversión a HSV (matiz, saturación, valor).
    /// </summary>
    private Color SaturateAndBrighten(Color color, float saturationBoost, float brightnessBoost)
    {
        // Convierte el color de RGB a HSV
        Color.RGBToHSV(color, out float h, out float s, out float v);

        // Aumenta la saturación y el brillo según los factores especificados
        s = Mathf.Clamp01(s * saturationBoost);  // Limita la saturación entre 0 y 1
        v = Mathf.Clamp01(v * brightnessBoost); // Limita el brillo entre 0 y 1

        // Convierte de vuelta el color ajustado desde HSV a RGB
        return Color.HSVToRGB(h, s, v);
    }
}