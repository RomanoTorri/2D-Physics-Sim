Imports System.Drawing.Drawing2D
Imports System.Drawing
Public Class Golf
    Dim StartOfVector As PointF
    Dim EndOfVector As PointF
    Dim FinalVector As PointF
    Structure Ball
        Dim Position As PointF
        Dim Velocity As PointF
        Dim Centre As PointF
        Dim Angle As Single
    End Structure
    Structure Wall
        Dim Ends() As PointF
        Dim Points() As PointF
        Dim Thickness As Integer
        Dim Angle As Single
        Dim Gradient As Single
        Dim cooldown As Integer
    End Structure
    Dim Gravity As Single = 9.81 / 1000
    Dim Radius As Integer = 5
    Dim MyBall As New Ball
    Dim g As Graphics
    Dim backbuffer As New Bitmap(500, 500)
    Dim Points() As PointF
    Dim arrowTime As Boolean
    Dim CurrentMousePoint As Point
    Dim stoploop As Boolean
    Dim TestWall As Wall
    Dim TestWall2 As Wall
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick



        g = Graphics.FromImage(backbuffer)
        g.Clear(Color.Aqua)


        UpdateBallPosition()
        '    DoCircumference()
        CheckEdges()
        If arrowTime = True Then
            If Math.Round(MyBall.Velocity.X, 2, MidpointRounding.AwayFromZero) = 0 And Math.Round(MyBall.Velocity.Y, 2, MidpointRounding.AwayFromZero) = 0 Then
                Points(0) = StartOfVector
                Points(1) = CurrentMousePoint
                g.DrawLine(Pens.Red, Points(0), Points(1))
            End If

        End If
        '   g.FillPolygon(Brushes.ForestGreen, TestWall.Points)
        '     g.FillPolygon(Brushes.ForestGreen, TestWall2.Points)
        ' g.DrawPolygon(Pens.White, MyBall.Points)
        g.DrawLine(Pens.ForestGreen, TestWall.Ends(0).X, ClientSize.Height - TestWall.Ends(0).Y, TestWall.Ends(1).X, ClientSize.Height - TestWall.Ends(1).Y)
        g.DrawLine(Pens.ForestGreen, TestWall2.Ends(0).X, ClientSize.Height - TestWall2.Ends(0).Y, TestWall2.Ends(1).X, ClientSize.Height - TestWall2.Ends(1).Y)
        g.FillEllipse(Brushes.White, MyBall.Position.X, ClientSize.Height - Radius * 2 - MyBall.Position.Y, Radius * 2, Radius * 2)
        g.DrawEllipse(Pens.Black, MyBall.Position.X, ClientSize.Height - Radius * 2 - MyBall.Position.Y, Radius * 2, Radius * 2)
        Me.CreateGraphics.DrawImage(backbuffer, 0, 0)

    End Sub
    'Sub DoCircumference()
    '    ReDim Preserve MyBall.Points(0)
    '    '    For looper = 0 To Radius Step 0.1
    '    For x = -Radius To Radius Step 0.1
    '        For y = -Radius To Radius Step 0.1
    '            If Math.Round(Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)), 2, MidpointRounding.AwayFromZero) = Radius Then
    '                ReDim Preserve MyBall.Points(MyBall.Points.Length)
    '                MyBall.Points(MyBall.Points.Length - 2).X = MyBall.Centre.X + x
    '                MyBall.Points(MyBall.Points.Length - 2).Y = MyBall.Centre.Y + y
    '            End If
    '        Next
    '    Next
    '    '    Next

    '    ReDim Preserve MyBall.Points(MyBall.Points.Length - 2)
    '    'For looper = 0 To MyBall.Circumference.Length - 1
    '    '    MyBall.Circumference(looper).Y = ClientSize.Height - (Radius * 2) - MyBall.Circumference(looper).Y
    '    'Next
    'End Sub
    Sub ApplyGravity()
        MyBall.Velocity.Y -= Gravity
    End Sub
    Sub CheckEdges()

        If MyBall.Position.X + MyBall.Velocity.X < 0 Or MyBall.Position.X + MyBall.Velocity.X > ClientSize.Width - Radius * 2 Then
            MyBall.Velocity.X = -MyBall.Velocity.X / 1.2
            '  MyBall.Velocity.Y = MyBall.Velocity.Y / 1.2

        End If
        If MyBall.Position.Y + MyBall.Velocity.Y > ClientSize.Height - Radius * 2 Then
            MyBall.Velocity.Y = -MyBall.Velocity.Y


        End If
        If MyBall.Position.Y + MyBall.Velocity.Y < 0 Then
            MyBall.Velocity.Y = -MyBall.Velocity.Y / 2
            If Gravity <> 0 Then
                '      MyBall.Velocity.X = MyBall.Velocity.X / 1.5
            End If

        End If


        For looper2 = 0 To TestWall.Points.Length - 1
            If Math.Sqrt(Math.Pow(MyBall.Centre.X - TestWall.Points(looper2).X, 2) + (Math.Pow(MyBall.Centre.Y - TestWall.Points(looper2).Y, 2))) <= Radius Then
                MyBall.Angle = CalculateAngleOfBall(MyBall)
                MyBall.Angle = CalculateBounceAngle(MyBall, TestWall)
                MyBall.Velocity = CalculateBouncedVelocities(MyBall)

                Exit For
            End If
        Next
        Dim Dist As Single

        If TestWall2.cooldown <= 0 Then
            For looper2 = 0 To TestWall2.Points.Length - 1
                Dist = Math.Sqrt(Math.Pow(MyBall.Centre.X - TestWall2.Points(looper2).X, 2) + (Math.Pow(MyBall.Centre.Y - TestWall2.Points(looper2).Y, 2)))
                If Dist <= Radius Then
                    MyBall.Angle = CalculateAngleOfBall(MyBall)
                    MyBall.Angle = CalculateBounceAngle(MyBall, TestWall2)
                    MyBall.Velocity = CalculateBouncedVelocities(MyBall)
                    TestWall2.cooldown = 15
                    Exit For
                End If
            Next
        End If

        TestWall2.cooldown -= 1






    End Sub
    Function CalculateBouncedVelocities(Myball As Ball)
        Dim Magnitude As Single
        Magnitude = Math.Sqrt(Math.Pow(Myball.Velocity.X, 2) + Math.Pow(Myball.Velocity.Y, 2))
        'Sector 1
        If Myball.Angle > 0 And Myball.Angle < Math.PI / 2 Then
            Myball.Velocity.X = Math.Cos(Myball.Angle) * Magnitude
            Myball.Velocity.Y = Math.Sin(Myball.Angle) * Magnitude
        Else
            If Myball.Angle > Math.PI / 2 And Myball.Angle < Math.PI Then
                Myball.Angle = Math.PI - Myball.Angle
                Myball.Velocity.X = -(Math.Cos(Myball.Angle) * Magnitude)
                Myball.Velocity.Y = Math.Sin(Myball.Angle) * Magnitude
            Else
                If Myball.Angle > -Math.PI And Myball.Angle < -Math.PI / 2 Then
                    Myball.Angle = Math.PI - Myball.Angle
                    Myball.Velocity.X = -Math.Cos(Myball.Angle) * Magnitude
                    Myball.Velocity.Y = Math.Sin(Myball.Angle) * Magnitude
                Else
                    If Myball.Angle < 0 And Myball.Angle > -Math.PI / 2 Then
                        Myball.Angle = Math.Abs(Myball.Angle)
                        Myball.Velocity.X = Math.Cos(Myball.Angle) * Magnitude
                        Myball.Velocity.Y = -Math.Sin(Myball.Angle) * Magnitude
                    End If


                End If
            End If


        End If
        'Sector 2

        'Sector 3

        'Sector 4


        'Special Cases
        If Myball.Angle = 0 Then
            Myball.Velocity.X = Magnitude
            Myball.Velocity.Y = 0
        End If
        If Myball.Angle = Math.PI / 2 Then
            Myball.Velocity.X = 0
            Myball.Velocity.Y = Magnitude
        End If
        If Myball.Angle = Math.PI Then
            Myball.Velocity.X = -Magnitude
            Myball.Velocity.Y = 0
        End If
        If Myball.Angle = -Math.PI / 2 Then
            Myball.Velocity.X = 0
            Myball.Velocity.Y = -Magnitude
        End If
        Return Myball.Velocity
    End Function
    Function CalculateAngleOfBall(MyBall As Ball)

        MyBall.Angle = Math.Atan(Math.Abs(MyBall.Velocity.Y) / Math.Abs(MyBall.Velocity.X))
        'Sector 1

        'Sector 2
        If MyBall.Velocity.Y > 0 And MyBall.Velocity.X < 0 Then
            MyBall.Angle = Math.PI - MyBall.Angle
        End If
        'Sector 3
        If MyBall.Velocity.Y < 0 And MyBall.Velocity.X < 0 Then
            MyBall.Angle = -Math.PI + MyBall.Angle
        End If
        'Sector 4
        If MyBall.Velocity.Y < 0 And MyBall.Velocity.X > 0 Then
            MyBall.Angle = -MyBall.Angle
        End If
        Return MyBall.Angle
    End Function
    Function CalculateBounceAngle(MyBall As Ball, MyWall As Wall)
        Dim tempangle As Single
        tempangle = Math.PI - MyBall.Angle - ((Math.PI / 2) - MyWall.Angle)
        If tempangle > Math.PI Then
            tempangle -= 2 * Math.PI
        End If
        If tempangle <= -Math.PI Then
            tempangle += 2 * Math.PI
        End If
        Return tempangle
    End Function
    Sub UpdateBallPosition()
        If Math.Round(MyBall.Velocity.X, 2) = 0 Then
            MyBall.Velocity.X = 0
        End If
        If Math.Round(MyBall.Velocity.Y, 2) = 0 Then
            MyBall.Velocity.Y = 0
        End If
        FloorCorrection()
        FloorFriction()
        ApplyGravity()
        MyBall.Position.X = MyBall.Position.X + MyBall.Velocity.X
        MyBall.Position.Y = MyBall.Position.Y + MyBall.Velocity.Y
        MyBall.Centre.X = MyBall.Position.X + Radius
        MyBall.Centre.Y = MyBall.Position.Y + Radius
    End Sub
    Sub FloorFriction()
        If Gravity = 0 Then
            If Math.Round(MyBall.Velocity.X, 2) <> 0 Then
                MyBall.Velocity.X = MyBall.Velocity.X / 1.005
            End If
        End If
    End Sub
    Sub FloorCorrection()
        If Math.Round(MyBall.Velocity.Y, 2) = 0 Then
            If Math.Round(MyBall.Position.Y, 2) < 0 Then

                Gravity = 0
            End If

        Else
            Gravity = 9.81 / 1000
        End If


    End Sub
    Private Sub Golf_Load(sender As Object, e As EventArgs) Handles Me.Load

        MyBall.Position.X = 200
        MyBall.Position.Y = 0
        MyBall.Velocity.X = 0
        MyBall.Velocity.Y = 0
        ReDim Points(1)
        TestWall = InitialiseLine(TestWall, 300, 80, 300, 300)
        TestWall2 = InitialiseLine(TestWall2, 100, 0, 300, 300)
        'ReDim TestWall2.Ends(1)
        'TestWall2.Ends(0).X = 380
        'TestWall2.Ends(1).X = 380
        'TestWall2.Ends(0).Y = 0
        'TestWall2.Ends(1).Y = 300
        'TestWall2.Thickness = 2
        'TestWall2.Angle = Math.PI / 2
        'TestWall2.Points = CalculatePointsOnAWall(TestWall2)
    End Sub
    Function InitialiseLine(Wall As Wall, x1 As Single, y1 As Single, x2 As Single, y2 As Single)
        ReDim Wall.Ends(1)
        Wall.Ends(0).X = x1
        Wall.Ends(1).X = x2
        Wall.Ends(0).Y = y1
        Wall.Ends(1).Y = y2
        Wall.Gradient = GetGradient(Wall)
        Wall.Angle = Math.Atan(Wall.Gradient)
        Wall.Points = CalculatePointsOnAWall(Wall)
        Return Wall
    End Function
    Function GetGradient(Wall As Wall)
        Return (Wall.Ends(1).Y - Wall.Ends(0).Y) / (Wall.Ends(1).X - Wall.Ends(0).X)
    End Function
    Function CalculatePointsOnAWall(Wall As Wall)

        Dim c As Single
        ReDim Wall.Points(0)
        If Wall.Ends(0).X = Wall.Ends(1).X Then

            For looper = Wall.Ends(0).Y To Wall.Ends(1).Y Step 1
                ReDim Preserve Wall.Points(Wall.Points.Length)
                Wall.Points(Wall.Points.Length - 1).X = Wall.Ends(0).X
                Wall.Points(Wall.Points.Length - 1).Y = looper
            Next
        Else
            c = Wall.Ends(1).Y - (Wall.Gradient * Wall.Ends(1).X)
            For looper = Wall.Ends(0).X To Wall.Ends(1).X Step 1
                ReDim Preserve Wall.Points(Wall.Points.Length)
                Wall.Points(Wall.Points.Length - 1).X = looper
                Wall.Points(Wall.Points.Length - 1).Y = looper * Wall.Gradient + c
            Next
        End If
        'OriginalLength = Wall.Points.Length
        'Dim counter = 0
        ''  For looper = 0 To Wall.Thickness Step 0.01

        'ReDim Preserve Wall.Points(Wall.Points.Length)
        '    Wall.Points(Wall.Points.Length - 1).Y = Wall.Points(counter).Y + looper
        '    Wall.Points(Wall.Points.Length - 1).Y = Wall.Points(counter).Y + looper
        '    counter += 1
        '    If counter > OriginalLength Then
        '        counter = 0
        '    End If
        ''  Next
        'ReDim Preserve Wall.Points(Wall.Points.Length - 1)
        Return Wall.Points
    End Function
    Private Sub StartButton_Click(sender As Object, e As EventArgs) Handles StartButton.Click
        Timer1.Enabled = True
        StartButton.Visible = False
    End Sub

    Private Sub Golf_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown

        If Math.Round(MyBall.Velocity.X, 2, MidpointRounding.AwayFromZero) = 0 And Math.Round(MyBall.Velocity.Y, 2, MidpointRounding.AwayFromZero) = 0 Then
            arrowTime = True

            StartOfVector.X = e.X
            StartOfVector.Y = e.Y

        End If
        MyBall.Velocity.X = 0



    End Sub

    Private Sub Golf_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        If Math.Round(MyBall.Velocity.X, 2, MidpointRounding.AwayFromZero) = 0 And Math.Round(MyBall.Velocity.Y, 2, MidpointRounding.AwayFromZero) = 0 Then
            arrowTime = False
            FinalVector.X = (e.X - StartOfVector.X) / 50
            FinalVector.Y = (e.Y - StartOfVector.Y) / 50
            MyBall.Velocity.X = FinalVector.X
            MyBall.Velocity.Y = -FinalVector.Y
        End If

    End Sub

    Private Sub Golf_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        CurrentMousePoint = e.Location
    End Sub

    'Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick

    '    '  g.FillPolygon(Brushes.Red, Points)
    '    g.FillRectangle(Brushes.Red, 5, 5, 5, 5)
    'End Sub
End Class
