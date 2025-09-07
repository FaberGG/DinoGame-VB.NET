' ====================================================================
' CLASE PLAYER (ACTUALIZADA CON SPRITES)
' ====================================================================
' Propósito: Representa al jugador (dinosaurio) con sprites animados y colisiones precisas
' 
' Nuevas características:
' - Sprites escalados para mejor visualización
' - Colisiones basadas en el tamaño real del sprite
' - Animación fluida entre correr y saltar
' ====================================================================
Public Class Player
    ' Variables de posición y dimensiones ORIGINALES del juego
    Private x As Integer, y As Integer          ' Posición actual del jugador
    Private width As Integer, height As Integer ' Dimensiones LÓGICAS para colisiones
    Private groundY As Integer                  ' Nivel del suelo (referencia)

    ' Variables de estado y física de salto
    Private isJumping As Boolean = False        ' True cuando está en el aire
    Private jumpSpeed As Single = 0            ' Velocidad vertical actual

    ' Constantes físicas para mecánicas de salto realistas
    Private Const GRAVITY As Integer = 2        ' Fuerza de gravedad por frame
    Private Const JUMP_POWER As Integer = 24    ' Impulso inicial del salto

    ' Variables para sprites del dinosaurio
    Private dinoRunSprite1 As Image            ' Primer frame de correr
    Private dinoRunSprite2 As Image            ' Segundo frame de correr
    Private dinoJumpSprite As Image             ' Sprite de salto
    Private animationTimer As Integer = 0       ' Contador para animación
    Private currentFrame As Integer = 0         ' Frame actual de animación

    ' Tamaños para renderizado (más grandes que las colisiones)
    Private Const SPRITE_RENDER_WIDTH As Integer = 64     ' Ancho visual del sprite
    Private Const SPRITE_RENDER_HEIGHT As Integer = 96    ' Alto visual del sprite

    ' Constructor: Inicializa el jugador con posición, tamaño LÓGICO y referencia del suelo
    ' Las dimensiones w,h son para COLISIONES, los sprites se renderizan más grandes
    Public Sub New(startX As Integer, startY As Integer, w As Integer, h As Integer, ground As Integer)
        x = startX
        y = startY
        width = w       ' Tamaño de colisión (ej: 40x60)
        height = h      ' Tamaño de colisión (ej: 40x60)
        groundY = ground

        ' Cargar sprites del dinosaurio
        LoadDinoSprites()
    End Sub

    ' Carga todos los sprites del dinosaurio desde recursos
    Private Sub LoadDinoSprites()
        Try
            dinoRunSprite1 = My.Resources.dino_run_1
            dinoRunSprite2 = My.Resources.dino_run_2
            dinoJumpSprite = My.Resources.dino_jump
        Catch ex As Exception
            ' Si falla la carga, los sprites quedan Nothing (fallback a rectángulo)
            dinoRunSprite1 = Nothing
            dinoRunSprite2 = Nothing
            dinoJumpSprite = Nothing
            System.Diagnostics.Debug.WriteLine("Error cargando sprites del dino: " & ex.Message)
        End Try
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

        ' Actualizar animación solo si está corriendo (no saltando)
        If Not isJumping Then
            animationTimer += 1
            If animationTimer > 10 Then  ' Cambiar frame cada 10 updates (5 FPS de animación)
                currentFrame = (currentFrame + 1) Mod 2
                animationTimer = 0
            End If
        End If
    End Sub

    ' Renderiza el jugador usando sprites escalados o fallback a rectángulo
    Public Sub Render(g As Graphics)
        Dim currentSprite As Image = Nothing

        ' Seleccionar sprite según estado
        If isJumping Then
            currentSprite = dinoJumpSprite
        Else
            ' Alternar entre sprites de correr para animación
            currentSprite = If(currentFrame = 0, dinoRunSprite1, dinoRunSprite2)
        End If

        ' Renderizar sprite o fallback
        If currentSprite IsNot Nothing Then
            ' Calcular posición centrada para que el sprite más grande se vea bien
            Dim renderX As Integer = x - (SPRITE_RENDER_WIDTH - width) \ 2
            Dim renderY As Integer = y - (SPRITE_RENDER_HEIGHT - height)

            ' Dibujar sprite escalado al tamaño de renderizado
            g.DrawImage(currentSprite, renderX, renderY, SPRITE_RENDER_WIDTH, SPRITE_RENDER_HEIGHT)

            ' OPCIONAL: Dibujar rectángulo de colisión para debug (comentar en producción)
            'g.DrawRectangle(Pens.Yellow, x, y, width, height)
        Else
            ' Fallback al rectángulo azul original
            g.FillRectangle(Brushes.Blue, x, y, width, height)
        End If
    End Sub

    ' Retorna el rectángulo de colisión LÓGICO (no el visual)
    ' Esto mantiene las colisiones justas independientemente del tamaño del sprite
    Public Function GetBounds() As Rectangle
        Return New Rectangle(x, y, width, height)
    End Function

    ' Liberar recursos del jugador
    Public Sub DisposeResources()
        dinoRunSprite1?.Dispose()
        dinoRunSprite2?.Dispose()
        dinoJumpSprite?.Dispose()
    End Sub
End Class