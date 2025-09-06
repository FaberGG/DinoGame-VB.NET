Imports System.Drawing
Imports System.Windows.Forms

Public Class DinoGameForm
    Inherits Form

#Region "Variables del Juego"
    Private WithEvents gameTimer As Timer
    Private gameEngine As GameEngine
    Private player As Player
    Private obstacleManager As ObstacleManager
    Private renderer As GameRenderer

    ' Constantes de juego
    Private Const GROUND_Y As Integer = 260
    Private Const PLAYER_WIDTH As Integer = 40
    Private Const PLAYER_HEIGHT As Integer = 60
    Private Const OBSTACLE_BASE_SPEED As Integer = 8

    ' Variable para controlar primera ejecución
    Private Shared firstTimeRun As Boolean = True
    Private Shared highScore As Integer = 0
#End Region

#Region "Constructor e Inicialización"
    Public Sub New()
        ' Configuración inicial de la aplicación
        InitializeApplication()

        ' Inicializar componentes del formulario
        InitializeComponent()

        ' Inicializar el juego
        InitializeGame()
    End Sub

    Private Sub InitializeApplication()
        ' Configuración global de la aplicación
        Application.EnableVisualStyles()
        'Application.SetCompatibleTextRenderingDefault(False)
    End Sub

    Private Sub InitializeComponent()
        ' Crear e inicializar el timer
        Me.gameTimer = New Timer()

        ' Configuración del timer
        Me.gameTimer.Interval = 20 ' 50 FPS
        Me.gameTimer.Enabled = False

        ' Configuración del formulario
        Me.SuspendLayout()

        Me.ClientSize = New Size(800, 400)
        Me.Text = "Juego Estilo Dino"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.KeyPreview = True
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or
                   ControlStyles.UserPaint Or
                   ControlStyles.DoubleBuffer Or
                   ControlStyles.ResizeRedraw, True)
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = True
        Me.Name = "DinoGameForm"

        ' Configurar eventos
        AddHandler Me.Paint, AddressOf OnPaint
        AddHandler Me.KeyDown, AddressOf OnKeyDown
        AddHandler Me.FormClosing, AddressOf OnFormClosing
        AddHandler Me.Load, AddressOf OnFormLoad

        Me.ResumeLayout(False)
    End Sub

    Private Sub InitializeGame()
        Try
            ' Inicializar componentes del juego
            gameEngine = New GameEngine()
            player = New Player(50, GROUND_Y - PLAYER_HEIGHT, PLAYER_WIDTH, PLAYER_HEIGHT, GROUND_Y)
            obstacleManager = New ObstacleManager(Me.Width, GROUND_Y, OBSTACLE_BASE_SPEED)
            renderer = New GameRenderer()

            ' Suscribirse a eventos del motor de juego
            AddHandler gameEngine.GameOver, AddressOf OnGameOver
            AddHandler gameEngine.LevelChanged, AddressOf OnLevelChanged

        Catch ex As Exception
            MessageBox.Show($"Error al inicializar el juego: {ex.Message}",
                          "Error de Inicialización",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Error)
            Application.Exit()
        End Try
    End Sub
#End Region

#Region "Eventos del Formulario"
    Private Sub OnFormLoad(sender As Object, e As EventArgs)
        ' Mostrar mensaje de bienvenida la primera vez
        If firstTimeRun Then
            ShowWelcomeMessage()
            firstTimeRun = False
        End If

        ' Iniciar el juego
        StartGame()
    End Sub

    Private Sub OnFormClosing(sender As Object, e As FormClosingEventArgs)
        ' Pausar el juego antes de cerrar
        If gameTimer IsNot Nothing Then
            gameTimer.Stop()
        End If

        ' Confirmar salida si el juego está en progreso
        If gameEngine IsNot Nothing AndAlso gameEngine.IsRunning AndAlso gameEngine.Score > 0 Then
            Dim result = MessageBox.Show("¿Estás seguro de que quieres salir? Se perderá el progreso actual.",
                                       "Confirmar Salida",
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Question)
            If result = DialogResult.No Then
                e.Cancel = True
                If gameTimer IsNot Nothing Then
                    gameTimer.Start() ' Reanudar el juego
                End If
                Return
            End If
        End If

        ' Guardar configuraciones
        SaveGameSettings()
    End Sub
#End Region

#Region "Lógica del Juego"
    Private Sub StartGame()
        If gameTimer IsNot Nothing Then
            gameTimer.Start()
        End If
        Me.Focus() ' Asegurar que el formulario tenga el foco para capturar teclas
    End Sub

    Private Sub GameTimer_Tick(sender As Object, e As EventArgs) Handles gameTimer.Tick
        If gameEngine IsNot Nothing AndAlso gameEngine.IsRunning Then
            UpdateGame()
        End If
        Me.Invalidate()
    End Sub

    Private Sub UpdateGame()
        Try
            ' Actualizar jugador
            If player IsNot Nothing Then
                player.Update()
            End If

            ' Actualizar obstáculos
            If obstacleManager IsNot Nothing Then
                obstacleManager.Update(gameEngine.GameSpeed)
            End If

            ' Verificar colisiones
            If obstacleManager IsNot Nothing AndAlso player IsNot Nothing AndAlso
               obstacleManager.CheckCollisionWith(player) Then
                gameEngine.EndGame()
                Return
            End If

            ' Actualizar puntuación
            gameEngine.UpdateScore(1)

        Catch ex As Exception
            ' Manejar errores durante el juego
            gameEngine.EndGame()
            MessageBox.Show($"Error durante el juego: {ex.Message}",
                          "Error",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Warning)
        End Try
    End Sub

    Private Sub RestartGame()
        Try
            If gameEngine IsNot Nothing Then
                gameEngine.ResetGame()
            End If

            player = New Player(50, GROUND_Y - PLAYER_HEIGHT, PLAYER_WIDTH, PLAYER_HEIGHT, GROUND_Y)

            If obstacleManager IsNot Nothing Then
                obstacleManager.Clear()
            End If

            If gameTimer IsNot Nothing Then
                gameTimer.Start()
            End If

        Catch ex As Exception
            MessageBox.Show($"Error al reiniciar el juego: {ex.Message}",
                          "Error",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Error)
        End Try
    End Sub
#End Region

#Region "Manejo de Eventos de Juego"
    Private Sub OnGameOver()
        If gameTimer IsNot Nothing Then
            gameTimer.Stop()
        End If

        ' Verificar si es un nuevo récord
        CheckHighScore()
    End Sub

    Private Sub OnLevelChanged(newLevel As Integer)
        ' Aumentar dificultad reduciendo el delay de spawn
        If obstacleManager IsNot Nothing Then
            obstacleManager.SpawnDelay = Math.Max(50, 100 - (newLevel * 10))
        End If

        ' Efectos visuales para cambio de nivel
        ShowLevelUpMessage(newLevel)
    End Sub
#End Region

#Region "Manejo de Entrada"
    Private Sub OnKeyDown(sender As Object, e As KeyEventArgs)
        Try
            Select Case e.KeyCode
                Case Keys.Space, Keys.Up, Keys.W
                    If gameEngine IsNot Nothing Then
                        If gameEngine.IsRunning Then
                            If player IsNot Nothing Then
                                player.Jump()
                            End If
                        Else
                            RestartGame()
                        End If
                    End If

                Case Keys.Escape
                    Me.Close()

                Case Keys.P ' Pausa
                    If gameEngine IsNot Nothing AndAlso gameEngine.IsRunning Then
                        TogglePause()
                    End If

                Case Keys.F11 ' Pantalla completa (opcional)
                    ToggleFullscreen()

            End Select

        Catch ex As Exception
            ' Manejar errores de entrada
            System.Diagnostics.Debug.WriteLine($"Error en OnKeyDown: {ex.Message}")
        End Try
    End Sub
#End Region

#Region "Renderizado"
    Private Sub OnPaint(sender As Object, e As PaintEventArgs)
        Try
            Dim g As Graphics = e.Graphics

            ' Mejorar calidad de renderizado
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias

            If renderer IsNot Nothing Then
                ' Renderizar fondo y suelo
                renderer.RenderBackground(g, Me.Width, Me.Height, GROUND_Y)

                ' Renderizar jugador
                If gameEngine IsNot Nothing Then
                    If gameEngine.IsRunning Then
                        If player IsNot Nothing Then
                            player.Render(g)
                        End If
                    ElseIf (DateTime.Now.Millisecond \ 200) Mod 2 = 0 Then
                        ' Parpadeo cuando está muerto
                        If player IsNot Nothing Then
                            player.Render(g)
                        End If
                    End If
                End If

                ' Renderizar obstáculos
                If obstacleManager IsNot Nothing Then
                    obstacleManager.Render(g)
                End If

                ' Renderizar UI
                If gameEngine IsNot Nothing Then
                    renderer.RenderUI(g, gameEngine, Me.Width, Me.Height)
                End If

                ' Renderizar game over si corresponde
                If gameEngine IsNot Nothing AndAlso Not gameEngine.IsRunning Then
                    renderer.RenderGameOver(g, Me.Width, Me.Height)
                End If
            End If

        Catch ex As Exception
            ' Manejar errores de renderizado
            System.Diagnostics.Debug.WriteLine($"Error en OnPaint: {ex.Message}")
        End Try
    End Sub
#End Region

#Region "Métodos Auxiliares"
    Private Sub ShowWelcomeMessage()
        MessageBox.Show("¡Bienvenido al Juego del Dinosaurio!" & vbCrLf & vbCrLf &
                       "Controles:" & vbCrLf &
                       "• ESPACIO/W/↑: Saltar" & vbCrLf &
                       "• P: Pausar" & vbCrLf &
                       "• ESC: Salir" & vbCrLf & vbCrLf &
                       "¡Evita los obstáculos y obtén la mayor puntuación!",
                       "Juego del Dinosaurio",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Information)
    End Sub

    Private Sub CheckHighScore()
        If gameEngine IsNot Nothing Then
            Dim currentScore = gameEngine.Score

            If currentScore > highScore Then
                highScore = currentScore

                MessageBox.Show($"¡Nuevo Récord! Puntuación: {currentScore}",
                              "¡Felicitaciones!",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Exclamation)
            End If
        End If
    End Sub

    Private Sub ShowLevelUpMessage(level As Integer)
        ' Implementar notificación visual de cambio de nivel
        System.Diagnostics.Debug.WriteLine($"¡Nivel {level}!")
    End Sub

    Private Sub TogglePause()
        If gameTimer IsNot Nothing Then
            If gameTimer.Enabled Then
                gameTimer.Stop()
                ' Mostrar mensaje de pausa
                Me.Text = "Juego del Dinosaurio - PAUSADO"
            Else
                gameTimer.Start()
                Me.Text = "Juego Estilo Dinosaurio - VB.NET"
            End If
        End If
    End Sub

    Private Sub ToggleFullscreen()
        If Me.WindowState = FormWindowState.Normal Then
            Me.FormBorderStyle = FormBorderStyle.None
            Me.WindowState = FormWindowState.Maximized
        Else
            Me.FormBorderStyle = FormBorderStyle.FixedSingle
            Me.WindowState = FormWindowState.Normal
        End If
    End Sub

    Private Sub SaveGameSettings()
        Try
            ' Guardar configuraciones del juego
            System.Diagnostics.Debug.WriteLine($"Récord guardado: {highScore}")
        Catch ex As Exception
            ' Error al guardar configuraciones
            System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}")
        End Try
    End Sub
#End Region

#Region "Liberación de Recursos"
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing Then
                If gameTimer IsNot Nothing Then
                    gameTimer.Stop()
                    gameTimer.Dispose()
                    gameTimer = Nothing
                End If

                ' Limpiar referencias
                renderer = Nothing
                gameEngine = Nothing
                player = Nothing
                obstacleManager = Nothing
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub
#End Region

End Class