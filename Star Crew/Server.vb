Imports System.Net
Imports System.Net.Sockets

Public Class Server
    Public comms As Threading.Thread
    Public Shared gameWorld As Galaxy

    Public Sub New()
        Server.gameWorld = New Galaxy(Me)
    End Sub

    Private Structure Communication
        Public Shared myServer As Server
        Private Shared myListener As New TcpListener(1224)
        Private Shared mySockets(0) As Socket

        Public Shared Sub OpenToNetwork()
            myListener.Start()
            mySockets(0) = myListener.Server

            While True
                Dim waiting As New ArrayList

                For Each i As Socket In mySockets
                    waiting.Add(i)
                Next

                Socket.Select(waiting, Nothing, Nothing, -1)

                For Each i As Socket In waiting
                    If i Is myListener.Server And UBound(mySockets) < 4 Then
                        ReDim Preserve mySockets(mySockets.Length)
                        mySockets(UBound(mySockets)) = myListener.AcceptSocket()
                    ElseIf i Is myListener.Server Then
                        myListener.Stop()
                        myListener.Start()
                    Else
                        RecieveMessage(i)
                    End If
                Next
            End While
        End Sub

        Private Shared Sub RecieveMessage(ByRef nSocket As Socket)
            Using nS As New NetworkStream(nSocket)
                Dim bF As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
                gameWorld.clientShip.IncomingMessage = bF.Deserialize(nS)
            End Using
        End Sub

    End Structure

End Class
