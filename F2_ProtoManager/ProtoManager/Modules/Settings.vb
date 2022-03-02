Imports System.IO
Imports System.Text

Friend Module Settings

    Friend ReadOnly WorkAppDIR As String = Application.StartupPath
    Friend ReadOnly Cache_Patch As String = WorkAppDIR & "\cache"

    ' текущие параметры
    Friend Game_Config As String = String.Empty
    Friend Game_Path As String 'Папка игры
    Friend GameDATA_Path As String 'Папка DATA игры
    Friend SaveMOD_Path As String 'Папка в которую сохраняются отредактированные файлы.
    Friend HEX_Path As String

    Friend saveIsEqualData As Boolean

    Friend ReadOnly defaultHEX As String = WorkAppDIR & "\hex\frhed.exe"

    ' значения для сохранения
    Friend sConfigPath, sSaveFolderPath As String
    Friend languagePath As String = "english"

    ' Program sets
    Friend SplitSize As Integer = -1 'default size
    Friend txtWin As Boolean = True
    Friend txtLvCp As Boolean = False
    Friend proRO As Boolean = True
    Friend cCache As Boolean = True
    Friend cArtCache As Boolean = True
    Friend HoverSelect As Boolean = True

    Friend ShowAIPacket As Boolean = True
    Friend SortedAIPacket As Boolean

    Friend ColumnItemSize(4) As Integer
    Friend ColumnCritterSize(4) As Integer

    Friend MsgEncoding As Encoding

    Friend Sub SetDoubleBuffered(ByVal control As Control)
        Dim doubleBufferPropertyInfo = control.GetType().GetProperty("DoubleBuffered", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
        doubleBufferPropertyInfo.SetValue(control, True, Nothing)
    End Sub

    Friend Sub SetEncoding()
        If txtWin Then
            MsgEncoding = Encoding.Default
        Else
            MsgEncoding = Encoding.GetEncoding("cp866")
        End If
    End Sub

    ' Load config from ini
    Friend Sub ReadConfigFile()
        Dim iniFile As StreamReader = File.OpenText(WorkAppDIR & "\config.ini")
        Dim appConfig = New Dictionary(Of String, String)
        Try
            Do Until iniFile.EndOfStream
                Dim param = iniFile.ReadLine.Split("="c)
                If param.Length > 1 Then appConfig.Add(param(0).Trim, param(1).Trim)
            Loop
        Catch ex As Exception
            GoTo SetDefault
        Finally
            iniFile.Close()
        End Try

        If (appConfig.TryGetValue("CommonPath", Game_Config) = False) Then GoTo SetDefault
        If Game_Config = String.Empty Then GoTo SetDefault

        GameConfig.ReadGameConfig(Game_Config)
        GameConfig.SetPatches()

        Dim strValue As String = String.Empty
        If (appConfig.TryGetValue("ModPath", strValue) = False) Then GoTo SetDefault
        SaveMOD_Path = strValue
        If SaveMOD_Path = String.Empty Then SaveMOD_Path = GameDATA_Path

        If (appConfig.TryGetValue("HexPath", strValue)) Then HEX_Path = strValue
        If (appConfig.TryGetValue("LangPath", strValue)) Then languagePath = strValue

        If (appConfig.TryGetValue("ReadOnly", strValue)) Then proRO = CBool(strValue)
        If (appConfig.TryGetValue("MsgWIN", strValue)) Then txtWin = CBool(strValue)
        If (appConfig.TryGetValue("MsgLC", strValue)) Then txtLvCp = CBool(strValue)
        If (appConfig.TryGetValue("ClearCache", strValue)) Then cCache = CBool(strValue)
        If (appConfig.TryGetValue("ClearArtCache", strValue)) Then cArtCache = CBool(strValue)
        If (appConfig.TryGetValue("HoverSelect", strValue)) Then HoverSelect = CBool(strValue)
        If (appConfig.TryGetValue("StatFormula", strValue)) Then CalcStats.SetFormula(CType(Convert.ToInt32(strValue), CalcStats.FormulaType))

        If (appConfig.TryGetValue("SplitSize", strValue)) Then SplitSize = CInt(strValue)
        Dim i As Integer = 0
        If (appConfig.TryGetValue("ColumnIt", strValue)) Then
            Dim sizes = Split(strValue, ",")
            For Each size As String In sizes
                ColumnItemSize(i) = CInt(size.Trim())
                i += 1
                If i > 4 Then Exit For
            Next
        End If
        If (appConfig.TryGetValue("ColumnCr", strValue)) Then
            Dim sizes = Split(strValue, ",")
            i = 0
            For Each size As String In sizes
                ColumnCritterSize(i) = CInt(size.Trim())
                i += 1
                If i > 4 Then Exit For
            Next
        End If

        sConfigPath = Game_Config
        sSaveFolderPath = SaveMOD_Path

        saveIsEqualData = GameDATA_Path.Equals(SaveMOD_Path, StringComparison.OrdinalIgnoreCase)

        For i = 0 To 999
            If (appConfig.TryGetValue(String.Format("ExtraPath{0}", i), strValue) = False) Then
                Exit For
            End If
            Dim values = strValue.Split(","c)
            Dim extra As ExtraModData = CheckExtraMod(values(0).Trim)
            If (extra IsNot Nothing) Then
                extra.isEnabled = CBool(values(1))
                GameConfig.gcExtraMods.Add(extra)
            End If
        Next

        Messages.SetMessageLangPath(languagePath)
        Settings.SetEncoding()
        Exit Sub
        '===============================================

SetDefault:
        Setting_Form.firstRun = True
        Setting_Form.settingExit = True
        SplashScreen.TopMost = False
        Setting_Form.ShowDialog()
        SplashScreen.TopMost = True
    End Sub

    ' Save config to ini
    Friend Sub SaveConfigFile()
        Dim settingParam As New List(Of String)
        settingParam.Add("[Path]")
        settingParam.Add("CommonPath=" & sConfigPath)
        settingParam.Add("ModPath=" & sSaveFolderPath)

        For i = 0 To GameConfig.gcExtraMods.Count - 1
            settingParam.Add(String.Format("ExtraPath{0}=" & String.Join(",", GameConfig.gcExtraMods(i).filePath, GameConfig.gcExtraMods(i).isEnabled), i))
        Next

        settingParam.Add("HexPath=" & HEX_Path)
        settingParam.Add("LangPath=" & languagePath)

        settingParam.Add(String.Empty)
        settingParam.Add("[Option]")
        settingParam.Add("ReadOnly=" & proRO)
        settingParam.Add("MsgWIN=" & txtWin)
        settingParam.Add("MsgLC=" & txtLvCp)
        settingParam.Add("ClearCache=" & cCache)
        settingParam.Add("ClearArtCache=" & cArtCache)
        settingParam.Add("Background=")
        settingParam.Add("HoverSelect=" & HoverSelect)
        settingParam.Add("StatFormula=" & CalcStats.GetFormula().ToString)
        settingParam.Add(String.Empty)
        settingParam.Add("[Size]")
        If Main_Form.WindowState = FormWindowState.Maximized Then
            settingParam.Add("SplitSize=" & Main_Form.SplitContainer1.SplitterDistance)
        Else
            settingParam.Add("SplitSize=" & SplitSize)
        End If
        '
        For i As Integer = 0 To Main_Form.ListView1.Columns.Count - 1
            ColumnCritterSize(i) = Main_Form.ListView1.Columns(i).Width
        Next
        For i As Integer = 0 To Main_Form.ListView2.Columns.Count - 1
            ColumnItemSize(i) = Main_Form.ListView2.Columns(i).Width
        Next
        settingParam.Add("ColumnIt=" & String.Join(",", ColumnItemSize))
        settingParam.Add("ColumnCr=" & String.Join(",", ColumnCritterSize))
        '
        File.WriteAllLines(WorkAppDIR & "\config.ini", settingParam)
    End Sub

    Friend Sub Clear_Cache()
        Clear_Art_Cache()
        On Error Resume Next
        Directory.Delete(Cache_Patch & "\proto", True)
        Directory.Delete(Cache_Patch & "\data", True)
        Directory.Delete(Cache_Patch & "\scripts", True)
        Directory.Delete(Cache_Patch & "\text", True)
        File.Delete(Cache_Patch & "\cache.id")
    End Sub

    Friend Sub Clear_Art_Cache()
        ThumbnailImage.Dispose()
        On Error Resume Next
        Directory.Delete(Cache_Patch & "\art", True)
    End Sub

    Friend Sub Clear_Sound_Cache()
        If Directory.Exists("cache\sound\") Then Directory.Delete("cache\sound\", True)
    End Sub

End Module
