Public MustInherit Class GameObject
    Private _x As Integer
    Private _y As Integer
    Private _width As Integer
    Private _height As Integer
    Private _color As Color
    Private _visible As Boolean

    ' Propiedades
    Public Property X As Integer
        Get
            Return _x
        End Get
        Set(value As Integer)
            _x = value
        End Set
    End Property

    Public Property Y As Integer
        Get
            Return _y
        End Get
        Set(value As Integer)
            _y = value
        End Set
    End Property

    Public Property Width As Integer
        Get
            Return _width
        End Get
        Set(value As Integer)
            _width = value
        End Set
    End Property

    Public Property Height As Integer
        Get
            Return _height
        End Get
        Set(value As Integer)
            _height = value
        End Set
    End Property

    Public Property Color As Color
        Get
            Return _color
        End Get
        Set(value As Color)
            _color = value
        End Set
    End Property

    Public Property Visible As Boolean
        Get
            Return _visible
        End Get
        Set(value As Boolean)
            _visible = value
        End Set
    End Property

    Public ReadOnly Property Bounds As Rectangle
        Get
            Return New Rectangle(_x, _y, _width, _height)
        End Get
    End Property

    Public Sub New(x As Integer, y As Integer, width As Integer, height As Integer)
        _x = x
        _y = y
        _width = width
        _height = height
        _visible = True
        _color = Color.Black
    End Sub

    Public Overridable Sub Update()
        ' Lógica base de actualización
    End Sub

    Public Overridable Sub Render(g As Graphics)
        If _visible Then
            Using brush As New SolidBrush(_color)
                g.FillRectangle(brush, _x, _y, _width, _height)
            End Using
        End If
    End Sub

    Public Function CollidesWith(other As GameObject) As Boolean
        Return Me.Bounds.IntersectsWith(other.Bounds)
    End Function
End Class
