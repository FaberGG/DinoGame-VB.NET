Imports System.Drawing

Public Class GameRenderer
    Implements IDisposable

    Private _backgroundBrush As SolidBrush
    Private _groundBrush As SolidBrush
    Private _scoreFont As Font
    Private _gameOverFont As Font

    Public Sub New()
        _backgroundBrush = New SolidBrush(Color.LightBlue)
        _groundBrush = New SolidBrush(Color.Green)
        _scoreFont = New Font("Arial", 16, FontStyle.Bold)
        _gameOverFont = New Font("Arial", 24, FontStyle.Bold)
    End Sub

    Public Sub RenderBackground(g As Graphics, width As Integer, height As Integer, groundY As Integer)
        ' Fondo
        g.FillRectangle(_backgroundBrush, 0, 0, width, height)
        ' Suelo
        g.FillRectangle(_groundBrush, 0, groundY, width, height - groundY)
    End Sub

    Public Sub RenderUI(g As Graphics, engine As GameEngine, width As Integer, height As Integer)
        ' Información del juego
        g.DrawString($"Puntuación: {engine.Score}", _scoreFont, Brushes.Black, 10.0F, 10.0F)
        g.DrawString($"Nivel: {engine.Level}", _scoreFont, Brushes.Black, 10.0F, 35.0F)
        g.DrawString($"Velocidad: {engine.GameSpeed:F1}x", _scoreFont, Brushes.Black, 10.0F, 60.0F)

        ' Instrucciones para nuevos jugadores
        If engine.Score < 100 Then
            Using instructionFont As New Font("Arial", 10)
                g.DrawString("Presiona ESPACIO para saltar", instructionFont,
                            Brushes.Black, 10.0F, CSng(height - 40))
            End Using
        End If
    End Sub

    Public Sub RenderGameOver(g As Graphics, width As Integer, height As Integer)
        Dim gameOverText As String = "¡GAME OVER!"
        Dim restartText As String = "Presiona ESPACIO para reiniciar"
        Dim exitText As String = "Presiona ESC para salir"

        Dim gameOverSize As SizeF = g.MeasureString(gameOverText, _gameOverFont)
        Dim restartSize As SizeF = g.MeasureString(restartText, _scoreFont)
        Dim exitSize As SizeF = g.MeasureString(exitText, _scoreFont)

        Dim centerX As Single = CSng(width) / 2.0F
        Dim centerY As Single = CSng(height) / 2.0F

        ' Overlay semi-transparente
        Using overlay As New SolidBrush(Color.FromArgb(128, Color.Black))
            g.FillRectangle(overlay, 0, 0, width, height)
        End Using

        ' Textos centrados - Convertir explícitamente a Single
        Dim gameOverX As Single = centerX - (gameOverSize.Width / 2.0F)
        Dim gameOverY As Single = centerY - 60.0F

        Dim restartX As Single = centerX - (restartSize.Width / 2.0F)
        Dim restartY As Single = centerY

        Dim exitX As Single = centerX - (exitSize.Width / 2.0F)
        Dim exitTextY As Single = centerY + 30.0F

        g.DrawString(gameOverText, _gameOverFont, Brushes.White, gameOverX, gameOverY)
        g.DrawString(restartText, _scoreFont, Brushes.White, restartX, restartY)
        g.DrawString(exitText, _scoreFont, Brushes.White, exitX, exitTextY)
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If disposing Then
            _backgroundBrush?.Dispose()
            _groundBrush?.Dispose()
            _scoreFont?.Dispose()
            _gameOverFont?.Dispose()
        End If
    End Sub

    ' Implementar IDisposable correctamente
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
End Class