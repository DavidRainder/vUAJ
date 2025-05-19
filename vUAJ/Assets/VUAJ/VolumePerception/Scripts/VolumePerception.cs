using System.Collections.Generic;
using UnityEngine;

// Componente que se ha de poner a todos los elementos que se quiera que tengan opcion
// de Volume Perception (colliders visuales mediante malla de color)
public class VolumePerception : MonoBehaviour
{
    // Material usado para la malla
    [SerializeField]
    Material material;
    // Collider del objeto
    [SerializeField]
    Collider2D _collider;

    // Referencias al mesh y al objeto que se crea para ponerlo
    Mesh mesh;
    GameObject border;

    // Se crea una malla con la forma y tamanyo del collider del objeto, y con el color
    // especificado en el manager, y se comprueba si debe estar activo o no
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

    // Activa o desactiva el objeto con la malla
    void showCollider(bool show)
    {
        if (_collider != null)
            border.SetActive(VolumePerceptionManager.Instance.showColliders);
        else Debug.LogWarning("The object does not have a Collider2D component so the script doesnt have an effect");
    }
}
