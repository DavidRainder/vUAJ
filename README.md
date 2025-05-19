# vision Unity Accesibility Junction (vUAJ)
Este es un proyecto desarrollado para la asignatura de Usabilida y Análisis de Juegos, del Grado en Desarrollo de Videojuegos de la Universidad Complutense de Madrid.

vUAJ trata de juntar una serie de herramientas de accesibilidad visual. Todas las herramientas de vUAJ son modulares y pueden integrarse de forma independiente. El proyecto está organizado por carpetas, y cada herramienta se encuentra contenida en su propio módulo. Puedes copiar directamente aquellas que necesites.

A continuación, se detallan las instrucciones básicas para integrar y utilizar cada una de ellas:


- **HUD personalizable**
Cómo usarlo:
1. Asigna tu prefab de HUD al campo correspondiente del prefab HUDManager.
2. Para que el HUD funcione, añade a la escena del juego un gameObject vacío con el script GameSceneController 
   y arrastra tu prefab de HUD a la variable correspondiente
3. Añade el HUDManager a la primera escena que se llame

- **Tipografías alternativas de fácil lectura y Aumento del tamaño de letra**
Cómo usarlo:
1. Pon el script FontOptions a cualquier texto que quieras que se accesible
2. Asegurate que el máximo tamaño de letra queda bien con el layout de la pantalla
3. Para que se aplique, el usuario deberá chekear el Toggle de "Dislexia Friendly Fonts" en el menu de Settings
4. Añade el TextAccesibilityManager a la primera escena que se llame

- **Percepción de volumen de los objetos**
Cómo usarlo:
1. Pon a los objetos que quieras que resalten su collider el script VolumenPerception
2. Al script tienes pasarle desde el editor el material VolumenPerceptionMaterial (cualquier material vale) 
3. Por defecto, el script coge el primer collider que encuentra, pero en caso de que un objeto tenga varios y quieras
   usar uno específico, se puede pasar por referencia.
4. Para cambiar el color que se usará para representar el volumen, se puede hacer desde el editor en el script VolumenPerceptionManager
5. Añade el VolumenPerceptionManager a la primera escena que se llame

- **Audiodescripción (TTS)**
Cómo usarlo:
1. Las clases de TTS Notifications y TTSHUD solo se añaden al objeto que las vaya a usar.
2. Para usar los subtítulos hay que tener un json en el application.persistentdatapath con este formato:
    {
        "Subtitles":[
            "Personaje1": "hola",
            "Personaje1": "adios",
        ]
    }
3. Crea clases que llamen a los métodos de el TTS que se quieran
4. Añade el TTSManager a la primera escena

- **Lupa de pantalla**
Cómo usarlo:
1. Sustituye tu cámara por la del prefab Camera&MagnifyingGlass.
    - En caso de no querer borrar tu cámara, puedes incluir como hijo de esta el prefab MagnifyingGlass:Camera
    - Debes asegurarte de que tu cámara es ortográfica
    - Todas las cámaras deben ser tipo 'Base', no 'Overlay'
2. Para que la Lupa pueda mostrar el HUD/UI de tu aplicación, este debe estar en *World Space*, ya que una cámara
   no es capaz de renderizar un canvas si no se encuentra en espacio del mundo
3. Para utilizarla en escena, utiliza los siguientes controles:
    - Activar/Desactivar: Z
    - Ampliar: Q
    - Reducir: E
    - Mover: WASD
    Todos estos controles son InputActions que se encuentran referenciadas en el componente MagnifyingGlassController, del objeto MagnifyingGlass:Camera. Estas pueden ser modificadas y sustituídas como el desarrollador quiera.
    Desde la escena opciones, bajo la configuración de Zoom, se encuentran opciones para que el usuario sea capaz de hacer *rebind* de las teclas utilizadas por las InputAction, gracias al componente InputRebinderUI

- **Notificaciones customizables**
Cómo usarlo:
1. En cualquier parte del código de lógica que quieras lanzar una notificación, llama a este método 
     NotificationManager.Instance.SpawnNotification(
        "texto de notificacion",
        "nombre del icono",
        color,
        "nombre de sonido",                           
        Escoge el tipo de feedback háptico que quieras de aqui -> HapticFeedbackType{ None, Light, Medium, Heavy, Pulse}  
    );
2. Ten en cuenta que hay dos tipos de notificaciones: 
    - Standard: son aquellas que quieres que reciban todos los usuarios
    - Assisted: aquellas notificaciones adicionales para asistir a personas con dificultades visuales
3. En la carpeta de Resources hay dos carpetas adicionales que contienen iconos y sonidos ya cargados, puedes usar cualquiera de ellos o meter nuevos.
   A la hora de lanzar una notificacion solo deberás poner el nombre del asset que quieras, que esté en esa carpeta
4. Añade a la escena del juego el prefab GameCanvas
5. Para que las notificaciones funcionen, al igual que el HUD, añade a la escena del juego un gameObject vacío con el script GameSceneController 
   y arrastra del GameCanvas los gameObjects black, transparency y NotificationContainer a las variables correspondiente en el editor del GameSceneController
6. Añade el NotificationManager a la primera escena que se llame

**Indicaciones básicas generales**
- Si quieres usar la lupa y tu canvas no tiene el tamaño correcto puedes ponerle el script de CanvasAutoScaler
- La escena de settings tiene menu para todas las herramientas, si no quires alguna la desactivas en el gameObject Buttons (hijo de Accesibility_UI) y
  en el gameObject Information (hijo de Accesibility_UI) la quitas de la lista del script ToggleObjectActivation 
- Todas las herramientas usan un sistema común de serialización:
    - Se guardan como archivos .json en Application.persistentDataPath.
    - Utiliza la clase Serializer para guardar y cargar cualquier configuración serializable.

El proyecto completo se encuentra en:
🔗 https://github.com/DavidRainder/vUAJ.git

### Autores:
- Andrea Vega Saugar
- Claudia Zarzuela Amor
- Muxu Rubia Luque
- Eva Feliu Aréjola
- David Rivera Martínez
