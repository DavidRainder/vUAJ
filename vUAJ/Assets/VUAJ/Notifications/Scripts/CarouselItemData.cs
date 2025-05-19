using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;
using ScrollCarousel;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// Esta clase controla un ítem individual dentro del carrusel interactivo (ScrollCarousel).
/// Cada ítem puede contener un video que se reproduce al enfocar el ítem, y está asociado a una
/// configuración específica (posición, tamaño o estilo) que se aplica al sistema de notificaciones
/// al seleccionar dicho ítem. 
/// </summary>

// Tipos de configuración que puede ser un Item
public enum NotificationConfigType { Position, Size, Style }

public class CarouselItemController :  MonoBehaviour, IPointerClickHandler
{
    public RawImage rawImage; // Referencia al componente RawImage donde se mostrará el video
    public VideoClip clip; // Clip de video asociado al ítem
    public float delayBeforePlay = 0.2f; // Retraso antes de comenzar la reproducción del video

    public VideoPlayer videoPlayer; // Reproductor de video adjunto al objeto
    private Coroutine playRoutine; // Rutina para manejar el retardo antes de reproducir

    private Carousel carousel; // Referencia al carrusel padre
    private bool isFocused = false; // Indica si este ítem está enfocado actualmente


    public NotificationConfigType configType; // Tipo de configuración que representa este ítem

    // Valores de configuración asociados
    public NotificationPosition position;
    public NotificationSize size;
    public NotificationStyle style;

    void Start()
    {
        // Obtiene las referencias necesarias al inicio
        carousel = GetComponentInParent<Carousel>();
        videoPlayer = GetComponent<VideoPlayer>();
        rawImage = GetComponent<RawImage>();

        // Configura el VideoPlayer para que no se reproduzca automáticamente
        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.Pause();
        }
    }

    /// <summary>
    /// Aplica el tipo de configuración que representa este ítem al NotificationManager.
    /// </summary>
    public void ApplyThisItem()
    {
        switch (configType)
        {
            case NotificationConfigType.Position:
                    NotificationManager.Instance.SetPosition(position);
                break;
            case NotificationConfigType.Size:
                    NotificationManager.Instance.SetSize(size);
                break;
            case NotificationConfigType.Style:
                    NotificationManager.Instance.SetStyle(style);
                break;
        }
    }

    /// <summary>
    /// Maneja el evento de clic del usuario en este ítem del carrusel.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isFocused)
        {
            // Si no está enfocado, solicita al carrusel que lo enfoque
            carousel.FocusItem(this.GetComponent<RectTransform>());
        }
    }

    /// <summary>
    /// Llamado por el carrusel para establecer si este ítem está enfocado o no.
    /// </summary>
    public void SetFocus(bool focus)
    {
        isFocused = focus;

        if (focus)
        {
            // Si se enfoca, se inicia la reproducción del video tras un pequeño retraso
            if (playRoutine != null) StopCoroutine(playRoutine);
            playRoutine = StartCoroutine(PlayWithDelay());

            // Aplica la configuración del ítem
            ApplyThisItem();
        }
        else
        {
            // Si se desenfoca, se detiene la reproducción
            if (playRoutine != null) StopCoroutine(playRoutine);
            PauseVideo();

        }
    }

    /// <summary>
    /// Rutina que espera un breve retraso antes de comenzar la reproducción del video.
    /// </summary>
    private IEnumerator PlayWithDelay()
    {
        if (clip == null || videoPlayer == null)
            yield break;

        videoPlayer.Pause();  // Pausa cualquier reproducción actual
        videoPlayer.clip = clip; // Asigna el nuevo clip

        yield return new WaitForSeconds(delayBeforePlay); // Espera el retraso

        videoPlayer.time = 0; // Reinicia el tiempo de reproducción
        videoPlayer.Play();  // Inicia la reproducción
    }

    /// <summary>
    /// Pausa la reproducción del video si está en curso.
    /// </summary>
    private void PauseVideo()
    {
        if (videoPlayer != null) videoPlayer.Pause();
    }

    
}
