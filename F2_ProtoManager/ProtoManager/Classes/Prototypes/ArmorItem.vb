Imports System.IO

Public Class ArmorItemObj
    Inherits ItemPrototype
    Implements IPrototype

    Private Const ProtoSize As Integer = Prototypes.ProtoMemberCount.Armor * 4

    Private mProto As Prototypes.ArmorItemProto

    Sub New(data As ItemPrototype)
        MyBase.New(data)

        ObjType = Enums.ItemType.Armor
        DREMP = 500
        FemaleFID = &H1000000
        MaleFID = &H1000000
        Perk = -1
    End Sub

    Sub New(proFile As String)
        MyBase.New(proFile)
    End Sub

    Public Overloads Sub Load() Implements IPrototype.Load
        Dim streamFile = MyBase.DataLoad()

        Dim data(ProtoSize - 1) As Byte

        streamFile.Read(data, 0, ProtoSize)
        streamFile.Close()

        ProFiles.ReverseLoadData(data, mProto)
    End Sub

    Public Overloads Sub Save(savePath As String) Implements IPrototype.Save
        Dim streamFile = MyBase.DataSave(savePath)

        streamFile.Write(ProFiles.SaveDataReverse(mProto), 0, ProtoSize)
        streamFile.Close()
    End Sub

    Public Sub LoadArmorData()
        Dim streamFile = File.Open(MyBase.PrototypeFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)

        streamFile.Position = Prototypes.DataOffset.InvenFID
        InventoryFID = ProFiles.ReverseBytes(New BinaryReader(streamFile).ReadInt32())

        Dim data(ProtoSize - 1) As Byte

        streamFile.Position = Prototypes.DataOffset.ArmorBlock + 4 ' skip AC
        streamFile.Read(data, 4, ProtoSize - 16) ' 4 * 4
        streamFile.Close()

        ProFiles.ReverseLoadData(data, mProto)
    End Sub

    Public Function ArmorScore() As Integer
        Dim DT As Integer = mProto.DTNormal + mProto.DTLaser + mProto.DTPlasma + mProto.DTFire + mProto.DTElectrical + mProto.DTExplode + mProto.DTEMP
        Dim DR As Integer = mProto.DRNormal + mProto.DRLaser + mProto.DRPlasma + mProto.DRFire + mProto.DRElectrical + mProto.DRExplode + mProto.DREMP
        Return mProto.AC + DT + DR
    End Function

#Region "Prototype propertes"

    Public Property InventoryFID As Integer

    Public Property AC As Integer
        Set(value As Integer)
            mProto.AC = value
        End Set
        Get
            Return mProto.AC
        End Get
    End Property

    Public Property DRNormal As Integer
        Set(value As Integer)
            mProto.DRNormal = value
        End Set
        Get
            Return mProto.DRNormal
        End Get
    End Property

    Public Property DRLaser As Integer
        Set(value As Integer)
            mProto.DRLaser = value
        End Set
        Get
            Return mProto.DRLaser
        End Get
    End Property

    Public Property DRFire As Integer
        Set(value As Integer)
            mProto.DRFire = value
        End Set
        Get
            Return mProto.DRFire
        End Get
    End Property

    Public Property DRPlasma As Integer
        Set(value As Integer)
            mProto.DRPlasma = value
        End Set
        Get
            Return mProto.DRPlasma
        End Get
    End Property

    Public Property DRElectrical As Integer
        Set(value As Integer)
            mProto.DRElectrical = value
        End Set
        Get
            Return mProto.DRElectrical
        End Get
    End Property

    Public Property DREMP As Integer
        Set(value As Integer)
            mProto.DREMP = value
        End Set
        Get
            Return mProto.DREMP
        End Get
    End Property

    Public Property DRExplode As Integer
        Set(value As Integer)
            mProto.DRExplode = value
        End Set
        Get
            Return mProto.DRExplode
        End Get
    End Property

    Public Property DTNormal As Integer
        Set(value As Integer)
            mProto.DTNormal = value
        End Set
        Get
            Return mProto.DTNormal
        End Get
    End Property

    Public Property DTLaser As Integer
        Set(value As Integer)
            mProto.DTLaser = value
        End Set
        Get
            Return mProto.DTLaser
        End Get
    End Property

    Public Property DTFire As Integer
        Set(value As Integer)
            mProto.DTFire = value
        End Set
        Get
            Return mProto.DTFire
        End Get
    End Property

    Public Property DTPlasma As Integer
        Set(value As Integer)
            mProto.DTPlasma = value
        End Set
        Get
            Return mProto.DTPlasma
        End Get
    End Property

    Public Property DTElectrical As Integer
        Set(value As Integer)
            mProto.DTElectrical = value
        End Set
        Get
            Return mProto.DTElectrical
        End Get
    End Property

    Public Property DTEMP As Integer
        Set(value As Integer)
            mProto.DTEMP = value
        End Set
        Get
            Return mProto.DTEMP
        End Get
    End Property

    Public Property DTExplode As Integer
        Set(value As Integer)
            mProto.DTExplode = value
        End Set
        Get
            Return mProto.DTExplode
        End Get
    End Property

    Public Property Perk As Integer
        Set(value As Integer)
            mProto.Perk = value
        End Set
        Get
            Return mProto.Perk
        End Get
    End Property

    Public Property MaleFID As Integer
        Set(value As Integer)
            mProto.MaleFID = value
        End Set
        Get
            Return mProto.MaleFID
        End Get
    End Property

    Public Property FemaleFID As Integer
        Set(value As Integer)
            mProto.FemaleFID = value
        End Set
        Get
            Return mProto.FemaleFID
        End Get
    End Property

#End Region

End Class
