using UnityEngine;
using UnityEngine.InputSystem;

[Tooltip("Genera y controla una cámara que funcionará como una lupa." +
    "Neceista un componente cámara, que esta sea ortográfica y que no sea de tipo 'Overlay'." +
    "Para que sea capaz de hacer Zoom a objetos de interfaz de usuario (UI), esta debe estar en WorldSpace y visible en la cámara"), 
    RequireComponent(typeof(Camera))]
public class MagnifyingGlassController : MonoBehaviour
{
    /// <summary>
    /// Referencia a la cámara.
    /// </summary>
    Camera _camera;

    #region InputActions
    [SerializeField,
     Tooltip("Acción con la que se activará la Lupa." +
        "Por defecto: Z.")]
    InputActionReference _activationAction;
    
    [SerializeField,
     Tooltip("Acción con la que se aumentará el tamaño de la Lupa.\n" +
        "Por defecto: Q." +
        "Este evento funciona subscribiendose a su performed para aumentar la lupa y a su canceled para dejar de aumentarla")] 
    InputActionReference _expandAction;
    
    [SerializeField,
     Tooltip("Acción con la que se reducirá el tamaño de la Lupa.\n" +
        "Por defecto: E." +
        "Este evento funciona subscribiendose a su performed para reducir la lupa y a su canceled para dejar de reducirla." +
        "AVISO: El tamaño MÍNIMO de la cámara es su ortagraphicSize inicial!!")] 
    InputActionReference _reduceAction;
    
    [SerializeField, 
     Tooltip("Acción con la que se moverá con la Lupa." +
        "IMPORTANTE: Debe devolver un Vector2.\n" +
        "Por defecto: WASD." +
        "Este evento funciona subscribiendose a su performed para mover la lupa y a su canceled para dejar de moverla")] 
    InputActionReference _movementAction;
    #endregion

    #region Parameters
    [SerializeField,
     Tooltip("Velocidad de aumento/reducción de la Lupa.")] 
    float _expandFactor = 0.1f;
    
    [SerializeField,
     Tooltip("Velocidad de movimiento de la Lupa.")] 
    float _movementSpeed = 5.0f;
    
    [SerializeField,
     Tooltip("Hijos de la Lupa que deban activarse o desactivarse cuando la Lupa se active.\n" +
        "Por defecto: Ambos hijos del MagnifyingGlass (Background:Canvas y RenderTexture:Camera.")] 
    GameObject[] _childToActivate = null;
    #endregion

    /// <summary>
    /// Flag de activación de la Lupa
    /// </summary>
    bool _isActive = false;

    /// <summary>
    /// Movimiento que va a sufrir la Lupa en cada frame
    /// </summary>
    Vector2 _movement = Vector2.zero;
    /// <summary>
    /// Expansión que va a sufrir la Lupa en cada frame
    /// </summary>
    float _currentExpandFactor = 0.0f;

    /// <summary>
    /// Tamaño mínimo de la cámara. Se settea al inicial
    /// </summary>
    float _initialSize = 5.0f;

    /// <summary>
    /// Componente auxiliar que nos da el tamaño de la cámara
    /// padre que hace que no pueda salir de los bordes de esta.
    /// 
    /// A pesar de que esa funcionalidad ya la implementa
    /// el propio componente, se ve cómo se está fijando
    /// en el sitio constantemente, haciendo que dé tirones.
    /// Por eso se utilizan los datos del componente
    /// para realizar el fix en este componente.
    /// 
    /// Para evitar que se ejecute la funcionalidad en el otro componente,
    /// se debería poner su flag "used by others" a true desde el componente usado
    /// </summary>
    FixMovementToCamera _parentFixing;

    /// <summary>
    /// Inicialización del componente
    /// </summary>
    void Start()
    {
        // Cogemos la cámara
        _camera = GetComponent<Camera>();
        if(_camera == null) // Si no la encontramos, no tiene sentido seguir vivo
        {
            Debug.LogError("Camera was not found on MagnifyingGlass prefab. Autodestroying ;(");
            Destroy(gameObject, 1f);
        }
        // Ponemos la cámara a ortográfica, que es lo que necesitamos
        _camera.orthographic = true;
        _initialSize = _camera.orthographicSize;

        // Activamos la cámara según nuestra flag de activación (por defecto, falsa)
        EnablaCamera(_isActive);

        // Suscribimos una función al evento de activación de la Lupa
        _activationAction.action.performed += OnTryActivate;

        // Cogemos el componente FixMovementToCamera. Si no lo tiene, se quedará a nulo, pero no importa
        // porque se comprueba en el Update
        _parentFixing = GetComponent<FixMovementToCamera>();
        if(_parentFixing) _parentFixing.UsedByOthers = true;
    }

    /// <summary>
    /// Movimiento y aumento/reducción de la lupa
    /// </summary>
    private void Update()
    {
        // Realizamos el movimiento de la cámara
        if(_parentFixing) // Si debe ajustarse a los bordes de la cámara, lo hacemos de esta forma
        {
            Vector2 fittings = _parentFixing.GetFittings();
            Vector3 pos = transform.position;
            pos = new Vector3(
                Mathf.Clamp(pos.x + _movement.x * _movementSpeed * Time.deltaTime, -fittings.x, fittings.x),
                Mathf.Clamp(pos.y + _movement.y * _movementSpeed * Time.deltaTime, -fittings.y, fittings.y),
                pos.z);
            transform.position = pos;
        }
        else // Si no debe ajustarse, se realiza un translate
        {
            transform.Translate(_movement * _movementSpeed * Time.deltaTime);
        }

        // Expandimos/Reducimos el tamaño de la lupa
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize + _currentExpandFactor * Time.deltaTime, 0.1f, _initialSize);
    }

    /// <summary>
    /// Activamos/Desactivamos la cámara
    /// </summary>
    /// <param name="enable"></param>
    void EnablaCamera(bool enable)
    {
        // Activamos/Desactivamos la cámara y sus hijos para que la Lupa funcione como debe
        if(_camera != null)
        {
            _camera.enabled = enable;
            foreach (GameObject child in _childToActivate)
            {
                child.SetActive(enable);
            }
        }
    }

    /// <summary>
    /// Evento de activación/desactivación de la Lupa
    /// </summary>
    /// <param name="ctx"> Contexto de Input Action </param>
    void OnTryActivate(InputAction.CallbackContext ctx)
    {
        // Cambiamos la flag y activamos la Lupa en función de ello
        _isActive = !_isActive;
        EnablaCamera(_isActive);

        // Suscribimos las acciones de movimiento y aumento de tamaño si estamos activos
        if(_isActive)
        {
            Subscribe();   
        }
        else // Desuscribimos en caso contrario
        {
            Unsubscribe();

            // Paramos el movimiento
            _currentExpandFactor = 0.0f;
            _movement = Vector2.zero;
        }
    }

    /// <summary>
    /// Método que subscribe a los eventos de movimiento y aumento/reducción de la Lupa.
    /// </summary>
    void Subscribe()
    {
        // performed -> empezamos a pulsar
        // canceled -> dejamos de pulsar
        _expandAction.action.performed += OnExpandPerformed;
        _expandAction.action.canceled += OnExpandReduceCanceled;

        _reduceAction.action.performed += OnReducePerformed;
        _reduceAction.action.canceled += OnExpandReduceCanceled;

        _movementAction.action.performed += OnMovePerformed;
        _movementAction.action.canceled += OnMoveCanceled;
    }

    /// <summary>
    /// Método que se desubscribe a los eventos de movimiento y aumento/reducción de la Lupa
    /// </summary>
    void Unsubscribe()
    {
        _expandAction.action.performed -= OnExpandPerformed;
        _expandAction.action.canceled -= OnExpandReduceCanceled;

        _reduceAction.action.performed -= OnReducePerformed;
        _reduceAction.action.canceled -= OnExpandReduceCanceled;

        _movementAction.action.performed -= OnMovePerformed;
        _movementAction.action.canceled -= OnMoveCanceled;
    }

    /// <summary>
    /// Callback llamado cuando aumenta la lupa.
    /// Se pone el factor de escalado en negativo como factor de aumento de la Lupa.
    /// </summary>
    /// <param name="ctx"> InputAction context </param>
    void OnExpandPerformed(InputAction.CallbackContext ctx)
    {
        _currentExpandFactor = -_expandFactor;
    }

    /// <summary>
    /// Callback llamado cuando dejamos de aumentar/reducir la lupa.
    /// Se pone a 0 el factor de aumento/reducción de la Lupa.
    /// </summary>
    /// <param name="ctx"> InputAction context </param>
    void OnExpandReduceCanceled(InputAction.CallbackContext ctx)
    {
        _currentExpandFactor = 0.0f;
    }

    /// <summary>
    /// Callback llamado cuando reduce la lupa.
    /// Se pone el factor de escalado como factor de reduce de la Lupa.
    /// </summary>
    /// <param name="ctx"> InputAction context </param>
    void OnReducePerformed(InputAction.CallbackContext ctx) 
    {
        _currentExpandFactor = _expandFactor;
    }

    /// <summary>
    /// Callback llamado cuando movemos la lupa.
    /// Se pone al Vector2 recibido como el factor de movimiento de la Lupa.
    /// </summary>
    /// <param name="ctx"> InputAction context </param>
    void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        _movement = ctx.action.ReadValue<Vector2>();
    }

    /// <summary>
    /// Callback llamado cuando dejamos de mover la lupa.
    /// Se pone a 0 el factor de movimiento de la Lupa.
    /// </summary>
    /// <param name="ctx"> InputAction context </param>
    void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        _movement = Vector2.zero;
    }
    /// <summary>
    /// Permite reasignar dinámicamente las acciones de input.
    /// </summary>
    public void SetActionReference(string actionName, InputActionReference newReference)
    {
        switch (actionName)
        {
            case "Activation": _activationAction = newReference; break;
            case "Expand": _expandAction = newReference; break;
            case "Reduce": _reduceAction = newReference; break;
            case "Move": _movementAction = newReference; break;
        }
    }
}
