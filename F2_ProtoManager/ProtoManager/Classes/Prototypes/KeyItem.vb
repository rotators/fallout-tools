Imports System.IO

Public Class KeyItemObj
    Inherits ItemPrototype
    Implements IPrototype

    Private Const ProtoSize As Integer = Prototypes.ProtoMemberCount.Key * 4

    Private mProto As Prototypes.KeyItemProto

    Sub New(data As ItemPrototype)
        MyBase.New(data)

        ObjType = Enums.ItemType.Key
        Unknown = -1
    End Sub

    Sub New(proFile As String)
        MyBase.New(proFile)
    End Sub

    Public Overloads Sub Load() Implements IPrototype.Load
        Dim streamFile As BinaryReader = New BinaryReader(MyBase.DataLoad())

        mProto.Unknown = ProFiles.ReverseBytes(streamFile.ReadInt32)
        streamFile.Close()
    End Sub

    Public Overloads Sub Save(savePath As String) Implements IPrototype.Save
        Dim streamFile As BinaryWriter = New BinaryWriter(MyBase.DataSave(savePath))
        streamFile.Write(ProFiles.ReverseBytes(mProto.Unknown))
        streamFile.Close()
    End Sub

#Region "Prototype propertes"

    Public Property Unknown As Integer
        Set(value As Integer)
            mProto.Unknown = value
        End Set
        Get
            Return mProto.Unknown
        End Get
    End Property

#End Region
End Class
