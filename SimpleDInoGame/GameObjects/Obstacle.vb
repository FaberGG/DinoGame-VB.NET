' ====================================================================
' CLASE OBSTACLE
' ====================================================================
' Propósito: Representa un obstáculo individual que se mueve horizontalmente
' de derecha a izquierda por la pantalla.
' 
' Responsabilidades:
' - Mantener posición y dimensiones del obstáculo
' - Moverse hacia la izquierda a velocidad constante
' - Proporcionar información de colisión (GetBounds)
' - Renderizarse como rectángulo rojo
' - Detectar cuando sale de pantalla para ser eliminado
' ====================================================================
Public Class Obstacle
    ' Variables de posición y dimensiones
    Private x As Integer, y As Integer          ' Posición actual (x,y)
    Private width As Integer, height As Integer ' Ancho y alto del obstáculo

    ' Variable de movimiento (Single es más eficiente que Double - 4 bytes vs 8 bytes)
    Private speed As Single                     ' Velocidad horizontal en píxeles/frame

    ' Constructor: Inicializa un obstáculo con posición, tamaño y velocidad específicos
    ' startX: Posición inicial X (normalmente fuera del borde derecho)
    ' startY: Posición inicial Y (normalmente a nivel del suelo)
    ' w, h: Dimensiones del obstáculo
    ' obstacleSpeed: Velocidad de movimiento hacia la izquierda
    Public Sub New(startX As Integer, startY As Integer, w As Integer, h As Integer, obstacleSpeed As Single)
        x = startX
        y = startY
        width = w
        height = h
        speed = obstacleSpeed
    End Sub

    ' Actualiza la posición del obstáculo moviéndolo hacia la izquierda
    ' Se llama en cada frame del juego (50 FPS)
    Public Sub Update()
        x -= CInt(speed)    ' Convertir Single a Integer para posición exacta
    End Sub

    ' Determina si el obstáculo ha salido completamente de la pantalla
    ' Retorna True cuando el borde derecho del obstáculo está fuera del borde izquierdo
    Public Function IsOffScreen() As Boolean
        Return x + width < 0
    End Function

    ' Renderiza el obstáculo como un rectángulo rojo sólido
    ' g: Contexto gráfico donde dibujar
    Public Sub Render(g As Graphics)
        g.FillRectangle(Brushes.Red, x, y, width, height)
    End Sub

    ' Retorna el rectángulo de colisión del obstáculo
    ' Usado por el sistema de detección de colisiones
    Public Function GetBounds() As Rectangle
        Return New Rectangle(x, y, width, height)
    End Function
End Class