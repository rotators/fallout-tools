Imports System.IO
Imports Microsoft.VisualBasic.FileIO

Public Class AI_Form

    Private Enum MoveDir As SByte
        Up = -1
        Down = 1
    End Enum

    Private aiPath As String                            'текущий путь к AI.txt
    Private AIPacket As Dictionary(Of String, Integer)  'Список всех имен, и их номера строк в файле

    Private fReady As Boolean
    Private aiCustom As Boolean

    Private Const _width As Integer = 287
    Private Const _capform As String = "] AI Packet Editor"

    Private Const AIGENMSG As String = "\data\aigenmsg.txt"
    Private Const AIBODYMSG As String = "\data\aibdymsg.txt"

    Friend Sub New(ByVal numPacket As Integer)
        InitializeComponent()

        ShowPacketIDMenuItem.Checked = ShowAIPacket
        SortedListMenuItem.Checked = SortedAIPacket
        cmbAIPacket.Sorted = SortedAIPacket

        aiPath = DatFiles.CheckFile(AI.AIFILE)
        AIPacket = AI.GetAllAIPackets(aiPath)
        aiCustom = False

        Initialize(numPacket)
    End Sub

    Private Sub Initialize(ByVal numPacket As Integer)
        PacketList(Me) 'ComboBox0.Items.AddRange((From t In AI_Packet Take (AI_Packet.GetLength(1))).ToArray)

        If numPacket <> -1 Then
            For Each packet As String In AIPacket.Keys
                If INIFile.GetInt(packet, "packet_num", aiPath) = numPacket Then
                    cmbAIPacket.SelectedItem = If(ShowPacketIDMenuItem.Checked, String.Format("{0}| {1}", Strings.RSet(numPacket.ToString, 3), packet), packet)
                    Exit For
                End If
            Next
            If cmbAIPacket.SelectedIndex = -1 Then
                MessageBox.Show("AI Packet with this number does not exist.", "Error")
                Me.Dispose()
                Exit Sub
            End If
        Else
            cmbAIPacket.SelectedIndex = 0
        End If

        For Each item As ItemsLst In Items_LST
            If item.itemType = Enums.ItemType.Drugs Then
                tscmbDrugsPIDs.Items.Add(String.Format("{0}| {1}", item.PID.ToString.PadLeft(4, " "c), item.itemName))
            End If
        Next

        Dim tmpPath As String = DatFiles.CheckFile(AIGENMSG)
        ComboBox8.Items.AddRange(AI.GetAllAIPackets(tmpPath).Keys.ToArray)
        ComboBox8.Items.RemoveAt(ComboBox8.Items.Count - 1)

        tmpPath = DatFiles.CheckFile(AIBODYMSG)
        ComboBox4.Items.AddRange(AI.GetAllAIPackets(tmpPath).Keys.ToArray)
        ComboBox4.Items.RemoveAt(ComboBox4.Items.Count - 1)

        Dim packetName As String = AI.GetPacketName(cmbAIPacket.SelectedItem.ToString)
        Me.Text = "[" & packetName & _capform
        Me.Width -= _width

        SetControlValue(packetName)

        Main.SetParent(Me.Handle.ToInt32, Main_Form.SplitContainer1.Handle.ToInt32)
        Me.Show()
    End Sub

    Private Sub Taunts_Show(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        If CInt(CType(sender, Button).Tag) = 0 Then
            Me.Width += _width
            CType(sender, Button).Tag = 1
            CType(sender, Button).Image = My.Resources.LeftArrow
        Else
            Me.Width -= _width
            CType(sender, Button).Tag = 0
            CType(sender, Button).Image = My.Resources.RightArrow
        End If
    End Sub

    Private Sub Select_AI_Packet(ByVal sender As Object, ByVal e As EventArgs) Handles cmbAIPacket.SelectedIndexChanged
        Dim Section As String = AI.GetPacketName(cmbAIPacket.Text)
        If Not (fReady) OrElse Section = String.Empty Then Exit Sub
        Me.Text = "[" & Section & _capform
        SetControlValue(Section)
    End Sub

    Private Sub SetControlValue(ByVal section As String)
        fReady = False
        SetToControls(Me, section)

        'chem_primary_desire
        ListView1.Items.Clear()
        Dim listDrugs() As String = Split(INIFile.GetString(section, "chem_primary_desire", aiPath, AI.NotSetValue), ",")

        If listDrugs(0) <> AI.NotSetValue AndAlso listDrugs(0) <> "-1" Then
            Dim count As Integer = listDrugs.Length - 1
            If count > 2 Then count = 2
            For i = 0 To count
                Dim pid As Integer
                If listDrugs(i).Length = 0 OrElse Integer.TryParse(listDrugs(i), pid) = False Then Continue For
                For Each item In Items_LST
                    If (item.PID = pid) Then
                        ListView1.Items.Add(New ListViewItem({listDrugs(i), item.itemName}))
                        pid = -1
                        Exit For
                    End If
                Next
                If (pid <> -1) Then
                    ListView1.Items.Add(New ListViewItem({listDrugs(i), "<Error PID>"}))
                End If
            Next
        ElseIf (listDrugs(0) = "-1") Then
            ListView1.Items.Add(New ListViewItem({"-1", String.Empty}))
        End If

        SaveButton.Enabled = False
        fReady = True
    End Sub

    Private Sub TextView(ByVal sender As Object, ByVal e As EventArgs) Handles Button6.Click
        Dim sKey As String = AI.GetPacketName(cmbAIPacket.SelectedItem.ToString)
        Dim eKey As String = String.Empty
        If SortedListMenuItem.Checked Then
            Dim keys() As String = AIPacket.Keys.ToArray
            For n = 0 To keys.Length - 1
                If sKey = keys(n) Then
                    eKey = keys(n + 1)
                    Exit For
                End If
            Next
        Else
            Dim sIndx As Integer = cmbAIPacket.SelectedIndex + 1
            If sIndx >= cmbAIPacket.Items.Count Then
                eKey = AI.endPackedID
            Else
                eKey = AI.GetPacketName(cmbAIPacket.Items(sIndx).ToString)
            End If
        End If
        Dim AITxtfrm As New AI_TextForm(AIPacket.Item(sKey), AIPacket.Item(eKey), aiPath, SaveButton.Enabled, aiCustom)

        AITxtfrm.Owner = Me
        AITxtfrm.Text &= sKey
        AITxtfrm.Show()
        Button1.Enabled = True
    End Sub

    Private Sub ToolStripComboBox1_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles tscmbDrugsPIDs.SelectedIndexChanged
        If ListView1.Items.Count < 3 Then
            AddDrugsToolStripMenuItem.Enabled = True
        End If
        ContextMenuStrip1.Focus()
    End Sub

    Private Sub AddDrugs_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AddDrugsToolStripMenuItem.Click
        For n As Integer = 0 To ListView1.Items.Count - 1
            If ListView1.Items(n).Text = "-1" Then
                ListView1.Items.RemoveAt(n)
                Exit For
            End If
        Next
        Dim strPid = tscmbDrugsPIDs.Text.Remove(4).TrimStart
        Dim pid As Integer = CInt(strPid)
        For Each item In Items_LST
            If (item.PID = pid) Then
                ListView1.Items.Add(New ListViewItem({strPid, item.itemName}))
                Exit For
            End If
        Next

        If ListView1.Items.Count > 2 Then
            AddDrugsToolStripMenuItem.Enabled = False
        End If
        SaveButton.Enabled = True
    End Sub

    Private Sub ListView1_AfterLabelEdit(ByVal sender As Object, ByVal e As LabelEditEventArgs) Handles ListView1.AfterLabelEdit
        If e.Label = Nothing OrElse e.CancelEdit Then Exit Sub

        Dim pid As Integer
        If Integer.TryParse(e.Label, pid) = False Then Exit Sub

        For Each item In Items_LST
            If (item.PID = pid) Then
                ListView1.Items(e.Item).SubItems(1).Text = item.itemName
                Exit Sub
            End If
        Next

        ListView1.Items(e.Item).SubItems(1).Text = "<Error PID>"
    End Sub

    Private Sub Delete(ByVal sender As Object, ByVal e As EventArgs) Handles DeleteToolStripMenuItem.Click
        On Error Resume Next
        ListView1.Items.RemoveAt(ListView1.FocusedItem.Index)
        If ListView1.Items.Count < 3 Then
            AddDrugsToolStripMenuItem.Enabled = True
        End If
        SaveButton.Enabled = True
    End Sub

    Private Sub MoveUp(ByVal sender As Object, ByVal e As EventArgs) Handles MoveUpToolStripMenuItem.Click
        MoveItem(MoveDir.Up)
    End Sub

    Private Sub MoveDown(ByVal sender As Object, ByVal e As EventArgs) Handles MoveDownToolStripMenuItem.Click
        MoveItem(MoveDir.Down)
    End Sub

    Private Sub OpenFile(ByVal sender As Object, ByVal e As EventArgs) Handles Button3.Click
        OpenFileDialog1.InitialDirectory = Game_Path
        If OpenFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            If OpenFileDialog1.FileName.Length = 0 Then OpenFile(Nothing, Nothing)
            aiPath = OpenFileDialog1.FileName
        Else
            Exit Sub
        End If
        '
        Try
            AIPacket = AI.GetAllAIPackets(aiPath)
            PacketList(Me)
            cmbAIPacket.SelectedIndex = 0
        Catch ex As Exception
            MsgBox("This file does not contain PacketAI information, or has an incorrect format.", MsgBoxStyle.Critical, "Wrong file format")
            Exit Sub
        End Try
        Select_AI_Packet(Nothing, Nothing)
        SaveButton.Enabled = False
        aiCustom = True
        'Log
        Main.PrintLog("Open AI: " & aiPath)
    End Sub

    Private Sub AI_Form_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
        If SaveButton.Enabled Then
            Dim btn As MsgBoxResult = MsgBox("Save changes to AI file?", MsgBoxStyle.YesNoCancel, "Attention!")
            If btn = MsgBoxResult.Yes Then
                SaveAI(Nothing, Nothing)
            ElseIf btn = MsgBoxResult.Cancel Then
                e.Cancel = True
                Exit Sub
            End If
        End If
        '
        ShowAIPacket = ShowPacketIDMenuItem.Checked
        SortedAIPacket = SortedListMenuItem.Checked
        Main_Form.ToolStripStatusLabel1.Text = String.Empty
        Main_Form.Focus()
    End Sub

    Private Sub SaveAI(ByVal sender As Object, ByVal e As EventArgs) Handles SaveButton.Click
        Dim Section As String = AI.GetPacketName(cmbAIPacket.Text)
        If Not aiCustom Then
            If Not (File.Exists(SaveMOD_Path & AI.AIFILE)) Then FileSystem.CopyFile(aiPath, SaveMOD_Path & AI.AIFILE)
            aiPath = SaveMOD_Path & AI.AIFILE
        End If

        'chem_primary_desire
        Dim List As String = String.Empty
        For Each item As ListViewItem In ListView1.Items
            If item.Text = "-1" Then Exit For
            If List.Length > 0 Then List &= ","
            List &= item.Text
        Next
        If List.Length > 0 Then INIFile.SetValue(Section, "chem_primary_desire", List, aiPath)

        SubSaveControl(Me, Section)
        SaveButton.Enabled = False

        'Log
        Main.PrintLog("Update AI: " & aiPath)
    End Sub

    ' Рекурсивный перебор контролов класса формы
    Private Sub SetToControls(ByRef сontrol As Control, ByVal section As String)
        Try
            For Each ctrl As Control In сontrol.Controls
                If TypeOf ctrl Is NumericUpDown Then
                    ctrl.Text = INIFile.GetInt(section, ctrl.Tag.ToString, aiPath).ToString
                ElseIf (TypeOf ctrl Is ComboBox) And ctrl.Tag IsNot Nothing Then
                    If (ctrl.Tag.ToString = "hurt_too_much") Then
                        ctrl.Text = INIFile.GetString(section, ctrl.Tag.ToString, aiPath, "")
                    Else
                        CType(ctrl, ComboBox).SelectedIndex = -1
                        Dim val = INIFile.GetString(section, ctrl.Tag.ToString, aiPath, AI.NotSetValue).Trim
                        If (val = "-1") Then
                            CType(ctrl, ComboBox).SelectedIndex = 0
                        Else
                            ctrl.Text = val
                        End If
                    End If
                ElseIf TypeOf ctrl Is GroupBox Then
                    SetToControls(ctrl, section)
                End If
            Next
        Catch ex As Exception
            MsgBox("An error occurred while reading of AI block parameters.")
            ReloadFile(Me, False)
            fReady = True
            cmbAIPacket.SelectedIndex = 0
        End Try

    End Sub

    Private Sub SubSaveControl(ByRef control As Control, ByVal section As String)
        For Each ctrl As Control In control.Controls
            If TypeOf ctrl Is NumericUpDown Then
                INIFile.SetValue(section, ctrl.Tag.ToString, ctrl.Text, aiPath)

            ElseIf (TypeOf ctrl Is ComboBox) And ctrl.Tag IsNot Nothing Then
                Dim value = ctrl.Text
                If value = AI.NotSetValue Then
                    If (INIFile.GetString(section, ctrl.Tag.ToString, aiPath, AI.NotSetValue) = AI.NotSetValue) Then
                        Continue For
                    End If
                    value = "-1"
                End If
                INIFile.SetValue(section, ctrl.Tag.ToString, value, aiPath)

            ElseIf TypeOf ctrl Is GroupBox Then
                SubSaveControl(ctrl, section)
            End If
        Next
    End Sub

    Private Sub NumericValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles NumericUpDown2.ValueChanged, NumericUpDown9.ValueChanged, NumericUpDown8.ValueChanged, NumericUpDown7.ValueChanged, NumericUpDown5.ValueChanged, NumericUpDown4.ValueChanged, NumericUpDown33.ValueChanged, NumericUpDown32.ValueChanged, NumericUpDown31.ValueChanged, NumericUpDown30.ValueChanged, NumericUpDown3.ValueChanged, NumericUpDown29.ValueChanged, NumericUpDown28.ValueChanged, NumericUpDown27.ValueChanged, NumericUpDown26.ValueChanged, NumericUpDown25.ValueChanged, NumericUpDown24.ValueChanged, NumericUpDown23.ValueChanged, NumericUpDown22.ValueChanged, NumericUpDown21.ValueChanged, NumericUpDown20.ValueChanged, NumericUpDown19.ValueChanged, NumericUpDown18.ValueChanged, NumericUpDown17.ValueChanged, NumericUpDown16.ValueChanged, NumericUpDown15.ValueChanged, NumericUpDown14.ValueChanged, NumericUpDown13.ValueChanged, NumericUpDown12.ValueChanged, NumericUpDown11.ValueChanged, NumericUpDown1.ValueChanged
        If fReady Then SaveButton.Enabled = True
    End Sub

    Private Sub SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ComboBox1.SelectedIndexChanged, ComboBox9.SelectedIndexChanged, ComboBox8.SelectedIndexChanged, ComboBox7.SelectedIndexChanged, ComboBox6.SelectedIndexChanged, ComboBox5.SelectedIndexChanged, ComboBox4.SelectedIndexChanged, ComboBox3.SelectedIndexChanged, ComboBox2.SelectedIndexChanged, ComboBox10.SelectedIndexChanged
        If fReady Then SaveButton.Enabled = True
    End Sub

    Private Sub AI_Form_Activated(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Activated
        Main_Form.ToolStripStatusLabel1.Text = aiPath
    End Sub

    Friend Shared Sub ReloadFile(ByVal owner As AI_Form, Optional ByVal ready As Boolean = True)
        If Not owner.aiCustom Then owner.aiPath = SaveMOD_Path & AI.AIFILE
        owner.AIPacket.Clear()
        owner.AIPacket = AI.GetAllAIPackets(owner.aiPath)

        Dim indx = owner.cmbAIPacket.SelectedIndex
        PacketList(owner)
        If ready Then owner.cmbAIPacket.SelectedIndex = indx
    End Sub

    Private Sub Sorted_Click(ByVal sender As Object, ByVal e As EventArgs) Handles SortedListMenuItem.Click
        Dim key As String = cmbAIPacket.SelectedItem.ToString
        fReady = False
        If SortedListMenuItem.Checked Then
            cmbAIPacket.Sorted = True
        Else
            cmbAIPacket.Sorted = False
            PacketList(Me)
        End If
        cmbAIPacket.SelectedItem = key
        fReady = True
    End Sub

    Private Sub ShowPacketID(ByVal sender As Object, ByVal e As EventArgs) Handles ShowPacketIDMenuItem.Click
        Dim packet As String = AI.GetPacketName(cmbAIPacket.SelectedItem.ToString)
        fReady = False
        PacketList(Me)
        If ShowPacketIDMenuItem.Checked Then
            For n = 0 To cmbAIPacket.Items.Count
                If AI.GetPacketName(cmbAIPacket.Items.Item(n).ToString) = packet Then
                    cmbAIPacket.SelectedIndex = n
                    Exit For
                End If
            Next
        Else
            cmbAIPacket.SelectedItem = packet
        End If
        fReady = True
    End Sub

    Private Shared Sub PacketList(ByVal frm As AI_Form)
        If frm.cmbAIPacket.Items.Count > 0 Then frm.cmbAIPacket.Items.Clear()
        Dim keys() As String = frm.AIPacket.Keys.ToArray
        Array.Resize(keys, keys.Count - 1)
        If frm.ShowPacketIDMenuItem.Checked Then
            For n = 0 To keys.Length - 1
                keys(n) = String.Format("{0}| {1}", Strings.RSet(INIFile.GetInt(keys(n), "packet_num", frm.aiPath).ToString, 3), keys(n).ToString)
            Next
        End If
        frm.cmbAIPacket.Items.AddRange(keys)
    End Sub

    Private Sub MoveItem(ByVal Direction As MoveDir)
        Try
            Dim selIndex As Integer = ListView1.FocusedItem.Index
            Dim source As ListViewItem = CType(ListView1.Items(selIndex).Clone, ListViewItem)
            Dim dest As ListViewItem = CType(ListView1.Items(selIndex + Direction).Clone, ListViewItem)
            ListView1.Items(selIndex + Direction).Text = source.Text
            ListView1.Items(selIndex + Direction).SubItems(1).Text = source.SubItems(1).Text
            ListView1.Items(selIndex).Text = dest.Text
            ListView1.Items(selIndex).SubItems(1).Text = dest.SubItems(1).Text
            ListView1.Items(selIndex + Direction).Selected = True
            ListView1.Items(selIndex + Direction).Focused = True
        Catch
        End Try
        SaveButton.Enabled = True
    End Sub

    Private Sub AddPacket_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button4.Click
        Dim keys() As String = AIPacket.Keys.ToArray
        Dim maxNumPacket As Integer = 0
        For Each k As String In keys
            If k = AI.endPackedID Then Continue For
            Dim num As Integer = INIFile.GetInt(k, "packet_num", aiPath)
            If num > maxNumPacket Then maxNumPacket = num
        Next
        maxNumPacket += 1
        Dim name As String = InputBox("Enter the name for new AI Packet", "Create AI Packet number: " & maxNumPacket)
        If name.Length = 0 Then Exit Sub

        Dim pathAI As String = aiPath
        If Not aiCustom Then
            pathAI = SaveMOD_Path & AI.AIFILE
            If Not (File.Exists(pathAI)) Then FileSystem.CopyFile(aiPath, pathAI)
        End If

        File.AppendAllText(pathAI, vbCrLf & "[" & name & "]" & vbCrLf)
        File.AppendAllText(pathAI, My.Resources.defaultAI, System.Text.Encoding.Default)

        INIFile.SetValue(name, "packet_num", maxNumPacket.ToString, pathAI)

        Main.PrintLog("Update AI: " & pathAI)
        If (Main.PacketAI IsNot Nothing) Then Main.PacketAI.Clear()

        ReloadFile(Me, False)

        If ShowPacketIDMenuItem.Checked Then
            cmbAIPacket.SelectedItem = String.Format("{0}| {1}", Strings.RSet(maxNumPacket.ToString, 3), name)
        Else
            cmbAIPacket.SelectedItem = name
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    Private Sub RemovePacket(sender As Object, e As EventArgs) Handles btnRemovePacket.Click
        Dim listPackets = AI.GetAllAIPacketNumber(aiPath)

        Dim maxPacket = New KeyValuePair(Of String, Integer)
        For Each packet In listPackets
            If (packet.Value > maxPacket.Value) Then maxPacket = packet
        Next
        If (maxPacket.Key Is Nothing) Then Exit Sub

        Dim name = maxPacket.Key.Remove(maxPacket.Key.LastIndexOf("(") - 1)

        If (MessageBox.Show("Do you want to delete [" + name + "] packet AI from the file?", "Delete packet AI", MessageBoxButtons.YesNo) = DialogResult.No) Then
            Exit Sub
        End If

        Dim startLine = AIPacket.Item(name)
        Dim endLine = startLine

        Dim buffer As List(Of String) = File.ReadAllLines(aiPath).ToList

        For index = startLine + 1 To buffer.Count - 1
            If buffer(index).StartsWith("[") Then
                endLine = index - 1
                Exit For
            End If
        Next
        If startLine = endLine Then
            endLine = buffer.Count
        End If
        If startLine > 0 AndAlso buffer(startLine - 1).Trim.Length = 0 Then startLine -= 1

        buffer.RemoveRange(startLine, endLine - startLine)
        File.WriteAllLines(aiPath, buffer)

        Main.PrintLog("Update AI: " & aiPath)
        If (Main.PacketAI IsNot Nothing) Then Main.PacketAI.Clear()

        ReloadFile(Me, False)
        cmbAIPacket.SelectedIndex = 0
    End Sub

End Class