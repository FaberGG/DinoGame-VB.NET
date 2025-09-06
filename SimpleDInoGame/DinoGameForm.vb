
' ====================================================================
' CLASE DINOGAMEFORM (FORMULARIO PRINCIPAL)
' ====================================================================
' Propósito: Controla el bucle principal del juego, interfaz de usuario 
' y coordina la interacción entre todas las demás clases del sistema.
' 
' Responsabilidades:
' - Configurar y mantener el formulario de Windows Forms
' - Ejecutar el bucle principal del juego a 50 FPS
' - Coordinar actualizaciones de jugador y obstáculos
' - Manejar entrada del usuario (teclado)
' - Renderizar todos los elementos gráficos
' - Gestionar estados del juego (corriendo/game over)
' - Controlar sistema de puntuación y progresión de dificultad
' ====================================================================
Imports System.Drawing
Imports System.Windows.Forms

Public Class DinoGameForm
    Inherits Form

#Region "Variables del Juego"
    ' Timer principal que controla el bucle del juego a 50 FPS (20ms por frame)
    Private WithEvents gameTimer As Timer

    ' Instancias de las clases principales del juego
    Private player As Player                    ' El jugador (dinosaurio)
    Private obstacleManager As ObstacleManager  ' Gestor de todos los obstáculos

    ' Variables de estado del juego
    Private score As Integer = 0                ' Puntuación actual del jugador
    Private isGameRunning As Boolean = True     ' Estado del juego (True = corriendo)
    Private gameSpeed As Single = 1.0F          ' Multiplicador de velocidad (aumenta con tiempo)

    ' Constantes que definen las características básicas del juego
    Private Const GROUND_Y As Integer = 260     ' Altura del suelo desde arriba (píxeles)
    Private Const PLAYER_WIDTH As Integer = 40  ' Ancho del jugador en píxeles
    Private Const PLAYER_HEIGHT As Integer = 60 ' Alto del jugador en píxeles
    Private Const BASE_SPEED As Single = 8.0F   ' Velocidad base de movimiento de obstáculos
#End Region

#Region "Constructor e Inicialización"
    ' Constructor principal: configura el formulario y comienza el juego
    Public Sub New()
        InitializeComponent()   ' Configurar interfaz de usuario
        InitializeGame()       ' Inicializar variables del juego
        StartGame()           ' Comenzar el bucle principal
    End Sub

    ' Configura todos los aspectos visuales y técnicos del formulario
    Private Sub InitializeComponent()
        ' Configuración del timer principal del juego
        Me.gameTimer = New Timer()
        Me.gameTimer.Interval = 20      ' 20ms = 50 FPS para jugabilidad fluida
        Me.gameTimer.Enabled = False    ' Inicialmente desactivado

        ' Configuración de la ventana del juego
        Me.ClientSize = New Size(800, 400)                  ' Tamaño de ventana
        Me.Text = "Juego Dinosaurio"                        ' Título de ventana
        Me.StartPosition = FormStartPosition.CenterScreen   ' Centrar en pantalla
        Me.KeyPreview = True                                ' Capturar eventos de teclado

        ' Optimizaciones de renderizado para mejor rendimiento
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or   ' Pintar en WmPaint únicamente
                   ControlStyles.UserPaint Or               ' Control total del pintado
                   ControlStyles.DoubleBuffer, True)        ' Buffer doble para evitar parpadeo

        ' Configuración de ventana fija (no redimensionable)
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.MaximizeBox = False

        ' Registro de manejadores de eventos
        AddHandler Me.Paint, AddressOf OnPaint      ' Evento de renderizado
        AddHandler Me.KeyDown, AddressOf OnKeyDown  ' Evento de teclas presionadas
    End Sub

    ' Inicializa o reinicia todas las variables del juego a su estado inicial
    Private Sub InitializeGame()
        ' Crear jugador en posición inicial (50px desde izquierda, sobre el suelo)
        player = New Player(50, GROUND_Y - PLAYER_HEIGHT, PLAYER_WIDTH, PLAYER_HEIGHT, GROUND_Y)

        ' Crear gestor de obstáculos con parámetros de pantalla y velocidad
        obstacleManager = New ObstacleManager(Me.Width, GROUND_Y, BASE_SPEED)

        ' Resetear variables de juego
        score = 0                   ' Puntuación inicial
        isGameRunning = True        ' Estado de juego activo
        gameSpeed = 1.0F           ' Velocidad inicial normal
    End Sub
#End Region

#Region "Lógica del Juego"
    ' Inicia el bucle principal del juego
    Private Sub StartGame()
        gameTimer.Start()   ' Activar timer para comenzar actualizaciones
        Me.Focus()          ' Dar foco al formulario para capturar teclas
    End Sub

    ' Manejador del timer: se ejecuta 50 veces por segundo (cada 20ms)
    ' Controla el bucle principal de actualización y renderizado
    Private Sub GameTimer_Tick(sender As Object, e As EventArgs) Handles gameTimer.Tick
        If isGameRunning Then
            UpdateGame()        ' Actualizar lógica del juego solo si está corriendo
        End If
        Me.Invalidate()        ' Forzar redibujado de la pantalla
    End Sub

    ' Actualiza toda la lógica del juego en cada frame
    Private Sub UpdateGame()
        ' Actualizar física y estado del jugador (gravedad, saltos)
        player.Update()

        ' Actualizar todos los obstáculos con velocidad actual del juego
        obstacleManager.Update(gameSpeed)

        ' Verificar si hay colisión entre jugador y algún obstáculo
        If obstacleManager.CheckCollisionWith(player) Then
            GameOver()      ' Terminar juego si hay colisión
            Return          ' Salir inmediatamente para evitar más actualizaciones
        End If

        ' Sistema de progresión: aumentar puntuación y velocidad
        score += 1      ' Incrementar puntuación cada frame

        ' Fórmula de progresión de dificultad: velocidad aumenta gradualmente
        ' Cada 1000 puntos aumenta la velocidad en 1x (score/1000 = incremento)
        gameSpeed = 1.0F + (score / 1000.0F)
    End Sub

    ' Reinicia completamente el juego tras un game over
    Private Sub RestartGame()
        InitializeGame()        ' Resetear todas las variables
        gameTimer.Start()       ' Reactivar el timer del juego
    End Sub

    ' Termina el juego actual y entra en estado de game over
    Private Sub GameOver()
        isGameRunning = False   ' Marcar juego como terminado
        gameTimer.Stop()        ' Detener actualizaciones del juego
    End Sub
#End Region

#Region "Manejo de Entrada"
    ' Procesa las teclas presionadas por el usuario
    ' Maneja controles de juego y opciones del sistema
    Private Sub OnKeyDown(sender As Object, e As KeyEventArgs)
        Select Case e.KeyCode
            Case Keys.Space, Keys.Up    ' Teclas de salto
                If isGameRunning Then
                    player.Jump()       ' Saltar durante el juego
                Else
                    RestartGame()       ' Reiniciar tras game over
                End If
            Case Keys.Escape           ' Tecla de salida
                Me.Close()             ' Cerrar aplicación
        End Select
    End Sub
#End Region

#Region "Renderizado"
    ' Método principal de renderizado: dibuja todos los elementos del juego
    ' Se ejecuta cada vez que la ventana necesita redibujarse
    Private Sub OnPaint(sender As Object, e As PaintEventArgs)
        Dim g As Graphics = e.Graphics

        ' Renderizar elementos en orden de capas (fondo a frente)
        RenderBackground(g)     ' Cielo y suelo

        ' Renderizar jugador con efecto de parpadeo durante game over
        ' El parpadeo se logra alternando la visibilidad cada 200ms
        If isGameRunning OrElse (DateTime.Now.Millisecond \ 200) Mod 2 = 0 Then
            player.Render(g)
        End If

        obstacleManager.Render(g)   ' Todos los obstáculos
        RenderUI(g)                 ' Interfaz de usuario (puntuación, velocidad)

        ' Renderizar overlay de game over si el juego ha terminado
        If Not isGameRunning Then
            RenderGameOver(g)
        End If
    End Sub

    ' Dibuja el fondo del juego (cielo y suelo)
    Private Sub RenderBackground(g As Graphics)
        ' Cielo celeste que cubre toda la pantalla
        g.FillRectangle(Brushes.LightBlue, 0, 0, Me.Width, Me.Height)

        ' Suelo verde desde GROUND_Y hasta el fondo
        g.FillRectangle(Brushes.Green, 0, GROUND_Y, Me.Width, Me.Height - GROUND_Y)
    End Sub

    ' Renderiza la interfaz de usuario (HUD)
    Private Sub RenderUI(g As Graphics)
        Using font As New Font("Arial", 16, FontStyle.Bold)
            ' Mostrar puntuación actual
            g.DrawString($"Puntuación: {score}", font, Brushes.Black, 10, 10)

            ' Mostrar velocidad actual con formato de 1 decimal
            g.DrawString($"Velocidad: {gameSpeed:F1}x", font, Brushes.Black, 10, 40)
        End Using
    End Sub

    ' Renderiza la pantalla de game over con overlay y texto centrado
    Private Sub RenderGameOver(g As Graphics)
        ' Crear overlay semi-transparente oscuro sobre toda la pantalla
        Using overlay As New SolidBrush(Color.FromArgb(128, Color.Black))
            g.FillRectangle(overlay, 0, 0, Me.Width, Me.Height)
        End Using

        ' Textos centrados con diferentes tamaños de fuente
        Using gameOverFont As New Font("Arial", 24, FontStyle.Bold)
            Using instructionFont As New Font("Arial", 16)
                ' Definir textos a mostrar
                Dim gameOverText As String = "¡GAME OVER!"
                Dim restartText As String = "Presiona ESPACIO para reiniciar"

                ' Calcular tamaños de texto para centrado perfecto
                Dim gameOverSize = g.MeasureString(gameOverText, gameOverFont)
                Dim restartSize = g.MeasureString(restartText, instructionFont)

                ' Calcular centro de la pantalla
                Dim centerX As Single = CSng(Me.Width) / 2.0F
                Dim centerY As Single = CSng(Me.Height) / 2.0F

                ' Dibujar texto "GAME OVER" centrado verticalmente arriba
                g.DrawString(gameOverText, gameOverFont, Brushes.White,
                           centerX - gameOverSize.Width / 2.0F, centerY - 40.0F)

                ' Dibujar instrucciones centradas verticalmente abajo
                g.DrawString(restartText, instructionFont, Brushes.White,
                           centerX - restartSize.Width / 2.0F, centerY + 20.0F)
            End Using
        End Using
    End Sub
#End Region

End Class
