Imports System.IO

Public Class CommonPrototype

    Private Const CommonSize As Integer = 32 '8 * 4

    Private proto As Prototypes.CommonProtoData

    Private proFile As String

    Public Property PrototypeFile As String
        Set(value As String)
            proFile = value
        End Set
        Get
            Return proFile
        End Get
    End Property

    Sub New(proFile As String)
        MyClass.proFile = proFile
    End Sub

    Sub New(data As CommonPrototype)
        proto = data.proto
        proFile = data.proFile

        FlagsExt = FlagsExt And &HCFFFFF ' default set
    End Sub

    Public Sub Load()
        DataLoad().Close()
    End Sub

    Public Sub Save(savePath As String)
        DataSave(savePath).Close()
    End Sub

    Protected Function DataLoad() As FileStream
        Dim streamFile = File.Open(proFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)

        Dim data(CommonSize - 1) As Byte
        streamFile.Read(data, 0, CommonSize)

        ProFiles.ReverseLoadData(data, proto)

        Return streamFile
    End Function

    Protected Function DataSave(savePath As String) As FileStream
        Dim streamFile = File.Open(savePath, FileMode.Create, FileAccess.Write, FileShare.None)

        streamFile.Write(ProFiles.SaveDataReverse(proto), 0, CommonSize)
        Return streamFile
    End Function

#Region "Prototype propertes"

    ''' <summary>
    '''  Устанавливает или возвращает интенсивность в процентах 0..100
    ''' </summary>
    Public Property LightIntensity As Integer
        Get
            Return CInt(Math.Round((proto.LightInt * 100) / 65535))
        End Get
        Set(value As Integer)
            proto.LightInt = CInt(Math.Round((value * 65535) / 100))
        End Set
    End Property

#Region "Flags"
    Public Property IsFlat As Boolean
        Set(value As Boolean)
            proto.Flags = If(value, proto.Flags Or Enums.Flags.Flat, proto.Flags And Not (Enums.Flags.Flat))
        End Set
        Get
            Return (proto.Flags And Enums.Flags.Flat) <> 0
        End Get
    End Property

    Public Property IsNoBlock As Boolean
        Set(value As Boolean)
            proto.Flags = If(value, proto.Flags Or Enums.Flags.NoBlock, proto.Flags And Not (Enums.Flags.NoBlock))
        End Set
        Get
            Return (proto.Flags And Enums.Flags.NoBlock) <> 0
        End Get
    End Property

    Public Property IsMultiHex As Boolean
        Set(value As Boolean)
            proto.Flags = If(value, proto.Flags Or Enums.Flags.MultiHex, proto.Flags And Not (Enums.Flags.MultiHex))
        End Set
        Get
            Return (proto.Flags And Enums.Flags.MultiHex) <> 0
        End Get
    End Property

    Public Property IsShootThru As Boolean
        Set(value As Boolean)
            proto.Flags = If(value, proto.Flags Or Enums.Flags.ShootThru, proto.Flags And Not (Enums.Flags.ShootThru))
        End Set
        Get
            Return (proto.Flags And Enums.Flags.ShootThru) <> 0
        End Get
    End Property

    Public Property IsLightThru As Boolean
        Set(value As Boolean)
            proto.Flags = If(value, proto.Flags Or Enums.Flags.LightThru, proto.Flags And Not (Enums.Flags.LightThru))
        End Set
        Get
            Return (proto.Flags And Enums.Flags.LightThru) <> 0
        End Get
    End Property

    Public ReadOnly Property IsLighting As Boolean
        Get
            Return (proto.Flags And Enums.Flags.Lighting) <> 0
        End Get
    End Property

    Public Property IsNoHighlight As Boolean
        Set(value As Boolean)
            proto.Flags = If(value, proto.Flags Or Enums.Flags.NoHighlight, proto.Flags And Not (Enums.Flags.NoHighlight))
        End Set
        Get
            Return (proto.Flags And Enums.Flags.NoHighlight) <> 0
        End Get
    End Property

    Public Property IsTransNone As Boolean
        Set(value As Boolean)
            proto.Flags = If(value, proto.Flags Or Enums.Flags.TransNone, proto.Flags And Not (Enums.Flags.TransNone))
            If (value) Then proto.Flags = proto.Flags And &HFFF0BFFF ' сбросить
        End Set
        Get
            Return (proto.Flags And Enums.Flags.TransNone) <> 0
        End Get
    End Property

    Public Property IsTransWall As Boolean
        Set(value As Boolean)
            proto.Flags = If(value, proto.Flags Or Enums.Flags.TransWall, proto.Flags And Not (Enums.Flags.TransWall))
        End Set
        Get
            Return (proto.Flags And Enums.Flags.TransWall) <> 0
        End Get
    End Property

    Public Property IsTransGlass As Boolean
        Set(value As Boolean)
            proto.Flags = If(value, proto.Flags Or Enums.Flags.TransGlass, proto.Flags And Not (Enums.Flags.TransGlass))
        End Set
        Get
            Return (proto.Flags And Enums.Flags.TransGlass) <> 0
        End Get
    End Property

    Public Property IsTransSteam As Boolean
        Set(value As Boolean)
            proto.Flags = If(value, proto.Flags Or Enums.Flags.TransSteam, proto.Flags And Not (Enums.Flags.TransSteam))
        End Set
        Get
            Return (proto.Flags And Enums.Flags.TransSteam) <> 0
        End Get
    End Property

    Public Property IsTransEnergy As Boolean
        Set(value As Boolean)
            proto.Flags = If(value, proto.Flags Or Enums.Flags.TransEnergy, proto.Flags And Not (Enums.Flags.TransEnergy))
        End Set
        Get
            Return (proto.Flags And Enums.Flags.TransEnergy) <> 0
        End Get
    End Property

    Public Property IsTransRed As Boolean
        Set(value As Boolean)
            proto.Flags = If(value, proto.Flags Or Enums.Flags.TransRed, proto.Flags And Not (Enums.Flags.TransRed))
        End Set
        Get
            Return (proto.Flags And Enums.Flags.TransRed) <> 0
        End Get
    End Property
#End Region

#Region "Flags Ext"
    Public Property IsUse As Boolean
        Set(value As Boolean)
            proto.FlagsExt = If(value, proto.FlagsExt Or Enums.FlagsExt.Use, proto.FlagsExt And Not (Enums.FlagsExt.Use))
        End Set
        Get
            Return (proto.FlagsExt And Enums.FlagsExt.Use) <> 0
        End Get
    End Property

    Public Property IsUseOn As Boolean
        Set(value As Boolean)
            proto.FlagsExt = If(value, proto.FlagsExt Or Enums.FlagsExt.UseOn, proto.FlagsExt And Not (Enums.FlagsExt.UseOn))
        End Set
        Get
            Return (proto.FlagsExt And Enums.FlagsExt.UseOn) <> 0
        End Get
    End Property

    Public Property IsLook As Boolean
        Set(value As Boolean)
            proto.FlagsExt = If(value, proto.FlagsExt Or Enums.FlagsExt.Look, proto.FlagsExt And Not (Enums.FlagsExt.Look))
        End Set
        Get
            Return (proto.FlagsExt And Enums.FlagsExt.Look) <> 0
        End Get
    End Property

    Public Property IsPickUp As Boolean
        Set(value As Boolean)
            proto.FlagsExt = If(value, proto.FlagsExt Or Enums.FlagsExt.PickUp, proto.FlagsExt And Not (Enums.FlagsExt.PickUp))
        End Set
        Get
            Return (proto.FlagsExt And Enums.FlagsExt.PickUp) <> 0
        End Get
    End Property

    Public Property IsTalk As Boolean
        Set(value As Boolean)
            proto.FlagsExt = If(value, proto.FlagsExt Or Enums.FlagsExt.Talk, proto.FlagsExt And Not (Enums.FlagsExt.Talk))
        End Set
        Get
            Return (proto.FlagsExt And Enums.FlagsExt.Talk) <> 0
        End Get
    End Property

    Public Property IsHiddenItem As Boolean
        Set(value As Boolean)
            proto.FlagsExt = If(value, proto.FlagsExt Or Enums.FlagsExt.HiddenItem, proto.FlagsExt And Not (Enums.FlagsExt.HiddenItem))
        End Set
        Get
            Return (proto.FlagsExt And Enums.FlagsExt.HiddenItem) <> 0
        End Get
    End Property
#End Region

    Public Property ProtoID As Integer
        Set(value As Integer)
            proto.ProtoID = value
        End Set
        Get
            Return proto.ProtoID
        End Get
    End Property

    Public Property DescID As Integer
        Set(value As Integer)
            proto.DescID = value
        End Set
        Get
            Return proto.DescID
        End Get
    End Property

    Public Property FrmID As Integer
        Set(value As Integer)
            proto.FrmID = value
        End Set
        Get
            Return proto.FrmID
        End Get
    End Property

    Public Property LightDis As Integer
        Set(value As Integer)
            proto.LightDis = value
        End Set
        Get
            Return proto.LightDis
        End Get
    End Property

    Public Property LightInt As Integer
        Set(value As Integer)
            proto.LightInt = value
        End Set
        Get
            Return proto.LightInt
        End Get
    End Property

    Public Property Flags As Integer
        Set(value As Integer)
            proto.Flags = value
        End Set
        Get
            Return proto.Flags
        End Get
    End Property

    Public Property FlagsExt As Integer
        Set(value As Integer)
            proto.FlagsExt = value
        End Set
        Get
            Return proto.FlagsExt
        End Get
    End Property

    Public Property ScriptID As Integer
        Set(value As Integer)
            proto.ScriptID = value
        End Set
        Get
            Return proto.ScriptID
        End Get
    End Property

#End Region

End Class
