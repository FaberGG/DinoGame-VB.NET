' ====================================================================
' CLASE OBSTACLE (CORREGIDA - SIN DESPLAZAMIENTO EN EJE X)
' ====================================================================
' Solución: Ajustar tanto hitbox como posición base al seleccionar sprite
' ====================================================================
Public Class Obstacle
    ' Variables de posición y dimensiones LÓGICAS (para colisiones)
    Private x As Integer, y As Integer          ' Posición actual (x,y)
    Private width As Integer, height As Integer ' Dimensiones de COLISIÓN
    Private originalX As Integer, originalY As Integer ' Posición original para referencia

    ' Variable de movimiento
    Private speed As Single                     ' Velocidad horizontal en píxeles/frame

    ' Variable para el sprite del cactus
    Private cactusSprite As Image              ' Imagen del cactus a renderizar
    Private cactusType As Integer              ' Tipo de cactus (para tamaños diferentes)

    ' Array estático de sprites de cactus
    Private Shared cactusSprites() As Image
    Private Shared isSpritesLoaded As Boolean = False

    ' Definir diferentes tamaños de renderizado para variedad
    Private Shared ReadOnly CactusRenderSizes() As Size = {
        New Size(32, 64),   ' Cactus pequeño
        New Size(40, 80),   ' Cactus mediano
        New Size(48, 96)    ' Cactus grande
    }

    ' Constructor con dimensiones lógicas para colisiones
    Public Sub New(startX As Integer, startY As Integer, w As Integer, h As Integer, obstacleSpeed As Single)
        ' Guardar posición original
        originalX = startX
        originalY = startY

        ' Inicializar con dimensiones temporales
        x = startX
        y = startY
        width = w
        height = h
        speed = obstacleSpeed

        ' Cargar sprites si es necesario
        If Not isSpritesLoaded Then
            LoadCactusSprites()
        End If

        ' Seleccionar sprite y AJUSTAR posición y dimensiones
        SelectRandomCactusSpriteAndAdjust()
    End Sub

    ' Carga todos los sprites de cactus una sola vez
    Private Shared Sub LoadCactusSprites()
        Try
            cactusSprites = {
                My.Resources.cactus1,      ' Cactus tipo 1
                My.Resources.cactus2,      ' Cactus tipo 2
                My.Resources.cactus3       ' Cactus tipo 3
            }
            isSpritesLoaded = True
        Catch ex As Exception
            cactusSprites = Nothing
            isSpritesLoaded = False
            System.Diagnostics.Debug.WriteLine("Error cargando sprites de cactus: " & ex.Message)
        End Try
    End Sub

    ' VERSIÓN CORREGIDA: Ajusta sprite, hitbox Y posición
    Private Sub SelectRandomCactusSpriteAndAdjust()
        Try
            If cactusSprites IsNot Nothing AndAlso cactusSprites.Length > 0 Then
                Dim random As New Random()

                ' Seleccionar tipo de cactus aleatorio
                cactusType = random.Next(0, Math.Min(cactusSprites.Length, CactusRenderSizes.Length))
                cactusSprite = cactusSprites(cactusType)

                ' Obtener tamaño de renderizado
                Dim renderSize As Size = CactusRenderSizes(cactusType)

                ' AJUSTAR HITBOX AL SPRITE (80% ancho, 90% alto para colisión más justa)
                Dim newWidth As Integer = CInt(renderSize.Width * 0.8)
                Dim newHeight As Integer = CInt(renderSize.Height * 0.9)

                ' CALCULAR NUEVA POSICIÓN PARA MANTENER ALINEACIÓN
                ' Mantener el punto inferior del cactus en el mismo lugar
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

    ' Renderiza con sprite escalado (SIN cálculo de centrado adicional)
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