Imports System.Windows.Threading

Class MainWindow

    Dim GameLoop As New DispatcherTimer

    Dim PADDLE_BUFFER As Integer = 15
    Dim PADDLE As New Rectangle()
    Const PADDLE_DISTANCE_FROM_BOTTOM As Double = 50
    Dim CENTER_OF_PADDLE As Double
    Private PADDLE_SPEED As Double = 5
    Dim PADDLE_TRANSLATE As New TranslateTransform(0, 0)
    Private MOVE_LEFT As Boolean
    Private MOVE_RIGHT As Boolean

    Private BALL As New Ellipse()
    Private BALL_BUFFER As Double = 5
    Private BALL_TRANSLATE As New TranslateTransform(0, 0)
    Private BALL_SPEED_Y As Double = 5
    Private BALL_SPEED_X As Double = 5

    Private WALL_TOP As Double = 0
    Private WALL_LEFT As Double = 0
    Private WALL_RIGHT As Double
    Private WALL_BOTTOM As Double


    Sub New()
        InitializeComponent()
        GameLoop.Interval = TimeSpan.FromMilliseconds(1)
        AddHandler GameLoop.Tick, AddressOf UpdateLoop


        DrawPaddle()
        DrawBall()
        SETWALLS()
        GameLoop.Start()

    End Sub

    Private Sub UpdateLoop(sender As Object, e As EventArgs)
        ' Console.WriteLine(90909)
        MovePaddle()
        MoveBall()
        Check_Collison()

    End Sub

    Private Sub Check_Collison()

        Dim pt As Point = New Point(BALL_TRANSLATE.X, BALL_TRANSLATE.Y)
        VisualTreeHelper.HitTest(MainCanvas, Nothing, New HitTestResultCallback(AddressOf MyHitTestResult), New PointHitTestParameters(pt))
        'Retrieve the coordinate of the ball's position

        If BALL_TRANSLATE.X <= WALL_LEFT And BALL_SPEED_X < 0 Then
            BALL_SPEED_X *= -1
        End If

        If BALL_TRANSLATE.X >= WALL_RIGHT And BALL_SPEED_X > 0 Then
            BALL_SPEED_X *= -1
        End If

        If BALL_TRANSLATE.Y < WALL_TOP And BALL_SPEED_Y < 0 Then
            BALL_SPEED_Y *= -1
        End If

        If BALL_TRANSLATE.Y > WALL_BOTTOM Then
            BALL_TRANSLATE.Y = 0
            BALL_TRANSLATE.X = 0
        End If

    End Sub

    Public Function MyHitTestResult(ByVal result As HitTestResult) As HitTestResultBehavior

        If result.VisualHit.GetType() Is GetType(Rectangle) Then
            BALL_SPEED_Y *= -1
            Dim centerOfPaddleX As Double = PADDLE_TRANSLATE.X + CENTER_OF_PADDLE
            Dim ballDistFromPaddleCenterX As Double = BALL_TRANSLATE.X - centerOfPaddleX
            BALL_SPEED_X = ballDistFromPaddleCenterX * 0.08
        End If

    End Function

    Private Sub DrawBall()
        With BALL
            .Fill = Brushes.Red
            .StrokeThickness = 2
            .Stroke = Brushes.Black
            .Width = 20
            .Height = 20
            .RenderTransform = BALL_TRANSLATE
        End With
        BALL_TRANSLATE.X = (MainCanvas.Width / 2)
        BALL_TRANSLATE.Y = (MainCanvas.Height / 2)
        MainCanvas.Children.Add(BALL)
    End Sub

    Private Sub DrawPaddle()


        With PADDLE
            .Fill = Brushes.LightBlue
            .Stroke = Brushes.Black
            .StrokeThickness = 2
            .Width = 124
            .Height = 20
            .RenderTransform = PADDLE_TRANSLATE
            CENTER_OF_PADDLE = Width / 2
        End With

        PADDLE_TRANSLATE.X = (MainCanvas.Width - PADDLE.Width - PADDLE_BUFFER)
        PADDLE_TRANSLATE.Y = MainCanvas.Height - PADDLE.Height - (PADDLE_DISTANCE_FROM_BOTTOM)
        MainCanvas.Children.Add(PADDLE)

    End Sub

    Private Sub MoveBall()
        BALL_TRANSLATE.X += BALL_SPEED_X
        BALL_TRANSLATE.Y += BALL_SPEED_Y
        BALL.RenderTransform = BALL_TRANSLATE
    End Sub

    Private Sub MovePaddle()
        If MOVE_LEFT Then
            PADDLE_TRANSLATE.X -= PADDLE_SPEED
        End If
        If MOVE_RIGHT Then
            PADDLE_TRANSLATE.X += PADDLE_SPEED
        End If
        PADDLE.RenderTransform = PADDLE_TRANSLATE
    End Sub

    Private Sub SETWALLS()
        WALL_RIGHT = MainCanvas.Width - (BALL.Width + BALL_BUFFER)
        WALL_TOP += BALL.Height
        WALL_BOTTOM = MainCanvas.Height
    End Sub
    Private Sub MyWindow_KeyDown(sender As Object, e As KeyEventArgs) Handles MyWindow.KeyDown
        Select Case e.Key
            Case Key.A
                'Console.WriteLine("<-- A was pressed")
                MOVE_LEFT = True
            Case Key.W
                'Console.WriteLine("<-- W was pressed")
            Case Key.S
                'Console.WriteLine("<-- S was pressed")
            Case Key.D
                'Console.WriteLine("<-- D was pressed")
                MOVE_RIGHT = True
            Case Key.Escape
                Me.Close()
        End Select
    End Sub

    Private Sub MyWindow_KeyUp(sender As Object, e As KeyEventArgs) Handles MyWindow.KeyUp
        Select Case e.Key
            Case Key.A
                'Console.WriteLine("<-- A was pressed")
                MOVE_LEFT = False
            Case Key.W
                'Console.WriteLine("<-- W was pressed")
            Case Key.S
                'Console.WriteLine("<-- S was pressed")
            Case Key.D
                'Console.WriteLine("<-- D was pressed")
                MOVE_RIGHT = False
            Case Key.Escape
                Me.Close()
        End Select
    End Sub
End Class
