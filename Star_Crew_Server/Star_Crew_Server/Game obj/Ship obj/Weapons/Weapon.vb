Public MustInherit Class Weapon
    Private _Parent As Ship 'The Actual value of Parent
    Public ReadOnly Property Parent As Ship 'A reference to the Ship object that the Weapon is mounted on
        Get
            Return _Parent
        End Get
    End Property
    Private _Range As Integer 'The actual value of Range
    Public ReadOnly Property Range As Integer 'An Integer value that represents the range of the Weapon
        Get
            Return _Range
        End Get
    End Property
    Private _Damage As Double 'The actual value of Damage
    Public ReadOnly Property Damage As Double 'A Double value which represents the damage the Weapon does
        Get
            Return _Damage
        End Get
    End Property
    Private _Ammunition As Game_Library.Game_Objects.StatInt 'The actual value of Ammunition
    Public ReadOnly Property Ammunition As Game_Library.Game_Objects.StatInt 'A StatInt object which represents the ammunition available to the Weapon
        Get
            Return _Ammunition
        End Get
    End Property
    Private _Reload As Game_Library.Game_Objects.StatInt 'The actual value of Reload
    Public ReadOnly Property Reload As Game_Library.Game_Objects.StatInt 'A StatInt object used to count the ticks before the Weapon is ready to fire
        Get
            Return _Reload
        End Get
    End Property

    Public Sub New(ByRef nParent As Ship, ByVal nRange As Integer, ByVal nDamage As Double, ByVal nAmmunition As Game_Library.Game_Objects.StatInt, ByVal nReload As Integer)
        _Parent = nParent
        _Range = nRange
        _Damage = nDamage
        _Ammunition = nAmmunition
        _Reload = New Game_Library.Game_Objects.StatInt(0, 0, nReload, True)
    End Sub

    Public Sub Destroy() 'Finalises the Weapon
        _Parent = Nothing 'Clear the parent reference
        _Ammunition = Nothing
        _Reload = Nothing
    End Sub

    Public Sub Fire(ByVal nTargets As Ship()) 'Fires the Weapon
        If Ammunition.Current <> 0 And Reload.Current = 0 Then 'The Weapon is able to fire
            Ammunition.Current -= 1 'Remove one ammunition
            Reload.Current = Reload.Maximum 'Start the cooldown
            Do_Damage(nTargets) 'Do Damage
            Parent.firing = True 'Set firing to true
        End If
    End Sub

    Public MustOverride Sub Do_Damage(ByVal targets As Ship()) 'Calculates doing damage to the target

    Public Sub Update() 'Updates the Weapon
        Reload.Current -= 1 'Step down the cooldown by 1
    End Sub

End Class
