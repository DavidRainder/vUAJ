using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;
using ScrollCarousel;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class CarouselItemController :  MonoBehaviour, IPointerClickHandler
{
    public RawImage rawImage; 
    public VideoClip clip;
    public float delayBeforePlay = 0.2f;

    public VideoPlayer videoPlayer;
    private Coroutine playRoutine;

    private Carousel carousel;
    private bool isFocused = false;
    public enum NotificationConfigType { Position, Size, Style }

    public NotificationConfigType configType;

    public NotificationPosition position;
    public NotificationSize size;
    public NotificationStyle style;

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

    void Start()
    {
        carousel = GetComponentInParent<Carousel>();
        videoPlayer = GetComponent<VideoPlayer>();
        rawImage = GetComponent<RawImage>();

        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.Pause();
        }
    }

    public  void OnPointerClick(PointerEventData eventData)
    {
        if (!isFocused)
        {
            carousel.FocusItem(this.GetComponent<RectTransform>());
        }
    }

    
    public void SetFocus(bool focus)
    {
        isFocused = focus;

        if (focus)
        {
            if (playRoutine != null) StopCoroutine(playRoutine);
            playRoutine = StartCoroutine(PlayWithDelay());
            ApplyThisItem();
        }
        else
        {
            if (playRoutine != null) StopCoroutine(playRoutine);
            PauseVideo();

        }
    }
    private IEnumerator PlayWithDelay()
    {
        if (clip == null || videoPlayer == null)
            yield break;

        videoPlayer.Pause();
        videoPlayer.clip = clip;

        yield return new WaitForSeconds(delayBeforePlay);

        videoPlayer.time = 0;
        videoPlayer.Play();
    }

    private void PauseVideo()
    {
        if (videoPlayer != null) videoPlayer.Pause();
    }

    
}
