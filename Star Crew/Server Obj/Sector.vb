Public Class Sector
    Public Shared centerFleet As Fleet
    Public Shared fleetList(-1) As Fleet

    Public Sub New(ByVal fleetCount As Integer)
        ReDim fleetList(fleetCount - 1)
        fleetList(0) = New FriendlyFleet(0)
        centerFleet = fleetList(0)
        fleetList(1) = New PirateFleet(1)
        If fleetCount > 2 Then
            For i As Integer = 3 To UBound(fleetList)
                If Int(Rnd() * 2) = 0 Then
                    fleetList(i) = New FriendlyFleet(i)
                Else
                    fleetList(i) = New PirateFleet(i)
                End If
            Next
        End If
    End Sub

    Public Shared Sub RemoveFleet(ByRef nFleet As Fleet)
        For i As Integer = nFleet.Index To UBound(fleetList)
            If i <> UBound(fleetList) Then
                fleetList(i) = fleetList(i + 1)
                fleetList(i).Index = i
            End If
        Next
        ReDim Preserve fleetList(UBound(fleetList) - 1)
    End Sub

    Public Shared Sub UpdateCombatSenario()
        Fleet.UpdateFleet_Call()
    End Sub

End Class
