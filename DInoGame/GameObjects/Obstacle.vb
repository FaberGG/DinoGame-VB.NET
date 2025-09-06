Public Class Obstacle
    Inherits GameObject

    Private _speed As Single

    Public Property Speed As Single
        Get
            Return _speed
        End Get
        Set(value As Single)
            _speed = value
        End Set
    End Property

    Public Sub New(x As Integer, y As Integer, width As Integer, height As Integer, speed As Single)
        MyBase.New(x, y, width, height)
        _speed = speed
        Me.Color = Drawing.Color.Red
    End Sub

    Public Overrides Sub Update()
        Me.X -= CInt(_speed)
        MyBase.Update()
    End Sub

    Public Function IsOffScreen() As Boolean
        Return Me.X + Me.Width < 0
    End Function
End Class