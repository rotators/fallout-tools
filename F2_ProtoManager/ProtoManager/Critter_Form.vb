Imports System.Drawing
Imports System.IO
Imports System.Text

Imports Prototypes
Imports Enums

Friend Class Critter_Form

    Private critter As CritterObj

    Private TabStatsView, TabDefenceView, TabMiscView As Boolean
    Private cPath As String = Nothing

    Private ReadOnly cLST_Index As Integer

    Sub New(ByVal cLST_Index As Integer)
        InitializeComponent()
        Me.cLST_Index = cLST_Index
    End Sub

    Friend Function LoadProData() As Boolean
        Dim proFile As String = PROTO_CRITTERS & Critter_LST(cLST_Index).proFile
        If cPath = Nothing Then cPath = DatFiles.CheckFile(proFile, False)

        critter = New CritterObj(cPath & proFile)
        If (critter.Load() = False) Then 'BadFormat
            MsgBox("The pro file: " & proFile & " does not have the correct format.", MsgBoxStyle.Critical)
            Return True ' error
        End If

        Return False
    End Function

    Private Sub Critter_Form_Enter(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Activated
        If TabStatsView Then Main_Form.ToolStripStatusLabel1.Text = cPath & PROTO_CRITTERS & Critter_LST(cLST_Index).proFile
    End Sub

    Private Sub Critter_Form_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Button6.Enabled Then
            Dim btn As MsgBoxResult = MsgBox("Save changes to the Pro-File?", MsgBoxStyle.YesNoCancel, "Attention!")
            If btn = MsgBoxResult.Yes Then
                Button_Save(sender, e)
            ElseIf btn = MsgBoxResult.Cancel Then
                e.Cancel = True
            End If
        End If

        If e.Cancel = False Then
            Main_Form.Focus()
        End If
    End Sub

    Private Sub Critter_Form_FormClosed(ByVal sender As Object, ByVal e As FormClosedEventArgs) Handles MyBase.FormClosed
        If pbCritterFID.Image IsNot Nothing Then pbCritterFID.Image.Dispose()
        If PictureBox2.Image IsNot Nothing Then PictureBox2.Image.Dispose()
        Me.Dispose()
    End Sub

    Private Sub Critter_Form_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Dim i As Integer = 1
        For n = 0 To UBound(Items_LST)
            If Items_LST(n).itemType = ItemType.Armor Then
                ListView1.Items.Add(Items_LST(n).itemName)
                ListView1.Items(i).Tag = Items_LST(n).proFile
                i += 1
            End If
        Next

        SetStatsValue_Tab()
        CalcSpecialParam()
    End Sub

    'Возвращает Имя или Описание криттера из msg файла
    Private Function GetNameCritterMsg(ByVal NameID As Integer, Optional ByVal Desc As Boolean = False) As String
        If Desc Then NameID += 1
        Messages.GetMsgData("pro_crit.msg")

        Return Messages.GetNameObject(NameID)
    End Function

    Private Sub TabControl1_Selected(ByVal sender As Object, ByVal e As TabControlEventArgs) Handles TabControl1.Selected
        If e.TabPageIndex = 1 And Not (TabDefenceView) Then
            SetDefenceValue_Tab()
        ElseIf e.TabPageIndex = 2 And Not (TabMiscView) Then
            SetMiscValue_Tab()
        End If
    End Sub

    Private Sub SetStatsValue_Tab()
        On Error Resume Next

        tbCritterName.Text = GetNameCritterMsg(critter.DescID)
        Me.Text = tbCritterName.Text & " [" & Critter_LST(cLST_Index).proFile & "]"

        TextBox33.Text = critter.ProtoID.ToString
        ComboBox1.SelectedIndex = critter.FrmID - &H1000000I

        'SPECIAL
        numStrength.Value = critter.proto.Strength
        numPerception.Value = critter.proto.Perception
        numEndurance.Value = critter.proto.Endurance
        numCharisma.Value = critter.proto.Charisma
        numIntelligence.Value = critter.proto.Intelligence
        numAgility.Value = critter.proto.Agility
        numLuck.Value = critter.proto.Luck

        'Skills
        NumericUpDown8.Value = critter.proto.SmallGuns
        NumericUpDown9.Value = critter.proto.BigGuns
        NumericUpDown10.Value = critter.proto.EnergyGun
        NumericUpDown11.Value = critter.proto.Melee
        NumericUpDown12.Value = critter.proto.Unarmed
        NumericUpDown13.Value = critter.proto.Throwing
        NumericUpDown14.Value = critter.proto.FirstAid
        NumericUpDown15.Value = critter.proto.Doctor
        NumericUpDown16.Value = critter.proto.Outdoorsman
        NumericUpDown17.Value = critter.proto.Sneak
        NumericUpDown18.Value = critter.proto.Lockpick
        NumericUpDown19.Value = critter.proto.Steal
        NumericUpDown20.Value = critter.proto.Traps
        NumericUpDown21.Value = critter.proto.Science
        NumericUpDown22.Value = critter.proto.Repair
        NumericUpDown23.Value = critter.proto.Speech
        NumericUpDown24.Value = critter.proto.Barter
        NumericUpDown25.Value = critter.proto.Gambling

        NumericUpDown26.Value = critter.proto.b_HP
        NumericUpDown27.Value = critter.proto.b_AC
        NumericUpDown28.Value = critter.proto.b_AP
        NumericUpDown29.Value = critter.proto.b_Weight
        NumericUpDown30.Value = critter.proto.b_MeleeDmg
        NumericUpDown31.Value = critter.proto.b_Sequence
        NumericUpDown32.Value = critter.proto.b_Healing
        NumericUpDown33.Value = critter.proto.b_Critical
        NumericUpDown34.Value = critter.proto.b_Better
        NumericUpDown35.Value = critter.proto.b_UnarmedDmg

        TextBox27.Text = critter.proto.Better.ToString
        NumericUpDown1.Value = critter.proto.UnarmedDmg

        NumericUpDown55.Value = critter.proto.b_DRPoison
        NumericUpDown54.Value = critter.proto.b_DRRadiation

        TabStatsView = True
    End Sub

    Private Sub CalcSpecialParam()
        On Error Resume Next

        tbSmallGun.Text = (CalcStats.SmallGun_Skill(numAgility.Value) + NumericUpDown8.Value).ToString
        TextBox2.Text = (CalcStats.BigEnergyGun_Skill(numAgility.Value) + NumericUpDown9.Value).ToString
        TextBox3.Text = (CalcStats.EnergyGun_Skill(numAgility.Value) + NumericUpDown10.Value).ToString
        TextBox5.Text = (CalcStats.Unarmed_Skill(numAgility.Value, numStrength.Value) + NumericUpDown12.Value).ToString
        TextBox4.Text = (CalcStats.Melee_Skill(numAgility.Value, numStrength.Value) + NumericUpDown11.Value).ToString
        TextBox6.Text = (CalcStats.Throwing_Skill(numAgility.Value) + NumericUpDown13.Value).ToString
        TextBox7.Text = (CalcStats.FirstAid_Skill(numPerception.Value, numIntelligence.Value) + NumericUpDown14.Value).ToString
        TextBox8.Text = (CalcStats.Doctor_Skill(numPerception.Value, numIntelligence.Value) + NumericUpDown15.Value).ToString
        TextBox9.Text = (CalcStats.Outdoorsman_Skill(numEndurance.Value, numIntelligence.Value) + NumericUpDown16.Value).ToString
        TextBox10.Text = (CalcStats.Sneak_Skill(numAgility.Value) + NumericUpDown17.Value).ToString
        TextBox11.Text = (CalcStats.Lockpick_Skill(numPerception.Value, numAgility.Value) + NumericUpDown18.Value).ToString
        TextBox12.Text = (CalcStats.Steal_Skill(numAgility.Value) + NumericUpDown19.Value).ToString
        TextBox13.Text = (CalcStats.Trap_Skill(numPerception.Value, numAgility.Value) + NumericUpDown20.Value).ToString
        TextBox14.Text = (CalcStats.Science_Skill(numIntelligence.Value) + NumericUpDown21.Value).ToString
        TextBox15.Text = (CalcStats.Repair_Skill(numIntelligence.Value) + NumericUpDown22.Value).ToString
        TextBox16.Text = (CalcStats.Speech_Skill(numCharisma.Value) + NumericUpDown23.Value).ToString
        TextBox17.Text = (CalcStats.Barter_Skill(numCharisma.Value) + NumericUpDown24.Value).ToString
        TextBox18.Text = (CalcStats.Gamblings_Skill(numLuck.Value) + NumericUpDown25.Value).ToString

        TextBox19.Text = (CalcStats.Health_Point(numStrength.Value, numEndurance.Value) + NumericUpDown26.Value).ToString
        TextBox20.Text = (numAgility.Value + NumericUpDown27.Value).ToString
        TextBox21.Text = (CalcStats.Action_Point(numAgility.Value) + NumericUpDown28.Value).ToString
        TextBox22.Text = (CalcStats.Carry_Weight(numStrength.Value) + NumericUpDown29.Value).ToString
        TextBox23.Text = (CalcStats.Melee_Damage(numStrength.Value) + NumericUpDown30.Value).ToString
        TextBox24.Text = (CalcStats.Sequence(numPerception.Value) + NumericUpDown31.Value).ToString
        TextBox25.Text = (CalcStats.Healing_Rate(numEndurance.Value) + NumericUpDown32.Value).ToString

        TextBox26.Text = (numLuck.Value + NumericUpDown33.Value).ToString
        TextBox27.Text = (critter.proto.Better + NumericUpDown34.Value).ToString

        tbPoison.Text = (CalcStats.Poison(numEndurance.Value) + NumericUpDown55.Value).ToString  'DRPoison
        tbRadiation.Text = (CalcStats.Radiation(numEndurance.Value) + NumericUpDown54.Value).ToString  'DRRadiation
    End Sub

    Private Sub SetDefenceValue_Tab()
        On Error Resume Next

        NumericUpDown56.Value = critter.proto.DTNormal
        NumericUpDown57.Value = critter.proto.DTLaser
        NumericUpDown58.Value = critter.proto.DTFire
        NumericUpDown59.Value = critter.proto.DTPlasma
        NumericUpDown60.Value = critter.proto.DTElectrical
        NumericUpDown61.Value = critter.proto.DTEMP
        NumericUpDown62.Value = critter.proto.DTExplode

        NumericUpDown71.Value = critter.proto.DRNormal
        NumericUpDown70.Value = critter.proto.DRLaser
        NumericUpDown69.Value = critter.proto.DRFire
        NumericUpDown68.Value = critter.proto.DRPlasma
        NumericUpDown67.Value = critter.proto.DRElectrical
        NumericUpDown65.Value = critter.proto.DREMP
        NumericUpDown63.Value = critter.proto.DRExplode

        SetBonusDefenceValue()
        TabDefenceView = True
    End Sub

    Private Sub SetBonusDefenceValue()
        On Error Resume Next

        NumericUpDown40.Value = critter.proto.b_DTNormal
        NumericUpDown41.Value = critter.proto.b_DTLaser
        NumericUpDown42.Value = critter.proto.b_DTFire
        NumericUpDown43.Value = critter.proto.b_DTPlasma
        NumericUpDown44.Value = critter.proto.b_DTElectrical
        NumericUpDown45.Value = critter.proto.b_DTEMP
        NumericUpDown46.Value = critter.proto.b_DTExplode

        NumericUpDown47.Value = critter.proto.b_DRNormal
        NumericUpDown48.Value = critter.proto.b_DRLaser
        NumericUpDown49.Value = critter.proto.b_DRFire
        NumericUpDown50.Value = critter.proto.b_DRPlasma
        NumericUpDown51.Value = critter.proto.b_DRElectrical
        NumericUpDown52.Value = critter.proto.b_DREMP
        NumericUpDown53.Value = critter.proto.b_DRExplode
    End Sub

    Private Sub SetMiscValue_Tab()
        On Error Resume Next

        TextBox30.Text = GetNameCritterMsg(critter.DescID, True)
        Button4.Enabled = False
        Button5.Enabled = False

        NumericUpDown64.Value = critter.DescID
        If critter.ScriptID <> -1 Then
            ComboBox9.SelectedIndex = 1 + (critter.ScriptID - &H4000000I)
        Else
            ComboBox9.SelectedIndex = 0
        End If
        ComboBox8.SelectedIndex = critter.proto.Gender
        ComboBox2.SelectedIndex = PacketAI.IndexOfValue(critter.AIPacket)
        ComboBox3.SelectedIndex = critter.TeamNum
        ComboBox4.SelectedIndex = critter.proto.BodyType
        ComboBox5.SelectedIndex = critter.proto.DamageType
        ComboBox6.SelectedIndex = critter.proto.KillType

        NumericUpDown36.Value = critter.LightDis
        NumericUpDown37.Value = critter.LightIntensity
        NumericUpDown38.Value = critter.proto.ExpVal
        NumericUpDown39.Value = critter.proto.Age

        'Flags
        SetFlags()
        SetFlagsExt()
        SetFlagsCritter()

        TabMiscView = True
    End Sub

    Private Sub SetFlags()
        CheckBox1.Checked = critter.IsFlat
        CheckBox2.Checked = critter.IsNoBlock
        CheckBox3.Checked = critter.IsMultiHex
        CheckBox4.Checked = critter.IsShootThru
        CheckBox5.Checked = critter.IsLightThru

        CheckBox6.Checked = critter.IsTransNone
        If Not critter.IsTransNone Then
            RadioButton1.Checked = critter.IsTransWall
            RadioButton4.Checked = critter.IsTransGlass
            RadioButton2.Checked = critter.IsTransSteam
            RadioButton5.Checked = critter.IsTransEnergy
            RadioButton3.Checked = critter.IsTransRed
        End If
    End Sub

    Private Sub SetFlagsExt()
        CheckBox7.Checked = critter.IsUse
        CheckBox8.Checked = critter.IsUseOn
        CheckBox9.Checked = critter.IsLook
        CheckBox10.Checked = critter.IsTalk
        CheckBox11.Checked = critter.IsPickUp
    End Sub

    Private Sub SetFlagsCritter()
        CheckBox13.Checked = critter.IsNoSteal
        CheckBox14.Checked = critter.IsNoDrop
        CheckBox15.Checked = critter.IsNoLimbs
        CheckBox16.Checked = critter.IsRangeHtH
        CheckBox17.Checked = critter.IsNoHeal
        CheckBox18.Checked = critter.IsAges
        CheckBox19.Checked = critter.IsNoFlatten
        CheckBox20.Checked = critter.IsSpecialDeath
        CheckBox21.Checked = critter.IsNoKnockBack
        CheckBox22.Checked = critter.IsInvulnerable
        CheckBox23.Checked = critter.IsBarter
    End Sub

    Private Sub Save_CritterPro()
        'Tab Special
        critter.proto.Strength = CInt(numStrength.Value)
        critter.proto.Perception = CInt(numPerception.Value)
        critter.proto.Endurance = CInt(numEndurance.Value)
        critter.proto.Charisma = CInt(numCharisma.Value)
        critter.proto.Intelligence = CInt(numIntelligence.Value)
        critter.proto.Agility = CInt(numAgility.Value)
        critter.proto.Luck = CInt(numLuck.Value)
        'Skills
        critter.proto.SmallGuns = CInt(NumericUpDown8.Value)
        critter.proto.BigGuns = CInt(NumericUpDown9.Value)
        critter.proto.EnergyGun = CInt(NumericUpDown10.Value)
        critter.proto.Melee = CInt(NumericUpDown11.Value)
        critter.proto.Unarmed = CInt(NumericUpDown12.Value)
        critter.proto.Throwing = CInt(NumericUpDown13.Value)
        critter.proto.FirstAid = CInt(NumericUpDown14.Value)
        critter.proto.Doctor = CInt(NumericUpDown15.Value)
        critter.proto.Outdoorsman = CInt(NumericUpDown16.Value)
        critter.proto.Sneak = CInt(NumericUpDown17.Value)
        critter.proto.Lockpick = CInt(NumericUpDown18.Value)
        critter.proto.Steal = CInt(NumericUpDown19.Value)
        critter.proto.Traps = CInt(NumericUpDown20.Value)
        critter.proto.Science = CInt(NumericUpDown21.Value)
        critter.proto.Repair = CInt(NumericUpDown22.Value)
        critter.proto.Speech = CInt(NumericUpDown23.Value)
        critter.proto.Barter = CInt(NumericUpDown24.Value)
        critter.proto.Gambling = CInt(NumericUpDown25.Value)

        critter.proto.b_HP = CInt(NumericUpDown26.Value)
        critter.proto.b_AC = CInt(NumericUpDown27.Value)
        critter.proto.b_AP = CInt(NumericUpDown28.Value)
        critter.proto.b_Weight = CInt(NumericUpDown29.Value)
        critter.proto.b_MeleeDmg = CInt(NumericUpDown30.Value)
        critter.proto.b_Sequence = CInt(NumericUpDown31.Value)
        critter.proto.b_Healing = CInt(NumericUpDown32.Value)
        critter.proto.b_Critical = CInt(NumericUpDown33.Value)
        critter.proto.b_Better = CInt(NumericUpDown34.Value)
        critter.proto.b_UnarmedDmg = CInt(NumericUpDown35.Value)

        critter.proto.HP = CInt(TextBox19.Text) - critter.proto.b_HP
        critter.proto.AC = CInt(TextBox20.Text) - critter.proto.b_AC
        critter.proto.AP = CInt(TextBox21.Text) - critter.proto.b_AP
        critter.proto.Weight = CInt(TextBox22.Text) - critter.proto.b_Weight
        critter.proto.MeleeDmg = CInt(TextBox23.Text) - critter.proto.b_MeleeDmg
        critter.proto.Sequence = CInt(TextBox24.Text) - critter.proto.b_Sequence
        critter.proto.Healing = CInt(TextBox25.Text) - critter.proto.b_Healing
        critter.proto.Critical = CInt(TextBox26.Text) - critter.proto.b_Critical

        '*************
        critter.proto.Better = CInt(TextBox27.Text) - critter.proto.b_Better
        critter.proto.UnarmedDmg = CInt(NumericUpDown1.Value)
        '*************

        'Tab Defence
        critter.proto.b_DTNormal = CInt(NumericUpDown40.Value)
        critter.proto.b_DTLaser = CInt(NumericUpDown41.Value)
        critter.proto.b_DTFire = CInt(NumericUpDown42.Value)
        critter.proto.b_DTPlasma = CInt(NumericUpDown43.Value)
        critter.proto.b_DTElectrical = CInt(NumericUpDown44.Value)
        critter.proto.b_DTEMP = CInt(NumericUpDown45.Value)
        critter.proto.b_DTExplode = CInt(NumericUpDown46.Value)

        critter.proto.DTNormal = CInt(NumericUpDown56.Value)
        critter.proto.DTLaser = CInt(NumericUpDown57.Value)
        critter.proto.DTFire = CInt(NumericUpDown58.Value)
        critter.proto.DTPlasma = CInt(NumericUpDown59.Value)
        critter.proto.DTElectrical = CInt(NumericUpDown60.Value)
        critter.proto.DTEMP = CInt(NumericUpDown61.Value)
        critter.proto.DTExplode = CInt(NumericUpDown62.Value)

        critter.proto.b_DRNormal = CInt(NumericUpDown47.Value)
        critter.proto.b_DRLaser = CInt(NumericUpDown48.Value)
        critter.proto.b_DRFire = CInt(NumericUpDown49.Value)
        critter.proto.b_DRPlasma = CInt(NumericUpDown50.Value)
        critter.proto.b_DRElectrical = CInt(NumericUpDown51.Value)
        critter.proto.b_DREMP = CInt(NumericUpDown52.Value)
        critter.proto.b_DRExplode = CInt(NumericUpDown53.Value)
        critter.proto.b_DRPoison = CInt(NumericUpDown55.Value)
        critter.proto.b_DRRadiation = CInt(NumericUpDown54.Value)

        critter.proto.DRNormal = CInt(NumericUpDown71.Value)
        critter.proto.DRLaser = CInt(NumericUpDown70.Value)
        critter.proto.DRFire = CInt(NumericUpDown69.Value)
        critter.proto.DRPlasma = CInt(NumericUpDown68.Value)
        critter.proto.DRElectrical = CInt(NumericUpDown67.Value)
        critter.proto.DREMP = CInt(NumericUpDown65.Value)
        critter.proto.DRExplode = CInt(NumericUpDown63.Value)
        critter.proto.DRPoison = CInt(tbPoison.Text) - critter.proto.b_DRPoison
        critter.proto.DRRadiation = CInt(tbRadiation.Text) - critter.proto.b_DRRadiation

        'Tab Misc
        critter.DescID = CInt(NumericUpDown64.Value)
        critter.ScriptID = If(ComboBox9.SelectedIndex > 0, (ComboBox9.SelectedIndex - 1) + &H4000000, -1)
        critter.proto.Gender = ComboBox8.SelectedIndex

        If ComboBox2.SelectedItem IsNot Nothing Then
            Dim packedNum = ComboBox2.SelectedItem.ToString
            If (PacketAI.Count = 0) Then
                Dim n = packedNum.LastIndexOf("("c) + 1
                critter.AIPacket = CInt(packedNum.Substring(n, packedNum.Length - n - 1))
            ElseIf (PacketAI.ContainsKey(packedNum)) Then
                critter.AIPacket = PacketAI.Item(packedNum)
            End If
        End If

        critter.TeamNum = ComboBox3.SelectedIndex
        critter.proto.BodyType = ComboBox4.SelectedIndex
        critter.proto.DamageType = ComboBox5.SelectedIndex
        critter.proto.KillType = ComboBox6.SelectedIndex

        critter.LightDis = CInt(NumericUpDown36.Value)
        critter.LightIntensity = CInt(NumericUpDown37.Value)
        critter.proto.ExpVal = CInt(NumericUpDown38.Value)
        critter.proto.Age = CInt(NumericUpDown39.Value)

        'Flags
        critter.IsFlat = CheckBox1.Checked
        critter.IsNoBlock = CheckBox2.Checked
        critter.IsMultiHex = CheckBox3.Checked
        critter.IsShootThru = CheckBox4.Checked
        critter.IsLightThru = CheckBox5.Checked

        ' удаляем взаимозаменяемые флаги TransNone/TransRed/TransWall/TransGlass/TransSteam/TransEnergy
        critter.Flags = critter.Flags And &HFFF03FFF
        If CheckBox6.Checked Then
            critter.IsTransNone = True
        Else
            If RadioButton1.Checked Then
                critter.IsTransWall = True
            ElseIf RadioButton4.Checked Then
                critter.IsTransGlass = True
            ElseIf RadioButton2.Checked Then
                critter.IsTransSteam = True
            ElseIf RadioButton5.Checked Then
                critter.IsTransEnergy = True
            ElseIf RadioButton3.Checked Then
                critter.IsTransRed = True
            End If
        End If

        ' Flags Ext
        critter.IsLook = CheckBox9.Checked
        critter.IsTalk = CheckBox10.Checked

        'Flags Critter
        critter.IsNoSteal = CheckBox13.Checked
        critter.IsNoDrop = CheckBox14.Checked
        critter.IsNoLimbs = CheckBox15.Checked
        critter.IsRangeHtH = CheckBox16.Checked
        critter.IsNoHeal = CheckBox17.Checked
        critter.IsAges = CheckBox18.Checked
        critter.IsNoFlatten = CheckBox19.Checked
        critter.IsSpecialDeath = CheckBox20.Checked
        critter.IsNoKnockBack = CheckBox21.Checked
        critter.IsInvulnerable = CheckBox22.Checked
        critter.IsBarter = CheckBox23.Checked

        critter.FrmID = ComboBox1.SelectedIndex + &H1000000I

        'Save data to Pro file
        Dim proFile As String = SaveMOD_Path & PROTO_CRITTERS
        If Not Directory.Exists(proFile) Then Directory.CreateDirectory(proFile)

        proFile &= Critter_LST(cLST_Index).proFile
        ProFiles.SaveCritterProData(proFile, critter)

        'Log
        Main.PrintLog("Save Pro: " & proFile)
    End Sub

    Private Sub SaveCritterMsg(ByVal str As String, Optional ByRef Desc As Boolean = False)
        Dim ID As Integer = CInt(NumericUpDown64.Value)

        Messages.GetMsgData("pro_crit.msg", False)
        If Messages.AddTextMSG(str, ID, Desc) Then
            MsgBox("You can not add value to the Msg file." & vbLf & "Not found msg line #:" & ID, MsgBoxStyle.SystemModal And MsgBoxStyle.Critical, "Error: pro_crit.msg")
            Exit Sub
        End If
        'Save
        Messages.SaveMSGFile("pro_crit.msg")

        'Update Name List
        If Not (Desc) Then
            Button4.Enabled = False
            Critter_LST(cLST_Index).crtName = str
            For Each item As ListViewItem In Main_Form.ListView1.Items
                If CInt(item.Tag) = cLST_Index Then
                    Main_Form.ListView1.Items(item.Index).SubItems(0).Text = str
                    Exit For
                End If
            Next
        Else
            Button5.Enabled = False
        End If
    End Sub

    Private Sub ValueChanged_Tab1(ByVal sender As Object, ByVal e As EventArgs) Handles numStrength.ValueChanged, numPerception.ValueChanged,
    numEndurance.ValueChanged, numCharisma.ValueChanged, numIntelligence.ValueChanged, numAgility.ValueChanged, numLuck.ValueChanged,
    NumericUpDown8.ValueChanged, NumericUpDown9.ValueChanged, NumericUpDown10.ValueChanged, NumericUpDown11.ValueChanged, NumericUpDown12.ValueChanged,
    NumericUpDown13.ValueChanged, NumericUpDown14.ValueChanged, NumericUpDown15.ValueChanged, NumericUpDown16.ValueChanged, NumericUpDown17.ValueChanged,
    NumericUpDown18.ValueChanged, NumericUpDown19.ValueChanged, NumericUpDown20.ValueChanged, NumericUpDown21.ValueChanged, NumericUpDown22.ValueChanged,
    NumericUpDown23.ValueChanged, NumericUpDown24.ValueChanged, NumericUpDown25.ValueChanged, NumericUpDown26.ValueChanged, NumericUpDown27.ValueChanged,
    NumericUpDown28.ValueChanged, NumericUpDown29.ValueChanged, NumericUpDown30.ValueChanged, NumericUpDown31.ValueChanged, NumericUpDown32.ValueChanged,
    NumericUpDown33.ValueChanged, NumericUpDown34.ValueChanged
        '
        If TabStatsView Then
            CalcSpecialParam()
            Button6.Enabled = True
        Else
            Dim nudControl As NumericUpDown = CType(sender, NumericUpDown)
            If nudControl.Value <> -999 AndAlso nudControl.BackColor <> Color.Black Then
                nudControl.BackColor = Color.White
            End If
        End If
    End Sub

    Private Sub ValueChanged_Tab1a(ByVal sender As Object, ByVal e As EventArgs) Handles NumericUpDown54.ValueChanged, NumericUpDown55.ValueChanged
        If TabStatsView = True Then
            tbPoison.Text = (CalcStats.Poison(numEndurance.Value) + NumericUpDown55.Value).ToString 'DRPoison
            tbRadiation.Text = (CalcStats.Radiation(numEndurance.Value) + NumericUpDown54.Value).ToString 'DRRadiation
            Button6.Enabled = True
        Else
            If CType(sender, NumericUpDown).Value <> -200 Then
                CType(sender, NumericUpDown).BackColor = Color.White
            End If
        End If
    End Sub

    Private Sub ValueChanged_Tab1b(sender As Object, e As EventArgs) Handles NumericUpDown1.ValueChanged, NumericUpDown35.ValueChanged
        If TabStatsView = True Then
            Button6.Enabled = True
        Else
            If CType(sender, NumericUpDown).Value <> -999 Then
                CType(sender, NumericUpDown).BackColor = Color.White
            End If
        End If
    End Sub

    Private Sub ValueChanged_Tab2b(ByVal sender As Object, ByVal e As EventArgs) Handles NumericUpDown56.ValueChanged, NumericUpDown57.ValueChanged,
    NumericUpDown59.ValueChanged, NumericUpDown60.ValueChanged, NumericUpDown61.ValueChanged, NumericUpDown62.ValueChanged, NumericUpDown63.ValueChanged, NumericUpDown58.ValueChanged,
    NumericUpDown65.ValueChanged, NumericUpDown67.ValueChanged, NumericUpDown68.ValueChanged, NumericUpDown69.ValueChanged, NumericUpDown70.ValueChanged, NumericUpDown71.ValueChanged,
    NumericUpDown53.ValueChanged, NumericUpDown52.ValueChanged, NumericUpDown51.ValueChanged, NumericUpDown50.ValueChanged, NumericUpDown49.ValueChanged, NumericUpDown48.ValueChanged,
    NumericUpDown45.ValueChanged, NumericUpDown44.ValueChanged, NumericUpDown47.ValueChanged, NumericUpDown46.ValueChanged, NumericUpDown43.ValueChanged, NumericUpDown42.ValueChanged,
    NumericUpDown41.ValueChanged, NumericUpDown40.ValueChanged
        '
        If TabDefenceView Then
            Button6.Enabled = True
        Else
            If CType(sender, NumericUpDown).Value <> -999 Then
                CType(sender, NumericUpDown).BackColor = Color.White
            End If
        End If
    End Sub

    Private Sub ValueChanged_Tab3(ByVal sender As Object, ByVal e As EventArgs) Handles NumericUpDown64.ValueChanged, NumericUpDown39.ValueChanged, NumericUpDown38.ValueChanged,
    NumericUpDown37.ValueChanged, NumericUpDown36.ValueChanged, CheckBox1.CheckedChanged, CheckBox2.CheckedChanged, CheckBox3.CheckedChanged, CheckBox4.CheckedChanged, CheckBox5.CheckedChanged,
    CheckBox9.CheckedChanged, CheckBox10.CheckedChanged, CheckBox13.CheckedChanged, CheckBox14.CheckedChanged, CheckBox15.CheckedChanged, CheckBox16.CheckedChanged, ComboBox9.SelectedIndexChanged,
    CheckBox17.CheckedChanged, CheckBox18.CheckedChanged, CheckBox19.CheckedChanged, CheckBox20.CheckedChanged, CheckBox21.CheckedChanged, CheckBox22.CheckedChanged, CheckBox23.CheckedChanged,
    ComboBox4.SelectedIndexChanged, ComboBox8.SelectedIndexChanged, ComboBox6.SelectedIndexChanged, ComboBox5.SelectedIndexChanged, ComboBox3.SelectedIndexChanged, ComboBox2.SelectedIndexChanged
        '
        If TabMiscView Then Button6.Enabled = True
    End Sub

    Private Sub ValueChanged_Tab3a(ByVal sender As Object, ByVal e As EventArgs) Handles RadioButton1.CheckedChanged, RadioButton2.CheckedChanged,
    RadioButton3.CheckedChanged, RadioButton4.CheckedChanged, RadioButton5.CheckedChanged
        '
        If TabMiscView Then
            Button6.Enabled = True
            TabMiscView = False
            CheckBox6.Checked = False
            TabMiscView = True
        End If
    End Sub

    Private Sub ValueChanged_Tab3b(ByVal sender As Object, ByVal e As EventArgs) Handles CheckBox6.CheckedChanged
        If TabMiscView Then
            TabMiscView = False
            RadioButton1.Checked = False
            RadioButton2.Checked = False
            RadioButton3.Checked = False
            RadioButton4.Checked = False
            RadioButton5.Checked = False
            TabMiscView = True
            Button6.Enabled = True
        End If
    End Sub

    Dim armorItem As ArmorItemObj

    Private Sub ListView1_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ListView1.SelectedIndexChanged
        If ListView1.FocusedItem IsNot Nothing Then
            If ListView1.FocusedItem.Index > 0 Then
                Dim proFile As String = ListView1.FocusedItem.Tag.ToString
                Dim filePath = DatFiles.CheckFile(PROTO_ITEMS & proFile)

                armorItem = New ArmorItemObj(filePath)
                armorItem.LoadArmorData()

                If ListView1.FocusedItem.ToolTipText IsNot Nothing Then
                    Dim tip As StringBuilder = New StringBuilder("Armor stats:")
                    tip.AppendLine()
                    tip.AppendLine("Normal DT:" & Strings.RSet(armorItem.DTNormal.ToString, 3) & " | DR: " & armorItem.DRNormal)
                    tip.AppendLine(" Laser DT:" & Strings.RSet(armorItem.DTLaser.ToString, 3) & " | DR: " & armorItem.DRLaser)
                    tip.AppendLine("  Fire DT:" & Strings.RSet(armorItem.DTFire.ToString, 3) & " | DR: " & armorItem.DRFire)
                    tip.AppendLine("Plasma DT:" & Strings.RSet(armorItem.DTPlasma.ToString, 3) & " | DR: " & armorItem.DRPlasma)
                    tip.AppendLine("Electr DT:" & Strings.RSet(armorItem.DTElectrical.ToString, 3) & " | DR: " & armorItem.DRElectrical)
                    tip.AppendLine("   Emp DT:" & Strings.RSet(armorItem.DTEMP.ToString, 3) & " | DR: " & armorItem.DREMP)
                    tip.Append("  Expl DT:" & Strings.RSet(armorItem.DTExplode.ToString, 3) & " | DR: " & armorItem.DRExplode)
                    ListView1.FocusedItem.ToolTipText = tip.ToString
                End If

                If armorItem.InventoryFID = -1 Then
                    PictureBox2.Image = Nothing
                Else
                    Main.GetItemsLstFRM()
                    Dim fidIndex = armorItem.InventoryFID - &H7000000
                    proFile = Strings.Left(Iven_FRM(fidIndex), RTrim(Iven_FRM(fidIndex)).Length - 4)
                    If Not File.Exists(Cache_Patch & ART_INVEN & proFile & ".gif") Then
                        DatFiles.ItemFrmToGif("inven\", proFile)
                    End If
                    PictureBox2.Image = Image.FromFile(Cache_Patch & ART_INVEN & proFile & ".gif")
                End If
            Else
                PictureBox2.Image = Nothing
                armorItem = Nothing
            End If
            Button7.Enabled = True
        End If
    End Sub

    Private Sub ArmorApply(ByVal sender As Object, ByVal e As EventArgs) Handles Button7.Click
        If armorItem Is Nothing Then
            SetBonusDefenceValue()
            Exit Sub
        End If

        NumericUpDown40.Value = armorItem.DTNormal
        NumericUpDown41.Value = armorItem.DTLaser
        NumericUpDown42.Value = armorItem.DTFire
        NumericUpDown43.Value = armorItem.DTPlasma
        NumericUpDown44.Value = armorItem.DTElectrical
        NumericUpDown45.Value = armorItem.DTEMP
        NumericUpDown46.Value = armorItem.DTExplode

        NumericUpDown47.Value = armorItem.DRNormal
        NumericUpDown48.Value = armorItem.DRLaser
        NumericUpDown49.Value = armorItem.DRFire
        NumericUpDown50.Value = armorItem.DRPlasma
        NumericUpDown51.Value = armorItem.DRElectrical
        NumericUpDown52.Value = armorItem.DREMP
        NumericUpDown53.Value = armorItem.DRExplode
    End Sub

    Private Sub ComboBox1_Changed(ByVal sender As Object, ByVal e As EventArgs) Handles ComboBox1.SelectedIndexChanged

        tbFrmID.Text = CType((ComboBox1.SelectedIndex + &H1000000), String)

        Dim frm As String = CType(ComboBox1.SelectedItem, String)
        Dim fileFrm As String = Cache_Patch & ART_CRITTERS_PATH & frm & "aa.gif"
        If Not File.Exists(fileFrm) Then DatFiles.CritterFrmToGif(frm)

        Dim img As Bitmap = My.Resources.RESERVAA 'BadFrm
        If File.Exists(fileFrm) Then
            img = New Bitmap(fileFrm)
            If img.Width > pbCritterFID.Size.Width OrElse img.Size.Height > pbCritterFID.Size.Height Then
                If img.GetFrameCount(Imaging.FrameDimension.Time) = 1 Then img.MakeTransparent(Color.White)
                pbCritterFID.SizeMode = PictureBoxSizeMode.Zoom
            Else
                pbCritterFID.SizeMode = PictureBoxSizeMode.CenterImage
            End If
            If TabStatsView Then Button6.Enabled = True
        End If

        pbCritterFID.Image = img
    End Sub

    Private Sub InitFormData()
        LoadProData()
        TabStatsView = False
        TabDefenceView = False
        TabMiscView = False
        SetStatsValue_Tab()
        CalcSpecialParam()
        SetDefenceValue_Tab()
        SetMiscValue_Tab()
    End Sub

    Private Sub Reload(ByVal sender As Object, ByVal e As EventArgs) Handles Button2.Click
        InitFormData()
        Button2.Enabled = False
        Button6.Enabled = False
    End Sub

    Private Sub Restore(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        Dim tempPath As String = cPath

        Dim size = If(Critter_LST(cLST_Index).formatF1, 412, 416)
        If DatFiles.UnDatFile(PROTO_CRITTERS & Critter_LST(cLST_Index).proFile, size) Then
            cPath = Cache_Patch
            InitFormData()
            Button6.Enabled = True
        Else
            MsgBox("The prototype file does not exist in Master.dat.", MsgBoxStyle.Exclamation)
        End If

        cPath = tempPath
    End Sub

    Private Sub Button_Save(ByVal sender As Object, ByVal e As EventArgs) Handles Button6.Click
        If TabDefenceView = False Then SetDefenceValue_Tab()
        If TabMiscView = False Then SetMiscValue_Tab()
        Save_CritterPro()
        For Each item As ListViewItem In Main_Form.ListView1.Items
            If CInt(item.Tag) = cLST_Index Then
                Dim attr = If(Settings.proRO, "R/O", String.Empty)

                Dim isF1 = (critter.proto.DamageType = 7)
                If (isF1) Then attr += If(attr = "", "[F1]", " [F1]")
                Critter_LST(cLST_Index).formatF1 = isF1

                Main_Form.ListView1.Items(item.Index).SubItems(2).Text = attr
                Main_Form.ListView1.Items(item.Index).ForeColor = Color.DarkBlue
                Exit For
            End If
        Next
        Button6.Enabled = False
        cPath = DatFiles.CheckFile(PROTO_CRITTERS & Critter_LST(cLST_Index).proFile, False)
    End Sub

    Private Sub Button4_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button4.Click
        SaveCritterMsg(tbCritterName.Text)
    End Sub

    Private Sub Button5_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button5.Click
        SaveCritterMsg(TextBox30.Text, True)
    End Sub

    Private Sub Button3_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button3.Click
        Me.Close()
    End Sub

    Private Sub TextBox29_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles tbCritterName.TextChanged
        Button4.Enabled = True
    End Sub

    Private Sub TextBox30_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TextBox30.TextChanged
        Button5.Enabled = True
    End Sub

    Private Sub Button6_EnabledChanged(ByVal sender As Object, ByVal e As EventArgs) Handles Button6.EnabledChanged
        If Button6.Enabled Then Button2.Enabled = True
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        Dim sf = New SetFlagsForm(critter)
        sf.ShowDialog(Me)
        If (sf.sets.flags <> critter.Flags) Then
            critter.Flags = sf.sets.flags
            TabMiscView = False
            SetFlags()
            TabMiscView = True
            Button6.Enabled = True
        End If
        If (sf.sets.flagsExt <> critter.FlagsExt) Then
            critter.FlagsExt = sf.sets.flagsExt
            TabMiscView = False
            SetFlagsExt()
            TabMiscView = True
            Button6.Enabled = True
        End If
        If (sf.sets.flagsCrt <> critter.CritterFlags) Then
            critter.CritterFlags = sf.sets.flagsCrt
            'TabMiscView = False
            'SetFlagsCritter()
            'TabMiscView = True
            Button6.Enabled = True
        End If
        sf.Dispose()
    End Sub

    Private Sub Button8_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button8.Click
        Main.CreateAIEditForm(critter.AIPacket)
    End Sub

    Private Sub NumericUpDown_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles NumericUpDown71.KeyPress, NumericUpDown70.KeyPress, NumericUpDown69.KeyPress,
        NumericUpDown68.KeyPress, NumericUpDown67.KeyPress, NumericUpDown65.KeyPress, NumericUpDown64.KeyPress, NumericUpDown63.KeyPress, NumericUpDown62.KeyPress,
        NumericUpDown61.KeyPress, NumericUpDown60.KeyPress, NumericUpDown59.KeyPress, NumericUpDown58.KeyPress, NumericUpDown57.KeyPress, NumericUpDown56.KeyPress,
        NumericUpDown53.KeyPress, NumericUpDown52.KeyPress, NumericUpDown51.KeyPress, NumericUpDown50.KeyPress, NumericUpDown49.KeyPress, NumericUpDown48.KeyPress,
        NumericUpDown47.KeyPress, NumericUpDown46.KeyPress, NumericUpDown45.KeyPress, NumericUpDown44.KeyPress, NumericUpDown43.KeyPress, NumericUpDown42.KeyPress,
        NumericUpDown41.KeyPress, NumericUpDown40.KeyPress, NumericUpDown39.KeyPress, NumericUpDown38.KeyPress, NumericUpDown37.KeyPress, NumericUpDown36.KeyPress
        '
        If Char.IsDigit(e.KeyChar) Then Button6.Enabled = True
    End Sub

End Class