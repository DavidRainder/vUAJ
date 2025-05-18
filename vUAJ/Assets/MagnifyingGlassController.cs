using UnityEngine;
using UnityEngine.InputSystem;

[Tooltip("Genera y controla una c�mara que funcionar� como una lupa." +
    "Neceista un componente c�mara, que esta sea ortogr�fica y que no sea de tipo 'Overlay'." +
    "Para que sea capaz de hacer Zoom a objetos de interfaz de usuario (UI), esta debe estar en WorldSpace y visible en la c�mara"), 
    RequireComponent(typeof(Camera))]
public class MagnifyingGlassController : MonoBehaviour
{
    /// <summary>
    /// Referencia a la c�mara.
    /// </summary>
    Camera _camera;

    #region InputActions
    [SerializeField,
     Tooltip("Acci�n con la que se activar� la Lupa." +
        "Por defecto: Z.")]
    InputActionReference _activationAction;
    
    [SerializeField,
     Tooltip("Acci�n con la que se aumentar� el tama�o de la Lupa.\n" +
        "Por defecto: Q." +
        "Este evento funciona subscribiendose a su performed para aumentar la lupa y a su canceled para dejar de aumentarla")] 
    InputActionReference _expandAction;
    
    [SerializeField,
     Tooltip("Acci�n con la que se reducir� el tama�o de la Lupa.\n" +
        "Por defecto: E." +
        "Este evento funciona subscribiendose a su performed para reducir la lupa y a su canceled para dejar de reducirla")] 
    InputActionReference _reduceAction;
    
    [SerializeField, 
     Tooltip("Acci�n con la que se mover� con la Lupa." +
        "IMPORTANTE: Debe devolver un Vector2.\n" +
        "Por defecto: WASD." +
        "Este evento funciona subscribiendose a su performed para mover la lupa y a su canceled para dejar de moverla")] 
    InputActionReference _movementAction;
    #endregion

    #region Parameters
    [SerializeField,
     Tooltip("Velocidad de aumento/reducci�n de la Lupa.")] 
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
    /// Flag de activaci�n de la Lupa
    /// </summary>
    bool _isActive = false;

    /// <summary>
    /// Movimiento que va a sufrir la Lupa en cada frame
    /// </summary>
    Vector2 _movement = Vector2.zero;
    /// <summary>
    /// Expansi�n que va a sufrir la Lupa en cada frame
    /// </summary>
    float _currentExpandFactor = 0.0f;

    /// <summary>
    /// Componente auxiliar que nos da el tama�o de la c�mara
    /// padre que hace que no pueda salir de los bordes de esta.
    /// 
    /// A pesar de que esa funcionalidad ya la implementa
    /// el propio componente, se ve c�mo se est� fijando
    /// en el sitio constantemente, haciendo que d� tirones.
    /// Por eso se utilizan los datos del componente
    /// para realizar el fix en este componente.
    /// 
    /// Para evitar que se ejecute la funcionalidad en el otro componente,
    /// se deber�a poner su flag "used by others" a true desde el componente usado
    /// </summary>
    FixMovementToCamera _parentFixing;

    /// <summary>
    /// Inicializaci�n del componente
    /// </summary>
    void Start()
    {
        // Cogemos la c�mara
        _camera = GetComponent<Camera>();
        if(_camera == null) // Si no la encontramos, no tiene sentido seguir vivo
        {
            Debug.LogError("Camera was not found on MagnifyingGlass prefab. Autodestroying ;(");
            Destroy(gameObject, 1f);
        }
        // Ponemos la c�mara a ortogr�fica, que es lo que necesitamos
        _camera.orthographic = true;

        // Activamos la c�mara seg�n nuestra flag de activaci�n (por defecto, falsa)
        EnablaCamera(_isActive);

        // Suscribimos una funci�n al evento de activaci�n de la Lupa
        _activationAction.action.performed += OnTryActivate;

        // Cogemos el componente FixMovementToCamera. Si no lo tiene, se quedar� a nulo, pero no importa
        // porque se comprueba en el Update
        _parentFixing = GetComponent<FixMovementToCamera>();
        if(_parentFixing) _parentFixing.UsedByOthers = true;
    }

    /// <summary>
    /// Movimiento y aumento/reducci�n de la lupa
    /// </summary>
    private void Update()
    {
        // Realizamos el movimiento de la c�mara
        if(_parentFixing) // Si debe ajustarse a los bordes de la c�mara, lo hacemos de esta forma
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

        // Expandimos/Reducimos el tama�o de la lupa
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize + _currentExpandFactor * Time.deltaTime, 0.1f, int.MaxValue);
    }

    /// <summary>
    /// Activamos/Desactivamos la c�mara
    /// </summary>
    /// <param name="enable"></param>
    void EnablaCamera(bool enable)
    {
        // Activamos/Desactivamos la c�mara y sus hijos para que la Lupa funcione como debe
        _camera.enabled = enable;
        foreach (GameObject child in _childToActivate)
        {
            child.SetActive(enable);
        }
    }

    /// <summary>
    /// Evento de activaci�n/desactivaci�n de la Lupa
    /// </summary>
    /// <param name="ctx"> Contexto de Input Action </param>
    void OnTryActivate(InputAction.CallbackContext ctx)
    {
        // Cambiamos la flag y activamos la Lupa en funci�n de ello
        _isActive = !_isActive;
        EnablaCamera(_isActive);

        // Suscribimos las acciones de movimiento y aumento de tama�o si estamos activos
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
    /// M�todo que subscribe a los eventos de movimiento y aumento/reducci�n de la Lupa.
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
    /// M�todo que se desubscribe a los eventos de movimiento y aumento/reducci�n de la Lupa
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
    /// Se pone a 0 el factor de aumento/reducci�n de la Lupa.
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
}
