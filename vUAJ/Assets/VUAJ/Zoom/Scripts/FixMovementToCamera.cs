using UnityEngine;

[Tooltip("Utilizado para fijar el movimiento de un objeto a, como m�ximo, los bordes de una c�mara ortogr�fica 2D.")]
public class FixMovementToCamera : MonoBehaviour
{
    [SerializeField, 
     Tooltip("La c�mara de la que cogeremos el tama�o.")] 
    Camera _mainCamera = null;

    // ancho y alto de la c�mara
    float _width;
    float _height;

    /// <summary>
    /// Variable privada que nos dice si se est�n utilizando las bounds en otro script
    /// y no queremos por lo tanto que se ejecute el Update de este componente
    /// </summary>
    bool _usedByOthers = false;

    /// <summary>
    /// Variable privada que nos dice si se est�n utilizando las bounds en otro script
    /// y no queremos por lo tanto que se ejecute el Update de este componente
    /// </summary>
    public bool UsedByOthers { 
        get { return _usedByOthers; } 
        set {
            _usedByOthers = value;
            // Descativamos el script si si est� usando por otros para evitar ejecutar cosas
            // innecesarias
            this.enabled = !_usedByOthers;
        } }

    // calculamos el ancho y alto de la c�mara
    void Start()
    {
        // el alto es su ortographic size
        _height = _mainCamera.orthographicSize;
        // el ancho es una operaci�n realizada con su aspect ratio
        _width = _mainCamera.aspect * _height;
        
        // si el usuario no pone c�mara, se asume la MainCamera
        if(_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }
    }

    /// <summary>
    /// M�todo que devuelve el ancho y alto de la c�mara en un vector2
    /// </summary>
    /// <returns> Ancho (x) y alto (y) de c�mara </returns>
    public Vector2 GetFittings() {
        return new Vector2(_width, _height);
    }

    // Clampeo de la posici�n a los l�mites dados
    void Update()
    {
        Vector3 position;
        position = transform.position;

        position = new Vector3(Mathf.Clamp(position.x, -_width, _width),
            Mathf.Clamp(position.y, -_height, _height), position.z);

        transform.position = position;
    }
}
