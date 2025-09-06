
' ====================================================================
' CLASE PLAYER
' ====================================================================
' Propósito: Representa al jugador (dinosaurio) con mecánicas de salto y gravedad
' 
' Responsabilidades:
' - Mantener posición y estado del jugador
' - Implementar física de salto realista con gravedad
' - Restringir movimiento al nivel del suelo
' - Proporcionar información de colisión
' - Renderizarse como rectángulo azul
' ====================================================================
Public Class Player
    ' Variables de posición y dimensiones
    Private x As Integer, y As Integer          ' Posición actual del jugador
    Private width As Integer, height As Integer ' Dimensiones del jugador
    Private groundY As Integer                  ' Nivel del suelo (referencia)

    ' Variables de estado y física de salto
    Private isJumping As Boolean = False        ' True cuando está en el aire
    Private jumpSpeed As Single = 0            ' Velocidad vertical actual

    ' Constantes físicas para mecánicas de salto realistas
    Private Const GRAVITY As Integer = 2        ' Fuerza de gravedad por frame
    Private Const JUMP_POWER As Integer = 20    ' Impulso inicial del salto

    ' Constructor: Inicializa el jugador con posición, tamaño y referencia del suelo
    ' startX, startY: Posición inicial
    ' w, h: Dimensiones del jugador
    ' ground: Nivel Y del suelo para cálculos de física
    Public Sub New(startX As Integer, startY As Integer, w As Integer, h As Integer, ground As Integer)
        x = startX
        y = startY
        width = w
        height = h
        groundY = ground
    End Sub

    ' Inicia un salto si el jugador está en el suelo
    ' Previene saltos múltiples en el aire
    Public Sub Jump()
        ' Solo permite saltar si no está saltando Y está en el suelo
        If Not isJumping AndAlso y >= groundY - height Then
            isJumping = True
            jumpSpeed = JUMP_POWER  ' Aplicar impulso inicial hacia arriba
        End If
    End Sub

    ' Actualiza la física del jugador (gravedad y movimiento vertical)
    ' Se ejecuta cada frame del juego
    Public Sub Update()
        ' Aplicar física solo si está saltando o en el aire
        If isJumping OrElse y < groundY - height Then
            jumpSpeed -= GRAVITY    ' La gravedad reduce la velocidad hacia arriba
            y -= CInt(jumpSpeed)   ' Actualizar posición vertical

            ' Verificar aterrizaje en el suelo
            If y >= groundY - height Then
                y = groundY - height    ' Posicionar exactamente en el suelo
                isJumping = False       ' Terminar estado de salto
                jumpSpeed = 0          ' Resetear velocidad vertical
            End If
        End If
    End Sub

    ' Renderiza el jugador como un rectángulo azul sólido
    Public Sub Render(g As Graphics)
        g.FillRectangle(Brushes.Blue, x, y, width, height)
    End Sub

    ' Retorna el rectángulo de colisión del jugador
    ' Usado para detectar colisiones con obstáculos
    Public Function GetBounds() As Rectangle
        Return New Rectangle(x, y, width, height)
    End Function
End Class