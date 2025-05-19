using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GameManager es un singleton que sirve como base para la gesti�n global del juego. 
/// 
/// **Uso del GameManager como base de la l�gica del juego:**
/// El GameManager proporciona una estructura b�sica que los desarrolladores pueden ampliar con la l�gica interna de su juego. 
/// Aunque puede ser utilizado tal cual, se recomienda extenderlo para incluir las caracter�sticas espec�ficas del juego que se est� desarrollando.
/// </summary>
public class GameManager : vUAJBaseGameManager
{
// ---------------------------------------------------------------------------------------------------------------
    //Variables para la l�gica del juego
    public int coinsCounter = 0;
    public GameObject playerGameObject;
    private PlayerController player;
    public GameObject deathPlayerPrefab;
    public Text coinText;


    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        coinText.text = coinsCounter.ToString();

        if (player.deathState == true)
        {
            playerGameObject.SetActive(false);
            GameObject deathPlayer = Instantiate(deathPlayerPrefab, playerGameObject.transform.position, playerGameObject.transform.rotation);
            deathPlayer.transform.localScale = playerGameObject.transform.localScale;
            player.deathState = false;
            Invoke("ReloadLevel", 3);
        }
    }

    private void ReloadLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
