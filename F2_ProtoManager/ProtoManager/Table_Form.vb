Imports System.Text
Imports System.IO
Imports Microsoft.VisualBasic.FileIO

Imports Prototypes
Imports Enums

Public Class Table_Form

    Private Enum TabType As Integer
        Critter
        Weapon
        Ammo
        Armor
        Drugs
        Misc
    End Enum

    Private CheckedList As CheckedListBox.CheckedItemCollection

    Private Const spr As String = ";"
    Private Const splt As Char = "|"c

    Private ReadOnly DmgType() As String = {"Normal", "Laser", "Fire", "Plasma", "Electrical", "EMP", "Explode", "Unused"}

    Private ReadOnly AnimCodes() As String = {"None", "Knife (D)", "Club (E)", "2Hnd Club (F)", "Spear (G)", "Pistol (H)", "Uzi (I)", "Rifle (J)", "Laser (K)", "Minigun (L)", "Rocket Launcher (M)", "Custom Anim (S)", "Custom Anim (O)", "Custom Anim (P)", "Custom Anim (Q)", "Custom Anim (T)"}

    Private ReadOnly AttackTypes() As String = {"None", "Punch", "Kick", "Swing", "Thrust", "Throw", "Single", "Burst", "Flame"}

    Private ReadOnly DrugEffect() As String = {"Drug Stat (Special)", "None", "Strength", "Perception", "Endurance",
                                               "Charisma", "Intelligence", "Agility", "Luck", "Max.Healing Point",
                                               "Max.Action Point", "Calss Armor", "Unarmed Damage", "Melee Damage",
                                               "Max.Weight", "Sequence", "Healing Rate", "Critical Chance",
                                               "Better Critical", "Normal Tresholds Damage", "Laser Tresholds Damage",
                                               "Fire Tresholds Damage", "Plasma Tresholds Damage",
                                               "Electrical Tresholds Damage", "EMP Tresholds Damage",
                                               "Explode Tresholds Damage", "Normal Damage Resistance",
                                               "Laser Damage Resistance", "Fire Damage Resistance",
                                               "Plasma Damage Resistance", "Electrical Damage Resistance",
                                               "EMP Damage Resistance", "Explode Damage Resistance",
                                               "Radiation Resistance", "Poison Resistance", "Age", "Gender", "Current HP",
                                               "Current Poison Level", "Current Radiation Level"}


    Private Sub CreateTable()
        Dim critterPro As CritterProto
        Dim commonItem As CommonItemProto
        Dim weaponItem As WeaponItemProto
        Dim armorItem As ArmorItemProto
        Dim ammoItem As AmmoItemProto
        Dim drugItem As DrugsItemProto
        Dim miscItem As MiscItemProto

        Dim fFile As Integer, iType As Integer = TabControl1.SelectedIndex
        Dim cPath, pathFile As String

        Dim table As List(Of String) = New List(Of String)
        table.Add("Import" & spr & "ProFILE" & spr & "NAME")

        If iType <> TabType.Critter Then
            For n = 0 To UBound(Items_LST)
                If Items_LST(n).itemType = Array.IndexOf(ItemTypesName, TabControl1.SelectedTab.Text) Then
                    table.Add(Items_LST(n).proFile)
                End If
            Next

            Dim IsRead As Boolean = False

            Main.GetItemsData()
            Messages.GetMsgData("pro_item.msg")

            'Dim tableLine As StringBuilder = New StringBuilder()

            Dim count As Integer
            Select Case iType
                Case TabType.Weapon
                    count = ProtoMemberCount.Weapon
                Case TabType.Ammo
                    count = ProtoMemberCount.Ammo
                Case TabType.Armor
                    count = ProtoMemberCount.Armor
                Case TabType.Drugs
                    count = ProtoMemberCount.Drugs
                Case TabType.Misc
                    count = ProtoMemberCount.Misc
            End Select

            Dim dataBuffer(count - 1) As Integer
            Dim cmDataBuffer(ProtoMemberCount.Common - 1) As Integer

            For n = 1 To table.Count - 1
                cPath = DatFiles.CheckFile(PROTO_ITEMS & table(n), False)
                pathFile = String.Concat(cPath, PROTO_ITEMS, table(n))

                fFile = FreeFile()
                FileOpen(fFile, pathFile, OpenMode.Binary, OpenAccess.Read, OpenShare.Shared)

                FileGet(fFile, cmDataBuffer)
                ProFiles.ReverseLoadData(cmDataBuffer, commonItem)
                FileGet(fFile, commonItem.data.SoundID)

                'tableLine.Clear()

                If cPath.Equals(SaveMOD_Path, StringComparison.OrdinalIgnoreCase) Then
                    table(n) = spr & table(n)
                Else
                    table(n) = "#" & spr & table(n) ' # - ignore mark
                End If

                table(n) &= (spr & Messages.GetNameObject(commonItem.common.DescID)) ' get name

                For m = 0 To CheckedList.Count - 1
                    If n = 1 Then table(0) &= spr & CheckedList.Item(m).ToString

                    If (CreateTable_Common(table(n), CheckedList.Item(m).ToString, commonItem)) Then Continue For

                    Select Case iType
                        Case TabType.Weapon
                            If Not IsRead Then
                                FileGet(fFile, dataBuffer)
                                ProFiles.ReverseLoadData(dataBuffer, weaponItem)
                                FileGet(fFile, weaponItem.wSoundID)
                                FileClose(fFile)
                                IsRead = True
                            End If
                            CreateTable_Weapon(table(n), CheckedList.Item(m).ToString, weaponItem, commonItem)
                        Case TabType.Ammo
                            If Not IsRead Then
                                FileGet(fFile, dataBuffer)
                                FileClose(fFile)
                                ProFiles.ReverseLoadData(dataBuffer, ammoItem)
                                IsRead = True
                            End If
                            CreateTable_Ammo(table(n), CheckedList.Item(m).ToString, ammoItem)
                        Case TabType.Armor
                            If Not IsRead Then
                                FileGet(fFile, dataBuffer)
                                FileClose(fFile)
                                ProFiles.ReverseLoadData(dataBuffer, armorItem)
                                IsRead = True
                            End If
                            CreateTable_Armor(table(n), CheckedList.Item(m).ToString, armorItem)
                        Case TabType.Drugs
                            If Not IsRead Then
                                FileGet(fFile, dataBuffer)
                                FileClose(fFile)
                                ProFiles.ReverseLoadData(dataBuffer, drugItem)
                                IsRead = True
                            End If
                            CreateTable_Drugs(table(n), CheckedList.Item(m).ToString, drugItem)
                        Case Else ' misc
                            If Not IsRead Then
                                FileGet(fFile, dataBuffer)
                                FileClose(fFile)
                                ProFiles.ReverseLoadData(dataBuffer, miscItem)
                                IsRead = True
                            End If
                            CreateTable_Misc(table(n), CheckedList.Item(m).ToString, miscItem)
                    End Select
                Next
                IsRead = False
                'table(n) &= tableLine.ToString
            Next
        Else
            ' Critter table
            If Critter_LST Is Nothing Then Main.CreateCritterList()

            Progress_Form.ShowProgressBar(CInt(UBound(Critter_LST) / 2))

            GetMsgData("pro_crit.msg")

            For n = 1 To UBound(Critter_LST) + 1

                Dim proFile As String = Critter_LST(n - 1).proFile
                cPath = DatFiles.CheckFile(PROTO_CRITTERS & proFile, False)
                pathFile = String.Concat(cPath, PROTO_CRITTERS & proFile)

                If (ProFiles.LoadCritterProData(pathFile, critterPro) = False) Then
                    table.Add("#" & spr & proFile & spr & "<BadFormat>")
                    'Log
                    Main.PrintLog("Bad Format: " & pathFile)
                    Application.DoEvents()
                    Continue For
                End If

                If cPath.Equals(SaveMOD_Path, StringComparison.OrdinalIgnoreCase) Then
                    table.Add(spr & proFile)
                Else
                    table.Add("#" & spr & proFile) ' # - ignore mark
                End If

                table(n) &= spr & Messages.GetNameObject(critterPro.common.DescID)
                For m = 0 To CheckedList.Count - 1
                    'создаем строку с параметрами
                    If n = 1 Then table(0) &= spr & CheckedList.Item(m).ToString
                    CreateTable_Critter(table(n), CheckedList.Item(m).ToString, critterPro)
                Next
                If ((n Mod 2) <> 0) Then Progress_Form.ProgressIncrement()
            Next
        End If

        SaveTable(TabControl1.SelectedTab.Text, table)
        Progress_Form.Close()
    End Sub

    Private Sub SaveTable(ByVal fileName As String, table As List(Of String))
        SaveFileDialog1.FileName = fileName
        If SaveFileDialog1.ShowDialog = DialogResult.Cancel Then Exit Sub
        fileName = SaveFileDialog1.FileName

SaveRetry:
        Try
            File.WriteAllLines(fileName, table, ASCIIEncoding.Default)
        Catch
            If MsgBox("Error save table file!", MsgBoxStyle.RetryCancel) = MsgBoxResult.Retry Then GoTo SaveRetry
            SaveTable(fileName, table)
        End Try

        If MsgBox("Open saved table file?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then Process.Start(SaveFileDialog1.FileName)
    End Sub

    Private Function CreateTable_Common(ByRef tableLine As String, param As String, ByRef item As CommonItemProto) As Boolean
        Select Case param
            Case "PID"
                tableLine &= spr & item.common.ProtoID
            Case "FrmID"
                tableLine &= spr & item.common.FrmID
            Case "Cost"
                tableLine &= spr & item.data.Cost
            Case "Weight"
                tableLine &= spr & item.data.Weight
            Case "Size"
                tableLine &= spr & item.data.Size
            Case "Shoot Thru [Flag]"
                tableLine &= spr & CBool(item.common.Flags And Flags.ShootThru)
            Case "Light Thru [Flag]"
                tableLine &= spr & CBool(item.common.Flags And Flags.LightThru)
            Case Else
                Return False
        End Select
        Return True
    End Function

    Private Sub CreateTable_Weapon(ByRef tableLine As String, param As String, ByRef item As WeaponItemProto, ByRef commonItem As CommonItemProto)
        Select Case param
            Case "Min Strength"
                tableLine &= spr & item.MinST
            Case "Damage Type"
                tableLine &= spr & DmgType(item.DmgType)
            Case "Min Damage"
                tableLine &= spr & item.MinDmg
            Case "Max Damage"
                tableLine &= spr & item.MaxDmg
            Case "Attack Primary"
                tableLine &= spr & AttackTypes(commonItem.common.FlagsExt And &HF)
            Case "Attack Secondary"
                tableLine &= spr & AttackTypes((commonItem.common.FlagsExt >> 4) And &HF)
            Case "Range Primary"
                tableLine &= spr & item.MaxRangeP
            Case "Range Secondary"
                tableLine &= spr & item.MaxRangeS
            Case "AP Cost Primary"
                tableLine &= spr & item.MPCostP
            Case "AP Cost Secondary"
                tableLine &= spr & item.MPCostS
            Case "Max Ammo"
                tableLine &= spr & item.MaxAmmo
            Case "Burst Rounds"
                tableLine &= spr & item.Rounds
            Case "Caliber"
                If item.Caliber <> &HFFFFFFFF Then
                    tableLine &= spr & CaliberNAME(item.Caliber) & " [" & item.Caliber & "]"
                Else
                    tableLine &= spr
                End If
            Case "Ammo PID"
                If item.AmmoPID <> &HFFFFFFFF Then
                    tableLine &= spr & Items_LST(item.AmmoPID - 1).itemName & " [" & item.AmmoPID & "]"
                Else
                    tableLine &= spr
                End If
            Case "Critical Fail"
                tableLine &= spr & item.CritFail
            Case "Perk"
                If item.Perk <> &HFFFFFFFF Then
                    tableLine &= spr & Perk_NAME(item.Perk) & " [" & item.Perk & "]"
                Else
                    tableLine &= spr
                End If
            Case "Anim Code"
                tableLine &= spr & AnimCodes(item.AnimCode) & " [" & item.AnimCode & "]"
            Case "Big Gun [Flag]"
                tableLine &= spr & CBool((commonItem.common.FlagsExt And Enums.FlagsExt.BigGun) <> 0)
            Case "Two Hand [Flag]"
                tableLine &= spr & CBool((commonItem.common.FlagsExt And Enums.FlagsExt.TwoHand) <> 0)
            Case "Energy [Flag]"
                tableLine &= spr & CBool((commonItem.common.FlagsExt And Enums.FlagsExt.Energy) <> 0)
        End Select
    End Sub

    Private Sub CreateTable_Ammo(ByRef tableLine As String, param As String, ByRef item As AmmoItemProto)
        Select Case param
            Case "Dam Div"
                tableLine &= spr & item.DamDiv
            Case "Dam Mult"
                tableLine &= spr & item.DamMult
            Case "AC Adjust"
                tableLine &= spr & item.ACAdjust
            Case "DR Adjust"
                tableLine &= spr & item.DRAdjust
            Case "Quantity"
                tableLine &= spr & item.Quantity
            Case "Caliber"
                If item.Caliber <> -1 Then
                    tableLine &= spr & CaliberNAME(item.Caliber) & " [" & item.Caliber & "]"
                Else
                    tableLine &= spr
                End If
        End Select
    End Sub

    Private Sub CreateTable_Armor(ByRef tableLine As String, param As String, ByRef item As ArmorItemProto)
        Select Case param
            Case "Armor Class"
                tableLine &= spr & item.AC
            Case "Normal DT|DR"
                tableLine &= spr & item.DTNormal & "|" & item.DRNormal
            Case "Laser DT|DR"
                tableLine &= spr & item.DTLaser & "|" & item.DRLaser
            Case "Fire DT|DR"
                tableLine &= spr & item.DTFire & "|" & item.DRFire
            Case "Plasma DT|DR"
                tableLine &= spr & item.DTPlasma & "|" & item.DRPlasma
            Case "Electrical DT|DR"
                tableLine &= spr & item.DTElectrical & "|" & item.DRElectrical
            Case "EMP DT|DR"
                tableLine &= spr & item.DTEMP & "|" & item.DREMP
            Case "Explosion DT|DR"
                tableLine &= spr & item.DTExplode & "|" & item.DRExplode
            Case "Perk"
                If item.Perk <> -1 Then
                    tableLine &= spr & Perk_NAME(item.Perk) & " [" & item.Perk & "]"
                Else
                    tableLine &= spr
                End If
        End Select
    End Sub

    Private Sub CreateTable_Drugs(ByRef tableLine As String, param As String, ByRef item As DrugsItemProto)
        Select Case param
            Case "Modify Stat 0"
                If item.Stat0 <> &HFFFFFFFF Then
                    tableLine &= spr & DrugEffect(2 + (item.Stat0)).ToString & " [" & item.Stat0 & "]"
                Else
                    tableLine &= spr
                End If
            Case "Modify Stat 1"
                If item.Stat1 <> &HFFFFFFFF Then
                    tableLine &= spr & DrugEffect(2 + (item.Stat1)).ToString & " [" & item.Stat1 & "]"
                Else
                    tableLine &= spr
                End If
            Case "Modify Stat 2"
                If item.Stat2 <> &HFFFFFFFF Then
                    tableLine &= spr & DrugEffect(2 + (item.Stat2)).ToString & " [" & item.Stat2 & "]"
                Else
                    tableLine &= spr
                End If
            Case "Instant Amount 0"
                tableLine &= spr & item.iAmount0
            Case "Instant Amount 1"
                tableLine &= spr & item.iAmount1
            Case "Instant Amount 2"
                tableLine &= spr & item.iAmount2
            Case "First Amount 0"
                tableLine &= spr & item.fAmount0
            Case "First Amount 1"
                tableLine &= spr & item.fAmount1
            Case "First Amount 2"
                tableLine &= spr & item.fAmount2
            Case "First Duration Time"
                tableLine &= spr & item.Duration1
            Case "Second Amount 0"
                tableLine &= spr & item.fAmount0
            Case "Second Amount 1"
                tableLine &= spr & item.fAmount1
            Case "Second Amount 2"
                tableLine &= spr & item.fAmount2
            Case "Second Duration Time"
                tableLine &= spr & item.Duration2
            Case "Addiction Effect"
                If item.W_Effect <> -1 Then
                    tableLine &= spr & Perk_NAME(item.W_Effect) & " [" & item.W_Effect & "]"
                Else
                    tableLine &= spr
                End If
            Case "Addiction Onset Time"
                tableLine &= spr & item.W_Onset
            Case "Addiction Rate"
                tableLine &= spr & item.AddictionRate
        End Select
    End Sub

    Private Sub CreateTable_Misc(ByRef tableLine As String, param As String, ByRef item As MiscItemProto)
        Select Case param
            Case "Power PID"
                If item.PowerPID <> &HFFFFFFFF Then
                    tableLine &= spr & Items_LST(item.PowerPID - 1).itemName & " [" & item.PowerPID & "]"
                Else
                    tableLine &= spr
                End If
            Case "Power Type"
                If item.PowerType < UBound(CaliberNAME) Then
                    tableLine &= spr & CaliberNAME(item.PowerType)
                Else
                    tableLine &= spr
                End If
            Case "Charges"
                tableLine &= spr & item.Charges
        End Select
    End Sub

    Private Sub CreateTable_Critter(ByRef tableLine As String, param As String, ByRef critterProto As CritterProto)
        Dim critter As CritterProtoData = critterProto.data
        Select Case param
            Case "PID"
                tableLine &= spr & critterProto.common.ProtoID
            Case "FrmID"
                tableLine &= spr & critterProto.common.FrmID
            Case "Strength"
                tableLine &= spr & critter.Strength
            Case "Perception"
                tableLine &= spr & critter.Perception
            Case "Endurance"
                tableLine &= spr & critter.Endurance
            Case "Charisma"
                tableLine &= spr & critter.Charisma
            Case "Intelligence"
                tableLine &= spr & critter.Intelligence
            Case "Agility"
                tableLine &= spr & critter.Agility
            Case "Luck"
                tableLine &= spr & critter.Luck
                '
            Case "Health Point"
                tableLine &= spr & (critter.HP + critter.b_HP)
            Case "Action Point"
                tableLine &= spr & (critter.AP + critter.b_AP)
            Case "Armor Class"
                tableLine &= spr & (critter.AC + critter.b_AC)
            Case "Melee Damage"
                tableLine &= spr & (critter.MeleeDmg + critter.b_MeleeDmg)
            Case "Damage Type"
                tableLine &= spr & DmgType(critter.DamageType)
            Case "Critical Chance"
                tableLine &= spr & (critter.Critical + critter.b_Critical)
            Case "Sequence"
                tableLine &= spr & (critter.Sequence + critter.b_Sequence)
            Case "Healing Rate"
                tableLine &= spr & (critter.Healing + critter.b_Healing)
            Case "Exp Value"
                tableLine &= spr & critter.ExpVal
                '
            Case "Small Guns [Skill]"
                tableLine &= spr & CStr(CalcStats.SmallGun_Skill(critter.Agility) + critter.SmallGuns)
            Case "Big Guns [Skill]"
                tableLine &= spr & CStr(CalcStats.BigEnergyGun_Skill(critter.Agility) + critter.BigGuns)
            Case "Energy Weapons [Skill]"
                tableLine &= spr & CStr(CalcStats.BigEnergyGun_Skill(critter.Agility) + critter.EnergyGun)
            Case "Unarmed [Skill]"
                tableLine &= spr & CStr(CalcStats.Unarmed_Skill(critter.Agility, critter.Strength) + critter.Unarmed)
            Case "Melee [Skill]"
                tableLine &= spr & CStr(CalcStats.Melee_Skill(critter.Agility, critter.Strength) + critter.Melee)
            Case "Throwing [Skill]"
                tableLine &= spr & CStr(CalcStats.Throwing_Skill(critter.Agility) + critter.Throwing)
            Case "First Aid [Skill]"
                tableLine &= spr & CStr(CalcStats.FirstAid_Skill(critter.Perception, critter.Intelligence) + critter.FirstAid)
            Case "Doctor [Skill]"
                tableLine &= spr & CStr(CalcStats.Doctor_Skill(critter.Perception, critter.Intelligence) + critter.Doctor)
            Case "Sneak [Skill]"
                tableLine &= spr & CStr(CalcStats.Sneak_Skill(critter.Agility) + critter.Sneak)
            Case "Lockpick [Skill]"
                tableLine &= spr & CStr(CalcStats.Lockpick_Skill(critter.Perception, critter.Agility) + critter.Lockpick)
            Case "Steal [Skill]"
                tableLine &= spr & CStr(CalcStats.Steal_Skill(critter.Agility) + critter.Steal)
            Case "Traps [Skill]"
                tableLine &= spr & CStr(CalcStats.Trap_Skill(critter.Perception, critter.Agility) + critter.Traps)
            Case "Science [Skill]"
                tableLine &= spr & CStr(CalcStats.Science_Skill(critter.Intelligence) + critter.Science)
            Case "Repear [Skill]"
                tableLine &= spr & CStr(CalcStats.Repair_Skill(critter.Intelligence) + critter.Repair)
            Case "Speech [Skill]"
                tableLine &= spr & CStr(CalcStats.Speech_Skill(critter.Charisma) + critter.Speech)
            Case "Barter [Skill]"
                tableLine &= spr & CStr(CalcStats.Barter_Skill(critter.Charisma) + critter.Barter)
            Case "Gambling [Skill]"
                tableLine &= spr & CStr(CalcStats.Gamblings_Skill(critter.Luck) + critter.Gambling)
            Case "Outdoorsman [Skill]"
                tableLine &= spr & CStr(CalcStats.Outdoorsman_Skill(critter.Endurance, critter.Intelligence) + critter.Outdoorsman)
                '
            Case "Resistance Radiation"
                tableLine &= spr & (critter.DRRadiation + critter.b_DRRadiation)
            Case "Resistance Poison"
                tableLine &= spr & (critter.DRPoison + critter.b_DRPoison)
                '
            Case "Normal DT|DR"
                tableLine &= spr & critter.b_DTNormal & "|" & critter.b_DRNormal
            Case "Laser DT|DR"
                tableLine &= spr & critter.b_DTLaser & "|" & critter.b_DRLaser
            Case "Fire DT|DR"
                tableLine &= spr & critter.b_DTFire & "|" & critter.b_DRFire
            Case "Plasma DT|DR"
                tableLine &= spr & critter.b_DTPlasma & "|" & critter.b_DRPlasma
            Case "Electrical DT|DR"
                tableLine &= spr & critter.b_DTElectrical & "|" & critter.b_DRElectrical
            Case "EMP DT|DR"
                tableLine &= spr & critter.b_DTEMP & "|" & critter.b_DREMP
            Case "Explosion DT|DR"
                tableLine &= spr & critter.b_DTExplode & "|" & critter.b_DRExplode
                '
            Case "Base Normal DT|DR"
                tableLine &= spr & critter.DTNormal & "|" & critter.DRNormal
            Case "Base Laser DT|DR"
                tableLine &= spr & critter.DTLaser & "|" & critter.DRLaser
            Case "Base Fire DT|DR"
                tableLine &= spr & critter.DTFire & "|" & critter.DRFire
            Case "Base Plasma DT|DR"
                tableLine &= spr & critter.DTPlasma & "|" & critter.DRPlasma
            Case "Base Electrical DT|DR"
                tableLine &= spr & critter.DTElectrical & "|" & critter.DRElectrical
            Case "Base EMP DT|DR"
                tableLine &= spr & critter.DTEMP & "|" & critter.DREMP
            Case "Base Explosion DT|DR"
                tableLine &= spr & critter.DTExplode & "|" & critter.DRExplode
        End Select
    End Sub

    Friend Sub Items_ImportTable(ByVal tableFile As String)
        Dim table() As String
        Try
            table = File.ReadAllLines(tableFile, Encoding.Default)
        Catch ex As Exception
            MsgBox("Cannot open this table file!", MsgBoxStyle.Critical, "Open error")
            Exit Sub
        End Try

        Dim tableParam() As String = Split(table(0), spr)
        Dim tableValue(UBound(table) - 1, UBound(tableParam)) As String

        Dim n, m As Integer
        'разделить
        For n = 1 To UBound(table)
            Dim tLine() As String = Split(table(n), spr)
            If tLine(0) <> Nothing OrElse tLine.Length < tableParam.Length Then
                If tLine(0) <> Nothing Then
                    TableLog_Form.ListBox1.Items.Add("Skip Line: Table Line " & (n + 1) & " - Used '#' symbol in begin line.")
                Else
                    TableLog_Form.ListBox1.Items.Add("Warning: Table Line " & (n + 1) & " - Error count value parametr.")
                End If
                Continue For
            End If
            For m = 0 To UBound(tableParam)
                tableValue(n - 1, m) = tLine(m)
            Next
        Next

        If Not Directory.Exists(SaveMOD_Path & PROTO_ITEMS) Then Directory.CreateDirectory(SaveMOD_Path & PROTO_ITEMS)

        Dim item As ItemPrototype

        'открыть профайл
        For n = 0 To UBound(tableValue)
            Dim ProFile As String = tableValue(n, 1)
            If ProFile = Nothing Then Continue For

            Dim pPath = DatFiles.CheckFile(PROTO_ITEMS & ProFile)

            Dim iType As ItemType

            Select Case FileSystem.GetFileInfo(pPath).Length
                Case PrototypeSize.Misc 'Misc
                    iType = ItemType.Misc
                    item = New MiscItemObj(pPath)
                Case PrototypeSize.Weapon 'Оружие
                    iType = ItemType.Weapon
                    item = New WeaponItemObj(pPath)
                Case PrototypeSize.Drug 'Наркотик
                    iType = ItemType.Drugs
                    item = New DrugsItemObj(pPath)
                Case PrototypeSize.Ammo 'Патрон
                    iType = ItemType.Ammo
                    item = New AmmoItemObj(pPath)
                Case PrototypeSize.Armor 'Броня
                    iType = ItemType.Armor
                    item = New ArmorItemObj(pPath)
                Case Else
                    TableLog_Form.ListBox1.Items.Add("Error: PRO file '" & ProFile & "' item type does not match the file size.")
                    Continue For
            End Select

            CType(item, IPrototype).Load()

            Try
                'изменить значения
                For m = 3 To UBound(tableParam)
                    Dim param = tableParam(m).ToLowerInvariant
                    Select Case param
                        'Common
                        Case "cost"
                            item.Cost = CInt(tableValue(n, m))
                        Case "weight"
                            item.Weight = CInt(tableValue(n, m))
                        Case "size"
                            item.Size = CInt(tableValue(n, m))
                        Case "shoot thru [flag]"
                            If tableValue(n, m).Equals("true", StringComparison.OrdinalIgnoreCase) Then
                                item.Flags = item.Flags Or Flags.ShootThru
                            Else
                                item.Flags = item.Flags And (Not (Flags.ShootThru))
                            End If
                        Case "light thru [flag]"
                            If tableValue(n, m).Equals("true", StringComparison.OrdinalIgnoreCase) Then
                                item.Flags = item.Flags Or Flags.LightThru
                            Else
                                item.Flags = item.Flags And (Not (Flags.LightThru))
                            End If
                        Case Else
                            Select Case iType
                                Case ItemType.Weapon
                                    SetWeaponParams(param, tableValue(n, m), CType(item, WeaponItemObj))
                                Case ItemType.Ammo
                                    SetAmmoParams(param, tableValue(n, m), CType(item, AmmoItemObj))
                                Case ItemType.Armor
                                    SetArmorParams(param, tableValue(n, m), CType(item, ArmorItemObj))
                                Case ItemType.Drugs
                                    SetDrugsParams(param, tableValue(n, m), CType(item, DrugsItemObj))
                                Case ItemType.Misc
                                    SetMiscParams(param, tableValue(n, m), CType(item, MiscItemObj))
                            End Select
                    End Select
                Next
            Catch
                MsgBox("Error: Param " & tableParam(m).ToUpper & " PRO Line: " & tableValue(n, 0), MsgBoxStyle.Critical, "Error table import")
                Continue For
            End Try

            'save profile and goto next profile
            ProFiles.SaveItemProData(pPath, item)
        Next
        If TableLog_Form.ListBox1.Items.Count > 0 Then TableLog_Form.Show()
        MsgBox("Successfully!", MsgBoxStyle.Information, "Import table")
    End Sub

    Private Function GetTable_Param(ByRef tParam As String) As Integer
        If tParam <> Nothing Then
            Dim y As Integer = InStr(tParam, "[")
            Return Convert.ToInt32(tParam.Substring(y, tParam.Length - (y + 1)))
        End If
        Return -1
    End Function

    Private Sub SetWeaponParams(ByVal tParam As String, ByVal tValue As String, item As WeaponItemObj)
        Select Case tParam
            Case "min strength"
                item.MinST = CInt(tValue)
            Case "damage type"
                For z = 0 To UBound(DmgType)
                    If tValue.Equals(DmgType(z), StringComparison.OrdinalIgnoreCase) Then
                        item.DmgType = CType(z, WeaponItemObj.DamageType)
                        Exit For
                    End If
                Next
            Case "min damage"
                item.MinDmg = CInt(tValue)
            Case "max damage"
                item.MaxDmg = CInt(tValue)
            Case "range primary attack"
                item.MaxRangeP = CInt(tValue)
            Case "range secondary attack"
                item.MaxRangeS = CInt(tValue)
            Case "ap cost primary attack"
                item.MPCostP = CInt(tValue)
            Case "ap cost secondary attack"
                item.MPCostS = CInt(tValue)
            Case "max ammo"
                item.MaxAmmo = CInt(tValue)
            Case "rounds brust"
            Case "burst rounds"
                item.Rounds = CInt(tValue)
            Case "caliber"
                item.Caliber = GetTable_Param(tValue)
            Case "ammo pid"
                item.AmmoPID = GetTable_Param(tValue)
            Case "critical fail"
                item.CritFail = CInt(tValue)
            Case "perk"
                item.Perk = GetTable_Param(tValue)
            Case "anim code"
                item.AnimCode = GetTable_Param(tValue)
        End Select
    End Sub

    Private Sub SetAmmoParams(ByVal tParam As String, ByVal tValue As String, item As AmmoItemObj)
        Select Case tParam
            Case "dam div"
                item.DamDiv = CInt(tValue)
            Case "dam mult"
                item.DamMult = CInt(tValue)
            Case "ac adjust"
                item.ACAdjust = CInt(tValue)
            Case "dr adjust"
                item.DRAdjust = CInt(tValue)
            Case "quantity"
                item.Quantity = CInt(tValue)
            Case "caliber"
                item.Caliber = GetTable_Param(tValue)
        End Select
    End Sub

    Private Sub SetArmorParams(ByVal tParam As String, ByVal tValue As String, item As ArmorItemObj)
        Dim strSplit() As String
        Select Case tParam
            Case "armor class"
                item.AC = CInt(tValue)
            Case "normal dt|dr"
                strSplit = tValue.Split(splt)
                item.DTNormal = CInt(strSplit(0))
                item.DRNormal = CInt(strSplit(1))
            Case "laser dt|dr"
                strSplit = tValue.Split(splt)
                item.DTLaser = CInt(strSplit(0))
                item.DRLaser = CInt(strSplit(1))
            Case "fire dt|dr"
                strSplit = tValue.Split(splt)
                item.DTFire = CInt(strSplit(0))
                item.DRFire = CInt(strSplit(1))
            Case "plasma dt|dr"
                strSplit = tValue.Split(splt)
                item.DTPlasma = CInt(strSplit(0))
                item.DRPlasma = CInt(strSplit(1))
            Case "electrical dt|dr"
                strSplit = tValue.Split(splt)
                item.DTElectrical = CInt(strSplit(0))
                item.DRElectrical = CInt(strSplit(1))
            Case "emp dt|dr"
                strSplit = tValue.Split(splt)
                item.DTEMP = CInt(strSplit(0))
                item.DREMP = CInt(strSplit(1))
            Case "explosion dt|dr"
                strSplit = tValue.Split(splt)
                item.DTExplode = CInt(strSplit(0))
                item.DRExplode = CInt(strSplit(1))
            Case "perk"
                item.Perk = GetTable_Param(tValue)
        End Select
    End Sub

    Private Sub SetDrugsParams(ByVal tParam As String, ByVal tValue As String, item As DrugsItemObj)
        Select Case tParam
            Case "modify stat 0"
                item.Stat0 = GetTable_Param(tValue)
            Case "modify stat 1"
                item.Stat1 = GetTable_Param(tValue)
            Case "modify stat 2"
                item.Stat2 = GetTable_Param(tValue)
            Case "instant amount 0"
                item.iAmount0 = CInt(tValue)
            Case "instant amount 1"
                item.iAmount1 = CInt(tValue)
            Case "instant amount 2"
                item.iAmount2 = CInt(tValue)
            Case "first amount 0"
                item.fAmount0 = CInt(tValue)
            Case "first amount 1"
                item.fAmount1 = CInt(tValue)
            Case "first amount 2"
                item.fAmount2 = CInt(tValue)
            Case "first duration time"
                item.Duration1 = CInt(tValue)
            Case "second amount 0"
                item.fAmount0 = CInt(tValue)
            Case "second amount 1"
                item.fAmount1 = CInt(tValue)
            Case "second amount 2"
                item.fAmount2 = CInt(tValue)
            Case "second duration time"
                item.Duration2 = CInt(tValue)
            Case "addiction effect"
                item.W_Effect = GetTable_Param(tValue)
            Case "addiction onset time"
                item.W_Onset = CInt(tValue)
            Case "addiction rate"
                item.AddictionRate = CInt(tValue)
        End Select
    End Sub

    Private Sub SetMiscParams(ByVal tParam As String, ByVal tValue As String, item As MiscItemObj)
        Select Case tParam
            Case "power pid"
                item.PowerPID = GetTable_Param(tValue)
            Case "power type"
                If tValue <> Nothing Then
                    For z = 0 To UBound(CaliberNAME)
                        If tValue.Equals(CaliberNAME(z), StringComparison.OrdinalIgnoreCase) Then
                            item.PowerType = z
                            Exit For
                        End If
                    Next
                End If
            Case "charges"
                If tValue <> Nothing Then item.Charges = CInt(tValue)
        End Select
    End Sub

    Friend Sub Critters_ImportTable(ByVal tableFile As String)
        Dim critter As CritterProto

        Dim ProFile As String
        Dim table() As String
        Try
            table = File.ReadAllLines(tableFile, Encoding.Default)
        Catch ex As Exception
            MsgBox("Cannot open this table file!", MsgBoxStyle.Critical, "Open error")
            Exit Sub
        End Try

        Dim tableParam() As String = Split(table(0), spr)
        Dim tableValue(UBound(table) - 1, UBound(tableParam)) As String
        Dim i As Integer

        'разделить
        For n = 1 To UBound(table)
            Dim tLine() As String = Split(table(n), spr)
            If tLine(0) <> String.Empty OrElse tLine.Length < tableParam.Length Then
                If tLine(0) <> String.Empty Then
                    TableLog_Form.ListBox1.Items.Add("Skip Line #" & (n + 1) & " : Used ignore symbol '#' in table line.")
                Else
                    TableLog_Form.ListBox1.Items.Add("Warning Line #" & (n + 1) & " : Error count value param.")
                End If
                Continue For
            End If
            For m = 0 To UBound(tableParam)
                tableValue(i, m) = tLine(m)
            Next
            i += 1
        Next

        If (i > 0) Then
            If Not Directory.Exists(SaveMOD_Path & PROTO_CRITTERS) Then Directory.CreateDirectory(SaveMOD_Path & PROTO_CRITTERS)

            'Open profile
            For n = 0 To i - 1
                ProFile = tableValue(n, 1)
                If ProFile = Nothing Then Continue For

                Dim filePath = DatFiles.CheckFile(PROTO_CRITTERS & ProFile)
                If (ProFiles.LoadCritterProData(filePath, critter) = False) Then
                    Main.PrintLog("Bad Format: " & filePath)
                    Application.DoEvents()
                    Continue For
                End If

                'Changed values
                If (SetParams(critter.data, n, tableParam, tableValue) = False) Then Continue For

                'Save the PRO file and go to next one
                ProFiles.SaveCritterProData(filePath, critter)
            Next
        End If

        If TableLog_Form.ListBox1.Items.Count > 0 Then TableLog_Form.Show()
        MsgBox("Import done!", MsgBoxStyle.Information, "Import table")
    End Sub

    Private Function SetParams(ByRef critter As CritterProtoData, ByVal n As Integer, ByRef tableParam() As String, ByRef tableValue(,) As String) As Boolean
        Dim m As Integer
        Dim strSplit() As String
        Try
            'Common pass 1
            For m = 3 To UBound(tableParam)
                Select Case tableParam(m).ToLowerInvariant
                    Case "strength"
                        critter.Strength = CInt(tableValue(n, m))
                    Case "perception"
                        critter.Perception = CInt(tableValue(n, m))
                    Case "endurance"
                        critter.Endurance = CInt(tableValue(n, m))
                    Case "charisma"
                        critter.Charisma = CInt(tableValue(n, m))
                    Case "intelligence"
                        critter.Intelligence = CInt(tableValue(n, m))
                    Case "agility"
                        critter.Agility = CInt(tableValue(n, m))
                    Case "luck"
                        critter.Luck = CInt(tableValue(n, m))
                    Case "exp value"
                        critter.ExpVal = CInt(tableValue(n, m))
                    Case "damage type"
                        critter.DamageType = 0
                        For z = 0 To UBound(DmgType)
                            If (String.Equals(tableValue(n, m), DmgType(z), StringComparison.OrdinalIgnoreCase)) Then
                                critter.DamageType = z
                                Exit For
                            End If
                        Next
                    'Armor
                    Case "normal dt|dr"
                        strSplit = tableValue(n, m).Split(splt)
                        critter.b_DTNormal = CInt(strSplit(0))
                        critter.b_DRNormal = CInt(strSplit(1))
                    Case "laser dt|dr"
                        strSplit = tableValue(n, m).Split(splt)
                        critter.b_DTLaser = CInt(strSplit(0))
                        critter.b_DRLaser = CInt(strSplit(1))
                    Case "fire dt|dr"
                        strSplit = tableValue(n, m).Split(splt)
                        critter.b_DTFire = CInt(strSplit(0))
                        critter.b_DRFire = CInt(strSplit(1))
                    Case "plasma dt|dr"
                        strSplit = tableValue(n, m).Split(splt)
                        critter.b_DTPlasma = CInt(strSplit(0))
                        critter.b_DRPlasma = CInt(strSplit(1))
                    Case "electrical dt|dr"
                        strSplit = tableValue(n, m).Split(splt)
                        critter.b_DTElectrical = CInt(strSplit(0))
                        critter.b_DRElectrical = CInt(strSplit(1))
                    Case "emp dt|dr"
                        strSplit = tableValue(n, m).Split(splt)
                        critter.b_DTEMP = CInt(strSplit(0))
                        critter.b_DREMP = CInt(strSplit(1))
                    Case "explosion dt|dr"
                        strSplit = tableValue(n, m).Split(splt)
                        critter.b_DTExplode = CInt(strSplit(0))
                        critter.b_DRExplode = CInt(strSplit(1))
                            '
                    Case "base normal dt|dr"
                        strSplit = tableValue(n, m).Split(splt)
                        critter.DTNormal = CInt(strSplit(0))
                        critter.DRNormal = CInt(strSplit(1))
                    Case "base laser dt|dr"
                        strSplit = tableValue(n, m).Split(splt)
                        critter.DTLaser = CInt(strSplit(0))
                        critter.DRLaser = CInt(strSplit(1))
                    Case "base fire dt|dr"
                        strSplit = tableValue(n, m).Split(splt)
                        critter.DTFire = CInt(strSplit(0))
                        critter.DRFire = CInt(strSplit(1))
                    Case "base plasma dt|dr"
                        strSplit = tableValue(n, m).Split(splt)
                        critter.DTPlasma = CInt(strSplit(0))
                        critter.DRPlasma = CInt(strSplit(1))
                    Case "base electrical dt|dr"
                        strSplit = tableValue(n, m).Split(splt)
                        critter.DTElectrical = CInt(strSplit(0))
                        critter.DRElectrical = CInt(strSplit(1))
                    Case "base emp dt|dr"
                        strSplit = tableValue(n, m).Split(splt)
                        critter.DTEMP = CInt(strSplit(0))
                        critter.DREMP = CInt(strSplit(1))
                    Case "base explosion dt|dr"
                        strSplit = tableValue(n, m).Split(splt)
                        critter.DTExplode = CInt(strSplit(0))
                        critter.DRExplode = CInt(strSplit(1))
                End Select
            Next
            'Calculate pass 2
            For m = 3 To UBound(tableParam)
                Select Case tableParam(m).ToLowerInvariant
                    Case "action point"
                        critter.AP = CalcStats.Action_Point(critter.Agility)
                        critter.b_AP = CInt(tableValue(n, m)) - critter.AP
                    Case "armor class"
                        critter.AC = critter.Agility
                        critter.b_AC = CInt(tableValue(n, m)) - critter.Agility
                    Case "health point"
                        critter.HP = CalcStats.Health_Point(critter.Strength, critter.Endurance)
                        critter.b_HP = CInt(tableValue(n, m)) - critter.HP
                    Case "healing rate"
                        critter.Healing = CalcStats.Healing_Rate(critter.Endurance)
                        critter.b_Healing = CInt(tableValue(n, m)) - critter.Healing
                    Case "melee damage"
                        critter.MeleeDmg = CalcStats.Melee_Damage(critter.Strength)
                        critter.b_MeleeDmg = CInt(tableValue(n, m)) - critter.MeleeDmg
                    Case "critical chance"
                        critter.Critical = critter.Luck
                        critter.b_Critical = CInt(tableValue(n, m)) - critter.Luck
                    Case "sequence"
                        critter.Sequence = CalcStats.Sequence(critter.Perception)
                        critter.b_Sequence = CInt(tableValue(n, m)) - critter.Sequence
                    Case "resistance radiation"
                        critter.DRRadiation = CalcStats.Radiation(critter.Endurance)
                        critter.b_DRRadiation = CInt(tableValue(n, m)) - critter.DRRadiation
                    Case "resistance poison"
                        critter.DRPoison = CalcStats.Poison(critter.Endurance)
                        critter.b_DRPoison = CInt(tableValue(n, m)) - critter.DRPoison
                    'Skill
                    Case "small guns [skill]"
                        critter.SmallGuns = CInt(tableValue(n, m)) - CalcStats.SmallGun_Skill(critter.Agility)
                    Case "big guns [skill]"
                        critter.BigGuns = CInt(tableValue(n, m)) - CalcStats.BigEnergyGun_Skill(critter.Agility)
                    Case "energy weapons [skill]"
                        critter.EnergyGun = CInt(tableValue(n, m)) - CalcStats.BigEnergyGun_Skill(critter.Agility)
                    Case "unarmed [skill]"
                        critter.Unarmed = CInt(tableValue(n, m)) - CalcStats.Unarmed_Skill(critter.Agility, critter.Strength)
                    Case "melee [skill]"
                        critter.Melee = CInt(tableValue(n, m)) - CalcStats.Melee_Skill(critter.Agility, critter.Strength)
                    Case "throwing [skill]"
                        critter.Throwing = CInt(tableValue(n, m)) - CalcStats.Throwing_Skill(critter.Agility)
                    Case "first aid [skill]"
                        critter.Throwing = CInt(tableValue(n, m)) - CalcStats.FirstAid_Skill(critter.Perception, critter.Intelligence)
                    Case "doctor [skill]"
                        critter.Throwing = CInt(tableValue(n, m)) - CalcStats.Doctor_Skill(critter.Perception, critter.Intelligence)
                    Case "sneak [skill]"
                        critter.Throwing = CInt(tableValue(n, m)) - CalcStats.Sneak_Skill(critter.Agility)
                    Case "lockpick [skill]"
                        critter.Throwing = CInt(tableValue(n, m)) - CalcStats.Lockpick_Skill(critter.Perception, critter.Agility)
                    Case "steal [skill]"
                        critter.Throwing = CInt(tableValue(n, m)) - CalcStats.Steal_Skill(critter.Agility)
                    Case "traps [skill]"
                        critter.Throwing = CInt(tableValue(n, m)) - CalcStats.Trap_Skill(critter.Perception, critter.Agility)
                    Case "science [skill]"
                        critter.Throwing = CInt(tableValue(n, m)) - CalcStats.Science_Skill(critter.Intelligence)
                    Case "repair [skill]"
                        critter.Throwing = CInt(tableValue(n, m)) - CalcStats.Repair_Skill(critter.Intelligence)
                    Case "speech [skill]"
                        critter.Throwing = CInt(tableValue(n, m)) - CalcStats.Speech_Skill(critter.Charisma)
                    Case "barter [skill]"
                        critter.Throwing = CInt(tableValue(n, m)) - CalcStats.Barter_Skill(critter.Charisma)
                    Case "gambling [skill]"
                        critter.Throwing = CInt(tableValue(n, m)) - CalcStats.Gamblings_Skill(critter.Luck)
                    Case "outdoorsman [skill]"
                        critter.Throwing = CInt(tableValue(n, m)) - CalcStats.Outdoorsman_Skill(critter.Endurance, critter.Intelligence)
                End Select
            Next
        Catch
            MsgBox("Error: Param " & tableParam(m).ToUpper & " PRO: " & tableValue(n, 1), MsgBoxStyle.Critical, "Import Error")
            TableLog_Form.ListBox1.Items.Add("Error pro line: " & tableValue(n, 1) & " : in value param (" & tableParam(m) & ")")
            Return False
        End Try
        Return True
    End Function

    Private Sub CheckAllToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CheckAllToolStripMenuItem.Click
        CheckedItemsAll(True)
    End Sub

    Private Sub DeselecAllToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles DeselecAllToolStripMenuItem.Click
        CheckedItemsAll(False)
    End Sub

    Private Sub CheckedItemsAll(ByVal value As Boolean)
        Dim control As CheckedListBox = GetCheckList()

        For n = 0 To control.Items.Count - 1
            control.SetItemChecked(n, value)
        Next
    End Sub

    Private Sub Create_Button(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        CheckedList = GetCheckList.CheckedItems
        CreateTable()
    End Sub

    Private Function GetCheckList() As CheckedListBox
        Select Case TabControl1.SelectedIndex
            Case TabType.Critter
                Return CheckedListBox6
            Case TabType.Weapon
                Return CheckedListBox1
            Case TabType.Ammo
                Return CheckedListBox2
            Case TabType.Armor
                Return CheckedListBox3
            Case TabType.Drugs
                Return CheckedListBox4
            Case Else 'misc
                Return CheckedListBox5
        End Select
    End Function
End Class