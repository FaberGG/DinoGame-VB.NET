Public Class ObstacleManager
    Private _obstacles As List(Of Obstacle)
    Private _spawnTimer As Integer
    Private _spawnDelay As Integer
    Private _baseSpeed As Single
    Private _screenWidth As Integer
    Private _groundY As Integer

    Public ReadOnly Property Obstacles As List(Of Obstacle)
        Get
            Return _obstacles
        End Get
    End Property

    Public Property SpawnDelay As Integer
        Get
            Return _spawnDelay
        End Get
        Set(value As Integer)
            _spawnDelay = Math.Max(value, 30) ' Mínimo delay
        End Set
    End Property

    Public Sub New(screenWidth As Integer, groundY As Integer, baseSpeed As Single)
        _obstacles = New List(Of Obstacle)
        _screenWidth = screenWidth
        _groundY = groundY
        _baseSpeed = baseSpeed
        _spawnDelay = 100
        _spawnTimer = 0
    End Sub

    Public Sub Update(gameSpeed As Single)
        _spawnTimer += 1

        ' Generar nuevos obstáculos
        If _spawnTimer >= _spawnDelay / gameSpeed Then
            SpawnObstacle(gameSpeed)
            _spawnTimer = 0
        End If

        ' Actualizar obstáculos existentes
        For Each obstacle In _obstacles
            obstacle.Speed = _baseSpeed * gameSpeed
            obstacle.Update()
        Next

        ' Eliminar obstáculos fuera de pantalla
        _obstacles.RemoveAll(Function(o) o.IsOffScreen())
    End Sub

    Private Sub SpawnObstacle(gameSpeed As Single)
        Dim width As Integer = 20 + (New Random().Next(0, 20))
        Dim height As Integer = 40 + (New Random().Next(0, 30))
        Dim x As Integer = _screenWidth
        Dim y As Integer = _groundY - height
        Dim speed As Single = _baseSpeed * gameSpeed

        _obstacles.Add(New Obstacle(x, y, width, height, speed))
    End Sub

    Public Sub Clear()
        _obstacles.Clear()
    End Sub

    Public Function CheckCollisionWith(player As Player) As Boolean
        For Each obstacle In _obstacles
            If player.CollidesWith(obstacle) Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Sub Render(g As Graphics)
        For Each obstacle In _obstacles
            obstacle.Render(g)
        Next
    End Sub
End Class