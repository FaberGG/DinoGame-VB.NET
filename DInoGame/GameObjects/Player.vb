Public Class Player
    Inherits GameObject

    Private _isJumping As Boolean
    Private _jumpSpeed As Single
    Private _groundY As Integer

    Public Const GRAVITY As Integer = 2
    Public Const JUMP_POWER As Integer = 20

    Public Property IsJumping As Boolean
        Get
            Return _isJumping
        End Get
        Set(value As Boolean)
            _isJumping = value
        End Set
    End Property

    Public Property GroundY As Integer
        Get
            Return _groundY
        End Get
        Set(value As Integer)
            _groundY = value
        End Set
    End Property

    Public Sub New(x As Integer, y As Integer, width As Integer, height As Integer, groundY As Integer)
        MyBase.New(x, y, width, height)
        _groundY = groundY
        Me.Y = _groundY - height
        _isJumping = False
        _jumpSpeed = 0
        Me.Color = Drawing.Color.Blue
    End Sub

    Public Sub Jump()
        If Not _isJumping AndAlso Me.Y >= _groundY - Me.Height Then
            _isJumping = True
            _jumpSpeed = JUMP_POWER
        End If
    End Sub

    Public Overrides Sub Update()
        ' Aplicar física de salto y gravedad
        If _isJumping OrElse Me.Y < _groundY - Me.Height Then
            _jumpSpeed -= GRAVITY
            Me.Y -= CInt(_jumpSpeed)

            ' Verificar aterrizaje
            If Me.Y >= _groundY - Me.Height Then
                Me.Y = _groundY - Me.Height
                _isJumping = False
                _jumpSpeed = 0
            End If
        End If

        MyBase.Update()
    End Sub

    Public Function IsOnGround() As Boolean
        Return Me.Y >= _groundY - Me.Height AndAlso Not _isJumping
    End Function
End Class