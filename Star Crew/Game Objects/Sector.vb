Public Class Sector
    Public Shared centerFleet As Fleet
    Public fleetList(-1) As Fleet

    Public Sub New(ByVal fleetCount As Integer)
        ReDim fleetList(fleetCount - 1)
        fleetList(0) = New PirateFleet(0)
        If fleetCount > 1 Then
            For i As Integer = 0 To UBound(fleetList)
                If Int(Rnd() * 2) = 0 Then
                    fleetList(i) = New FriendlyFleet(i)
                Else
                    fleetList(i) = New PirateFleet(i)
                End If
            Next
        End If
    End Sub

    Public Sub AddFleet(ByRef nFleet As Fleet)
        ReDim Preserve fleetList(fleetList.Length)
        fleetList(UBound(fleetList)) = nFleet
        nFleet.Index = UBound(fleetList)
    End Sub

    Public Sub RemoveFleet(ByRef nFleet As Fleet)
        For i As Integer = nFleet.Index To UBound(fleetList)
            If i <> UBound(fleetList) Then
                fleetList(i) = fleetList(i + 1)
                fleetList(i).Index = i
            End If
        Next
        ReDim Preserve fleetList(UBound(fleetList) - 1)
    End Sub

    Public Shared Sub UpdateSector()
        Fleet.UpdateFleet_Call()
    End Sub

End Class
