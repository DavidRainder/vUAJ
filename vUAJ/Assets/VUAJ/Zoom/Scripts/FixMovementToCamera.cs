using UnityEngine;

[Tooltip("Utilizado para fijar el movimiento de un objeto a, como máximo, los bordes de una cámara ortográfica 2D.")]
public class FixMovementToCamera : MonoBehaviour
{
    [SerializeField, 
     Tooltip("La cámara de la que cogeremos el tamaño.")] 
    Camera _mainCamera = null;

    // ancho y alto de la cámara
    float _width;
    float _height;

    /// <summary>
    /// Variable privada que nos dice si se están utilizando las bounds en otro script
    /// y no queremos por lo tanto que se ejecute el Update de este componente
    /// </summary>
    bool _usedByOthers = false;

    /// <summary>
    /// Variable privada que nos dice si se están utilizando las bounds en otro script
    /// y no queremos por lo tanto que se ejecute el Update de este componente
    /// </summary>
    public bool UsedByOthers { 
        get { return _usedByOthers; } 
        set {
            _usedByOthers = value;
            // Descativamos el script si si está usando por otros para evitar ejecutar cosas
            // innecesarias
            this.enabled = !_usedByOthers;
        } }

    // calculamos el ancho y alto de la cámara
    void Start()
    {
        // si el usuario no pone cámara, se asume la MainCamera
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }

        // el alto es su ortographic size
        _height = _mainCamera.orthographicSize;
        // el ancho es una operación realizada con su aspect ratio
        _width = _mainCamera.aspect * _height;
    }

    /// <summary>
    /// Método que devuelve el ancho y alto de la cámara en un vector2
    /// </summary>
    /// <returns> Ancho (x) y alto (y) de cámara </returns>
    public Vector2 GetFittings() {
        return new Vector2(_width, _height);
    }

    // Clampeo de la posición a los límites dados
    void Update()
    {
        Vector3 position;
        position = transform.position;

        position = new Vector3(Mathf.Clamp(position.x, -_width, _width),
            Mathf.Clamp(position.y, -_height, _height), position.z);

        transform.position = position;
    }
}
