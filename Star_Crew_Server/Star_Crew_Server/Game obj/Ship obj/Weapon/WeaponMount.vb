Public Class WeaponMount 'Object that holds a Weapon object, defines how far it can turn and it's offset from forward on the Ship
    Public MountedWeapon As Weapon 'A Weapon object that is mounted on this WeaponMount
    Public Sweep As Game_Library.StatDbl 'A StatDbl object representing the minimum, current and maximum offset from centered on the mount of the Weapon
    Public Offset As Double 'A Double value representing the offset of centered on this mount from forward on the Ship

End Class
