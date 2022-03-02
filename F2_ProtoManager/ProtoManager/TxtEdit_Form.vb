Imports System.IO
Imports System.Drawing
Imports System.Globalization

Imports Prototypes
Imports Enums

Friend Class TxtEdit_Form

    Private Enum GroupsType As Byte
        Common
        Stats
        Skills
        Defence
        Weapon
        Ammo
        Armor
        Drugs
        Misc
    End Enum

    Private LocIndex As Integer
    Private ReadOnly index As Integer
    Private ReadOnly type As ProType

    Private critterData(ProtoMemberCount.Critter - 1) As Integer
    Private itemData(ProtoMemberCount.Common - 1) As Integer '1+byte
    Private itemSubTypeData() As Integer
    Private itemKeyData As Integer
    Private itemSoundData As Byte
    Private itemwSoundData As Byte

#Region "Массивы параметров"
    Private ReadOnly CrtNamePro As List(Of String) = New List(Of String)() From {
       "pid",
       "msg_id",
       "fid",
       "light_distance",
       "light_intensity",
       "flags",
       "flags_ext",
       "script_id",
       "head_fid",
       "ai_packet",
       "team_num",
       "critter_flags",
       "base_stat_srength",
       "base_stat_prception",
       "base_stat_endurance",
       "base_stat_charisma",
       "base_stat_intelligence",
       "base_stat_agility",
       "base_stat_luck",
       "base_stat_hp",
       "base_stat_ap",
       "base_stat_ac",
       "base_stat_unarmed_damage",
       "base_stat_melee_damage",
       "base_stat_carry_weight",
       "base_stat_sequence",
       "base_stat_healing_rate",
       "base_stat_critical_chance",
       "base_stat_better_criticals",
       "base_dt_normal",
       "base_dt_laser",
       "base_dt_fire",
       "base_dt_plasma",
       "base_dt_electrical",
       "base_dt_emp",
       "base_dt_explode",
       "base_dr_normal",
       "base_dr_laser",
       "base_dr_fire",
       "base_dr_plasma",
       "base_dr_electrical",
       "base_dr_emp",
       "base_dr_explode",
       "base_dr_radiation",
       "base_dr_poison",
       "base_age",
       "base_gender",
       "bonus_stat_srength",
       "bonus_stat_prception",
       "bonus_stat_endurance",
       "bonus_stat_charisma",
       "bonus_stat_intelligence",
       "bonus_stat_agility",
       "bonus_stat_luck",
       "bonus_stat_hp",
       "bonus_stat_ap",
       "bonus_stat_ac",
       "bonus_stat_unarmed_damage",
       "bonus_stat_melee_damage",
       "bonus_stat_carry_weight",
       "bonus_stat_sequence",
       "bonus_stat_healing_rate",
       "bonus_stat_critical_chance",
       "bonus_stat_better_criticals",
       "bonus_dt_normal",
       "bonus_dt_laser",
       "bonus_dt_fire",
       "bonus_dt_plasma",
       "bonus_dt_electrical",
       "bonus_dt_emp",
       "bonus_dt_explode",
       "bonus_dr_normal",
       "bonus_dr_laser",
       "bonus_dr_fire",
       "bonus_dr_plasma",
       "bonus_dr_electrical",
       "bonus_dr_emp",
       "bonus_dr_explode",
       "bonus_dr_radiation",
       "bonus_dr_poison",
       "bonus_age",
       "bonus_gender",
       "skill_small_guns",
       "skill_big_guns",
       "skill_energy_weapons",
       "skill_unarmed",
       "skill_melee",
       "skill_throwing",
       "skill_first_aid",
       "skill_doctor",
       "skill_sneak",
       "skill_lockpick",
       "skill_steal",
       "skill_traps",
       "skill_science",
       "skill_repair",
       "skill_speech",
       "skill_barter",
       "skill_gambling",
       "skill_outdoorsman",
       "body_type",
       "exp_val",
       "kill_type",
       "damage_type"}

    Private Const SndNamePro As String = "sound_id"
    Private Const wSndNamePro As String = "weapon_sound_id"
    Private Const KeyNamePro As String = "unknown"

    Private ReadOnly ItmNamePro() As String = {
       "pid",
       "msg_id",
       "fid",
       "light_distance",
       "light_intensity",
       "flags",
       "flags_ext",
       "script_id",
       "obj_subtype",
       "material_id",
       "size",
       "weight",
       "cost",
       "inventory_fid"}

    Private ReadOnly ArmNamePro() As String = {
       "armor_class",
       "armor_dr_normal",
       "armor_dr_laser",
       "armor_dr_fire",
       "armor_dr_plasma",
       "armor_dr_electrical",
       "armor_dr_emp",
       "armor_dr_explode",
       "armor_dt_normal",
       "armor_dt_laser",
       "armor_dt_fire",
       "armor_dt_plasma",
       "armor_dt_electrical",
       "armor_dt_emp",
       "armor_dt_explode",
       "armor_perk_id",
       "armor_male_fid",
       "armor_female_fid"}

    Private ReadOnly DrgNamePro() As String = {
        "modify_stat0",
        "modify_stat1",
        "modify_stat2",
        "instant_amount0",
        "instant_amount1",
        "instant_amount2",
        "first_duration",
        "first_amount0",
        "first_amount1",
        "first_amount2",
        "second_duration",
        "second_amount0",
        "second_amount1",
        "second_amount2",
        "addiction_rate",
        "addiction_effect",
        "addiction_onset_time"}

    Private ReadOnly WpnNamePro() As String = {
        "weapon_anim_code",
        "min_dmg",
        "max_dmg",
        "dmg_type",
        "primary_attack_max_range",
        "secondary_attack_max_range",
        "proj_pid",
        "min_str",
        "primary_attack_ap_cost",
        "secondary_attack_ap_cost",
        "critical_fail",
        "weapon_perk",
        "weapon_rounds",
        "weapon_caliber",
        "weapon_ammo_pid",
        "weapon_ammo_max"}

    Private ReadOnly AmmNamePro() As String = {
        "ammo_caliber",
        "ammo_quantity",
        "ammo_ac_adjust",
        "ammo_dr_adjust",
        "ammo_damage_mult",
        "ammo_damage_div"}

    Private ReadOnly MscNamePro() As String = {
        "misc_power_pid",
        "misc_power_type",
        "misc_charges"}

    Private ReadOnly CntNamePro() As String = {
        "container_max_size",
        "container_open_flags"}
#End Region

    Sub New(ByVal index As Integer, ByVal type As ProType)
        InitializeComponent()
        Me.index = index
        Me.type = type
    End Sub

    Friend Sub Init_Data()
        If type = ProType.Critter Then
            If ProFiles.LoadCritterProData(Critter_LST(index).proFile, critterData) Then GoTo BadFormat

            For n = 0 To critterData.Length - 1      ' collumn param           value             hex
                ListView1.Items.Add(New ListViewItem({CrtNamePro(n), critterData(n).ToString, "0x" + Hex(critterData(n))}))
                ' Groups
                If (n > 63 And n < 80) OrElse (n > 28 And n < 45) Then
                    ListView1.Items(n).Group = ListView1.Groups.Item(GroupsType.Defence)
                ElseIf (n > 11 And n < 29) OrElse (n >= 47 And n < 64) Then
                    ListView1.Items(n).Group = ListView1.Groups.Item(GroupsType.Stats)
                ElseIf (n > 81 And n < 100) Then
                    ListView1.Items(n).Group = ListView1.Groups.Item(GroupsType.Skills)
                Else
                    ListView1.Items(n).Group = ListView1.Groups.Item(GroupsType.Common)
                End If
            Next
        Else
            Dim fFile As Integer = FreeFile()
            Dim filePath As String = DatFiles.CheckFile(PROTO_ITEMS & Items_LST(index).proFile)

            On Error GoTo BadFormat

            FileOpen(fFile, filePath, OpenMode.Binary, OpenAccess.Read, OpenShare.Shared)
            GetProData(ItmNamePro, itemData, fFile, 0, GroupsType.Common)
            FileGet(fFile, itemSoundData)
            ListView1.Items.Add(New ListViewItem({SndNamePro, itemSoundData.ToString, "0x" + Hex(itemSoundData)}))
            ListView1.Items(14).Group = ListView1.Groups.Item(GroupsType.Common)

            Const count As Integer = 15
            Select Case itemData(Prototypes.DataOffset.ItemSubTypeIndex)
                Case ItemType.Weapon
                    ReDim itemSubTypeData(ProtoMemberCount.Weapon - 1) '1+byte
                    GetProData(WpnNamePro, itemSubTypeData, fFile, count, GroupsType.Weapon)
                    FileGet(fFile, itemwSoundData)
                    ListView1.Items.Add(New ListViewItem({wSndNamePro, itemwSoundData.ToString, "0x" + Hex(itemwSoundData)}))
                    ListView1.Items(count + 16).Group = ListView1.Groups.Item(GroupsType.Weapon)

                Case ItemType.Armor
                    ReDim itemSubTypeData(ProtoMemberCount.Armor - 1)
                    GetProData(ArmNamePro, itemSubTypeData, fFile, count, GroupsType.Armor)

                Case ItemType.Ammo
                    ReDim itemSubTypeData(ProtoMemberCount.Ammo - 1)
                    GetProData(AmmNamePro, itemSubTypeData, fFile, count, GroupsType.Ammo)

                Case ItemType.Container
                    ReDim itemSubTypeData(ProtoMemberCount.Container - 1)
                    GetProData(CntNamePro, itemSubTypeData, fFile, count, GroupsType.Misc)

                Case ItemType.Drugs
                    ReDim itemSubTypeData(ProtoMemberCount.Drugs - 1)
                    GetProData(DrgNamePro, itemSubTypeData, fFile, count, GroupsType.Drugs)

                Case ItemType.Misc
                    ReDim itemSubTypeData(ProtoMemberCount.Misc - 1)
                    GetProData(MscNamePro, itemSubTypeData, fFile, count, GroupsType.Misc)

                Case Else 'ItemType.Key
                    FileGet(fFile, itemKeyData)
                    itemKeyData = ProFiles.ReverseBytes(itemKeyData)
                    ListView1.Items.Add(New ListViewItem({KeyNamePro, itemKeyData.ToString, "0x" + Hex(itemKeyData)}))
                    ListView1.Items(count).Group = ListView1.Groups.Item(GroupsType.Misc)
            End Select
            FileClose(fFile)
        End If
        On Error GoTo 0

        If Not (Me.Visible) Then
            Me.Text = GetNameMsg() & Me.Text
            Me.Show()
        End If
        Exit Sub

BadFormat:
        FileClose()
        MsgBox("Error reading Pro file, maybe he does have the not correct format.", MsgBoxStyle.Critical)
        Main_Form.Focus()
        Me.Dispose()
    End Sub

    Private Sub GetProData(ByRef name() As String, ByRef data() As Integer, ByVal fFile As Integer, ByVal count As Integer, ByVal groups As GroupsType)
        FileGet(fFile, data)
        For n = 0 To data.Length - 1
            data(n) = ProFiles.ReverseBytes(data(n))
            ListView1.Items.Add(New ListViewItem({name(n), data(n).ToString, "0x" + Hex(data(n))}))
            ListView1.Items(count + n).Group = ListView1.Groups.Item(groups)
        Next
    End Sub

    Private Function GetNameMsg() As String
        Dim msg As String, NameID As Integer

        If type = ProType.Critter Then
            NameID = critterData(1)
            msg = "pro_crit.msg"
        Else
            NameID = itemData(1)
            msg = "pro_item.msg"
        End If
        GetMsgData(msg)

        Return GetNameObject(NameID)
    End Function

    Private Sub StartEdit(ByVal sender As Object, ByVal e As EventArgs) Handles ListView1.DoubleClick
        LocIndex = ListView1.FocusedItem.Index
        TextBox1.Enabled = True
        TextBox1.Text = ListView1.Items.Item(LocIndex).SubItems(1).Text

        If CheckBox1.Checked Then TextBox1.Text = Hex(TextBox1.Text)
        TextBox1.Tag = TextBox1.Text

        If ListView1.Items.Item(LocIndex).BackColor = Color.MistyRose Then
            ListView1.Items.Item(LocIndex).BackColor = Color.Pink
        Else
            ListView1.Items.Item(LocIndex).BackColor = Color.LightPink
        End If

        TextBox1.Focus()
    End Sub

    Private Sub EndEdit(ByVal sender As Object, ByVal e As EventArgs) Handles TextBox1.Leave
        Dim TxtChangEn As Boolean = False

        TextBox1.Enabled = False

        Dim valueText As String = TextBox1.Text
        If (valueText <> TextBox1.Tag.ToString) AndAlso valueText.Length > 0 Then TxtChangEn = True

        If TxtChangEn OrElse ListView1.Items.Item(LocIndex).BackColor = Color.Pink Then
            ListView1.Items.Item(LocIndex).BackColor = Color.MistyRose
        Else
            ListView1.Items.Item(LocIndex).BackColor = Color.White
        End If
        If TxtChangEn Then
            Dim value As Integer
            If CheckBox1.Checked Then
                value = Integer.Parse(valueText, NumberStyles.AllowHexSpecifier)
                valueText = value.ToString()
            Else
                If (Integer.TryParse(valueText, value) = False) Then Exit Sub
            End If

            ListView1.Items.Item(LocIndex).SubItems(1).Text = valueText
            ListView1.Items.Item(LocIndex).SubItems(2).Text = "0x" & Convert.ToString(value, 16).ToUpper

            If type = ProType.Critter Then
                critterData(LocIndex) = value
            Else
                If LocIndex <= 13 Then
                    itemData(LocIndex) = value
                Else
                    If LocIndex = 14 Then
                        itemSoundData = CByte(value)
                    Else
                        LocIndex -= 15
                        Select Case itemData(Prototypes.DataOffset.ItemSubTypeIndex)
                            Case ItemType.Key
                                itemKeyData = value
                            Case ItemType.Weapon
                                If LocIndex > 14 Then
                                    itemwSoundData = CByte(value)
                                Else
                                    itemSubTypeData(LocIndex) = value
                                End If
                            Case Else 'ItemType.Armor, ItemType.Ammo, ItemType.Container, ItemType.Drugs, ItemType.Misc
                                itemSubTypeData(LocIndex) = value
                        End Select
                    End If
                End If
            End If
        End If
        TextBox1.Text = String.Empty
    End Sub

    Private Sub Reload(ByVal sender As Object, ByVal e As EventArgs) Handles Button3.Click
        ListView1.BeginUpdate()
        ListView1.Items.Clear()
        ListView1.ShowGroups = True
        Init_Data()
        ListView1.EndUpdate()
    End Sub

    Private Sub SavePro(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        Dim filePath As String
        Dim fFile As Integer = FreeFile()
        Dim f1Format As Boolean = False

        If type = ProType.Critter Then
            filePath = SaveMOD_Path & PROTO_CRITTERS
            If Not Directory.Exists(filePath) Then Directory.CreateDirectory(filePath)
            filePath &= Critter_LST(index).proFile

            If File.Exists(filePath) Then
                File.SetAttributes(filePath, FileAttributes.Normal Or FileAttributes.NotContentIndexed)
            End If

            ' for Falout 1 format
            If critterData(ProtoMemberCount.Critter - 1) = -1 Then ' DamageType = -1
                f1Format = True
                File.Delete(filePath) ' удаляем файл для перезаписи его размера
            End If

            'Save to Pro critter
            FileOpen(fFile, filePath, OpenMode.Binary, OpenAccess.Write, OpenShare.Shared)
            PutDataToPro(critterData, fFile, f1Format)
        Else
            filePath = SaveMOD_Path & PROTO_ITEMS
            If Not Directory.Exists(filePath) Then Directory.CreateDirectory(filePath)
            filePath &= Items_LST(index).proFile
            If File.Exists(filePath) Then
                File.SetAttributes(filePath, FileAttributes.Normal Or FileAttributes.NotContentIndexed)
            End If

            'Save to Pro item
            FileOpen(fFile, filePath, OpenMode.Binary, OpenAccess.Write, OpenShare.Shared)
            PutDataToPro(itemData, fFile)
            FilePut(fFile, itemSoundData)

            Select Case itemData(Prototypes.DataOffset.ItemSubTypeIndex)
                Case ItemType.Weapon
                    PutDataToPro(itemSubTypeData, fFile)
                    FilePut(fFile, itemwSoundData)
                Case ItemType.Armor, ItemType.Ammo, ItemType.Container, ItemType.Drugs, ItemType.Misc
                    PutDataToPro(itemSubTypeData, fFile)
                Case ItemType.Key
                    FilePut(fFile, ProFiles.ReverseBytes(itemKeyData))
                Case Else
                    MsgBox("Invalid object type item. Check pro file data format.", MsgBoxStyle.Critical, "Error pro data")
                    FileClose(fFile)
                    Exit Sub
            End Select
        End If

        FileClose(fFile)
        SetReadOnly(filePath)

        Main.PrintLog("Save Pro: " & filePath)
    End Sub

    ' isF1Format - сохраняет в формате F1
    Private Sub PutDataToPro(ByRef data() As Integer, ByVal fFile As Integer, Optional ByVal isF1Format As Boolean = False)
        Dim len = If(isF1Format, ProtoMemberCount.Critter - 1, data.Length) - 1
        Dim saveData(len) As Integer
        For n = 0 To len
            saveData(n) = ProFiles.ReverseBytes(data(n))
        Next
        FilePut(fFile, saveData)
    End Sub

    Private Sub ColumnClick(ByVal sender As Object, ByVal e As ColumnClickEventArgs) Handles ListView1.ColumnClick
        ListView1.ShowGroups = Not (ListView1.ShowGroups)
    End Sub

    Private Sub TextBoxKeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TextBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            ListView1.Focus()
        ElseIf e.KeyCode = Keys.Escape Then
            TextBox1.Text = TextBox1.Tag.ToString
            ListView1.Focus()
        End If
    End Sub

    Private Sub TextBoxKeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TextBox1.KeyPress
        If Not (e.KeyChar = ChrW(Keys.Back) OrElse e.KeyChar = ChrW(Keys.Delete)) Then
            If CheckBox1.Checked = False Then
                If e.KeyChar <> "-"c AndAlso Not (Char.IsDigit(e.KeyChar)) Then e.Handled = True
            Else
                e.KeyChar = Char.ToUpper(e.KeyChar)
                If Not (Char.IsDigit(e.KeyChar)) AndAlso Not (e.KeyChar >= "A"c And e.KeyChar <= "F"c) Then e.Handled = True
            End If
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        'Dim value As Integer
        'Dim valueText = TextBox1.Text.Trim
        'If (valueText <> String.Empty) Then
        '    If (CheckBox1.Checked) Then ' dec > hex
        '        TextBox1.Text = Hex(valueText)
        '    Else ' hex > dec
        '        value = Integer.Parse(valueText, NumberStyles.AllowHexSpecifier)
        '        TextBox1.Text = value.ToString()
        '    End If
        'End If
    End Sub

End Class