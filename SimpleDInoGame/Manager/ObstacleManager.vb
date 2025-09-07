
' ====================================================================
' CLASE OBSTACLEMANAGER
' ====================================================================
' Propósito: Administra la creación, actualización y eliminación de obstáculos
' 
' Responsabilidades:
' - Mantener lista de obstáculos activos
' - Generar nuevos obstáculos con timing y características aleatorias
' - Actualizar todos los obstáculos cada frame
' - Eliminar obstáculos que salen de pantalla
' - Detectar colisiones con el jugador
' - Ajustar dificultad según velocidad del juego
' ====================================================================

Public Class ObstacleManager
    ' Constantes de configuración
    Private Const BASE_SPAWN_DELAY As Integer = 100        ' Intervalo base entre spawns (frames)
    Private Const MIN_SPAWN_DELAY As Integer = 30          ' Intervalo mínimo entre spawns
    Private Const GROUND_VISUAL_OFFSET As Integer = 6      ' Ajuste visual del suelo
    Private Const MAX_DIFFICULTY_FACTOR As Single = 3.0F   ' Factor máximo de dificultad

    ' Lista de todos los obstáculos activos en pantalla
    Private obstacles As New List(Of Obstacle)
    ' Variables de control de generación de obstáculos
    Private spawnTimer As Integer = 0           ' Contador de frames desde último spawn
    Private spawnDelay As Integer = BASE_SPAWN_DELAY ' Intervalo base entre spawns (en frames)

    ' Variables de configuración
    Private screenWidth As Integer              ' Ancho de pantalla para posicionar nuevos obstáculos
    Private groundY As Integer                  ' Nivel del suelo
    Private baseSpeed As Single                 ' Velocidad base de obstáculos
    Private random As New Random()              ' Generador para selección aleatoria de tipos

    ' Constructor: Inicializa el gestor con parámetros de pantalla y velocidad
    ' width: Ancho de la pantalla
    ' ground: Nivel Y del suelo
    ' speed: Velocidad base para los obstáculos
    Public Sub New(width As Integer, ground As Integer, speed As Single)
        ' Validación de parámetros
        If width <= 0 Then
            Throw New ArgumentException("El ancho de pantalla debe ser mayor a cero", "width")
        End If

        If speed <= 0 Then
            Throw New ArgumentException("La velocidad base debe ser mayor a cero", "speed")
        End If

        screenWidth = width
        groundY = ground + GROUND_VISUAL_OFFSET  ' Ajuste para alinear con el suelo visual
        baseSpeed = speed
    End Sub

    ' Actualiza todos los obstáculos y maneja la generación de nuevos
    ' gameSpeed: Multiplicador de velocidad del juego (aumenta con el tiempo)
    Public Sub Update(gameSpeed As Single)
        spawnTimer += 1

        ' Generar nuevo obstáculo basado en velocidad del juego
        ' Aplicar dificultad progresiva pero limitada para evitar spawn excesivo
        Dim difficultyFactor As Single = Math.Min(gameSpeed, MAX_DIFFICULTY_FACTOR)
        Dim adjustedDelay As Integer = Math.Max(MIN_SPAWN_DELAY, CInt(spawnDelay / difficultyFactor))

        ' Verificar si es tiempo de generar un nuevo obstáculo
        If spawnTimer >= adjustedDelay Then
            SpawnObstacle(gameSpeed)
            spawnTimer = 0  ' Resetear contador
        End If

        ' Actualizar posición de todos los obstáculos existentes
        For Each obstacle In obstacles
            obstacle.Update()
        Next

        ' Limpiar obstáculos que han salido de pantalla (optimización de memoria)
        obstacles.RemoveAll(Function(o) o.IsOffScreen())
    End Sub

    ' Crea un nuevo obstáculo con tipo aleatorio
    ' gameSpeed: Velocidad actual del juego para ajustar velocidad del obstáculo
    Private Sub SpawnObstacle(gameSpeed As Single)
        ' Seleccionar tipo de obstáculo aleatorio
        ' Usar constantes de la clase Obstacle para mayor claridad
        Dim obstacleType As Integer = random.Next(Obstacle.SMALL_CACTUS, Obstacle.LARGE_CACTUS + 1)

        ' Calcular velocidad del obstáculo basada en velocidad del juego
        Dim currentSpeed As Single = baseSpeed * gameSpeed

        ' Crear obstáculo en el borde derecho, a nivel del suelo
        ' Ahora solo pasamos el tipo, Obstacle maneja las dimensiones
        obstacles.Add(New Obstacle(screenWidth, groundY - 60, obstacleType, currentSpeed))
    End Sub

    ' Detecta colisiones entre el jugador y cualquier obstáculo
    ' player: Instancia del jugador para verificar colisión
    ' Retorna True si hay colisión, False si no
    Public Function CheckCollisionWith(player As Player) As Boolean
        For Each obstacle In obstacles
            ' Usar intersección de rectángulos para detección precisa
            If player.GetBounds().IntersectsWith(obstacle.GetBounds()) Then
                Return True     ' Colisión detectada
            End If
        Next
        Return False           ' No hay colisiones
    End Function

    ' Renderiza todos los obstáculos activos
    ' g: Contexto gráfico donde dibujar
    Public Sub Render(g As Graphics)
        For Each obstacle In obstacles
            obstacle.Render(g)
        Next
    End Sub

    ' Propiedad de solo lectura para obtener número de obstáculos activos
    ' Útil para debugging o estadísticas
    Public ReadOnly Property ActiveObstacleCount As Integer
        Get
            Return obstacles.Count
        End Get
    End Property

    ' Método para limpiar todos los obstáculos (útil para reiniciar juego)
    Public Sub ClearAllObstacles()
        obstacles.Clear()
    End Sub

    ' Método para ajustar dificultad dinámicamente
    Public Sub SetSpawnDelay(newDelay As Integer)
        If newDelay > 0 Then
            spawnDelay = newDelay
        End If
    End Sub
End Class