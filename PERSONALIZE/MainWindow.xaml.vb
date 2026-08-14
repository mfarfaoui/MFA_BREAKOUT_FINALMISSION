Imports System.Windows.Threading

'made a game very similar to the breakout game we made

Class MainWindow

    Dim GameLoop As New DispatcherTimer

    'here I changed the ball to be a "ghost" like figure that the player is able to move with the arrow keys
    'added parameters to allow to movement 

    Private GHOST As New Ellipse()
    Private GHOST_TRANSLATE As New TranslateTransform(0, 0)
    Private GHOST_SPEED_X, GHOST_SPEED_Y As Double
    Private Const GHOST_ACCEL As Double = 0.65
    Private Const GHOST_FRICTION As Double = 0.85
    Private Const MAX_SPEED_X As Double = 7
    Private Const GRAVITY As Double = 0.35
    Private Const JUMP_FORCE As Double = -11
    Private IS_GROUNDED As Boolean = False

    'changed set up walls to be desired movement trajectories 

    Private MOVE_LEFT, MOVE_RIGHT, WANT_JUMP As Boolean
    Private IsGameWon As Boolean = False
    Private CameraOffsetY As Double = 0

    Private Structure Platform
        Dim X, Y, Width, Height As Double
    End Structure
    Private PlatformsList As New List(Of Platform)()

    Sub New()
        InitializeComponent()
        AddHandler Me.Loaded, AddressOf MainWindow_Loaded
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs)
        Me.Width = 400
        Me.Height = 800
        MainCanvas.Width = 400
        MainCanvas.Height = 800

        GameLoop.Interval = TimeSpan.FromMilliseconds(16)
        AddHandler GameLoop.Tick, AddressOf UpdateLoop

        'used paddle model to make platforms for our ball/ghost to jump on

        GenerateTowerPlatforms()
        DrawGhost()

        GameLoop.Start()
    End Sub

    Private Sub UpdateLoop(sender As Object, e As EventArgs)
        If IsGameWon Then Exit Sub

        If MOVE_LEFT Then
            GHOST_SPEED_X -= GHOST_ACCEL
        ElseIf MOVE_RIGHT Then
            GHOST_SPEED_X += GHOST_ACCEL
        Else
            GHOST_SPEED_X *= GHOST_FRICTION
        End If

        ' Custom cross-platform clamping replacement for legacy .NET targets
        GHOST_SPEED_X = Math.Max(-MAX_SPEED_X, Math.Min(GHOST_SPEED_X, MAX_SPEED_X))

        If WANT_JUMP And IS_GROUNDED Then
            GHOST_SPEED_Y = JUMP_FORCE
            IS_GROUNDED = False
        End If
        GHOST_SPEED_Y += GRAVITY

        GHOST_TRANSLATE.X += GHOST_SPEED_X
        GHOST_TRANSLATE.Y += GHOST_SPEED_Y

        If GHOST_TRANSLATE.X < -24 Then GHOST_TRANSLATE.X = 400 Else If GHOST_TRANSLATE.X > 400 Then GHOST_TRANSLATE.X = -24

        Dim currentlyOnGround As Boolean = False
        If GHOST_SPEED_Y >= 0 Then
            Dim feetX As Double = GHOST_TRANSLATE.X + 12
            Dim feetY As Double = GHOST_TRANSLATE.Y + 32
            For i As Integer = 0 To PlatformsList.Count - 1
                Dim plat = PlatformsList(i)
                If feetX >= plat.X AndAlso feetX <= (plat.X + plat.Width) Then
                    If feetY >= plat.Y AndAlso feetY <= (plat.Y + plat.Height + GHOST_SPEED_Y) Then
                        GHOST_TRANSLATE.Y = plat.Y - 32
                        GHOST_SPEED_Y = 0
                        currentlyOnGround = True
                        If i = 50 Then
                            IsGameWon = True
                            Me.Close()
                            Exit Sub
                        End If
                        Exit For
                    End If
                End If
            Next
        End If
        IS_GROUNDED = currentlyOnGround

        Dim targetCamY As Double = 400 - GHOST_TRANSLATE.Y
        If targetCamY > CameraOffsetY Then
            CameraOffsetY += (targetCamY - CameraOffsetY) * 0.1
            MainCanvas.RenderTransform = New TranslateTransform(0, CameraOffsetY)
        End If
    End Sub

    Private Sub GenerateTowerPlatforms()
        Dim currentY As Double = 740
        Dim rand As New Random()
        For i As Integer = 0 To 50
            Dim plat As New Platform() With {.Height = 15, .Y = currentY}
            If i = 0 Then
                plat.Width = 400
                plat.X = 0
            Else
                plat.Width = Math.Max(40, 160 - (i * 2.5))
                plat.X = rand.Next(0, CInt(400 - plat.Width))
            End If

            Dim rect As New Rectangle() With {
                .Fill = New SolidColorBrush(Color.FromRgb(70, 80, 95)),
                .Stroke = If(i = 50, Brushes.Gold, Brushes.MediumPurple),
                .StrokeThickness = If(i = 50, 4, 2),
                .Width = plat.Width,
                .Height = plat.Height,
                .RadiusX = 5, .RadiusY = 5,
                .RenderTransform = New TranslateTransform(plat.X, plat.Y)
            }
            PlatformsList.Add(plat)
            MainCanvas.Children.Add(rect)
            currentY -= 120
        Next
    End Sub

    Private Sub DrawGhost()
        With GHOST
            .Fill = New SolidColorBrush(Color.FromArgb(200, 230, 245, 255))
            .Stroke = Brushes.White
            .StrokeThickness = 2
            .Width = 24
            .Height = 32
            .RenderTransform = GHOST_TRANSLATE
        End With
        MainCanvas.Children.Add(GHOST)
        GHOST_TRANSLATE.X = 188
        GHOST_TRANSLATE.Y = 708
    End Sub

    Private Sub MyWindow_KeyDown(sender As Object, e As KeyEventArgs) Handles MyWindow.KeyDown
        If e.Key = Key.Escape Then Me.Close()
        If IsGameWon Then Exit Sub
        Select Case e.Key
            Case Key.Left : MOVE_LEFT = True
            Case Key.Right : MOVE_RIGHT = True
            Case Key.Up : WANT_JUMP = True
        End Select
    End Sub

    Private Sub MyWindow_KeyUp(sender As Object, e As KeyEventArgs) Handles MyWindow.KeyUp
        Select Case e.Key
            Case Key.Left : MOVE_LEFT = False
            Case Key.Right : MOVE_RIGHT = False
            Case Key.Up : WANT_JUMP = False
        End Select
    End Sub
End Class
