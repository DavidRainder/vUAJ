using System;
using System.Collections;
using UnityEngine;

[Tooltip("Este script trata de coger una lista de objetos, activarla durante " +
    "un frame para que se ejecute su Start, y luego desactivarlos según lo que diga el usuario")]
public class ToggleObjectActivation : MonoBehaviour
{
    [Serializable]
    struct ObjectActivation
    {
        public GameObject _gameObject;
        public bool _activation;
    }

    [SerializeField]
    ObjectActivation[] _gameObjects = null;

    private void Start()
    {
        StartCoroutine(deactivateObjects());
    }

    IEnumerator deactivateObjects()
    {
        foreach (ObjectActivation go in _gameObjects)
        {
            go._gameObject.SetActive(true);
        }

        yield return new WaitForEndOfFrame();
        foreach (ObjectActivation go in _gameObjects)
        {
            go._gameObject.SetActive(go._activation);
        }
    }
}
