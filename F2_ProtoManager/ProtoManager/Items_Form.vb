Imports System.Drawing
Imports System.IO

Imports Prototypes
Imports Enums

Friend Class Items_Form

    Private Shared weaponScoreType As Integer = 0

    Private itemObject As ItemPrototype
    Private backupItem As ItemPrototype

    Private ReadOnly iLST_Index As Integer  'индекс предмета в lst файле

    Private frmReady As Boolean
    Private ReloadPro As Boolean
    Private cPath As String = Nothing

    Sub New(ByVal iLST_Index As Integer)
        InitializeComponent()
        Me.iLST_Index = iLST_Index
    End Sub

    Friend Sub IniItemsForm()
        ComboBox10.Items.AddRange(Misc_NAME)
        ComboBox11.Items.AddRange(Perk_NAME)
        ComboBox12.Items.AddRange(CaliberNAME)

        For n = 0 To UBound(AmmoPID)
            ComboBox13.Items.Add(AmmoNAME(n))
        Next

        If Critters_FRM IsNot Nothing Then
            cbArmorMaleFid.Items.AddRange(Critters_FRM)
            cbArmorFemaleFid.Items.AddRange(Critters_FRM)
        End If
        cbArmorPerk.Items.AddRange(Perk_NAME)
        ComboBox22.Items.AddRange(Perk_NAME)
        ComboBox23.Items.AddRange(CaliberNAME)

        For n = 0 To UBound(AmmoPID)
            ComboBox24.Items.Add(AmmoNAME(n))
        Next

        ComboBox25.Items.AddRange(CaliberNAME)

        'тип предмета
        Select Case Items_LST(iLST_Index).itemType
            Case ItemType.Weapon
                TabControl1.TabPages.RemoveAt(3)
                TabControl1.TabPages.RemoveAt(2)
                TabControl1.TabPages.RemoveAt(1)
            Case ItemType.Armor
                TabControl1.TabPages.Remove(TabControl1.TabPages.Item(3))
                TabControl1.TabPages.Remove(TabControl1.TabPages.Item(2))
                TabControl1.TabPages.Remove(TabControl1.TabPages.Item(0))
            Case ItemType.Drugs
                TabControl1.TabPages.Remove(TabControl1.TabPages.Item(3))
                TabControl1.TabPages.Remove(TabControl1.TabPages.Item(1))
                TabControl1.TabPages.Remove(TabControl1.TabPages.Item(0))
            Case ItemType.Key, ItemType.Container
                TabControl1.TabPages.Remove(TabControl1.TabPages.Item(3))
                TabControl1.TabPages.Remove(TabControl1.TabPages.Item(2))
                TabControl1.TabPages.Remove(TabControl1.TabPages.Item(1))
                TabControl1.TabPages.Remove(TabControl1.TabPages.Item(0))
            Case Else
                TabControl1.TabPages.Remove(TabControl1.TabPages.Item(2))
                TabControl1.TabPages.Remove(TabControl1.TabPages.Item(1))
                TabControl1.TabPages.Remove(TabControl1.TabPages.Item(0))
        End Select

        ComboBox1.Items.AddRange(Items_FRM)
        ComboBox2.Items.AddRange(Iven_FRM)
        ComboBox9.Items.AddRange(Scripts_Lst)

        LoadProData()
        Me.Show()
    End Sub

    Private Sub LoadProData()
        Dim ProFile As String = PROTO_ITEMS & Items_LST(iLST_Index).proFile

        If cPath = Nothing Then cPath = DatFiles.CheckFile(ProFile, False)
        ProFile = cPath & ProFile

        Select Case Items_LST(iLST_Index).itemType
            Case ItemType.Armor
                itemObject = New ArmorItemObj(ProFile)
            Case ItemType.Container
                itemObject = New ContainerItemObj(ProFile)
            Case ItemType.Drugs
                itemObject = New DrugsItemObj(ProFile)
            Case ItemType.Weapon
                itemObject = New WeaponItemObj(ProFile)
            Case ItemType.Ammo
                itemObject = New AmmoItemObj(ProFile)
            Case ItemType.Misc
                itemObject = New MiscItemObj(ProFile)
            Case ItemType.Key
                itemObject = New KeyItemObj(ProFile)
        End Select
        CType(itemObject, IPrototype).Load()
    End Sub

    Private Sub Items_Form_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        SetCommonValue_Form(itemObject)
        SetFormValues(Items_LST(iLST_Index).itemType)

        Me.Text = TextBox29.Text & " [" & Items_LST(iLST_Index).proFile & "]"
        frmReady = True
    End Sub

    Private Sub SetFormValues(iType As ItemType)
        Select Case iType
            Case ItemType.Weapon
                SetWeaponValue_Form(CType(itemObject, WeaponItemObj))
            Case ItemType.Armor
                SetArmorValue_Form(CType(itemObject, ArmorItemObj))
            Case ItemType.Ammo
                SetAmmoValue_Form(CType(itemObject, AmmoItemObj))
            Case ItemType.Container
                SetContanerValue_Form(CType(itemObject, ContainerItemObj))
            Case ItemType.Drugs
                SetDrugsValue_Form(CType(itemObject, DrugsItemObj))
            Case ItemType.Misc
                SetMiscValue_Form(CType(itemObject, MiscItemObj))
        End Select
    End Sub

    Private Sub SetCommonValue_Form(item As ItemPrototype)
        On Error Resume Next

        ComboBox7.SelectedIndex = item.ObjType

        TextBox29.Text = Misc.GetNameItemMsg(item.DescID)
        TextBox30.Text = Misc.GetNameItemMsg(item.DescID, True)
        TextBox33.Text = item.ProtoID.ToString()
        TextBox33.Text = StrDup(8 - Len(TextBox33.Text), "0") & TextBox33.Text

        ComboBox1.SelectedIndex = item.FrmID
        ComboBox2.SelectedIndex = If((item.InvFID <> -1), 1 + (item.InvFID - &H7000000), 0)
        ComboBox8.SelectedIndex = item.MaterialID
        ComboBox9.SelectedIndex = If((item.ScriptID <> -1), 1 + (item.ScriptID - &H3000000), 0)
        '
        SetSoundID(item.SoundID, ComboBox3)
        '
        NumericUpDown1.Value = item.Cost
        NumericUpDown36.Value = item.LightDis
        NumericUpDown37.Value = CDec(Math.Round(item.LightInt * 100 / &HFFFF)) ' процент 0-100
        NumericUpDown38.Value = item.Weight
        NumericUpDown39.Value = item.Size
        NumericUpDown64.Value = item.DescID

        'Flags
        SetFlags(item)
        SetFlagsExt(item)
    End Sub

    Private Sub SetFlags(item As ItemPrototype)
        CheckBox1.Checked = item.IsFlat
        CheckBox2.Checked = item.IsNoBlock
        CheckBox3.Checked = item.IsMultiHex
        CheckBox4.Checked = item.IsShootThru
        CheckBox5.Checked = item.IsLightThru
        CheckBox24.Checked = item.IsNoHighlight

        CheckBox6.Checked = item.IsTransNone
        If Not CheckBox6.Checked Then
            RadioButton1.Checked = item.IsTransWall
            RadioButton4.Checked = item.IsTransGlass
            RadioButton2.Checked = item.IsTransSteam
            RadioButton5.Checked = item.IsTransEnergy
            RadioButton3.Checked = item.IsTransRed
        End If
    End Sub

    Private Sub SetFlagsExt(item As ItemPrototype)
        cbUse.Checked = item.IsUse
        cbUseOn.Checked = item.IsUseOn
        cbLook.Checked = item.IsLook
        CheckBox10.Checked = item.IsTalk
        cbPickup.Checked = item.IsPickUp

        cbHiddenItem.Checked = item.IsHiddenItem
    End Sub

    Private Sub SetWeaponValue_Form(weapon As WeaponItemObj)
        On Error Resume Next

        NumericUpDown2.Value = weapon.MinDmg
        NumericUpDown3.Value = weapon.MaxDmg
        NumericUpDown4.Value = weapon.MaxRangeP
        NumericUpDown5.Value = weapon.MaxRangeS
        NumericUpDown6.Value = weapon.MinST
        NumericUpDown7.Value = weapon.MPCostP
        NumericUpDown8.Value = weapon.MPCostS
        NumericUpDown9.Value = weapon.CritFail
        NumericUpDown10.Value = weapon.Rounds
        NumericUpDown11.Value = weapon.MaxAmmo

        ComboBox4.SelectedIndex = weapon.AnimCode
        ComboBox5.SelectedIndex = weapon.DmgType

        SetSoundID(weapon.wSoundID, cmbWeaponSoundID)

        ComboBox10.SelectedIndex = If((weapon.ProjPID <> -1), weapon.ProjPID - &H5000000, 0)
        ComboBox11.SelectedIndex = If((weapon.Perk <> -1), weapon.Perk + 1, 0)
        ComboBox12.SelectedIndex = weapon.Caliber

        If weapon.AmmoPID <> -1 Then
            Dim aPid As Integer = weapon.AmmoPID
            For n As Integer = 0 To UBound(AmmoPID)
                If aPid = AmmoPID(n) Then
                    ComboBox13.SelectedIndex = n + 1
                    Exit For
                End If
            Next
        Else
            ComboBox13.SelectedIndex = 0
        End If

        ComboBox14.SelectedIndex = weapon.PrimaryAttackType
        ComboBox15.SelectedIndex = weapon.SecondaryAttackType

        SetWeaponFlags(weapon)

        lblWeaponScore.Text = weapon.WeaponScore(weaponScoreType).ToString
        cmbWScoreType.SelectedIndex = weaponScoreType
    End Sub

    Private Sub SetWeaponFlags(weapon As WeaponItemObj)
        cbBigGun.Checked = weapon.IsBigGun
        cbTwoHand.Checked = weapon.IsTwoHand
        cbEnergyGun.Checked = weapon.IsEnergy
    End Sub

    Private Sub SetArmorValue_Form(armor As ArmorItemObj)
        On Error Resume Next

        NumericUpDown12.Value = armor.AC

        NumericUpDown56.Value = armor.DTNormal
        NumericUpDown57.Value = armor.DTLaser
        NumericUpDown58.Value = armor.DTFire
        NumericUpDown59.Value = armor.DTPlasma
        NumericUpDown60.Value = armor.DTElectrical
        NumericUpDown61.Value = armor.DTEMP
        NumericUpDown62.Value = armor.DTExplode

        NumericUpDown71.Value = armor.DRNormal
        NumericUpDown70.Value = armor.DRLaser
        NumericUpDown69.Value = armor.DRFire
        NumericUpDown68.Value = armor.DRPlasma
        NumericUpDown67.Value = armor.DRElectrical
        NumericUpDown65.Value = armor.DREMP
        NumericUpDown63.Value = armor.DRExplode

        cbArmorMaleFid.SelectedIndex = If(armor.MaleFID <> -1, (armor.MaleFID - &H1000000) + 1, 0)
        cbArmorFemaleFid.SelectedIndex = If(armor.FemaleFID <> -1, (armor.FemaleFID - &H1000000) + 1, 0)

        cbArmorPerk.SelectedIndex = If((armor.Perk <> -1), armor.Perk + 1, 0)

        lblArmorScore.Text = armor.ArmorScore().ToString
    End Sub

    Private Sub SetAmmoValue_Form(ammo As AmmoItemObj)
        On Error Resume Next

        ComboBox23.SelectedIndex = ammo.Caliber
        NumericUpDown26.Value = ammo.Quantity
        NumericUpDown27.Value = ammo.ACAdjust
        NumericUpDown28.Value = ammo.DRAdjust
        NumericUpDown29.Value = ammo.DamMult
        NumericUpDown30.Value = ammo.DamDiv

        GroupBox24.Enabled = False
    End Sub

    Private Sub SetMiscValue_Form(item As MiscItemObj)
        On Error Resume Next

        If item.PowerPID <> -1 Then
            Dim Pid As Integer = item.PowerPID
            For n = 0 To UBound(AmmoPID)
                If Pid = AmmoPID(n) Then
                    ComboBox24.SelectedIndex = n + 1
                    Exit For
                End If
            Next
        Else
            ComboBox24.SelectedIndex = 0
        End If

        If (item.Charges >= 0) And (item.Charges <= 32000) Then
            NumericUpDown31.Value = item.Charges
        Else
            NumericUpDown31.Value = -1
        End If

        ComboBox25.SelectedIndex = item.PowerType
        GroupBox23.Enabled = False
    End Sub

    Private Sub SetContanerValue_Form(item As ContainerItemObj)
        On Error Resume Next

        NumericUpDown32.Value = item.MaxSize
        CheckBox15.Checked = item.GetOpenFlag
        GroupBox25.Enabled = True
    End Sub

    Private Sub SetDrugsValue_Form(drugs As DrugsItemObj)
        On Error Resume Next

        ComboBox19.SelectedIndex = If((drugs.Stat0 <> -1), 2 + drugs.Stat0, 1)
        ComboBox20.SelectedIndex = If((drugs.Stat1 <> -1), 2 + drugs.Stat1, 1)
        ComboBox21.SelectedIndex = If((drugs.Stat2 <> -1), 2 + drugs.Stat2, 1)

        NumericUpDown13.Value = drugs.iAmount0
        NumericUpDown14.Value = drugs.iAmount1
        NumericUpDown15.Value = drugs.iAmount2
        NumericUpDown16.Value = drugs.fAmount0
        NumericUpDown17.Value = drugs.fAmount1
        NumericUpDown18.Value = drugs.fAmount2
        NumericUpDown19.Value = drugs.sAmount0
        NumericUpDown20.Value = drugs.sAmount1
        NumericUpDown21.Value = drugs.sAmount2
        NumericUpDown22.Value = drugs.Duration1
        NumericUpDown23.Value = drugs.Duration2

        ComboBox22.SelectedIndex = If((drugs.W_Effect <> -1), 1 + drugs.W_Effect, 0)

        NumericUpDown24.Value = drugs.AddictionRate
        NumericUpDown25.Value = drugs.W_Onset
    End Sub

    Private Sub ComboBox1_Changed(ByVal sender As Object, ByVal e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Dim frm As String = GetImageName(ComboBox1.SelectedItem.ToString, ".")

        Dim img As Bitmap = My.Resources.RESERVAA 'BadFrm
        If frm IsNot Nothing Then
            Dim pfile As String = Cache_Patch & ART_ITEMS & frm & ".gif"
            If Not File.Exists(pfile) Then ItemFrmToGif("items\", frm)
            If File.Exists(pfile) Then
                Try
                    img = New Bitmap(pfile)
                Catch ex As Exception
                    Main.PrintLog("Error frm convert: " + pfile)
                End Try

                If img.Width > PictureBox1.Size.Width OrElse img.Size.Height > PictureBox1.Size.Height Then
                    PictureBox1.BackgroundImageLayout = ImageLayout.Zoom
                    img.MakeTransparent(Color.White)
                Else
                    PictureBox1.BackgroundImageLayout = ImageLayout.Center
                End If
                If frmReady Then btnSave.Enabled = True
            Else
                Main.PrintLog("Error frm convert: " + pfile)
            End If
            If ThumbnailImage.ItemsImage.ContainsKey(frm) = False Then ThumbnailImage.ItemsImage.Add(frm, CType(img.Clone, Image))
        End If
        PictureBox1.BackgroundImage = img
    End Sub

    Private Sub ComboBox2_Changed(ByVal sender As Object, ByVal e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        Dim frm As String = ComboBox2.SelectedItem.ToString

        If frm = "None" Then
            If (PictureBox4.BackgroundImage IsNot Nothing) Then PictureBox4.BackgroundImage.Dispose()
            PictureBox4.BackgroundImage = Nothing
            If frmReady Then btnSave.Enabled = True
            Exit Sub
        End If

        Dim img As Bitmap = My.Resources.RESERVAA 'BadFrm
        frm = GetImageName(frm, ".")
        If frm IsNot Nothing Then
            Dim pfile As String = Cache_Patch & ART_INVEN & frm & ".gif"
            If Not File.Exists(pfile) Then ItemFrmToGif("inven\", frm)
            If File.Exists(pfile) Then
                Try
                    img = New Bitmap(pfile)
                Catch ex As Exception
                    Main.PrintLog("Error frm convert: " + pfile)
                End Try
                If img.Width > PictureBox4.Size.Width Then
                    PictureBox4.BackgroundImageLayout = ImageLayout.Zoom
                    img.MakeTransparent(Color.White)
                Else
                    PictureBox4.BackgroundImageLayout = ImageLayout.Center
                End If
                If frmReady Then btnSave.Enabled = True
            Else
                Main.PrintLog("Error frm convert: " + pfile)
            End If
            If ThumbnailImage.InventImage.ContainsKey(frm) = False Then ThumbnailImage.InventImage.Add(frm, CType(img.Clone, Image))
        End If
        PictureBox4.BackgroundImage = img
    End Sub

    Private Sub GenderFID(ByVal sender As Object, ByVal e As EventArgs) Handles cbArmorFemaleFid.SelectedIndexChanged, cbArmorMaleFid.SelectedIndexChanged
        Dim pbox As PictureBox
        If CInt(CType(sender, ComboBox).Tag) = Gender.Male Then
            pbox = PictureBox2
        Else
            pbox = PictureBox3
        End If

        If (CType(sender, ComboBox).SelectedIndex = 0) Then
            pbox.Image = Nothing
            If frmReady Then btnSave.Enabled = True
            Exit Sub
        End If

        Dim img As Bitmap = My.Resources.RESERVAA 'BadFrm
        Dim frm As String = CType(sender, ComboBox).SelectedItem.ToString

        Dim frmFile As String = Cache_Patch & ART_CRITTERS_PATH & frm & "aa.gif"
        If Not File.Exists(frmFile) Then DatFiles.CritterFrmToGif(frm)

        If File.Exists(frmFile) Then
            img = New Bitmap(frmFile)
            If img.Height > pbox.Size.Height OrElse img.Width > pbox.Size.Width Then
                If img.GetFrameCount(Imaging.FrameDimension.Time) = 1 Then img.MakeTransparent(Color.White)
                pbox.SizeMode = PictureBoxSizeMode.Zoom
            Else
                pbox.SizeMode = PictureBoxSizeMode.CenterImage
            End If

            If frmReady Then btnSave.Enabled = True
        End If

        pbox.Image = img
    End Sub

    Private Function GetImageName(ByVal frm As String, ByVal symbol As String) As String
        Dim n As Integer = frm.IndexOf(symbol)

        If n <= 0 Then Return Nothing 'BadFrm
        frm = frm.Remove(n)

        Return frm
    End Function

    'Save to Pro
    Private Sub SaveProto(ByVal sender As Object, ByVal e As EventArgs) Handles btnSave.Click
        'Common
        'CommonItem.ObjType = ComboBox7.SelectedIndex
        itemObject.FrmID = ComboBox1.SelectedIndex
        itemObject.InvFID = If(ComboBox2.SelectedIndex > 0, (ComboBox2.SelectedIndex - 1) + &H7000000, -1)
        itemObject.MaterialID = ComboBox8.SelectedIndex
        itemObject.ScriptID = If(ComboBox9.SelectedIndex > 0, (ComboBox9.SelectedIndex - 1) + &H3000000, -1)
        itemObject.SoundID = GetSoundID(ComboBox3.Text)

        itemObject.Cost = CInt(NumericUpDown1.Value)
        itemObject.LightDis = CInt(NumericUpDown36.Value)
        itemObject.LightInt = CInt(Math.Round((NumericUpDown37.Value * &HFFFF) / 100))
        itemObject.Weight = CInt(NumericUpDown38.Value)
        itemObject.Size = CInt(NumericUpDown39.Value)
        itemObject.DescID = CInt(NumericUpDown64.Value)

        'Flags
        itemObject.IsFlat = CheckBox1.Checked
        itemObject.IsNoBlock = CheckBox2.Checked
        itemObject.IsMultiHex = CheckBox3.Checked
        itemObject.IsShootThru = CheckBox4.Checked
        itemObject.IsLightThru = CheckBox5.Checked
        itemObject.IsNoHighlight = CheckBox24.Checked

        ' удаляем взаимозаменяемые флаги TransNone/TransRed/TransWall/TransGlass/TransSteam/TransEnergy
        itemObject.Flags = itemObject.Flags And &HFFF03FFF
        If CheckBox6.Checked Then
            itemObject.IsTransNone = True
        Else
            If RadioButton1.Checked Then
                itemObject.IsTransWall = True
            ElseIf RadioButton4.Checked Then
                itemObject.IsTransGlass = True
            ElseIf RadioButton2.Checked Then
                itemObject.IsTransSteam = True
            ElseIf RadioButton5.Checked Then
                itemObject.IsTransEnergy = True
            ElseIf RadioButton3.Checked Then
                itemObject.IsTransRed = True
            End If
        End If

        itemObject.IsUse = cbUse.Checked
        itemObject.IsUseOn = cbUseOn.Checked
        itemObject.IsLook = cbLook.Checked
        itemObject.IsPickUp = cbPickup.Checked
        itemObject.IsTalk = CheckBox10.Checked
        itemObject.IsHiddenItem = cbHiddenItem.Checked

        Dim iType As ItemType = CType(ComboBox7.SelectedIndex, ItemType)
        Select Case iType
            Case ItemType.Weapon
                Items_LST(iLST_Index).itemType = ItemType.Weapon

                Dim weapon = CType(itemObject, WeaponItemObj)
                weapon.MinDmg = CInt(NumericUpDown2.Value)
                weapon.MaxDmg = CInt(NumericUpDown3.Value)
                weapon.MaxRangeP = CInt(NumericUpDown4.Value)
                weapon.MaxRangeS = CInt(NumericUpDown5.Value)
                weapon.MinST = CInt(NumericUpDown6.Value)
                weapon.MPCostP = CInt(NumericUpDown7.Value)
                weapon.MPCostS = CInt(NumericUpDown8.Value)
                weapon.CritFail = CInt(NumericUpDown9.Value)
                weapon.Rounds = CInt(NumericUpDown10.Value)
                weapon.MaxAmmo = CInt(NumericUpDown11.Value)
                weapon.AnimCode = ComboBox4.SelectedIndex
                weapon.DmgType = CType(ComboBox5.SelectedIndex, WeaponItemObj.DamageType)
                weapon.wSoundID = GetSoundID(cmbWeaponSoundID.Text)

                weapon.ProjPID = If(ComboBox10.SelectedIndex > 0, ComboBox10.SelectedIndex + &H5000000, -1)
                weapon.Perk = If(ComboBox11.SelectedIndex > 0, ComboBox11.SelectedIndex - 1, -1)
                weapon.Caliber = ComboBox12.SelectedIndex
                weapon.AmmoPID = If(ComboBox13.SelectedIndex > 0, AmmoPID(ComboBox13.SelectedIndex - 1), -1)

                weapon.PrimaryAttackType = ComboBox14.SelectedIndex
                weapon.SecondaryAttackType = ComboBox15.SelectedIndex

                weapon.IsTwoHand = cbTwoHand.Checked
                weapon.IsBigGun = cbBigGun.Checked
                weapon.IsEnergy = cbEnergyGun.Checked

            Case ItemType.Armor
                Items_LST(iLST_Index).itemType = ItemType.Armor

                Dim armor = CType(itemObject, ArmorItemObj)
                armor.DTNormal = CInt(NumericUpDown56.Value)
                armor.DTLaser = CInt(NumericUpDown57.Value)
                armor.DTFire = CInt(NumericUpDown58.Value)
                armor.DTPlasma = CInt(NumericUpDown59.Value)
                armor.DTElectrical = CInt(NumericUpDown60.Value)
                armor.DTEMP = CInt(NumericUpDown61.Value)
                armor.DTExplode = CInt(NumericUpDown62.Value)
                armor.DRNormal = CInt(NumericUpDown71.Value)
                armor.DRLaser = CInt(NumericUpDown70.Value)
                armor.DRFire = CInt(NumericUpDown69.Value)
                armor.DRPlasma = CInt(NumericUpDown68.Value)
                armor.DRElectrical = CInt(NumericUpDown67.Value)
                armor.DREMP = CInt(NumericUpDown65.Value)
                armor.DRExplode = CInt(NumericUpDown63.Value)
                armor.AC = CInt(NumericUpDown12.Value)

                Dim id = cbArmorMaleFid.SelectedIndex
                If (id <> -1) Then armor.MaleFID = If(id > 0, (id + &H1000000) - 1, -1)

                id = cbArmorFemaleFid.SelectedIndex
                If (id <> -1) Then armor.FemaleFID = If(id > 0, (id + &H1000000) - 1, -1)

                id = cbArmorPerk.SelectedIndex
                If (id <> -1) Then armor.Perk = If(id > 0, id - 1, -1)

            Case ItemType.Drugs
                Items_LST(iLST_Index).itemType = ItemType.Drugs

                Dim drugs = CType(itemObject, DrugsItemObj)
                drugs.Stat0 = If(ComboBox19.SelectedIndex > 1, ComboBox19.SelectedIndex - 2, (&HFFFFFFFE + ComboBox19.SelectedIndex))
                drugs.Stat1 = If(ComboBox20.SelectedIndex > 1, ComboBox20.SelectedIndex - 2, (&HFFFFFFFE + ComboBox20.SelectedIndex))
                drugs.Stat2 = If(ComboBox21.SelectedIndex > 1, ComboBox21.SelectedIndex - 2, (&HFFFFFFFE + ComboBox21.SelectedIndex))
                drugs.iAmount0 = CInt(NumericUpDown13.Value)
                drugs.iAmount1 = CInt(NumericUpDown14.Value)
                drugs.iAmount2 = CInt(NumericUpDown15.Value)
                drugs.fAmount0 = CInt(NumericUpDown16.Value)
                drugs.fAmount1 = CInt(NumericUpDown17.Value)
                drugs.fAmount2 = CInt(NumericUpDown18.Value)
                drugs.sAmount0 = CInt(NumericUpDown19.Value)
                drugs.sAmount1 = CInt(NumericUpDown20.Value)
                drugs.sAmount2 = CInt(NumericUpDown21.Value)
                drugs.Duration1 = CInt(NumericUpDown22.Value)
                drugs.Duration2 = CInt(NumericUpDown23.Value)
                drugs.W_Effect = If(ComboBox22.SelectedIndex > 0, ComboBox22.SelectedIndex - 1, -1)
                drugs.AddictionRate = CInt(NumericUpDown24.Value)
                drugs.W_Onset = CInt(NumericUpDown25.Value)

            Case ItemType.Ammo
                Items_LST(iLST_Index).itemType = ItemType.Ammo

                Dim ammo = CType(itemObject, AmmoItemObj)
                ammo.Caliber = ComboBox23.SelectedIndex
                ammo.Quantity = CInt(NumericUpDown26.Value)
                ammo.ACAdjust = CInt(NumericUpDown27.Value)
                ammo.DRAdjust = CInt(NumericUpDown28.Value)
                ammo.DamMult = CInt(NumericUpDown29.Value)
                ammo.DamDiv = CInt(NumericUpDown30.Value)

            Case ItemType.Misc
                Items_LST(iLST_Index).itemType = ItemType.Misc

                Dim item = CType(itemObject, MiscItemObj)
                If NumericUpDown31.Value <> -1 Then item.Charges = CInt(NumericUpDown31.Value)
                item.PowerPID = If(ComboBox24.SelectedIndex > 0, AmmoPID(ComboBox24.SelectedIndex - 1), -1)
                If ComboBox25.SelectedIndex <> -1 Then item.PowerType = ComboBox25.SelectedIndex

            Case ItemType.Container
                Items_LST(iLST_Index).itemType = ItemType.Container

                Dim item = CType(itemObject, ContainerItemObj)
                item.MaxSize = CInt(NumericUpDown32.Value)
                item.SetOpenFlag = CheckBox15.Checked

            Case Else 'Key
                Items_LST(iLST_Index).itemType = ItemType.Key
        End Select

        'Save
        SubSaveProto()

        Dim indx As Integer = LW_SearhItemIndex(iLST_Index, Main_Form.ListView2)
        If indx <> Nothing Then
            If Main_Form.ListView2.Items(indx).SubItems(2).Text <> ComboBox7.Text Then
                Main_Form.ListView2.Items(indx).SubItems(2).Text = ComboBox7.Text
            End If
            Main_Form.ListView2.Items(indx).ForeColor = Color.DarkBlue
            Main_Form.ListView2.Items(indx).SubItems(3).Text = If(Settings.proRO, "R/O", String.Empty)
        End If

        cPath = DatFiles.CheckFile(PROTO_ITEMS & Items_LST(iLST_Index).proFile, False)
        btnSave.Enabled = False
    End Sub

    Private Sub SubSaveProto()
        Dim proFile As String = SaveMOD_Path & PROTO_ITEMS
        If Not Directory.Exists(proFile) Then Directory.CreateDirectory(proFile)

        proFile += Items_LST(iLST_Index).proFile
        ProFiles.SaveItemProData(proFile, itemObject)
        'Log
        Main.PrintLog("Save Pro: " & proFile)
    End Sub

    Private Sub SaveItemMsg(ByVal str As String, Optional ByRef Desc As Boolean = False)
        Dim ID As Integer = CInt(NumericUpDown64.Value) 'DescID

        Messages.GetMsgData("pro_item.msg", False)
        If Messages.AddTextMSG(str, ID, Desc) Then
            MsgBox("You cannot add value to the MSG file." & vbLf & "Not found msg line #:" & ID, MsgBoxStyle.Critical, "Error: pro_item.msg")
            Exit Sub
        End If
        'Save
        Messages.SaveMSGFile("pro_item.msg")

        'Update Name Item List
        If Not (Desc) Then
            Button4.Enabled = False
            Items_LST(iLST_Index).itemName = str
            Dim indx As Integer = LW_SearhItemIndex(iLST_Index, Main_Form.ListView2)
            If indx <> Nothing Then Main_Form.ListView2.Items(indx).SubItems(0).Text = str
        Else
            Button5.Enabled = False
        End If
    End Sub

    Private Sub CheckBox6_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CheckBox6.CheckedChanged
        If frmReady And CheckBox6.Focused Then
            frmReady = False
            RadioButton1.Checked = False
            RadioButton3.Checked = False
            RadioButton4.Checked = False
            RadioButton5.Checked = False
            btnSave.Enabled = True
            frmReady = True
        End If
    End Sub

    Private Sub SaveEnable(ByVal sender As Object, ByVal e As EventArgs) Handles NumericUpDown2.ValueChanged, NumericUpDown9.ValueChanged,
        NumericUpDown8.ValueChanged, NumericUpDown7.ValueChanged, NumericUpDown6.ValueChanged, NumericUpDown5.ValueChanged, NumericUpDown4.ValueChanged,
        NumericUpDown3.ValueChanged, NumericUpDown11.ValueChanged, NumericUpDown10.ValueChanged, NumericUpDown71.ValueChanged, NumericUpDown70.ValueChanged,
        NumericUpDown69.ValueChanged, NumericUpDown68.ValueChanged, NumericUpDown67.ValueChanged, NumericUpDown65.ValueChanged, NumericUpDown63.ValueChanged,
        NumericUpDown62.ValueChanged, NumericUpDown61.ValueChanged, NumericUpDown60.ValueChanged, NumericUpDown59.ValueChanged, NumericUpDown58.ValueChanged,
        NumericUpDown57.ValueChanged, NumericUpDown56.ValueChanged, NumericUpDown12.ValueChanged, NumericUpDown64.ValueChanged, NumericUpDown39.ValueChanged,
        NumericUpDown38.ValueChanged, NumericUpDown37.ValueChanged, NumericUpDown36.ValueChanged, NumericUpDown32.ValueChanged, NumericUpDown31.ValueChanged,
        NumericUpDown30.ValueChanged, NumericUpDown29.ValueChanged, NumericUpDown28.ValueChanged, NumericUpDown27.ValueChanged, NumericUpDown25.ValueChanged,
        NumericUpDown24.ValueChanged, NumericUpDown23.ValueChanged, NumericUpDown22.ValueChanged, NumericUpDown21.ValueChanged, NumericUpDown20.ValueChanged,
        NumericUpDown19.ValueChanged, NumericUpDown18.ValueChanged, NumericUpDown17.ValueChanged, NumericUpDown16.ValueChanged, NumericUpDown15.ValueChanged,
        NumericUpDown14.ValueChanged, NumericUpDown13.ValueChanged, NumericUpDown1.ValueChanged, NumericUpDown26.ValueChanged
        '
        If frmReady Then btnSave.Enabled = True
    End Sub

    Private Sub SaveEnable1(ByVal sender As Object, ByVal e As EventArgs) Handles cmbWeaponSoundID.TextChanged, ComboBox3.TextChanged
        If frmReady Then
            If CType(sender, ComboBox).Text <> String.Empty Then
                btnSave.Enabled = True
            End If
        End If
    End Sub

    Private Sub SaveEnable2(ByVal sender As Object, ByVal e As EventArgs) Handles ComboBox5.SelectedIndexChanged,
        ComboBox4.SelectedIndexChanged, ComboBox15.SelectedIndexChanged, ComboBox14.SelectedIndexChanged, ComboBox13.SelectedIndexChanged, ComboBox12.SelectedIndexChanged,
        ComboBox11.SelectedIndexChanged, ComboBox10.SelectedIndexChanged, cbArmorPerk.SelectedIndexChanged, ComboBox9.SelectedIndexChanged, ComboBox8.SelectedIndexChanged,
        ComboBox25.SelectedIndexChanged, ComboBox24.SelectedIndexChanged, ComboBox23.SelectedIndexChanged, ComboBox22.SelectedIndexChanged,
        ComboBox21.SelectedIndexChanged, ComboBox20.SelectedIndexChanged, ComboBox19.SelectedIndexChanged
        '
        If frmReady Then btnSave.Enabled = True
    End Sub

    Private Sub SaveEnable3(ByVal sender As Object, ByVal e As EventArgs) Handles cbBigGun.CheckedChanged, cbEnergyGun.CheckedChanged, cbTwoHand.CheckedChanged, cbLook.CheckedChanged,
        cbUseOn.CheckedChanged, cbUse.CheckedChanged, CheckBox5.CheckedChanged, CheckBox4.CheckedChanged, CheckBox3.CheckedChanged, CheckBox24.CheckedChanged, CheckBox2.CheckedChanged,
        CheckBox15.CheckedChanged, cbHiddenItem.CheckedChanged, cbPickup.CheckedChanged, CheckBox1.CheckedChanged
        '
        If frmReady Then btnSave.Enabled = True
    End Sub

    Private Sub SaveEnable4(ByVal sender As Object, ByVal e As EventArgs) Handles RadioButton2.CheckedChanged, RadioButton5.CheckedChanged, RadioButton4.CheckedChanged, RadioButton3.CheckedChanged, RadioButton1.CheckedChanged
        If frmReady Then
            btnSave.Enabled = True
            CheckBox6.Checked = False
        End If
    End Sub

    Private Sub Restore(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        Dim tPatch As String = cPath

        If DatFiles.UnDatFile(PROTO_ITEMS & Items_LST(iLST_Index).proFile, Prototypes.GetSizeProByType(Items_LST(iLST_Index).itemType)) Then
            btnSave.Enabled = True
            frmReady = False
            ReloadPro = True
            cPath = Cache_Patch
            LoadProData()
            Items_Form_Load(Nothing, Nothing)
            ReloadPro = False
        Else
            MsgBox("This prototype file does not exist in Master.dat.", MsgBoxStyle.Exclamation)
        End If

        cPath = tPatch
    End Sub

    Private Sub Reload(ByVal sender As Object, ByVal e As EventArgs) Handles Button2.Click
        LoadProData()
        frmReady = False
        ReloadPro = True
        Items_Form_Load(Nothing, Nothing)
        ReloadPro = False
        Button2.Enabled = False
        btnSave.Enabled = False
    End Sub

    Private Sub ComboBox7_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ComboBox7.SelectedIndexChanged
        If ReloadPro OrElse (frmReady AndAlso MsgBox("Do you want to change the subtype of this item?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes) Then
            TabControl1.Visible = False
            If TabControl1.TabCount > 1 Then
                TabControl1.SelectTab(1)
                TabControl1.TabPages.RemoveAt(0)
            End If

            If (backupItem Is Nothing) Then
                backupItem = itemObject
            End If

            GroupBox25.Enabled = False

            Select Case ComboBox7.SelectedIndex
                Case ItemType.Armor
                    If (TypeOf backupItem Is ArmorItemObj) Then
                        itemObject = backupItem
                        backupItem = Nothing
                    Else
                        itemObject = New ArmorItemObj(itemObject)
                    End If
                    TabControl1.TabPages.Insert(0, TabPage2)

                Case ItemType.Drugs
                    If (TypeOf backupItem Is DrugsItemObj) Then
                        itemObject = backupItem
                        backupItem = Nothing
                    Else
                        itemObject = New DrugsItemObj(itemObject)
                    End If
                    TabControl1.TabPages.Insert(0, TabPage4)

                Case ItemType.Weapon
                    If (TypeOf backupItem Is WeaponItemObj) Then
                        itemObject = backupItem
                        backupItem = Nothing
                    Else
                        itemObject = New WeaponItemObj(itemObject)
                    End If
                    TabControl1.TabPages.Insert(0, TabPage1)

                Case ItemType.Ammo
                    If (TypeOf backupItem Is AmmoItemObj) Then
                        itemObject = backupItem
                        backupItem = Nothing
                    Else
                        itemObject = New AmmoItemObj(itemObject)
                    End If
                    TabControl1.TabPages.Insert(0, TabPage5)
                    GroupBox23.Enabled = True
                    GroupBox24.Enabled = False

                Case ItemType.Misc
                    If (TypeOf backupItem Is MiscItemObj) Then
                        itemObject = backupItem
                        backupItem = Nothing
                    Else
                        itemObject = New MiscItemObj(itemObject)
                    End If
                    TabControl1.TabPages.Insert(0, TabPage5)
                    GroupBox23.Enabled = False
                    GroupBox24.Enabled = True

                Case ItemType.Container
                    If (TypeOf backupItem Is ContainerItemObj) Then
                        itemObject = backupItem
                        backupItem = Nothing
                    Else
                        itemObject = New ContainerItemObj(itemObject)
                    End If

                Case ItemType.Key
                    If (TypeOf backupItem Is KeyItemObj) Then
                        itemObject = backupItem
                        backupItem = Nothing
                    Else
                        itemObject = New KeyItemObj(itemObject)
                    End If
            End Select

            If frmReady Then
                frmReady = False
                SetFormValues(CType(ComboBox7.SelectedIndex, ItemType))
                frmReady = True
            End If

            TabControl1.SelectTab(0)
            btnSave.Enabled = True
            TabControl1.Visible = True
        ElseIf frmReady Then
            frmReady = False
            ComboBox7.SelectedIndex = itemObject.ObjType
            frmReady = True
        End If
    End Sub

    Private Sub TextBox29_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TextBox29.TextChanged
        If frmReady Then Button4.Enabled = True
    End Sub

    Private Sub Button4_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button4.Click
        SaveItemMsg(TextBox29.Text)
    End Sub

    Private Sub TextBox30_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TextBox30.TextChanged
        If frmReady Then Button5.Enabled = True
    End Sub

    Private Sub Button5_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button5.Click
        SaveItemMsg(TextBox30.Text, True)
    End Sub

    Private Sub Button3_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button3.Click
        Me.Close()
    End Sub

    Private Sub Items_Form_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
        If btnSave.Enabled Then
            Dim btn As MsgBoxResult = MsgBox("Save changes to PRO file?", MsgBoxStyle.YesNoCancel, "Attention!")
            If btn = MsgBoxResult.Yes Then
                SaveProto(sender, e)
            ElseIf btn = MsgBoxResult.Cancel Then
                e.Cancel = True
            End If
        End If

        If e.Cancel = False Then
            Main_Form.Focus()
        End If
    End Sub

    Private Sub Items_Form_FormClosed(ByVal sender As Object, ByVal e As FormClosedEventArgs) Handles MyBase.FormClosed
        If PictureBox1.BackgroundImage IsNot Nothing Then PictureBox1.BackgroundImage.Dispose()
        If PictureBox4.BackgroundImage IsNot Nothing Then PictureBox4.BackgroundImage.Dispose()
        If PictureBox2.Image IsNot Nothing Then PictureBox2.Image.Dispose()
        If PictureBox3.Image IsNot Nothing Then PictureBox3.Image.Dispose()
        Me.Dispose()
    End Sub

    Private Sub Items_Form_Activated(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Activated
        If frmReady Then Main_Form.ToolStripStatusLabel1.Text = cPath & PROTO_ITEMS & Items_LST(iLST_Index).proFile
    End Sub

    Private Sub Button6_EnabledChanged(ByVal sender As Object, ByVal e As EventArgs) Handles btnSave.EnabledChanged
        If btnSave.Enabled Then Button2.Enabled = True
    End Sub

    Private Sub NumericUpDown_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles NumericUpDown1.KeyPress, NumericUpDown9.KeyPress, NumericUpDown8.KeyPress,
        NumericUpDown71.KeyPress, NumericUpDown70.KeyPress, NumericUpDown7.KeyPress, NumericUpDown69.KeyPress, NumericUpDown68.KeyPress, NumericUpDown67.KeyPress,
        NumericUpDown65.KeyPress, NumericUpDown64.KeyPress, NumericUpDown63.KeyPress, NumericUpDown62.KeyPress, NumericUpDown61.KeyPress, NumericUpDown60.KeyPress,
        NumericUpDown6.KeyPress, NumericUpDown59.KeyPress, NumericUpDown58.KeyPress, NumericUpDown57.KeyPress, NumericUpDown56.KeyPress, NumericUpDown5.KeyPress,
        NumericUpDown4.KeyPress, NumericUpDown39.KeyPress, NumericUpDown38.KeyPress, NumericUpDown37.KeyPress, NumericUpDown36.KeyPress, NumericUpDown32.KeyPress,
        NumericUpDown31.KeyPress, NumericUpDown30.KeyPress, NumericUpDown3.KeyPress, NumericUpDown29.KeyPress, NumericUpDown28.KeyPress, NumericUpDown27.KeyPress,
        NumericUpDown26.KeyPress, NumericUpDown25.KeyPress, NumericUpDown24.KeyPress, NumericUpDown23.KeyPress, NumericUpDown22.KeyPress, NumericUpDown21.KeyPress,
        NumericUpDown20.KeyPress, NumericUpDown2.KeyPress, NumericUpDown19.KeyPress, NumericUpDown18.KeyPress, NumericUpDown17.KeyPress, NumericUpDown16.KeyPress,
        NumericUpDown15.KeyPress, NumericUpDown14.KeyPress, NumericUpDown13.KeyPress, NumericUpDown12.KeyPress, NumericUpDown11.KeyPress, NumericUpDown10.KeyPress
        If (Char.IsDigit(e.KeyChar)) Then btnSave.Enabled = True
    End Sub

    'Private Sub Items_Form_Shown(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Shown
    'For Each upDown As NumericUpDown In Me.Controls.OfType(Of NumericUpDown)()
    'AddHandler upDown.ValueChanged, AddressOf SaveEnable
    'Next
    'End Sub

    Private Sub SetSoundID(ByVal ID As Byte, ByRef control As ComboBox)
        For n = 0 To control.Items.Count - 1
            If (control.Items(n).ToString.StartsWith(ID.ToString)) Then
                control.SelectedIndex = n
                Exit Sub
            End If
        Next
        control.Text = ID.ToString
    End Sub

    Private Function GetSoundID(ByVal sound As String) As Byte
        Dim pos As Integer = sound.IndexOf(" "c)
        If pos <> -1 Then sound = sound.Remove(pos)
        Dim value As Byte = 0
        If (Byte.TryParse(sound, value) = False) Then
            MessageBox.Show(String.Format("Using an invalid value '{0}' for the SoundID parameter.", sound), "Error SoundID", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
        Return value
    End Function

    Private Sub cbBigGun_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cbBigGun.Click
        If cbBigGun.Checked Then cbEnergyGun.Checked = False
    End Sub

    Private Sub cbEnergyGun_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cbEnergyGun.Click
        If cbEnergyGun.Checked Then cbBigGun.Checked = False
    End Sub

    Dim num As Byte = 1
    Dim sec As Byte = 1

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Dim id = GetSoundID(cmbWeaponSoundID.Text)
        If id = 0 Then Exit Sub

        Dim fileFormat As String = String.Format("\sound\sfx\WA{0}{1}XXX{2}.acm", Convert.ToChar(id), If(sec = 1, "1", "2"), If(num = 1, "1", "2"))

        If sec = 2 AndAlso num = 2 Then
            num = 1
            sec = 1
        ElseIf sec = 1 AndAlso num = 1 Then
            num = 2 ' s1 n2
        ElseIf num = 2 Then
            sec = 2
            num = 1
        ElseIf sec = 2 Then
            num = 2 ' s2 n2
        End If

        Dim sfxPathFile = DatFiles.ExtractSFXFile(fileFormat)
        If (sfxPathFile = Nothing) Then Return

        Dim snd = New Media.SoundPlayer(sfxPathFile)
        snd.Play()

        Main.PrintLog("Playing Sound: " & Path.GetFileName(fileFormat))
    End Sub

    Private Sub cmbWeaponSoundID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbWeaponSoundID.SelectedIndexChanged
        num = 1
        sec = 1
    End Sub

    Private Sub cmbWScoreType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbWScoreType.SelectedIndexChanged
        If frmReady Then
            weaponScoreType = cmbWScoreType.SelectedIndex
            lblWeaponScore.Text = CType(itemObject, WeaponItemObj).WeaponScore(weaponScoreType).ToString
        End If
    End Sub

    Private Sub RecalcScore() Handles NumericUpDown2.ValueChanged, NumericUpDown3.ValueChanged, ComboBox11.SelectedIndexChanged
        If frmReady Then
            Dim item = CType(itemObject, WeaponItemObj)
            item.MinDmg = CInt(NumericUpDown2.Value)
            item.MaxDmg = CInt(NumericUpDown3.Value)
            item.Perk = ComboBox11.SelectedIndex - 1

            lblWeaponScore.Text = item.WeaponScore(weaponScoreType).ToString
        End If
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim sf = New SetFlagsForm(itemObject)
        sf.ShowDialog(Me)
        If (sf.sets.flags <> itemObject.Flags) Then
            itemObject.Flags = sf.sets.flags
            SetFlags(itemObject)
            btnSave.Enabled = True
        End If
        If (sf.sets.flagsExt <> itemObject.FlagsExt) Then
            itemObject.FlagsExt = sf.sets.flagsExt
            SetFlagsExt(itemObject)
            If (itemObject.ObjType = ItemType.Weapon) Then
                SetWeaponFlags(CType(itemObject, WeaponItemObj))
            End If
            btnSave.Enabled = True
        End If
        sf.Dispose()
    End Sub

End Class