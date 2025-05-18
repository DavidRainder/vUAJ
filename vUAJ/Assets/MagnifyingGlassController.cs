using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MagnifyingGlassController : MonoBehaviour
{
    Camera _camera;

    #region InputActions
    [SerializeField] InputActionReference _activationAction;
    [SerializeField] InputActionReference _expandAction;
    [SerializeField] InputActionReference _reduceAction;
    [SerializeField] InputActionReference _movementAction;
    #endregion

    #region Parameters
    [SerializeField] float _expandFactor = 0.1f;
    [SerializeField] float _movementSpeed = 5.0f;
    [SerializeField] GameObject[] _childToActivate = null;
    #endregion

    bool _isActive = false;

    Vector2 _movement = Vector2.zero;
    float _currentExpandFactor = 0.0f;

    FixMovementToCamera _parentFixing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _camera = GetComponent<Camera>();
        if(_camera == null)
        {
            Debug.LogError("Camera was not found on MagnifyingGlass prefab. Autodestroying ;(");
            Destroy(gameObject, 1f);
        }

        EnablaCamera(_isActive);
        _activationAction.action.performed += OnTryActivate;

        _parentFixing = GetComponent<FixMovementToCamera>();
    }

    private void Update()
    {
        if(_parentFixing != null)
        {
            Vector2 fittings = _parentFixing.GetFittings();
            Vector3 pos = transform.position;
            pos = new Vector3(
                Mathf.Clamp(pos.x + _movement.x * _movementSpeed * Time.deltaTime, -fittings.x, fittings.x),
                Mathf.Clamp(pos.y + _movement.y * _movementSpeed * Time.deltaTime, -fittings.y, fittings.y),
                pos.z);
            transform.position = pos;
        }
        else
        {
            transform.Translate(_movement * _movementSpeed * Time.deltaTime);
        }
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize + _currentExpandFactor * Time.deltaTime, 0.1f, int.MaxValue);
    }

    void EnablaCamera(bool enable)
    {
        _camera.enabled = enable;
        foreach (GameObject child in _childToActivate)
        {
            child.SetActive(enable);
        }
    }

    void OnTryActivate(InputAction.CallbackContext ctx)
    {
        _isActive = !_isActive;
        EnablaCamera(_isActive);

        if(_isActive)
        {
            Subscribe();   
        }
        else
        {
            Unsubscribe();

            _currentExpandFactor = 0.0f;
            _movement = Vector2.zero;
        }
    }

    void Subscribe()
    {
        _expandAction.action.performed += OnExpandPerformed;
        _expandAction.action.canceled += OnExpandReduceCanceled;

        _reduceAction.action.performed += OnReducePerformed;
        _reduceAction.action.canceled += OnExpandReduceCanceled;

        _movementAction.action.performed += OnMovePerformed;
        _movementAction.action.canceled += OnMoveCanceled;
    }
    void Unsubscribe()
    {
        _expandAction.action.performed -= OnExpandPerformed;
        _expandAction.action.canceled -= OnExpandReduceCanceled;

        _reduceAction.action.performed -= OnReducePerformed;
        _reduceAction.action.canceled -= OnExpandReduceCanceled;

        _movementAction.action.performed -= OnMovePerformed;
        _movementAction.action.canceled -= OnMoveCanceled;
    }
    void OnExpandPerformed(InputAction.CallbackContext ctx)
    {
        _currentExpandFactor = -_expandFactor;
    }

    void OnExpandReduceCanceled(InputAction.CallbackContext ctx)
    {
        _currentExpandFactor = 0.0f;
    }

    void OnReducePerformed(InputAction.CallbackContext ctx) 
    {
        _currentExpandFactor = _expandFactor;
    }

    void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        /// WARNING: Esto debe cambiarse al vector2d obtenido 
        /// por la input action
        _movement = ctx.action.ReadValue<Vector2>();
    }

    void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        _movement = Vector2.zero;
    }
}
