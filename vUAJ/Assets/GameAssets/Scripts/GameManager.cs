using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Platformer
{
    public class GameManager : MonoBehaviour
    {
        // Singleton instance
        public static GameManager Instance { get; private set; }

        public int coinsCounter = 0;

        public GameObject playerGameObject;
        private PlayerController player;
        public GameObject deathPlayerPrefab;
        public Text coinText;
        public bool GameIsPaused = false;

        private void Awake()
        {
            // Singleton setup
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // Avoid duplicates
                return;
            }

            Instance = this;
        }

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
}
