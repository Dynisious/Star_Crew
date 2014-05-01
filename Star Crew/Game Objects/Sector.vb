<Serializable()>
Public Class Sector
    Public Shared centerFleet As Fleet
    Public fleetList As New List(Of Fleet)

    Public Sub New(ByVal fleetCount As Integer)
        If fleetCount <> 0 Then
            For i As Integer = 0 To fleetCount - 1
                If Int(Rnd() * 2) = 0 Then
                    AddFleet(New FriendlyFleet(i))
                Else
                    AddFleet(New PirateFleet(i))
                End If
            Next
        End If
        For i As Integer = 0 To 2
            AddFleet(New NeutralFleet(-1))
        Next
    End Sub

    Public Sub AddFleet(ByRef nFleet As Fleet)
        fleetList.Add(nFleet)
        nFleet.currentSector = Me
        If ReferenceEquals(nFleet, centerFleet) = True Then
            ConsoleWindow.GameServer.GameWorld.centerSector = Me
        End If
    End Sub

    Public Sub RemoveFleet(ByRef nFleet As Fleet, ByVal KillFleet As Boolean, ByVal KillShips As Boolean)
        If nFleet.MyAllegence <> Galaxy.Allegence.Neutral Then
            fleetList.RemoveAt(nFleet.Index)
            fleetList.TrimExcess()
            For i As Integer = 0 To fleetList.Count - 1
                fleetList(i).Index = i
            Next
            If KillFleet = True Then
                nFleet.Dead = True
                If KillShips = True Then
                    For Each i As Ship In nFleet.ShipList
                        i.DestroyShip()
                    Next
                    nFleet.ShipList.Clear()
                    nFleet.ShipList.TrimExcess()
                End If
            Else
                Dim a = 1
            End If
        End If
    End Sub

End Class
