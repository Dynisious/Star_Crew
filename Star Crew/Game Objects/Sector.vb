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
        For i As Integer = 0 To Int(4 * Rnd())
            AddFleet(New NeutralFleet(i))
        Next
    End Sub

    Public Sub AddFleet(ByRef nFleet As Fleet)
        fleetList.Add(nFleet)
        nFleet.currentSector = Me
        If ReferenceEquals(nFleet, centerFleet) = True Then
            Galaxy.centerSector = Me
        End If
    End Sub

    Public Sub RemoveFleet(ByRef nFleet As Fleet)
        If nFleet.MyAllegence <> Galaxy.Allegence.Neutral Then
            fleetList.RemoveAt(nFleet.Index)
            fleetList.TrimExcess()
            For i As Integer = nFleet.Index To fleetList.Count - 1
                fleetList(i).Index = i
            Next
            nFleet.Dead = True
            nFleet.ShipList.Clear()
            nFleet.ShipList.TrimExcess()
        End If
    End Sub

    Public Shared Sub UpdateSector()
        Fleet.UpdateFleet_Call()
    End Sub

End Class
