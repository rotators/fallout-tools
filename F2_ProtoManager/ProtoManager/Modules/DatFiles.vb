Imports System.IO
Imports Microsoft.VisualBasic.FileIO

Imports DATLib

Module DatFiles

    ' Example using zlib
    'Declare zlib functions "Compress" and "Uncompress" for compressing Byte Arrays
    '    <DllImport("zlib.DLL", EntryPoint:="compress")> _
    '    Private Function CompressByteArray(ByVal dest As Byte(), ByRef destLen As Integer, ByVal src As Byte(), ByVal srcLen As Integer) As Integer
    ' Leave function empty - DLLImport attribute forwards calls to CompressByteArray to compress in zlib.dLL
    '    End Function
    '    <DllImport("zlib.DLL", EntryPoint:="uncompress")> _
    '    Private Function UncompressByteArray(ByVal dest As Byte(), ByRef destLen As Integer, ByVal src As Byte(), ByVal srcLen As Integer) As Integer
    ' Leave function empty - DLLImport attribute forwards calls to UnCompressByteArray to Uncompress in zlib.dLL
    '   End Function

    ' текуший путь к master.dat и critter.dat
    Friend MasterDAT As String = "master.dat"
    Friend CritterDAT As String = "critter.dat"

    Friend Const DEFAULT_DATA_DIR As String = "data"

    Friend Const ART_CRITTERS_PATH As String = "\art\critters\"
    Friend Const ART_INVEN As String = "\art\inven\"
    Friend Const ART_ITEMS As String = "\art\items\"

    Friend Const PROTO_CRITTERS As String = "\proto\critters\"
    Friend Const PROTO_ITEMS As String = "\proto\items\"

    ' List path files
    Friend Const itemsLstPath As String = "\proto\items\items.lst"
    Friend Const crittersLstPath As String = "\proto\critters\critters.lst"
    Friend Const scriptsLstPath As String = "\scripts\scripts.lst"
    Friend Const miscLstPath As String = "\proto\misc\misc.lst"

    Friend Const artCrittersLstPath As String = "\art\critters\critters.lst"
    Friend Const artItemsLstPath As String = "\art\items\items.lst"
    Friend Const artInvenLstPath As String = "\art\inven\inven.lst"

    'Friend Const sfxLstPath As String = "\sound\sfx\sndlist.lst"

    'Friend Const proCritMsgPath As String = "\Text\English\Game\pro_crit.msg"
    'Friend Const proItemMsgPath As String = "\Text\English\Game\pro_item.msg"

    ' текущий список
    Friend extraMods As List(Of ExtraModData) = New List(Of ExtraModData)

    Friend Sub AddExtraMod(ByRef modFile As ExtraModData)
        extraMods.Add(modFile)
        If (modFile.isDat AndAlso modFile.isEnabled) Then
            Dim message As String = String.Empty
            DATManage.OpenDatFile(modFile.filePath, message)
        End If
    End Sub

    Friend Sub OpenDatFiles()
        Dim message As String = String.Empty
        If File.Exists(Game_Path & MasterDAT) Then
            If (DATManage.OpenDatFile(Game_Path & MasterDAT, message) = False) Then
                MsgBox(message)
            End If
        End If
        If File.Exists(Game_Path & CritterDAT) Then
            DATManage.OpenDatFile(Game_Path & CritterDAT, message)
        End If
        For Each eMod As ExtraModData In GameConfig.gcExtraMods
            DatFiles.AddExtraMod(eMod)
        Next
    End Sub

    Friend Sub ClearExtraMods()
        For Each modFile As ExtraModData In extraMods
            If (modFile.isDat AndAlso modFile.isEnabled) Then DATManage.CloseDatFile(modFile.filePath)
        Next
        extraMods.Clear()
    End Sub

    'Friend Function GetFilesList(folder As String) As List(Of String)
    '    Dim files As List(Of String) = New List(Of String)

    '    For i = 0 To DatFiles.extraMods.Count - 1
    '        If (DatFiles.extraMods(i).isEnabled) Then
    '            Dim filesList As List(Of String)
    '            If (DatFiles.extraMods(i).isDat) Then
    '                filesList = DATManage.GetFileList(DatFiles.extraMods(i).filePath, folder)
    '            Else
    '                filesList = Directory.GetFiles(DatFiles.extraMods(i).filePath + folder).ToList
    '            End If
    '            AddToList(files, filesList)
    '        End If
    '    Next

    '    Dim datFile = Game_Path & CritterDAT
    '    If (File.Exists(datFile)) Then
    '        AddToList(files, DATManage.GetFileList(datFile, folder))
    '    Else
    '        AddToList(files, Directory.GetFiles(datFile + folder).ToList)
    '    End If

    '    datFile = Game_Path & MasterDAT
    '    If (File.Exists(Game_Path & MasterDAT)) Then
    '        AddToList(files, DATManage.GetFileList(datFile, folder))
    '    Else
    '        AddToList(files, Directory.GetFiles(datFile + folder).ToList)
    '    End If

    '    Return files
    'End Function

    'Private Sub AddToList(files As List(Of String), filesList As List(Of String))
    '    If (files.Count = 0) Then
    '        files.AddRange(filesList)
    '    Else
    '        For Each file In filesList
    '            If (files.Contains(file) = False) Then
    '                files.Add(file)
    '            End If
    '        Next
    '    End If
    'End Sub

    Friend Sub UnpackedFilesByList(ByRef files As String(), ByVal datPath As String, ByVal unpackPath As String)
        DATManage.ExtractFileList(unpackPath, files, datPath)
    End Sub

    Friend Sub UnpackedFilesByList(ByVal files As List(Of String), ByVal unpackPath As String)
        Dim arrayFiles = files.ToArray()

        DatFiles.UnpackedFilesByList(arrayFiles, Game_Path & MasterDAT, unpackPath)
        DatFiles.UnpackedFilesByList(arrayFiles, Game_Path & CritterDAT, unpackPath)

        For i = DatFiles.extraMods.Count - 1 To 0 Step -1
            If (DatFiles.extraMods(i).isEnabled AndAlso DatFiles.extraMods(i).isDat) Then
                DatFiles.UnpackedFilesByList(arrayFiles, DatFiles.extraMods(i).filePath, unpackPath)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Проверить наличие файла и возвратить путь к нему, если такого файла не найдено то извлечь его из Dat архива.
    ''' full = false, возвращает короткий путь к файлу.
    ''' </summary>
    Friend Function CheckFile(ByRef pFile As String, Optional ByVal full As Boolean = True,
                              Optional ByVal сDat As Boolean = False, Optional ByVal unpack As Boolean = True) As String
        ' save folder
        Dim filePath As String = String.Concat(SaveMOD_Path, pFile)
        If File.Exists(filePath) Then
            Return If(full, filePath, SaveMOD_Path)
        End If

        ' data folder
        If (Settings.saveIsEqualData = False) Then
            filePath = String.Concat(GameDATA_Path & pFile)
            If File.Exists(filePath) Then
                Return If(full, filePath, GameDATA_Path)
            End If
        End If

        ' проверка цепочки модов
        For Each eMod In extraMods
            If (eMod.isEnabled = False) Then Continue For
            If (eMod.isDat) Then
                If (UnDatFile(pFile, eMod)) Then
                    filePath = String.Concat(Settings.Cache_Patch, pFile)
                    Return If(full, filePath, Settings.Cache_Patch)
                End If
            Else
                filePath = String.Concat(eMod.filePath, pFile)
                If File.Exists(filePath) Then Return If(full, filePath, eMod.filePath)
            End If
        Next

        ' папки master.dat и critter.dat
        If (сDat) Then
            filePath = String.Concat(Game_Path, CritterDAT, pFile)
            If File.Exists(filePath) Then Return If(full, filePath, String.Concat(Game_Path, CritterDAT)) 'папка
        Else
            filePath = String.Concat(Game_Path, MasterDAT, pFile)
            If File.Exists(filePath) Then Return If(full, filePath, String.Concat(Game_Path, MasterDAT)) 'папка
        End If

        ' кеш папка
        filePath = String.Concat(Settings.Cache_Patch, pFile)
        If (File.Exists(filePath) = False) Then
            If unpack Then
                DatGetFile(pFile, сDat) ' извлечение из master.dat или critter.dat
            Else
                filePath = Nothing
            End If
        End If
        Return If(full, filePath, Settings.Cache_Patch)
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="pFile"></param>
    ''' <param name="сDat"></param>
    ''' <returns></returns>
    Friend Function GetFileAndPath(ByRef pFile As String, Optional ByVal сDat As Boolean = False) As Boolean
        'Check save folder
        Dim filePath As String = String.Concat(SaveMOD_Path, pFile)
        If File.Exists(filePath) Then
            pFile = filePath
            Return True
        End If

        'Check data folder
        If (Settings.saveIsEqualData = False) Then
            filePath = String.Concat(GameDATA_Path & pFile)
            If File.Exists(filePath) Then
                pFile = filePath
                Return True
            End If
        End If

        ' проверка цепочки модов
        For Each eMod In extraMods
            If (eMod.isEnabled = False) Then Continue For
            If (eMod.isDat) Then
                If (UnDatFile(pFile, eMod)) Then
                    pFile = Nothing 'указать что файл был извлечен и находится в кеше
                    Return True
                End If
            Else
                filePath = String.Concat(eMod.filePath, pFile)
                If File.Exists(filePath) Then
                    pFile = filePath
                    Return True
                End If
            End If
        Next

        'Check .dat folder
        filePath = String.Concat(Game_Path, If(сDat, CritterDAT, MasterDAT), pFile)
        If File.Exists(filePath) Then
            pFile = filePath
            Return True
        End If

        'Check cache folder and extract
        filePath = String.Concat(Settings.Cache_Patch, pFile)
        If File.Exists(filePath) Then
            pFile = Nothing 'указать что файл находится в кеше
            Return True
        ElseIf DatGetFile(pFile, сDat) Then
            pFile = Nothing 'указать что файл был извлечен и находится в кеше
            Return True
        End If

        Return False
    End Function

    ''' <summary>
    ''' Проверяет физическое наличие файла в папке
    ''' </summary>
    ''' <param name="pFile"></param>
    ''' <param name="cache">дополнительно проверка в кеше</param>
    ''' <param name="сDat"></param>
    ''' <returns></returns>
    Friend Function CheckDirFile(ByRef pFile As String, ByVal cache As Boolean, Optional ByVal сDat As Boolean = False) As Boolean
        'Check save folder
        Dim filePath As String = String.Concat(SaveMOD_Path, pFile)
        If File.Exists(filePath) Then Return True

        'Check data folder
        If (Settings.saveIsEqualData = False) Then
            filePath = String.Concat(GameDATA_Path & pFile)
            If File.Exists(filePath) Then Return True
        End If

        ' проверка цепочки модов
        For Each eMod In extraMods
            If (eMod.isEnabled = False OrElse eMod.isDat) Then Continue For
            filePath = String.Concat(eMod.filePath, pFile)
            If File.Exists(filePath) Then Return True
        Next

        'Check .dat folder
        filePath = String.Concat(Game_Path, If(сDat, CritterDAT, MasterDAT), pFile)
        If File.Exists(filePath) Then
            Return True
        End If

        If cache Then
            'Check cache folder
            filePath = String.Concat(Settings.Cache_Patch, pFile)
            If File.Exists(filePath) Then Return True
        End If

        Return False
    End Function

    ''' <summary>
    ''' Проверить содержится ли указанный файл в кэш папке, если его нет то извлечь его.
    ''' </summary>
    Friend Function UnDatFile(ByRef pFile As String, ByVal size As Integer) As Boolean
        Dim cPathFile As String = Settings.Cache_Patch & pFile
        If File.Exists(cPathFile) AndAlso FileSystem.GetFileInfo(cPathFile).Length = size Then Return True

        Return DatGetFile(pFile)
    End Function

    ''' <summary>
    ''' Ивлекает файл из указанного dat файла
    ''' </summary>
    ''' <param name="pFile"></param>
    ''' <param name="fileDAT"></param>
    ''' <returns></returns>
    Friend Function UnDatFile(ByRef pFile As String, ByRef dat As ExtraModData) As Boolean
        If File.Exists(Settings.Cache_Patch & pFile) Then Return True
        Return ExtraDatGetFile(pFile, dat)
    End Function

    ''' <summary>
    ''' Извлечь требуемый файл в кэш папку
    ''' </summary>
    ''' <param name="pFile"></param>
    ''' <param name="сDat"></param>
    Private Function DatGetFile(ByRef pFile As String, Optional ByVal сDat As Boolean = False) As Boolean
        Dim fileDAT As String
        If сDat Then
            fileDAT = CritterDAT
            Main.PrintLog("Extract Critter: " & pFile)
        Else
            fileDAT = MasterDAT
            Main.PrintLog("Extract Master: " & pFile)
        End If

        Dim result = DATManage.ExtractFile("cache\", pFile.Remove(0, 1), Game_Path & fileDAT)
        If (result = False) Then Main.PrintLog(" - Failed!", False)

        Return result
    End Function

    Private Function ExtraDatGetFile(ByRef pFile As String, ByRef dat As ExtraModData) As Boolean
        Dim result = DATManage.ExtractFile("cache\", pFile.Remove(0, 1), dat.filePath)
        If (result) Then Main.PrintLog("Extract: " & dat.fileName & pFile)
        Return result
    End Function

    ''' <summary>
    ''' Получить frm файл криттера в gif формате
    ''' </summary>
    Friend Sub CritterFrmToGif(ByRef FrmFile As String)
        Dim checkFile As String = ART_CRITTERS_PATH & FrmFile & "aa.frm"
        Dim cPath As String = SaveMOD_Path & checkFile

        ExtractConvertFRM(cPath, checkFile, FrmFile & "aa", CritterDAT)

        Dim artDir As String = Path.GetDirectoryName(WorkAppDIR & checkFile)
        If Not Directory.Exists(artDir) Then
            Exit Sub
        End If

        Dim gifFile As String = Path.ChangeExtension(checkFile, ".gif")

        On Error Resume Next
        If File.Exists(WorkAppDIR & gifFile) Then
            FileSystem.MoveFile(WorkAppDIR & gifFile, Settings.Cache_Patch & gifFile)
        Else
            FileSystem.MoveFile(WorkAppDIR & ART_CRITTERS_PATH & FrmFile & "aa_sw.gif", Settings.Cache_Patch & gifFile)
        End If
        On Error GoTo - 1
        Directory.Delete(artDir, True)
    End Sub

    ''' <summary>
    ''' Получить frm файл предмета в gif формате
    ''' </summary>
    Friend Sub ItemFrmToGif(ByRef iPath As String, ByRef FrmFile As String)
        Dim checkFile As String = "\art\" & iPath & FrmFile & ".frm"
        Dim cPath As String = SaveMOD_Path & checkFile

        ExtractConvertFRM(cPath, checkFile, FrmFile, MasterDAT)

        Dim artDir As String = Path.GetDirectoryName(WorkAppDIR & checkFile) & "\"
        If Not Directory.Exists(artDir) Then
            Exit Sub
        End If

        Dim gifFile As String = Path.ChangeExtension(checkFile, ".gif")

        On Error Resume Next
        If File.Exists(WorkAppDIR & gifFile) Then
            FileSystem.MoveFile(WorkAppDIR & gifFile, Settings.Cache_Patch & gifFile)
        Else
            FileSystem.MoveFile(artDir & FrmFile & "_ne.gif", Settings.Cache_Patch & gifFile)
        End If
        On Error GoTo - 1
        Directory.Delete(artDir, True)
    End Sub

    ''' <summary>
    ''' Преобразовать frm файл в gif формат
    ''' </summary>
    Private Sub ExtractConvertFRM(ByVal cPath As String, ByVal checkFile As String, ByVal FrmFile As String, ByVal nameDAT As String)
        If Not (File.Exists(cPath)) Then ' проверка save папки
            If (Settings.saveIsEqualData OrElse Not (File.Exists(GameDATA_Path & checkFile))) Then ' проверка data папки
                ' проверка цепочки модов
                For Each eMod In extraMods
                    If (eMod.isEnabled = False) Then Continue For
                    If (eMod.isDat) Then
                        If (DATManage.FileExists(checkFile, eMod.filePath)) Then
                            ' Извлекает и конверирует требуемый файл
                            Shell(WorkAppDIR & "\frm2gif.exe -d -f """ & eMod.filePath & """ -p color.pal " & FrmFile & ".frm", AppWinStyle.Hide, True, 2000)
                            Exit Sub
                        End If
                    Else
                        cPath = String.Concat(eMod.filePath, checkFile)
                        If File.Exists(cPath) Then
                            nameDAT = Nothing
                            Exit For
                        End If
                    End If
                Next
                If (nameDAT IsNot Nothing) Then
                    cPath = Game_Path & nameDAT & checkFile
                    If Not (File.Exists(cPath)) Then
                        'Извлекает и конверирует требуемый файл
                        Shell(WorkAppDIR & "\frm2gif.exe -d -f """ & Game_Path & nameDAT & """ -p color.pal " & FrmFile & ".frm", AppWinStyle.Hide, True, 2000)
                        Exit Sub
                    End If
                End If
            End If
        End If
        ' копирует файл в папку программы
        FileSystem.CopyFile(cPath, WorkAppDIR & checkFile, True)
        File.SetAttributes(WorkAppDIR & checkFile, FileAttributes.Normal)
        Shell(WorkAppDIR & "\frm2gif.exe -p color.pal ." & checkFile, AppWinStyle.Hide, True, 2000)
    End Sub

    Friend Function ExtractSFXFile(ByVal sfxFile As String) As String
        Dim cfilePath = String.Concat(Settings.Cache_Patch, sfxFile)
        If GetFileAndPath(sfxFile) = False Then Return Nothing 'file not found

        If (sfxFile <> Nothing) Then
            Directory.CreateDirectory(Path.GetDirectoryName(cfilePath))
            File.Copy(sfxFile, cfilePath, True) ' копирует acm файл в кеш папку
        End If

        Shell(WorkAppDIR & "\acm2wav.exe """ & cfilePath & """ -m", AppWinStyle.Hide, True, 5000) ' конвертирует acm в корневую папку программы

        cfilePath = Path.ChangeExtension(cfilePath, "wav")
        Dim sfxWavFile = Path.GetFileName(cfilePath)

        File.Copy(sfxWavFile, cfilePath, True) ' копирует wav файл в кеш папку
        File.Delete(sfxWavFile)

        Return cfilePath
    End Function

End Module
