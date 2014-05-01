Imports System.Net
Imports System.Net.Sockets
Public Class ServerSideClient
    Inherits Socket
    Public MyStation As Station.StationTypes = Station.StationTypes.Max
    Private bf As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
    Private MessageBuff(300) As Byte
    Private ByteBuff(3) As Byte
    Private BytesToReceive As Integer
    Private BytesReceived As Integer

    Public Sub New(ByVal nSocketInformation As SocketInformation)
        MyBase.New(nSocketInformation)
    End Sub

    Public Sub DecodeMessage()
        If MyStation = Station.StationTypes.Max Then
            While BytesReceived < 4
                BytesReceived = BytesReceived + Receive(ByteBuff, BytesReceived, 4 - BytesReceived, SocketFlags.None)
            End While
            MyStation = BitConverter.ToInt32(ByteBuff, 0)
            For Each i As ServerSideClient In ConsoleWindow.GameServer.Clients
                If ReferenceEquals(Me, i) = False And i.MyStation = MyStation Then
                    GameServer.RemoveClient(Me, False)
                    Exit Sub
                End If
            Next
            Console.WriteLine((MyStation.ToString() + ": Has been connected"))
            Select Case MyStation
                Case Station.StationTypes.Helm
                    ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.PlayerControled = True
                Case Station.StationTypes.Batteries
                    ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Batteries.PlayerControled = True
                Case Station.StationTypes.Shielding
                    ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Shielding.PlayerControled = True
                Case Station.StationTypes.Engineering
                    ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Engineering.PlayerControled = True
            End Select
        Else
            BytesReceived = 0
            BytesToReceive = 0
            Try
                While BytesReceived < 4
                    BytesReceived = BytesReceived + Receive(ByteBuff, BytesReceived, 4 - BytesReceived, SocketFlags.None)
                End While
                BytesToReceive = BitConverter.ToInt32(ByteBuff, 0)
                BytesReceived = 0
                While BytesReceived < BytesToReceive
                    BytesReceived = BytesReceived + Receive(MessageBuff, BytesReceived, BytesToReceive - BytesReceived, SocketFlags.None)
                End While
                ConsoleWindow.GameServer.GameWorld.RunCommand(bf.Deserialize(New IO.MemoryStream(MessageBuff, 0, BytesToReceive)))
            Catch ex As SocketException
                ConsoleWindow.GameServer.RemoveClient(Me, True)
            Catch ex As Exception
                Console.WriteLine()
                Console.WriteLine("Error : There was an unexpected and unhandled exception.")
                Console.WriteLine("please submit it as an issue at the URL bellow")
                Console.WriteLine("https://github.com/Dynisious/Star_Crew/issues")
                Console.WriteLine()
                Console.WriteLine(ex.ToString)
                Console.WriteLine()
            End Try
        End If
    End Sub
End Class