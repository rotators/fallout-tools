Imports System.Runtime.InteropServices
Imports System.IO
Imports Microsoft.VisualBasic.FileIO

Imports Prototypes
Imports Enums

Module ProFiles

    Friend Enum Status
        NotExist
        IsNormal
        IsModFolder
        IsBadFile
    End Enum

    Friend Sub SetReadOnly(proFile As String)
        If proRO Then File.SetAttributes(proFile, FileAttributes.ReadOnly Or FileAttributes.NotContentIndexed)
    End Sub

    ''' <summary>
    ''' Возвращает имя Frm файла для инвентаря(ivent), или имя FID предмета, если файл для инвентаря не определен.
    ''' </summary>
    Friend Function GetItemInvenFID(ByVal nPro As Integer, ByRef Inventory As Boolean) As String
        Dim FID As Integer = -1
        Dim iFID As Integer = -1

        Dim cPath As String = DatFiles.CheckFile(PROTO_ITEMS & Items_LST(nPro).proFile)
        Try
            Using readFile As New BinaryReader(File.Open(cPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                readFile.BaseStream.Seek(Prototypes.DataOffset.FrmID, SeekOrigin.Begin)
                FID = ReverseBytes(readFile.ReadInt32())
                readFile.BaseStream.Seek(Prototypes.DataOffset.InvenFID, SeekOrigin.Begin)
                iFID = ReverseBytes(readFile.ReadInt32())
            End Using
        Catch ex As Exception
            Return Nothing
        End Try

        Dim lstName As String
        If iFID > -1 Then
            iFID = iFID And (Not &H7000000)
            lstName = Iven_FRM.ElementAtOrDefault(iFID)
        Else
            If FID = -1 Then Return Nothing
            lstName = Items_FRM.ElementAtOrDefault(FID)
            Inventory = False
        End If

        If lstName Is Nothing Then
            Main.PrintLog("Invalid FID number of the prototype file: " & PROTO_ITEMS & Items_LST(nPro).proFile)
            Return lstName
        End If
        Return lstName.ToLower
    End Function

    ''' <summary>
    ''' Создает pro-файл по указаному имени и пути.
    ''' </summary>
    Friend Sub CreateProFile(ByVal path As String, ByVal pName As String)
        path = SaveMOD_Path & path
        Dim nProFile As String = path & pName

        If Not (Directory.Exists(path)) Then
            Directory.CreateDirectory(path)
        ElseIf File.Exists(nProFile) Then
            File.SetAttributes(nProFile, FileAttributes.Normal)
            File.Delete(nProFile)
        End If
        File.Move("template", nProFile)

        SetReadOnly(nProFile)

        Main.PrintLog("Create Pro: " & nProFile)
    End Sub

    ''' <summary>
    ''' Возвращает номер Description ID из про-файла предмета, и его тип.
    ''' </summary>
    Friend Function GetProItemsDataIDs(ByRef ProFile As String, ByVal n As Integer) As Integer
        Dim NameID, pID As Integer
        Dim type As ItemType = ItemType.Unknown

        Dim cPath As String = DatFiles.CheckFile(PROTO_ITEMS & ProFile)

        Try
            Using brFile As New BinaryReader(File.Open(cPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                pID = brFile.ReadInt32()
                NameID = brFile.ReadInt32()
                brFile.BaseStream.Seek(Prototypes.DataOffset.ItemSubType, SeekOrigin.Begin)
                type = CType(ReverseBytes(brFile.ReadInt32()), ItemType)
            End Using
        Catch ex As EndOfStreamException
            NameID = 0
            type = ItemType.Unknown
            MsgBox("The file is in an incorrect format or damaged." & vbLf & cPath)
        Catch ex As Exception
            type = ItemType.Unknown
        End Try

        If type >= 0 AndAlso type < ItemType.Unknown Then
            Items_LST(n).itemType = type
            Items_LST(n).PID = ReverseBytes(pID)
        Else
            Items_LST(n).itemType = ItemType.Unknown
        End If

        Return ReverseBytes(NameID)
    End Function

    ''' <summary>
    ''' Проверяет прото файл на соответствие размера и установленного атрибута только-чтения
    ''' </summary>
    ''' <param name="proFile"></param>
    ''' <param name="size"></param>
    ''' <param name="fileAttr"></param>
    ''' <returns>Возвращает результат проверки</returns>
    Friend Function ProtoCheckFile(ByVal proFile As String, ByVal size As Integer, ByRef fileAttr As String) As Status
        Dim cPath As String
        If size <> 416 Then
            cPath = DatFiles.CheckFile(PROTO_ITEMS & proFile, unpack:=False)
        Else
            cPath = DatFiles.CheckFile(PROTO_CRITTERS & proFile, unpack:=False)
        End If
        If cPath = Nothing Then Return Status.NotExist

        Dim pro As New FileInfo(cPath)
        If pro.Length <> size Then ' check valid size
            If size <> 416 OrElse pro.Length <> 412 Then
                fileAttr = "BAD!"
                Return Status.IsBadFile
            End If
        End If
        If pro.DirectoryName.StartsWith(SaveMOD_Path, StringComparison.OrdinalIgnoreCase) Then
            If (pro.IsReadOnly) Then fileAttr = "R/O"
            Return Status.IsModFolder
        End If

        Return Status.IsNormal
    End Function

    ''' <summary>
    ''' Возвращает номер FID из про-файла криттера.
    ''' </summary>
    Friend Function GetFID(ByVal nPro As Integer) As Integer
        Dim FID As Integer = -1

        Dim cPath As String = DatFiles.CheckFile(PROTO_CRITTERS & Critter_LST(nPro).proFile)
        Try
            Using readFile As New BinaryReader(File.Open(cPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                readFile.BaseStream.Seek(Prototypes.DataOffset.FrmID, SeekOrigin.Begin)
                FID = readFile.ReadInt32()
            End Using
        Catch ex As Exception
            Return Nothing
        End Try

        FID = ReverseBytes(FID)
        Critter_LST(nPro).FID = FID

        Return If(FID = -1, 0, FID)
    End Function

    ''' <summary>
    ''' Возвращает имя FID из про-файла криттера.
    ''' </summary>
    Friend Function GetCritterFID(ByVal nPro As Integer) As String
        Dim HP, bonusHP As Integer, FID As Integer = -1

        Dim cPath As String = DatFiles.CheckFile(PROTO_CRITTERS & Critter_LST(nPro).proFile)
        Try
            Using readFile As New BinaryReader(File.Open(cPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                readFile.BaseStream.Seek(Prototypes.DataOffset.FrmID, SeekOrigin.Begin)
                FID = readFile.ReadInt32()
                readFile.BaseStream.Seek(Prototypes.DataOffset.CritterHP, SeekOrigin.Begin)
                HP = readFile.ReadInt32()
                readFile.BaseStream.Seek(Prototypes.DataOffset.CritteBonusHP, SeekOrigin.Begin)
                bonusHP = readFile.ReadInt32()
            End Using
        Catch ex As Exception
            Return Nothing
        End Try

        FID = ReverseBytes(FID)
        Critter_LST(nPro).FID = FID
        Critter_LST(nPro).crtHP = ReverseBytes(HP) + ReverseBytes(bonusHP)

        If FID = -1 Then Return Nothing
        FID -= &H1000000I

        Return Critters_FRM(FID).ToLower
    End Function

    ''' <summary>
    ''' Возвращает номер Description ID из про-файла криттера.
    ''' </summary>
    Friend Function GetProCritterDataIDs(ByRef crtList As CrittersLst) As Integer
        Dim nameID, pID, fID As Integer

        Dim cPath = DatFiles.CheckFile(PROTO_CRITTERS & crtList.proFile)
        Try
            Using readFile = New BinaryReader(File.Open(cPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                pID = readFile.ReadInt32()
                nameID = readFile.ReadInt32()
                fID = readFile.ReadInt32()

                If (readFile.BaseStream.Length = 412) Then crtList.formatF1 = True
            End Using
        Catch ex As Exception
            Return 0
        End Try

        crtList.PID = ReverseBytes(pID)
        crtList.FID = ReverseBytes(fID)

        Return ReverseBytes(nameID)
    End Function

    ''' <summary>
    ''' Сохраняет структуру криттера в pro файл.
    ''' </summary>
    Friend Sub SaveCritterProData(ByVal proFile As String, ByRef critter As CritterProto)
        If File.Exists(proFile) Then
            File.SetAttributes(proFile, FileAttributes.Normal Or FileAttributes.NotContentIndexed)
        End If

        Dim data As Integer() = ReverseSaveData(critter, ProtoMemberCount.Critter)
        If critter.data.DamageType = 7 Then
            Array.Resize(data, ProtoMemberCount.Critter - 1)
            File.Delete(proFile) ' удаляем файл для перезаписи его размера
        End If

        Dim fFile As Integer = FreeFile()
        FileOpen(fFile, proFile, OpenMode.Binary, OpenAccess.Write, OpenShare.Shared)
        FilePut(fFile, data)
        FileClose(fFile)

        SetReadOnly(proFile)
    End Sub

    ''' <summary>
    ''' Сохраняет класс-структуру криттера в pro файл.
    ''' </summary>
    Friend Sub SaveCritterProData(ByVal proFile As String, ByVal critter As CritterObj)
        If File.Exists(proFile) Then
            File.SetAttributes(proFile, FileAttributes.Normal Or FileAttributes.NotContentIndexed)
        End If

        critter.Save(proFile)

        SetReadOnly(proFile)
    End Sub

    ''' <summary>
    ''' Считывает данные из pro файла криттера в структуру.
    ''' </summary>
    Friend Function LoadCritterProData(ByVal pathProFile As String, ByRef critter As CritterProto) As Boolean
        If (File.Exists(pathProFile) = False) Then Return False

        Dim count = ProtoMemberCount.Critter - 1
        Dim critterData(count) As Integer

        Dim readFile = New BinaryReader(File.Open(pathProFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        If readFile.BaseStream.Length = 412 Then
            critterData(count) = &H7000000 ' set DamageType = 7
            count -= 1
        ElseIf readFile.BaseStream.Length <> 416 Then
            readFile.Close()
            Return False
        End If

        For i = 0 To count
            critterData(i) = readFile.ReadInt32()
        Next
        readFile.Close()

        ProFiles.ReverseLoadData(critterData, critter)

        Return True
    End Function

    ''' <summary>
    ''' Считывает данные из pro файла криттера в массив.
    ''' </summary>
    Friend Function LoadCritterProData(ByVal pathProFile As String, ByRef critterData As Integer()) As Boolean
        Dim fFile As Integer = FreeFile()

        pathProFile = DatFiles.CheckFile(PROTO_CRITTERS & pathProFile)
        Try
            FileOpen(fFile, pathProFile, OpenMode.Binary, OpenAccess.Read, OpenShare.Shared)
            If FileSystem.GetFileInfo(pathProFile).Length = 412 Then
                Dim data(ProtoMemberCount.Critter - 2) As Integer ' read F1 buffer
                FileGet(fFile, data)
                data.CopyTo(critterData, 0)
                critterData(ProtoMemberCount.Critter - 1) = -1
            Else
                FileGet(fFile, critterData)
            End If
        Catch ex As Exception
            Return True
        Finally
            FileClose(fFile)
        End Try

        For n = 0 To critterData.Length - 1
            critterData(n) = ProFiles.ReverseBytes(critterData(n))
        Next

        Return False
    End Function

    ''' <summary>
    ''' Сохраняет структуру предмета в pro файл.
    ''' </summary>
    Friend Sub SaveItemProData(ByVal pathProFile As String, ByVal item As IPrototype)
        If File.Exists(pathProFile) Then
            File.SetAttributes(pathProFile, FileAttributes.Normal Or FileAttributes.NotContentIndexed)
        End If

        item.Save(pathProFile)

        SetReadOnly(pathProFile)
    End Sub

    Friend Sub ReverseLoadData(Of T As Structure)(ByRef buffer() As Integer, ByRef struct As T)
        For n = 0 To buffer.Length - 1
            buffer(n) = ReverseBytes(buffer(n))
        Next
        struct = CType(ConvertBytesToStruct(buffer, struct.GetType), T)
    End Sub

    Friend Sub ReverseLoadData(Of T As Structure)(ByRef buffer() As Byte, ByRef struct As T)
        ReverseBytes(buffer, buffer.Length)

        Dim mGC As GCHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned)
        struct = CType(Marshal.PtrToStructure(mGC.AddrOfPinnedObject, struct.GetType), T)
        mGC.Free()
    End Sub

    Friend Function ReverseSaveData(ByVal struct As Object, ByVal size As Integer) As Integer()
        Dim bSize As Integer = Marshal.SizeOf(struct)
        Dim bytes(bSize - 1) As Byte
        Dim buffer(size - 1) As Integer

        ConvertStructToBytes(bytes, bSize, struct)
        Array.Reverse(bytes)

        For n As Integer = 0 To buffer.Length - 1
            bSize -= 4
            buffer(n) = BitConverter.ToInt32(bytes, bSize)
        Next

        Return buffer
    End Function

    Friend Function SaveDataReverse(Of T As Structure)(ByVal struct As T) As Byte()
        Dim bSize As Integer = Marshal.SizeOf(struct)
        Dim buffer(bSize - 1) As Byte
        ConvertStructToBytes(buffer, bSize, struct)
        ReverseBytes(buffer, bSize And Not (&H3))
        Return buffer
    End Function

    ''' <summary>
    ''' Инвертирует значение в BigEndian и обратно.
    ''' </summary>
    Friend Function ReverseBytes(ByVal value As Integer) As Integer
        If value = 0 OrElse value = -1 Then Return value

        Return (value << 24) Or
               (value And &HFF00) << 8 Or
               (value And &HFF0000) >> 8 Or
               (value >> 24) And &HFF
    End Function

    Private Sub ReverseBytes(ByRef bytes() As Byte, ByVal length As Integer)
        While (length > 0)
            length -= 4
            Array.Reverse(bytes, length, 4)
        End While

        'Dim n = 0
        'Do
        '    Dim i = n + 3       ' i = 3
        '    Dim v As Byte = bytes(i)
        '    bytes(i) = bytes(n) ' [3] <- [0]
        '    bytes(n) = v        ' [0] <- [3]
        '    i = n + 1           ' i = 1
        '    n += 2              ' n = 2
        '    v = bytes(n)
        '    bytes(n) = bytes(i) ' [2] <- [1]
        '    bytes(i) = v        ' [1] <- [2]
        '    n += 2              ' n = 4
        'Loop While (n < count)
    End Sub

    ''' <summary>
    ''' Преобразовывает структуру в массив.
    ''' </summary>
    Private Sub ConvertStructToBytes(ByRef bytes() As Byte, ByVal bSize As Integer, ByVal struct As Object)
        Dim ptr As IntPtr = Marshal.AllocHGlobal(bSize)
        Marshal.StructureToPtr(struct, ptr, False)
        Marshal.Copy(ptr, bytes, 0, bSize)
        Marshal.FreeHGlobal(ptr)
    End Sub

    ''' <summary>
    ''' Преобразовывает массив в структуру.
    ''' </summary>
    Private Function ConvertBytesToStruct(ByVal data() As Integer, ByVal strcType As Type) As Object
        Dim mGC As GCHandle = GCHandle.Alloc(data, GCHandleType.Pinned)
        Dim obj As Object = Marshal.PtrToStructure(mGC.AddrOfPinnedObject, strcType)
        mGC.Free()
        Return obj
    End Function

End Module
