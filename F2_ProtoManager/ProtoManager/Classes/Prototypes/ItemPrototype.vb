Imports System.IO

Public Class ItemPrototype
    Inherits CommonPrototype
    Implements IPrototype

    Private Const CommonSize As Integer = 24 '6 * 4
    'Protected Friend Const CommonSize As Integer = Prototypes.ProtoMemberCount.Common * 4

    Private proto As Prototypes.ItemProtoData

    Sub New(proFile As String)
        MyBase.New(proFile)
    End Sub

    Sub New(data As ItemPrototype)
        MyBase.New(data)
    End Sub

    Public Overloads Sub Load() Implements IPrototype.Load
        DataLoad().Close()
    End Sub

    Public Overloads Sub Save(savePath As String) Implements IPrototype.Save
        DataSave(savePath).Close()
    End Sub

    Protected Overloads Function DataLoad() As FileStream
        Dim streamFile = MyBase.DataLoad()

        Dim data(CommonSize - 1) As Byte
        streamFile.Read(data, 0, CommonSize)

        ProFiles.ReverseLoadData(data, proto)
        proto.SoundID = CByte(streamFile.ReadByte())

        Return streamFile
    End Function

    Protected Overloads Function DataSave(savePath As String) As FileStream
        Dim streamFile = MyBase.DataSave(savePath)

        streamFile.Write(ProFiles.SaveDataReverse(proto), 0, CommonSize + 1)
        Return streamFile
    End Function

#Region "Prototype propertes"

    Public Property ObjType As Integer
        Set(value As Integer)
            proto.ObjType = value
        End Set
        Get
            Return proto.ObjType
        End Get
    End Property

    Public Property MaterialID As Integer
        Set(value As Integer)
            proto.MaterialID = value
        End Set
        Get
            Return proto.MaterialID
        End Get
    End Property

    Public Property Size As Integer
        Set(value As Integer)
            proto.Size = value
        End Set
        Get
            Return proto.Size
        End Get
    End Property

    Public Property Weight As Integer
        Set(value As Integer)
            proto.Weight = value
        End Set
        Get
            Return proto.Weight
        End Get
    End Property

    Public Property Cost As Integer
        Set(value As Integer)
            proto.Cost = value
        End Set
        Get
            Return proto.Cost
        End Get
    End Property

    Public Property InvFID As Integer
        Set(value As Integer)
            proto.InvFID = value
        End Set
        Get
            Return proto.InvFID
        End Get
    End Property

    Public Property SoundID As Byte
        Set(value As Byte)
            proto.SoundID = value
        End Set
        Get
            Return proto.SoundID
        End Get
    End Property

#End Region

End Class
