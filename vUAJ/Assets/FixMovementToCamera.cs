using UnityEngine;

[Tooltip("Component used to fix a child of the camera to its bounds in position." +
    "This is only meant to be used for a 2D Ortographic camera.")]
public class FixMovementToCamera : MonoBehaviour
{
    [SerializeField, Tooltip("The camera to fix to.")] Camera _mainCamera = null;

    float _width;
    float _height;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _height = _mainCamera.orthographicSize;
        _width = _mainCamera.aspect * _height;
        
        if(_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }
    }

    public Vector2 GetFittings() {
        return new Vector2(_width, _height);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position;
        position = transform.position;

        position = new Vector3(Mathf.Clamp(position.x, -_width, _width),
            Mathf.Clamp(position.y, -_height, _height), position.z);

        transform.position = position;
    }
}
