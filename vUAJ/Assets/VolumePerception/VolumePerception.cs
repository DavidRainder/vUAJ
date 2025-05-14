using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using System;
#endif

public class VolumePerception : MonoBehaviour
{
    Mesh mesh;
    [SerializeField]
    Material material;
    [SerializeField]
    Color volumeColor = new Color(1, 1, 1, 0.6f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var collider = gameObject.GetComponent<Collider2D>();
        if (collider != null)
        {
            GameObject border = new GameObject("border");
            border.transform.SetParent(gameObject.transform);

            mesh = collider.CreateMesh(true, true);
            border.AddComponent<MeshFilter>().mesh = mesh;

            border.AddComponent<MeshRenderer>().material = material;

            material.color = volumeColor;

            border.GetComponent<MeshRenderer>().sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;

        }
        else Debug.LogWarning("The object does not have a Collider2D component so the script doesnt have an effect");
    }
}
