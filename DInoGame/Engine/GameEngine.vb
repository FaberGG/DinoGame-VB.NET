Imports System.Drawing

Public Class GameEngine
    Private _isRunning As Boolean
    Private _score As Integer
    Private _gameSpeed As Single
    Private _level As Integer

    Public Event ScoreChanged(newScore As Integer)
    Public Event GameSpeedChanged(newSpeed As Single)
    Public Event LevelChanged(newLevel As Integer)
    Public Event GameOver()

    Public Property IsRunning As Boolean
        Get
            Return _isRunning
        End Get
        Set(value As Boolean)
            _isRunning = value
        End Set
    End Property

    Public ReadOnly Property Score As Integer
        Get
            Return _score
        End Get
    End Property

    Public ReadOnly Property GameSpeed As Single
        Get
            Return _gameSpeed
        End Get
    End Property

    Public ReadOnly Property Level As Integer
        Get
            Return _level
        End Get
    End Property

    Public Sub New()
        ResetGame()
    End Sub

    Public Sub ResetGame()
        _score = 0
        _gameSpeed = 1.0F
        _level = 1
        _isRunning = True
        RaiseEvent ScoreChanged(_score)
        RaiseEvent GameSpeedChanged(_gameSpeed)
        RaiseEvent LevelChanged(_level)
    End Sub

    Public Sub UpdateScore(points As Integer)
        _score += points
        RaiseEvent ScoreChanged(_score)

        ' Aumentar velocidad gradualmente
        Dim newSpeed As Single = 1.0F + (_score / 1000.0F)
        If newSpeed <> _gameSpeed Then
            _gameSpeed = Math.Min(newSpeed, 3.0F)
            RaiseEvent GameSpeedChanged(_gameSpeed)
        End If

        ' Cambiar nivel cada 1000 puntos
        Dim newLevel As Integer = (_score \ 1000) + 1
        If newLevel <> _level Then
            _level = newLevel
            RaiseEvent LevelChanged(_level)
        End If
    End Sub

    Public Sub EndGame()
        _isRunning = False
        RaiseEvent GameOver()
    End Sub
End Class