Imports System.Timers 

' email:   sariesbuenaventura2019@gmail.com

Module Program
    Private Const msg1 As String = "   GAME OVER!!!   "
    Private Const msg2 As String = " Press any key to exit. "
    Private Const msg3 As String = " Congratulations!!! "
        
    Structure Point
        Public x As Integer
        Public y As Integer 

        Public Sub New(newX As Integer, newY As Integer)
            x = newX
            y = newY
        End Sub 
    End Structure 

    Dim Direction  As New Point()
    Dim Snake      As New ArrayList()
    Dim isGameExit As Boolean 
    
    Dim GenerateRandom As New Random()
    Dim Food           As New Point()
    Dim Score          As Integer

    Dim WL As Integer ' Window-Left
    Dim WR As Integer ' Window-Right
    Dim WT As Integer ' Window-Top
    Dim WB As Integer ' Window-Bottom

    Dim kb      As ConsoleKeyInfo ' keyboard input
    Dim last_kb As ConsoleKey     ' save last key pressed

    Sub Main()
        GameDesign()
        DisplayFood()

        ' Set the default direction for the snake
        Direction.x = 1
        Direction.y = 0

        ' Create a timer with a 0.3 second interval. 
        '   Note:
        '     1000ms = 1sec
        Dim GameTimer As New Timer(300) 

        ' Define the length and position of the snake.
        Snake.Add(New Point(4, 0)) ' Head
        Snake.Add(New Point(3, 0)) ' Body
        Snake.Add(New Point(2, 0)) ' Body
        Snake.Add(New Point(1, 0)) ' Body
        Snake.Add(New Point(0, 0)) ' Tail
        
        ' Set the default value for last key pressed
        last_kb = ConsoleKey.RightArrow

        ' Show the snake
        For i As Integer = 0 To Snake.Count-1
            Console.SetCursorPosition(CType(Snake(i), Point).x + WL, CType(Snake(i), Point).y + WT)
            Console.Write("#")
        Next i
        
        ' Hook up the Elapsed event for the timer.
        AddHandler GameTimer.Elapsed, AddressOf GameStart

        ' Start the game.
        GameTimer.Start()
        
        Do While (Not isGameExit) ' continue looping until escape key has been pressed.
            If Console.KeyAvailable Then
                kb = Console.ReadKey(True) ' get key input
            End If
        Loop
        
        GameTimer.Stop()

        ' Press any key to exit.
        Console.SetCursorPosition((Console.WindowWidth - msg2.Length) / 2, _
                                   Console.WindowHeight/2)
        Console.Write(msg2)
        Console.ReadKey()
    End Sub

    Sub GameStart(source As Object, e As ElapsedEventArgs)
        If Not kb.Key.Equals(ConsoleKey.Escape) Then
            '   Check the last key input. This prevents the snake to move backward.
            ' For instance, when the user pressed the left or right arrow key, the next available keys
            ' for the user are arrow up/down, and vice versa.
            If (last_kb.Equals(ConsoleKey.LeftArrow)) Or (last_kb.Equals(ConsoleKey.RightArrow)) Then
                Select Case kb.Key
                    case ConsoleKey.UpArrow
                        Direction.x = 0  ' same direction
                        Direction.y = -1 ' above.
                    case ConsoleKey.DownArrow:
                        Direction.x = 0  ' same direction
                        Direction.y = +1 ' below.
                End Select
            Else If (last_kb.Equals(ConsoleKey.UpArrow)) Or (last_kb.Equals(ConsoleKey.DownArrow)) Then
                Select Case kb.Key
                    case ConsoleKey.LeftArrow:
                        Direction.x = -1 ' left.
                        Direction.y = 0  ' same direction
                    case ConsoleKey.RightArrow
                        Direction.x = +1 ' right.
                        Direction.y = 0  ' same direction
                End Select 
            End If
        Else
            isGameExit = True
            return
        End If 

        ' Is key has been pressed?
        If Not Convert.ToInt32(kb.Key).Equals(0)
            If kb.Key.Equals(ConsoleKey.LeftArrow) Or kb.Key.Equals(ConsoleKey.RightArrow) Or _
               kb.Key.Equals(ConsoleKey.UpArrow) Or kb.Key.Equals(ConsoleKey.DownArrow) Then
                    last_kb = kb.Key
            End If
        End If
        
        ' remove the trail
        Console.SetCursorPosition(CType(Snake(Snake.Count-1), Point).x+WL, CType(Snake(Snake.Count-1), Point).y+WT)
        Console.Write(" ")

        For i As Integer =(Snake.Count-1) To 0 Step -1
            If i.Equals(0) Then ' Is this the head of the snake?
                ' Move the snake to its new direction.
                Snake(0) = New Point(CType(Snake(0), Point).x + Direction.x, _
                                     CType(Snake(0), Point).y + Direction.y)
            Else
                ' Track the body.
                '
                ' [n].x <- [n-1].x
                ' [n].y <- [n-1].y
                ' 
                ' example:
                '     [4].x = [3].x : [4].y = [3].y
                '     [3].x = [2].x : [3].y = [2].y
                '     [2].x = [1].x : [2].y = [1].y
                '     [1].x = [0].x : [1].y = [0].y
                Snake(i) = New Point(CType(Snake(i-1), Point).x, _
                                     CType(Snake(i-1), Point).y)
            End If
        Next i

        If Not IsCollided() Then
            If ((CType(Snake(0), Point).x + WL).Equals(Food.x)) And ((CType(Snake(0), Point).y + WT).Equals(Food.y)) Then
                ' Is the length of the snake reached 100?
                If Snake.Count.Equals(100) Then
                    isGameExit = True
                    ' yes, let's end the game
                    ' Congratulations!!!
                    Console.SetCursorPosition((Console.WindowWidth - msg3.Length)/2, _
                                               Console.WindowHeight/2-1)
                    Console.Write(msg3)
                Else
                    ' no, continue playing
                    ' increase the length of the snake by 1
                    Snake.Add(New Point(CType(Snake(Snake.Count-1), Point).x, _
                                        CType(Snake(Snake.Count-1), Point).y))
                    Score += 10
                    DisplayScore()
                    DisplayFood()
                End If
            End If

            For i As Integer = 0 To Snake.Count-1
                Console.SetCursorPosition(CType(Snake(i), Point).x + WL, CType(Snake(i), Point).y + WT)
                Console.Write("#")
            Next i
        Else
            ' Game Over!!!
            Console.SetCursorPosition((Console.WindowWidth - msg1.Length)/2, _
                                       Console.WindowHeight/2-1)
            Console.Write(msg1)
            isGameExit = True
        End If
    End Sub 

    Sub GameDesign()
        Const dsgnTB As String = "▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒"
        Const dsgnLR As String = "▒                        ▒"
        
        ' Define window size.
        WL = (Console.WindowWidth-dsgnTB.Length )/2+1
        WR = WL+dsgnTB.Length-3
        WT = 3
        WB = Console.WindowHeight - 4

        ' hide the cursor
        Console.CursorVisible = False
        ' draw the top border
        Console.SetCursorPosition((Console.WindowWidth-dsgnTB.Length )/2, 2)
        Console.Write(dsgnTB)
        ' draw the bottom border
        Console.SetCursorPosition((Console.WindowWidth - dsgnTB.Length) / 2, _
                                   Console.WindowHeight - 3)
        Console.Write(dsgnTB)
        
        ' draw the left and right border
        For i As Integer = 3 To Console.WindowHeight-4
            Console.SetCursorPosition((Console.WindowWidth - dsgnTB.Length) / 2, i)
            Console.Write(dsgnLR)
        Next i

        DisplayScore()
    End Sub

    Sub DisplayFood()
        ' Creates a new food.
        Food.x = GenerateRandom.Next(WL, WR)
        Food.y = GenerateRandom.Next(WT, WB)

        Console.SetCursorPosition(Food.x, Food.y)
        Console.Write("@")
    End Sub 

    Sub DisplayScore()
        Console.SetCursorPosition(WR + 3, Console.WindowHeight / 2)
        Console.Write("SCORE : {0}", Score)
    End Sub 

    Function IsCollided() As Boolean 
        Dim x As Integer = CType(Snake(0), Point).x + WL ' head_x + window_left
        Dim y As Integer = CType(Snake(0), Point).y + WT ' head_y + window_top

        ' Is out of boundary?
        If (x < WL) Or (x > WR) Or (y < WT) Or (y > WB) Then
            return True
        Else
            For i As Integer = 1 To Snake.Count-1
                ' Is the head of the snake collided to the body?
                If((x.Equals(CType(Snake(i), Point).x + WL)) And (y.Equals((CType(Snake(i), Point).y + WT)))) Then
                    return True
                End If
            Next i
        End If

        return False
    End Function 
End Module
