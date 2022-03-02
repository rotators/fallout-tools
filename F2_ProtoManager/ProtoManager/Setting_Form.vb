Imports System.IO

Friend Class Setting_Form

    'Private Const SC_CLOSE As Int32 = &HF060
    'Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
    ' Нажата кнопка Close.
    '    If m.WParam.ToInt32() = SC_CLOSE Then SettingExt = True
    '    MyBase.WndProc(m)
    'End Sub

    Friend settingExit, firstRun As Boolean
    Private restoreList As Boolean

    Private Sub Setting_Form_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        tbMainPath.Text = Settings.Game_Config
        tbModDataPath.Text = Settings.SaveMOD_Path

        If HEX_Path <> String.Empty Then
            TextBox3.Enabled = True
            TextBox3.Text = HEX_Path
        Else
            TextBox3.Text = defaultHEX
        End If

        RadioButton1.Checked = Settings.txtWin
        RadioButton2.Checked = Not Settings.txtWin
        RadioButton3.Checked = Settings.txtLvCp

        CheckBox2.Checked = Settings.proRO
        CheckBox3.Checked = Settings.cCache
        'CheckBox1.Checked = ExtractBack
        tbMsgLang.Text = Settings.languagePath

        lstCheckMods.Items.Clear()
        For Each el As ExtraModData In DatFiles.extraMods
            lstCheckMods.Items.Add(el.filePath, el.isEnabled)
        Next

        btnUpdate.Enabled = True
    End Sub

    Private Sub Setting_Form_FormClosed(ByVal sender As Object, ByVal e As FormClosedEventArgs) Handles MyBase.FormClosed
        If (restoreList) Then
            Restore()
            restoreList = False
        End If
        selElement = -1
    End Sub

    Private Sub Restore()
        GameConfig.gcExtraMods.Clear()
        GameConfig.gcExtraMods.AddRange(DatFiles.extraMods)
    End Sub

    Private Sub ApplySetting(ByVal sender As Object, ByVal e As EventArgs) Handles btnOK.Click
        Dim config = tbMainPath.Text.Trim
        If (config = String.Empty) Then Return

        Settings.txtWin = RadioButton1.Checked
        Settings.txtLvCp = RadioButton3.Checked
        Settings.proRO = CheckBox2.Checked
        Settings.cCache = CheckBox3.Checked
        'Settings.ExtractBack = CheckBox1.Checked

        Dim tempLang = Settings.languagePath
        Settings.languagePath = tbMsgLang.Text.Trim
        Settings.SetEncoding()

        Dim savePath = tbModDataPath.Text.Trim
        Dim pathChange = Not (config.Equals(Game_Config, StringComparison.OrdinalIgnoreCase)) OrElse Not (savePath.Equals(SaveMOD_Path, StringComparison.OrdinalIgnoreCase))

        If (pathChange) Then
            Dim master = GameConfig.gameFolder + GameConfig.masterDatPath
            If (File.Exists(master) OrElse Directory.Exists(master)) Then
                settingExit = False

                If savePath = String.Empty Then
                    savePath = GameConfig.gameFolder & GameConfig.m_patches
                End If

                Settings.sConfigPath = config
                Settings.sSaveFolderPath = savePath

                UpdateExtraMods(firstRun)
                Settings.SaveConfigFile()

                If (firstRun = False) Then
                    MsgBox("A new path is set." & vbLf & "The editor will now be restarted for the set path to take effect.", MsgBoxStyle.SystemModal)
                    File.Delete(Cache_Patch & "\cache.id")
                    Main_Form.Dispose()
                    Application.Exit()
                    Application.Restart()
                Else
                    Settings.Game_Config = config
                    Settings.SaveMOD_Path = savePath
                    GameConfig.SetPatches()
                    Settings.saveIsEqualData = Settings.GameDATA_Path.Equals(savePath, StringComparison.OrdinalIgnoreCase)
                    Messages.SetMessageLangPath(Settings.languagePath)
                    Me.Close()
                End If
            Else
                Settings.languagePath = tempLang
                MsgBox(String.Format("The file or folder {0} could not be found." & vbLf & "Set the correct path to the folder or DAT file.", Path.GetFullPath(master)))
            End If
        Else
            UpdateExtraMods(True)
            Messages.SetMessageLangPath(Settings.languagePath)
            Settings.SaveConfigFile()
            Me.Close()
        End If
    End Sub

    Private Sub UpdateExtraMods(update As Boolean)
        restoreList = False

        For i As Integer = 0 To lstCheckMods.Items.Count - 1
            If (GameConfig.gcExtraMods(i).filePath.Equals(lstCheckMods.Items(i).ToString, StringComparison.OrdinalIgnoreCase) = False) Then
                ' найти положение
                For n = 0 To GameConfig.gcExtraMods.Count - 1
                    If (GameConfig.gcExtraMods(n).filePath.Equals(lstCheckMods.Items(i).ToString, StringComparison.OrdinalIgnoreCase)) Then

                        Dim temp = GameConfig.gcExtraMods(n)
                        temp.isEnabled = lstCheckMods.GetItemChecked(i)
                        GameConfig.gcExtraMods.RemoveAt(n)
                        GameConfig.gcExtraMods.Insert(i, temp)
                        Exit For

                    End If
                Next
            Else
                GameConfig.gcExtraMods(i).isEnabled = lstCheckMods.GetItemChecked(i)
            End If
        Next

        If (update) Then
            If (DatFiles.extraMods.Count <> 0) Then DatFiles.ClearExtraMods()
            For Each eMod As ExtraModData In GameConfig.gcExtraMods
                DatFiles.AddExtraMod(eMod)
            Next
        End If
    End Sub

    Private Sub OpenConfig(ByVal sender As Object, ByVal e As EventArgs) Handles btnConfig.Click
        OpenFileDialog1.Title = "Select the folder and the game configuration file."
        OpenFileDialog1.Filter = "Config file|*.cfg"

        If (tbMainPath.Text <> String.Empty) Then OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(tbMainPath.Text) + "\"

        If (OpenFileDialog1.ShowDialog() = DialogResult.OK) Then
            Dim cfgFile = OpenFileDialog1.FileName
            If cfgFile.Equals(tbMainPath.Text, StringComparison.OrdinalIgnoreCase) Then Return
            tbMainPath.Text = cfgFile

            GameConfig.ReadGameConfig(cfgFile)
            GameConfig.SearchExtraModFiles(GameConfig.gameFolder, GameConfig.masterDatPath, GameConfig.critterDatPath)
            tbMsgLang.Text = GameConfig.GameLanguage()

            If tbModDataPath.Text.Trim = String.Empty Then
                tbModDataPath.Text = GameConfig.gameFolder + GameConfig.m_patches
            End If

            lstCheckMods.Items.Clear()
            For Each item In GameConfig.gcExtraMods
                lstCheckMods.Items.Add(item.filePath)
            Next
            restoreList = True
            btnUpdate.Enabled = False
        End If
    End Sub

    Private Sub ChangeSaveFolder(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveFolder.Click
        If (String.IsNullOrWhiteSpace(tbMainPath.Text)) Then Return

        FolderBrowserDialog1.SelectedPath = If(tbModDataPath.Text = String.Empty, Path.GetDirectoryName(tbMainPath.Text), tbModDataPath.Text)

        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            If FolderBrowserDialog1.SelectedPath <> String.Empty Then tbModDataPath.Text = FolderBrowserDialog1.SelectedPath
        End If
    End Sub

    Private Sub ClearArtCache(ByVal sender As Object, ByVal e As EventArgs) Handles btnClear.Click
        Clear_Art_Cache()
    End Sub

    Private Sub btnMoveUp_Click(sender As Object, e As EventArgs) Handles btnMoveUp.Click
        Dim i = lstCheckMods.SelectedIndex
        If (i <= 0) Then Return

        Dim isCheck = lstCheckMods.GetItemChecked(i)
        lstCheckMods.Items.Insert(i - 1, lstCheckMods.Items(i))
        lstCheckMods.Items.RemoveAt(i + 1)
        i -= 1
        If (isCheck) Then lstCheckMods.SetItemChecked(i, True)
        lstCheckMods.SetSelected(i, True)
    End Sub

    Private Sub btnMoveDown_Click(sender As Object, e As EventArgs) Handles btnMoveDown.Click
        Dim i = lstCheckMods.SelectedIndex
        If (i = -1 OrElse i = lstCheckMods.Items.Count - 1) Then Return

        Dim isCheck = lstCheckMods.GetItemChecked(i)
        lstCheckMods.Items.Insert(i + 2, lstCheckMods.Items(i))
        lstCheckMods.Items.RemoveAt(i)
        i += 1
        If (isCheck) Then lstCheckMods.SetItemChecked(i, True)
        lstCheckMods.SetSelected(i, True)
    End Sub

    Private cancelCheck As Boolean = False
    Private selElement As Integer = -1

    Private Sub lstCheckMods_MouseClick(sender As Object, e As MouseEventArgs) Handles lstCheckMods.MouseClick
        Dim clickIndex = lstCheckMods.IndexFromPoint(e.Location)
        If clickIndex = -1 Then
            cancelCheck = True
            Return
        End If

        If (e.X < 20) Then
            If selElement <> clickIndex Then
                lstCheckMods.SetItemChecked(clickIndex, Not lstCheckMods.GetItemChecked(clickIndex))
                cancelCheck = True
            End If
            Return
        End If

        If selElement = -1 OrElse selElement <> clickIndex Then Return

        cancelCheck = True
    End Sub

    Private Sub lstCheckMods_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles lstCheckMods.ItemCheck
        If cancelCheck Then
            e.NewValue = e.CurrentValue
            cancelCheck = False
        End If
    End Sub

    Private Sub lstCheckMods_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstCheckMods.SelectedIndexChanged
        selElement = lstCheckMods.SelectedIndex
        cancelCheck = False
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Dim config = tbMainPath.Text.Trim
        If (config = String.Empty) Then Return

        GameConfig.ReadGameConfig(config)
        GameConfig.SetPatches()
        GameConfig.SearchExtraModFiles(Settings.Game_Path, DatFiles.MasterDAT, DatFiles.CritterDAT)
        lstCheckMods.Items.Clear()
        For Each item In GameConfig.gcExtraMods
            lstCheckMods.Items.Add(item.filePath)
        Next
        restoreList = True
    End Sub

    Private Sub Button4_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button4.Click
        OpenFileDialog1.Title = "Select the executable file of the Hex Editor application."
        OpenFileDialog1.Filter = "Execute file|*.exe"
        OpenFileDialog1.InitialDirectory = WorkAppDIR

        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            TextBox3.Text = OpenFileDialog1.FileName
            HEX_Path = OpenFileDialog1.FileName
            TextBox3.Enabled = True
        End If
    End Sub

End Class
