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
    [SerializeField]
    Material material;
    [SerializeField]
    Collider2D _collider;

    Mesh mesh;
    GameObject border;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VolumePerceptionManager.Instance.changeShowColliders += showCollider;

        if(_collider == null) _collider = gameObject.GetComponent<Collider2D>();
       
        if (_collider != null)
        {
            border = new GameObject("border");
            border.transform.SetParent(gameObject.transform);

            mesh = _collider.CreateMesh(true, true);
            border.AddComponent<MeshFilter>().mesh = mesh;

            border.AddComponent<MeshRenderer>().material = material;

            material.color = VolumePerceptionManager.Instance.volumeColor;

            border.GetComponent<MeshRenderer>().sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;

            border.SetActive(VolumePerceptionManager.Instance.showColliders);
        }
        else Debug.LogWarning("The object does not have a Collider2D component so the script doesnt have an effect");
    }

    void showCollider(bool show)
    {
        if (_collider != null)
            border.SetActive(VolumePerceptionManager.Instance.showColliders);
        else Debug.LogWarning("The object does not have a Collider2D component so the script doesnt have an effect");
    }
}
