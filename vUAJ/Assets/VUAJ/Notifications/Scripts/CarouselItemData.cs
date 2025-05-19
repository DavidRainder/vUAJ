using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;
using ScrollCarousel;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// Esta clase controla un �tem individual dentro del carrusel interactivo (ScrollCarousel).
/// Cada �tem puede contener un video que se reproduce al enfocar el �tem, y est� asociado a una
/// configuraci�n espec�fica (posici�n, tama�o o estilo) que se aplica al sistema de notificaciones
/// al seleccionar dicho �tem. 
/// </summary>

// Tipos de configuraci�n que puede ser un Item
public enum NotificationConfigType { Position, Size, Style }

public class CarouselItemController :  MonoBehaviour, IPointerClickHandler
{
    public RawImage rawImage; // Referencia al componente RawImage donde se mostrar� el video
    public VideoClip clip; // Clip de video asociado al �tem
    public float delayBeforePlay = 0.2f; // Retraso antes de comenzar la reproducci�n del video

    public VideoPlayer videoPlayer; // Reproductor de video adjunto al objeto
    private Coroutine playRoutine; // Rutina para manejar el retardo antes de reproducir

    private Carousel carousel; // Referencia al carrusel padre
    private bool isFocused = false; // Indica si este �tem est� enfocado actualmente


    public NotificationConfigType configType; // Tipo de configuraci�n que representa este �tem

    // Valores de configuraci�n asociados
    public NotificationPosition position;
    public NotificationSize size;
    public NotificationStyle style;

    void Start()
    {
        // Obtiene las referencias necesarias al inicio
        carousel = GetComponentInParent<Carousel>();
        videoPlayer = GetComponent<VideoPlayer>();
        rawImage = GetComponent<RawImage>();

        // Configura el VideoPlayer para que no se reproduzca autom�ticamente
        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.Pause();
        }

        LoadInitialConfig();
    }

    void LoadInitialConfig()
    {
        switch (configType)
        {
            case NotificationConfigType.Position:
                carousel.StartItem = ((int)NotificationManager.Instance.CurrentPosition);
                break;
            case NotificationConfigType.Size:
                carousel.StartItem = ((int)NotificationManager.Instance.CurrentSize);
                break;
            case NotificationConfigType.Style:
                carousel.StartItem = ((int)NotificationManager.Instance.CurrentStyle);
                break;
        }
    }

    /// <summary>
    /// Aplica el tipo de configuraci�n que representa este �tem al NotificationManager.
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
    /// Maneja el evento de clic del usuario en este �tem del carrusel.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isFocused)
        {
            // Si no est� enfocado, solicita al carrusel que lo enfoque
            carousel.FocusItem(this.GetComponent<RectTransform>());
        }
    }

    /// <summary>
    /// Llamado por el carrusel para establecer si este �tem est� enfocado o no.
    /// </summary>
    public void SetFocus(bool focus)
    {
        isFocused = focus;

        if (focus)
        {
            // Si se enfoca, se inicia la reproducci�n del video tras un peque�o retraso
            if (playRoutine != null) StopCoroutine(playRoutine);
            playRoutine = StartCoroutine(PlayWithDelay());

            // Aplica la configuraci�n del �tem
            ApplyThisItem();
        }
        else
        {
            // Si se desenfoca, se detiene la reproducci�n
            if (playRoutine != null) StopCoroutine(playRoutine);
            PauseVideo();

        }
    }

    /// <summary>
    /// Rutina que espera un breve retraso antes de comenzar la reproducci�n del video.
    /// </summary>
    private IEnumerator PlayWithDelay()
    {
        if (clip == null || videoPlayer == null)
            yield break;

        videoPlayer.Pause();  // Pausa cualquier reproducci�n actual
        videoPlayer.clip = clip; // Asigna el nuevo clip

        yield return new WaitForSeconds(delayBeforePlay); // Espera el retraso

        videoPlayer.time = 0; // Reinicia el tiempo de reproducci�n
        videoPlayer.Play();  // Inicia la reproducci�n
    }

    /// <summary>
    /// Pausa la reproducci�n del video si est� en curso.
    /// </summary>
    private void PauseVideo()
    {
        if (videoPlayer != null) videoPlayer.Pause();
    }

    
}
