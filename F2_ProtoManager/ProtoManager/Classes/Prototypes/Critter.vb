Imports System.IO

Public Class CritterObj
    Inherits CommonPrototype

    Private Const ProtoSize As Integer = 384 '96 * 4
    'Private Const ProtoSize As Integer = Prototypes.ProtoMemberCount.Critter * 4

    Friend proto As Prototypes.CritterProtoData

    Sub New(proFile As String)
        MyBase.New(proFile)
    End Sub

    Public Overloads Function Load() As Boolean
        Dim isFallout1 = False

        Dim data(ProtoSize - 1) As Byte

        Dim streamFile = MyBase.DataLoad()
        If (streamFile.Length = 412) Then
            streamFile.Read(data, 0, ProtoSize - 4)
            isFallout1 = True
        ElseIf (streamFile.Length = 416) Then
            streamFile.Read(data, 0, ProtoSize)
        Else
            streamFile.Close()
            Return False
        End If

        streamFile.Close()
        ProFiles.ReverseLoadData(data, proto)

        If (isFallout1) Then proto.DamageType = 7 ' устанавливаем для указания прототипа формата F1

        Return True
    End Function

    Public Overloads Sub Save(savePath As String)
        Dim size = If(proto.DamageType = 7, ProtoSize - 4, ProtoSize) ' 412

        Dim streamFile = MyBase.DataSave(savePath)
        streamFile.Write(ProFiles.SaveDataReverse(proto), 0, size)
        streamFile.Close()
    End Sub

    Public Function GetStat(statID As Integer) As Integer
        Select Case statID
            Case 0
                Return proto.Strength
            Case 1
                Return proto.Perception
            Case 2
                Return proto.Endurance
            Case 3
                Return proto.Charisma
            Case 4
                Return proto.Intelligence
            Case 5
                Return proto.Agility
            Case 6
                Return proto.Luck
        End Select
        Return 0
    End Function

#Region "Prototype propertes"

#Region "Critter Flags"

    Public Property IsBarter As Boolean
        Set(value As Boolean)
            proto.CritterFlags = If(value, proto.CritterFlags Or Enums.CritterFlags.Barter, proto.CritterFlags And Not (Enums.CritterFlags.Barter))
        End Set
        Get
            Return (proto.CritterFlags And Enums.CritterFlags.Barter) <> 0
        End Get
    End Property

    Public Property IsNoSteal As Boolean
        Set(value As Boolean)
            proto.CritterFlags = If(value, proto.CritterFlags Or Enums.CritterFlags.NoSteal, proto.CritterFlags And Not (Enums.CritterFlags.NoSteal))
        End Set
        Get
            Return (proto.CritterFlags And Enums.CritterFlags.NoSteal) <> 0
        End Get
    End Property

    Public Property IsNoDrop As Boolean
        Set(value As Boolean)
            proto.CritterFlags = If(value, proto.CritterFlags Or Enums.CritterFlags.NoDrop, proto.CritterFlags And Not (Enums.CritterFlags.NoDrop))
        End Set
        Get
            Return (proto.CritterFlags And Enums.CritterFlags.NoDrop) <> 0
        End Get
    End Property

    Public Property IsNoLimbs As Boolean
        Set(value As Boolean)
            proto.CritterFlags = If(value, proto.CritterFlags Or Enums.CritterFlags.NoLimbs, proto.CritterFlags And Not (Enums.CritterFlags.NoLimbs))
        End Set
        Get
            Return (proto.CritterFlags And Enums.CritterFlags.NoLimbs) <> 0
        End Get
    End Property

    Public Property IsAges As Boolean
        Set(value As Boolean)
            proto.CritterFlags = If(value, proto.CritterFlags Or Enums.CritterFlags.Ages, proto.CritterFlags And Not (Enums.CritterFlags.Ages))
        End Set
        Get
            Return (proto.CritterFlags And Enums.CritterFlags.Ages) <> 0
        End Get
    End Property

    Public Property IsNoHeal As Boolean
        Set(value As Boolean)
            proto.CritterFlags = If(value, proto.CritterFlags Or Enums.CritterFlags.NoHeal, proto.CritterFlags And Not (Enums.CritterFlags.NoHeal))
        End Set
        Get
            Return (proto.CritterFlags And Enums.CritterFlags.NoHeal) <> 0
        End Get
    End Property

    Public Property IsInvulnerable As Boolean
        Set(value As Boolean)
            proto.CritterFlags = If(value, proto.CritterFlags Or Enums.CritterFlags.Invulnerable, proto.CritterFlags And Not (Enums.CritterFlags.Invulnerable))
        End Set
        Get
            Return (proto.CritterFlags And Enums.CritterFlags.Invulnerable) <> 0
        End Get
    End Property

    Public Property IsNoFlatten As Boolean
        Set(value As Boolean)
            proto.CritterFlags = If(value, proto.CritterFlags Or Enums.CritterFlags.NoFlatten, proto.CritterFlags And Not (Enums.CritterFlags.NoFlatten))
        End Set
        Get
            Return (proto.CritterFlags And Enums.CritterFlags.NoFlatten) <> 0
        End Get
    End Property

    Public Property IsSpecialDeath As Boolean
        Set(value As Boolean)
            proto.CritterFlags = If(value, proto.CritterFlags Or Enums.CritterFlags.SpecialDeath, proto.CritterFlags And Not (Enums.CritterFlags.SpecialDeath))
        End Set
        Get
            Return (proto.CritterFlags And Enums.CritterFlags.SpecialDeath) <> 0
        End Get
    End Property

    Public Property IsRangeHtH As Boolean
        Set(value As Boolean)
            proto.CritterFlags = If(value, proto.CritterFlags Or Enums.CritterFlags.RangeHtH, proto.CritterFlags And Not (Enums.CritterFlags.RangeHtH))
        End Set
        Get
            Return (proto.CritterFlags And Enums.CritterFlags.RangeHtH) <> 0
        End Get
    End Property

    Public Property IsNoKnockBack As Boolean
        Set(value As Boolean)
            proto.CritterFlags = If(value, proto.CritterFlags Or Enums.CritterFlags.NoKnockBack, proto.CritterFlags And Not (Enums.CritterFlags.NoKnockBack))
        End Set
        Get
            Return (proto.CritterFlags And Enums.CritterFlags.NoKnockBack) <> 0
        End Get
    End Property

#End Region

    Public Property HeadFID As Integer
        Set(value As Integer)
            proto.HeadFID = value
        End Set
        Get
            Return proto.HeadFID
        End Get
    End Property

    Public Property AIPacket As Integer
        Set(value As Integer)
            proto.AIPacket = value
        End Set
        Get
            Return proto.AIPacket
        End Get
    End Property

    Public Property TeamNum As Integer
        Set(value As Integer)
            proto.TeamNum = value
        End Set
        Get
            Return proto.TeamNum
        End Get
    End Property

    Public Property CritterFlags As Integer
        Set(value As Integer)
            proto.CritterFlags = value
        End Set
        Get
            Return proto.CritterFlags
        End Get
    End Property

#End Region

End Class
