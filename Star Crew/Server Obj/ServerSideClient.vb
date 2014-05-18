Imports System.Net
Imports System.Net.Sockets
Public Class ServerSideClient 'An object that sends and receives messages too and from the Clients
    Inherits Socket 'Derived from Socket to allow for networking
    Public MyStation As Station.StationTypes = Station.StationTypes.Max 'The station the Client is controling
    Private bf As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter 'A Serialiser object to receive messages
    Private MessageBuff(300) As Byte 'An Array of Bytes to receive deserialise ClientMessage objects from
    Private ByteBuff(3) As Byte 'An Array of 4 Bytes that represent how many Bytes will be in the message object
    Private BytesToReceive As Integer 'The number of Bytes that will be in the message
    Private BytesReceived As Integer 'The number of Bytes that have been received

    Public Sub New(ByVal nSocketInformation As SocketInformation)
        MyBase.New(nSocketInformation)
        SendTimeout = 100
        ReceiveTimeout = 100
    End Sub

    Public Sub DecodeMessage() 'Receive a message from the Client
        If MyStation = Station.StationTypes.Max Then 'The Client is connecting
            Try
                While BytesReceived < 4 'Receive 4 Bytes to convert into MyStation
                    BytesReceived = BytesReceived + Receive(ByteBuff, BytesReceived, 4 - BytesReceived, SocketFlags.None)
                End While
                MyStation = BitConverter.ToInt32(ByteBuff, 0) 'Convert the 4 Bytes into the MyStation Integer
                For Each i As ServerSideClient In ConsoleWindow.GameServer.Clients 'Check that the Station is free
                    If ReferenceEquals(Me, i) = False And i.MyStation = MyStation Then 'A different Client already has this Station
                        GameServer.RemoveClient(Me, False) 'Remove this ServerSideClient from the List but do not reset control of the Station
                        Exit Sub 'Exit the Sub here
                    End If
                Next
                Console.WriteLine((MyStation.ToString() + ": Has been connected")) 'The connection was succesful
            Catch ex As SocketException
                GameServer.RemoveClient(Me, False)
            End Try
            Select Case MyStation 'Set the selected Station to be Player controled
                Case Station.StationTypes.Helm
                    ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Helm.PlayerControled = True
                Case Station.StationTypes.Batteries
                    ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Batteries.PlayerControled = True
                Case Station.StationTypes.Shielding
                    ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Shielding.PlayerControled = True
                Case Station.StationTypes.Engineering
                    ConsoleWindow.GameServer.GameWorld.CombatSpace.centerShip.Engineering.PlayerControled = True
            End Select
        ElseIf Available > 0 Then 'Receive a ClientMessage object
            BytesReceived = 0
            BytesToReceive = 0
            Try
                While BytesReceived < 4 'Receive 4 Bytes to deserialise into the BytesToReceive Integer
                    BytesReceived = BytesReceived + Receive(ByteBuff, BytesReceived, 4 - BytesReceived, SocketFlags.None)
                End While
                BytesToReceive = BitConverter.ToInt32(ByteBuff, 0) 'Deserialise into the BytesToReceive Integer
                BytesReceived = 0 'Reset the number of Bytes received
                While BytesReceived < BytesToReceive 'Receive the specified number of Bytes
                    BytesReceived = BytesReceived + Receive(MessageBuff, BytesReceived, BytesToReceive - BytesReceived, SocketFlags.None)
                End While
                ConsoleWindow.GameServer.GameWorld.RunCommand(bf.Deserialize(New IO.MemoryStream(MessageBuff, 0, BytesToReceive)))
                'Deserialise the bytes into the RunCommand Sub to receive key strokes
            Catch ex As SocketException
                ConsoleWindow.GameServer.RemoveClient(Me, True) 'The connection was closed so remove this ServerSideClient and reset control of
                'the station to AI
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