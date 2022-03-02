Imports System.IO

Module GameConfig

    Friend gameFolder As String
    Friend masterDatPath As String
    Friend critterDatPath As String
    Friend m_patches As String
    Private language As String

    Friend gcExtraMods As List(Of ExtraModData) = New List(Of ExtraModData)

    Friend Sub ReadGameConfig(ByVal cfgFile As String)
        gameFolder = Path.GetDirectoryName(cfgFile) + "\"

        masterDatPath = INIFile.GetString("system", "master_dat", cfgFile, DatFiles.MasterDAT)
        critterDatPath = INIFile.GetString("system", "critter_dat", cfgFile, DatFiles.CritterDAT)
        m_patches = INIFile.GetString("system", "master_patches", cfgFile, DatFiles.DEFAULT_DATA_DIR)
        language = INIFile.GetString("system", "language", cfgFile, "english")
    End Sub

    Friend Sub SetPatches()
        Settings.Game_Path = gameFolder
        Settings.GameDATA_Path = gameFolder & m_patches
        DatFiles.MasterDAT = masterDatPath
        DatFiles.CritterDAT = critterDatPath
    End Sub

    Friend Function GameLanguage() As String
        Return language
    End Function

    Friend Sub SearchExtraModFiles(ByVal gamePathFolder As String, ByVal masterPath As String, ByVal critterPath As String)
        gcExtraMods.Clear()

        ' PatchFile##
        Dim plist As List(Of ExtraModData) = New List(Of ExtraModData)
        Dim ddrawIni = gamePathFolder + "ddraw.ini"
        If (File.Exists(ddrawIni)) Then
            For index = 99 To 0 Step -1
                Dim strValue = INIFile.GetString("ExtraPatches", "PatchFile" + index.ToString, ddrawIni, String.Empty).Trim
                If (strValue <> String.Empty) Then
                    strValue = gamePathFolder + strValue
                    plist.Add(New ExtraModData(strValue, File.Exists(strValue)))
                End If
            Next
        End If

        ' Просмотр папки Mods
        Dim list As List(Of ExtraModData) = New List(Of ExtraModData)
        Dim gameFolderMods = gamePathFolder + "mods" ' <game>\mods
        If (Directory.Exists(gameFolderMods)) Then
            For Each file As String In Directory.GetFiles(gameFolderMods, "*.dat")
                If (file.EndsWith(".dat", StringComparison.OrdinalIgnoreCase) = False) Then Continue For
                If (plist.Exists(Function(x) String.Equals(x.filePath, file, StringComparison.OrdinalIgnoreCase))) Then Continue For

                list.Add(New ExtraModData(file, True))
            Next
            For Each file As String In Directory.GetDirectories(gameFolderMods, "*.dat")
                If (file.EndsWith(".dat", StringComparison.OrdinalIgnoreCase) = False) Then Continue For
                If (plist.Exists(Function(x) String.Equals(x.filePath, file, StringComparison.OrdinalIgnoreCase))) Then Continue For

                list.Add(New ExtraModData(file, False))
            Next
            list.Sort(New Comparer.ExtraModComparer())
            list.Reverse()
        End If

        gcExtraMods.AddRange(list)
        gcExtraMods.AddRange(plist)
        list.Clear()

        Dim masterDatName = Path.GetFileName(masterPath)
        Dim critterDatName = Path.GetFileName(critterPath)

        ' другие dat расположенные в корневой папке игры
        For Each item As String In Directory.GetFiles(gamePathFolder, "*.dat")
            If (item.EndsWith(".dat", StringComparison.OrdinalIgnoreCase) = False) Then Continue For

            If (gcExtraMods.Exists(Function(x) String.Equals(x.filePath, item, StringComparison.OrdinalIgnoreCase))) Then Continue For

            Dim name = Path.GetFileName(item)
            If (String.Equals(name, masterDatName, StringComparison.OrdinalIgnoreCase)) Then Continue For
            If (String.Equals(name, critterDatName, StringComparison.OrdinalIgnoreCase)) Then Continue For

            list.Add(New ExtraModData(item, True))
        Next
        For Each item As String In Directory.GetDirectories(gamePathFolder, "*.dat")
            If (item.EndsWith(".dat", StringComparison.OrdinalIgnoreCase) = False) Then Continue For

            If (gcExtraMods.Exists(Function(x) String.Equals(x.filePath, item, StringComparison.OrdinalIgnoreCase))) Then Continue For

            Dim name = Path.GetFileName(item)
            If (String.Equals(name, masterDatName, StringComparison.OrdinalIgnoreCase)) Then Continue For
            If (String.Equals(name, critterDatName, StringComparison.OrdinalIgnoreCase)) Then Continue For

            list.Add(New ExtraModData(item, False))
        Next
        list.Sort(New Comparer.ExtraModComparer())
        list.Reverse()
        gcExtraMods.AddRange(list)
    End Sub

    Friend Function CheckExtraMod(modPath As String) As ExtraModData
        Dim isDat = File.Exists(modPath)
        If (isDat = False AndAlso Not Directory.Exists(modPath)) Then
            Return Nothing
        End If
        Return New ExtraModData(modPath, isDat)
    End Function

End Module
