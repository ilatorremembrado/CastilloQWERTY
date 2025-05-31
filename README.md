# CastilloQWERTY
## Diagrama E/R
```mermaid
erDiagram
    USUARIOS {
        INT     id PK
        VARCHAR nombre
        VARCHAR password_hash
        DATETIME fecha_creacion
    }
    CONFIGURACION {
        INT     id PK
        INT     usuario_id FK
        VARCHAR dificultad
        INT     errores_max
        FLOAT   bonificaciones
    }
    ESTADISTICAS {
        INT     id PK
        INT     usuario_id FK
        FLOAT   total_tiempo
        INT     total_errores
        INT     total_palabras
    }
    PARTIDAS {
        INT     id PK
        INT     usuario_id FK
        DATETIME fecha
        VARCHAR nivel
        INT     puntuacion
        FLOAT   velocidad
        FLOAT   precision
        INT     cadena_id FK
    }
    LOGROS {
        INT     id PK
        INT     usuario_id FK
        VARCHAR descripcion
        DATETIME conseguido_en
    }
    CADENAS {
        INT     id PK
        VARCHAR texto
        VARCHAR tipo
        VARCHAR dificultad
    }

    USUARIOS ||--o{ CONFIGURACION : "tiene"
    USUARIOS ||--o{ ESTADISTICAS   : "tiene"
    USUARIOS ||--o{ PARTIDAS       : "juega"
    USUARIOS ||--o{ LOGROS         : "consigue"
    PARTIDAS }o--|| CADENAS        : "usa"
```

## Diagrama modular
```mermaid
flowchart TD
  %% Módulos principales
  UI["UI / Interfaz de Usuario"]
  Auth["Gestión de Usuarios"]
  GameSys["Sistema de Juego"]
  MatchEngine["Motor de Partidas"]
  AchievStats["Logros y Estadísticas"]
  API["Comunicación con API Remota"]
  Config["Configuración / Dificultad"]
  ChainGen["Generación de Cadenas"]

  %% Dependencias e interacciones
  UI       -->|invoca| Auth
  UI       -->|muestra| GameSys
  UI       -->|lee/escribe| Config

  Auth     -->|API REST| API

  Config   -->|ScriptableObject o JSON| GameSys
  Config   -->|API REST| API

  ChainGen -->|provee cadenas| GameSys

  GameSys  -->|registra eventos| MatchEngine
  GameSys  -->|solicita texto| ChainGen

  MatchEngine -->|envía datos de partida| AchievStats
  MatchEngine -->|API REST| API

  AchievStats -->|API REST| API

```
## Diagrama casos de uso
```mermaid
graph TD
    %% Actores
    Jugador((Usuario))
    Sistema((Sistema))

    %% Casos de uso del Usuario
    Registrarse[[Registrarse]]
    IniciarSesion[[Iniciar sesión]]
    EliminarUsuario[[Eliminar usuario]]
    SeleccionarDificultad[[Seleccionar nivel de dificultad]]
    VerTutorial[[Ver tutorial]]
    JugarNivel[[Jugar nivel]]
    VisualizarEstadisticas[[Visualizar estadísticas]]
    VisualizarLogros[[Visualizar logros]]
    CerrarSesion[[Cerrar sesión]]

    %% Casos de uso del Sistema
    ValidarCredenciales[[Validar credenciales]]
    CargarConfiguraciones[[Cargar configuraciones]]
    GenerarCadenas[[Generar cadenas para el nivel]]
    AlmacenarPartidas[[Almacenar partidas]]
    GuardarEstadisticas[[Guardar estadísticas]]
    CalcularLogros[[Calcular logros]]

    %% Relaciones Usuario → Casos
    Jugador --> Registrarse
    Jugador --> IniciarSesion
    Jugador --> EliminarUsuario
    Jugador --> SeleccionarDificultad
    Jugador --> VerTutorial
    Jugador --> JugarNivel
    Jugador --> VisualizarEstadisticas
    Jugador --> VisualizarLogros
    Jugador --> CerrarSesion

    %% Relaciones Sistema → Casos internos
    Sistema --> ValidarCredenciales
    Sistema --> CargarConfiguraciones
    Sistema --> GenerarCadenas
    Sistema --> AlmacenarPartidas
    Sistema --> GuardarEstadisticas
    Sistema --> CalcularLogros

    %% Relaciones «include»
    IniciarSesion -.->|<<include>>| ValidarCredenciales
    SeleccionarDificultad -.->|<<include>>| CargarConfiguraciones
    JugarNivel -.->|<<include>>| GenerarCadenas
    JugarNivel -.->|<<include>>| AlmacenarPartidas
    AlmacenarPartidas -.->|<<include>>| GuardarEstadisticas
    GuardarEstadisticas -.->|<<include>>| CalcularLogros

```
## Diagrama de secuencia
```mermaid
sequenceDiagram
    participant Usuario
    participant UIManager
    participant GameManager
    participant DatabaseManager
    participant API_PHP

    Usuario->>UIManager: Pulsa "Jugar"
    UIManager->>GameManager: Cargar panel de juego
    GameManager->>DatabaseManager: Solicitar cadenas (según dificultad)
    DatabaseManager-->>GameManager: Devolver lista de cadenas
    Note right of GameManager: Activa cronómetro\ny monitoriza escritura
    GameManager->>GameManager: Detectar fin de partida
    GameManager->>GameManager: Calcular estadísticas
    GameManager->>DatabaseManager: Enviar estadísticas y resultados
    DatabaseManager->>API_PHP: POST datos de partida y estadísticas
    API_PHP-->>DatabaseManager: Confirmación de actualización
    DatabaseManager-->>GameManager: Confirmación de guardado

```
## Diagrama de actividades
```mermaid
flowchart TD
    Inicio((Inicio)) --> Login[Iniciar sesión]
    Login -->|Credenciales válidas| SeleccionarDificultad[Seleccionar dificultad]
    Login -->|Credenciales inválidas| Login
    SeleccionarDificultad --> IniciarNivel[Iniciar nivel]
    IniciarNivel --> MostrarCadena[Mostrar cadena de texto]
    MostrarCadena --> CapturarEscritura[Capturar escritura del jugador]
    CapturarEscritura --> MedirTiempoErrores[Medir tiempo y errores]
    MedirTiempoErrores --> CalcularPuntuacion[Calcular puntuación]
    CalcularPuntuacion --> MostrarResultados[Mostrar resultados]
    MostrarResultados --> GuardarDatos[Guardar datos en base de datos]
    GuardarDatos --> DecisionContinuar{"¿Continuar jugando?"}
    DecisionContinuar -->|Sí| SeleccionarDificultad
    DecisionContinuar -->|No| Fin((Fin))
```
## Diagrama de estados
```mermaid
graph TD
    %% ESTILOS PERSONALIZADOS
    classDef pantalla fill:#f9f,stroke:#333,stroke-width:1px,font-size:16px;
    classDef panel fill:#bbf,stroke:#333,stroke-width:1px,font-size:16px;
    classDef submenu fill:#ccf,stroke:#333,stroke-width:1px,font-size:16px;
    classDef combate fill:#faa,stroke:#333,stroke-width:1px,font-size:16px;
    classDef exito fill:#afa,stroke:#333,stroke-width:1px,font-size:16px;
    classDef error fill:#f88,stroke:#333,stroke-width:1px,font-size:16px;

    %% ESTADOS PRINCIPALES
    Inicio["Inicio"]:::pantalla
    Fin["Fin"]:::pantalla
    PantallaSeleccionUsuario["Pantalla Selección Usuario"]:::pantalla
    RegistroUsuario["Registro Usuario"]:::panel
    Login["Pantalla Login"]:::panel
    ValidarCredenciales["Validar Credenciales"]:::panel
    MenuPrincipal["Menú Principal"]:::pantalla

    VerEstadisticas["Ver Estadísticas"]:::submenu
    VerTutorial["Ver Tutorial"]:::submenu
    Configuracion["Configuración"]:::submenu
    IniciarJuego["Iniciar Juego"]:::panel

    EnCombate["Combate"]:::combate
    Pausa["Pausa"]:::combate
    NivelCompletado["Nivel Completado"]:::exito
    NivelReiniciado["Nivel Reiniciado"]:::combate

    CerrarSesion["Cerrar Sesión"]:::error
    EliminarUsuario["Eliminar Usuario"]:::error
    ConfirmarEliminacion["Confirmar Eliminación"]:::error

    %% FLUJO CON ETIQUETAS (SIN COMILLAS DOBLES)
    Inicio --> PantallaSeleccionUsuario
    PantallaSeleccionUsuario -->|Click en Nuevo Usuario| RegistroUsuario
    PantallaSeleccionUsuario -->|Click en usuario existente| Login
    RegistroUsuario -->|Registro completado o Cancelar| PantallaSeleccionUsuario

    Login -->|Ingresar contraseña| ValidarCredenciales
    ValidarCredenciales -->|Credenciales correctas| MenuPrincipal
    ValidarCredenciales -->|Credenciales incorrectas| Login

    MenuPrincipal -->|Click en Estadísticas| VerEstadisticas
    VerEstadisticas -->|Volver| MenuPrincipal

    MenuPrincipal -->|Click en Tutorial| VerTutorial
    VerTutorial -->|Cerrar Tutorial| MenuPrincipal

    MenuPrincipal -->|Click en Configurar Dificultad| Configuracion
    Configuracion -->|Guardar y Volver| MenuPrincipal

    MenuPrincipal -->|Click en Jugar| IniciarJuego
    IniciarJuego -->|Comienza nivel| EnCombate

    EnCombate -->|Presionar Pausa| Pausa
    Pausa -->|Reanudar| EnCombate
    EnCombate -->|Palabras correctas| NivelCompletado
    EnCombate -->|Error fatal o tiempo agotado| NivelReiniciado
    NivelCompletado --> MenuPrincipal
    NivelReiniciado --> EnCombate

    MenuPrincipal -->|Cerrar Sesión| CerrarSesion
    CerrarSesion --> PantallaSeleccionUsuario

    MenuPrincipal -->|Eliminar Usuario| EliminarUsuario
    EliminarUsuario --> ConfirmarEliminacion
    ConfirmarEliminacion -->|Confirmado| PantallaSeleccionUsuario
    ConfirmarEliminacion -->|Cancelado| MenuPrincipal

    PantallaSeleccionUsuario -->|Cerrar aplicación| Fin


```
