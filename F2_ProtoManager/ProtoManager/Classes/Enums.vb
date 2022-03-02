NotInheritable Class Enums

    Enum ProType As Byte
        Critter
        Item
    End Enum

    Enum ItemType As Integer
        Armor           '0x0 - Armor (броня)
        Container       '0x1 - Container (контейнеры)
        Drugs           '0x2 - Drug (наркотики)
        Weapon          '0x3 - Weapon (оружие)
        Ammo            '0x4 - Ammo (патроны)
        Misc            '0x5 - Misc Item (разное)
        Key             '0x6 - Key (ключи)
        Unknown         '0x7...
    End Enum

    Enum Flags As Integer
        Mouse_3d     = &H00000001
        WalkThru     = &H00000004
        Flat         = &H00000008
        NoBlock      = &H00000010
        Lighting     = &H00000020
        Temp         = &H00000400
        MultiHex     = &H00000800
        NoHighlight  = &H00001000
        Used         = &H00002000
        TransRed     = &H00004000
        TransNone    = &H00008000
        TransWall    = &H00010000
        TransGlass   = &H00020000
        TransSteam   = &H00040000
        TransEnergy  = &H00080000
        Left_Hand    = &H01000000
        Right_Hand   = &H02000000
        Worn         = &H04000000
'       HiddenItem   = &H08000000
        WallTransEnd = &H10000000
        LightThru    = &H20000000
        Seen         = &H40000000
        ShootThru    = &H80000000
    End Enum

    Enum FlagsExt As Integer
        BigGun     = &H00000100 ' оружие относится к классу Big Guns
        TwoHand    = &H00000200 ' оружие относится к классу двуручных
        Energy     = &H00000400 ' оружие относится к классу Энергетического (sfall)

        Use        = &H00000800 ' объект можно использовать
        UseOn      = &H00001000
        Look       = &H00002000 ' объект можно осмотреть
        Talk       = &H00004000 ' с объектом можно поговорить
        PickUp     = &H00008000 ' объект можно поднять

        Unknown    = &H00800000
        HiddenItem = &H08000000
    End Enum

    Enum CritterFlags As Integer
        'Sneak       = &H0001 ' Can sneak ?
        Barter       = &H0002 ' Can trade With
        'Level       = &H0004 ' Level received ?
        'Addict      = &H0008 ' Drug addiction ?
        NoSteal      = &H0020 ' Can't be stolen from
        NoDrop       = &H0040 ' Doesn't drop items
        NoLimbs      = &H0080 ' Can't lose limbs
        Ages         = &H0100 ' Dead body does Not disappear
        NoHeal       = &H0200 ' Damage Is Not healed With time
        Invulnerable = &H0400 ' Is Invulnerable (cannot be hurt)
        NoFlatten    = &H0800 ' Doesn't flatten on death (leaves no dead body)
        SpecialDeath = &H1000 ' Has a special type Of death
        RangeHtH     = &H2000 ' Has extra hand-To-hand range
        NoKnockBack  = &H4000 ' Can't be knocked back
    End Enum

    Enum Gender As Integer
        Female = 0
        Male   = 1
    End Enum

End Class
