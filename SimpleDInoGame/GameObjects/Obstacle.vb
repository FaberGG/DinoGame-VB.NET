' ====================================================================
' CLASE OBSTACLE
' ====================================================================

Public Class Obstacle
    ' Constantes para tipos de obstáculos
    Public Const SMALL_CACTUS As Integer = 0
    Public Const MEDIUM_CACTUS As Integer = 1
    Public Const LARGE_CACTUS As Integer = 2

    ' Constantes para dimensiones base de hitbox
    Private Const BASE_HITBOX_WIDTH As Integer = 30
    Private Const BASE_HITBOX_HEIGHT As Integer = 60

    ' Constantes para ajuste de colisión
    Private Const COLLISION_WIDTH_FACTOR As Single = 0.8F
    Private Const COLLISION_HEIGHT_FACTOR As Single = 0.9F

    ' HITBOXES
    ' Variables de posición y dimensiones LÓGICAS (hitboxes)
    Private x As Integer, y As Integer          ' Posición actual (x,y) en la pantalla
    Private width As Integer, height As Integer ' Dimensiones de COLISIÓN
    Private originalX As Integer, originalY As Integer ' Posición original para referencia

    ' Variable de movimiento
    Private speed As Single                     ' Velocidad horizontal en píxeles/frame

    ' Variable para el sprite del cactus
    Private cactusSprite As Image              ' Imagen del cactus a renderizar
    Private cactusType As Integer              ' Tipo de cactus (para tamaños diferentes)

    ' Array estatic/shared de sprites de cactus
    Private Shared cactusSprites() As Image
    ' Indicador para evitar recargas múltiples
    Private Shared isSpritesLoaded As Boolean = False

    ' Definir diferentes tamaños de renderizado para variedad
    Private Shared ReadOnly CactusRenderSizes() As Size = {
        New Size(32, 64),   ' Cactus pequeño
        New Size(40, 80),   ' Cactus mediano
        New Size(48, 96)    ' Cactus grande
    }

    ' Constructor NUEVO - Recibe tipo específico de obstáculo
    Public Sub New(startX As Integer, startY As Integer, obstacleType As Integer, obstacleSpeed As Single)
        ' Guardar posición original
        originalX = startX
        originalY = startY

        ' Inicializar con dimensiones base
        x = startX
        y = startY
        width = BASE_HITBOX_WIDTH
        height = BASE_HITBOX_HEIGHT
        speed = obstacleSpeed

        ' Cargar sprites si es necesario
        If Not isSpritesLoaded Then
            LoadCactusSprites()
        End If

        ' Usar el tipo de cactus y ajusta posición/dimensiones
        SetCactusTypeAndAdjust(obstacleType)
    End Sub

    ' Carga todos los sprites de cactus una sola vez
    Private Shared Sub LoadCactusSprites()
        Try
            cactusSprites = {
                My.Resources.cactus1,      ' Cactus tipo 1
                My.Resources.cactus2,      ' Cactus tipo 2
                My.Resources.cactus3       ' Cactus tipo 3
            }
            isSpritesLoaded = True 'marcar como cargado
        Catch ex As Exception
            cactusSprites = Nothing
            isSpritesLoaded = False
            System.Diagnostics.Debug.WriteLine("Error cargando sprites de cactus: " & ex.Message)
        End Try
    End Sub

    ' NUEVO MÉTODO: Establece un tipo específico de cactus y ajusta dimensiones
    Private Sub SetCactusTypeAndAdjust(obstacleType As Integer)
        Try
            If cactusSprites IsNot Nothing AndAlso cactusSprites.Length > 0 Then
                ' Validar tipo y usar el especificado
                cactusType = Math.Max(0, Math.Min(obstacleType, Math.Min(cactusSprites.Length - 1, CactusRenderSizes.Length - 1)))
                cactusSprite = cactusSprites(cactusType)

                ' Obtener tamaño de renderizado
                Dim renderSize As Size = CactusRenderSizes(cactusType)

                ' AJUSTAR HITBOX AL SPRITE
                Dim newWidth As Integer = CInt(renderSize.Width * COLLISION_WIDTH_FACTOR)
                Dim newHeight As Integer = CInt(renderSize.Height * COLLISION_HEIGHT_FACTOR)

                ' CALCULAR NUEVA POSICIÓN PARA MANTENER ALINEACIÓN
                Dim newX As Integer = originalX - (newWidth - width) \ 2  ' Centrar horizontalmente
                Dim newY As Integer = originalY - (newHeight - height)    ' Alinear desde abajo

                ' Actualizar dimensiones y posición
                width = newWidth
                height = newHeight
                x = newX
                y = newY

            Else
                cactusSprite = Nothing
            End If
        Catch ex As Exception
            cactusSprite = Nothing
        End Try
    End Sub

    ' MÉTODO ORIGINAL: Selecciona un sprite aleatorio y ajusta posición y dimensiones
    Private Sub SelectRandomCactusSpriteAndAdjust()
        Try
            If cactusSprites IsNot Nothing AndAlso cactusSprites.Length > 0 Then
                Dim random As New Random()

                ' Seleccionar tipo de cactus aleatorio
                cactusType = random.Next(0, Math.Min(cactusSprites.Length, CactusRenderSizes.Length))
                cactusSprite = cactusSprites(cactusType)

                ' Obtener tamaño de renderizado
                Dim renderSize As Size = CactusRenderSizes(cactusType)

                ' AJUSTAR HITBOX AL SPRITE
                Dim newWidth As Integer = CInt(renderSize.Width * COLLISION_WIDTH_FACTOR)
                Dim newHeight As Integer = CInt(renderSize.Height * COLLISION_HEIGHT_FACTOR)

                ' CALCULAR NUEVA POSICIÓN PARA MANTENER ALINEACIÓN
                Dim newX As Integer = originalX - (newWidth - width) \ 2  ' Centrar horizontalmente
                Dim newY As Integer = originalY - (newHeight - height)    ' Alinear desde abajo

                ' Actualizar dimensiones y posición
                width = newWidth
                height = newHeight
                x = newX
                y = newY

            Else
                cactusSprite = Nothing
            End If
        Catch ex As Exception
            cactusSprite = Nothing
        End Try
    End Sub

    ' Actualiza posición
    Public Sub Update()
        x -= CInt(speed)
    End Sub

    ' Verifica si salió de pantalla
    Public Function IsOffScreen() As Boolean
        Return x + width < 0
    End Function

    ' Renderiza con sprite escalado
    Public Sub Render(g As Graphics)
        If cactusSprite IsNot Nothing Then
            Try
                ' Obtener tamaño de renderizado según el tipo
                Dim renderSize As Size = CactusRenderSizes(cactusType)

                ' POSICIÓN DE RENDERIZADO: Usar directamente x,y ajustada + centrado mínimo
                Dim renderX As Integer = x - (renderSize.Width - width) \ 2
                Dim renderY As Integer = y - (renderSize.Height - height)

                ' Dibujar sprite escalado
                g.DrawImage(cactusSprite, renderX, renderY, renderSize.Width, renderSize.Height)

                ' DEBUG: Mostrar hitbox (comentar en producción)
                'g.DrawRectangle(Pens.Red, x, y, width, height)

            Catch ex As Exception
                RenderFallback(g)
            End Try
        Else
            RenderFallback(g)
        End If
    End Sub

    ' Fallback a rectángulo rojo
    Private Sub RenderFallback(g As Graphics)
        g.FillRectangle(Brushes.Red, x, y, width, height)
    End Sub

    ' Retorna rectángulo de colisión LÓGICO
    Public Function GetBounds() As Rectangle
        Return New Rectangle(x, y, width, height)
    End Function

    ' Liberar recursos estáticos
    Public Shared Sub DisposeResources()
        If cactusSprites IsNot Nothing Then
            For Each sprite In cactusSprites
                sprite?.Dispose()
            Next
            cactusSprites = Nothing
            isSpritesLoaded = False
        End If
    End Sub
End Class