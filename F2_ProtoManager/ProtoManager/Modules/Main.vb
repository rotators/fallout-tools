Imports System.IO
Imports System.Drawing
Imports System.Net

Imports Prototypes
Imports Enums

Friend Module Main

    Structure CrittersLst
        Friend proFile As String
        Friend crtName As String
        Friend crtHP As Integer
        Friend PID As Integer
        Friend FID As Integer
        Friend formatF1 As Boolean
    End Structure

    Structure ItemsLst
        Friend proFile As String
        Friend itemType As ItemType
        Friend itemName As String
        Friend PID As Integer
    End Structure

    Friend Critter_LST() As CrittersLst
    Friend Items_LST() As ItemsLst

    Friend Critters_FRM() As String
    Friend Items_FRM() As String
    Friend Iven_FRM() As String

    Private Misc_LST() As String
    Friend Misc_NAME() As String

    Friend Scripts_Lst() As String

    Friend AmmoPID() As Integer
    Friend AmmoNAME() As String
    Friend CaliberNAME() As String
    Friend Perk_NAME() As String

    Private Teams As List(Of String) = New List(Of String)
    Friend PacketAI As SortedList(Of String, Integer)

    Friend Declare Function SetParent Lib "user32" (ByVal hWndChild As Integer, ByVal hWndNewParent As Integer) As Integer

    'Initialization...
    Friend Sub Main()
        DatFiles.OpenDatFiles()

        Dim clear = Settings.cCache OrElse Not (File.Exists(Cache_Patch & "\cache.id"))
        If (clear AndAlso Not Setting_Form.firstRun) Then
            Settings.Clear_Cache()
        End If

        Dim list() As String = File.ReadAllLines(DatFiles.CheckFile(itemsLstPath))
        Dim listCount = UBound(list)
        ReDim Items_LST(listCount)
        Dim pLST(listCount) As String

        Dim proCount = 0
        For n = 0 To listCount
            list(n) = list(n).Trim
            If (list(n).Length > 0) Then
                Items_LST(proCount).proFile = list(n)
                If (clear) Then pLST(proCount) = "proto\items\" & list(n)
                proCount += 1
            End If
        Next
        If (proCount - 1 <> listCount) Then ReDim Preserve Items_LST(proCount - 1)

        list = File.ReadAllLines(DatFiles.CheckFile(crittersLstPath))
        listCount = UBound(list)
        ReDim Critter_LST(listCount)

        If (clear) Then ReDim Preserve pLST(proCount + listCount)
        Dim count = 0
        For n = 0 To listCount
            list(n) = list(n).Trim
            If (list(n).Length > 0) Then
                Critter_LST(count).proFile = list(n)
                If (clear) Then
                    pLST(proCount) = "proto\critters\" & list(n)
                    proCount += 1
                End If
                count += 1
            End If
        Next
        count -= 1
        If (count <> listCount) Then
            ReDim Preserve Critter_LST(count)
            If (clear) Then ReDim Preserve pLST(proCount - 1)
        End If

        If (clear) Then
            SplashScreen.ProgressBar1.Value += 20
            SplashScreen.Label1.Text = "Loading: Extraction Pro-files..."
            Application.DoEvents()

            DatFiles.UnpackedFilesByList(pLST, Game_Path & MasterDAT, "cache\")

            SplashScreen.ProgressBar1.Value += 35
            Application.DoEvents()

            For i = DatFiles.extraMods.Count - 1 To 0 Step -1
                If (DatFiles.extraMods(i).isDat AndAlso DatFiles.extraMods(i).isEnabled) Then
                    DatFiles.UnpackedFilesByList(pLST, DatFiles.extraMods(i).filePath, "cache\")
                End If
            Next
            SplashScreen.ProgressBar1.Value += 35
            Application.DoEvents()
        End If

        File.Create(Cache_Patch & "\cache.id").Close()

        GetCrittersLstFRM()
        CreateItemsList(ItemType.Unknown)

        SplashScreen.ProgressBar1.Value = 100
        Application.DoEvents()
        Main_Form.Show()

        If Setting_Form.firstRun Then
            Setting_Form.firstRun = False
            AboutBox.ShowDialog()
        End If
    End Sub

    Friend Sub GetScriptLst()
        If Scripts_Lst IsNot Nothing Then Return

        Dim splt() As String

        Scripts_Lst = File.ReadAllLines(DatFiles.CheckFile(scriptsLstPath), System.Text.Encoding.Default)
        For n As Integer = 0 To UBound(Scripts_Lst)
            Dim last = Scripts_Lst(n).LastIndexOf("#"c)
            If (last > 0) Then Scripts_Lst(n) = Scripts_Lst(n).Remove(last)
            splt = Scripts_Lst(n).Split(";"c)
            If (splt.Length > 1) Then
                Dim tmp As String = String.Format("[{0}]", (n + 1))
                Scripts_Lst(n) = String.Format("{0} {1} - {2}", splt(0).TrimEnd.PadRight(14), tmp.PadLeft(9), splt(1).Trim)
            End If
        Next
    End Sub

    Private Function GetCrittersLstFRM() As Boolean
        Dim critterLstFile = DatFiles.CheckFile(artCrittersLstPath, , True)
        If (File.Exists(critterLstFile) = False) Then
            MsgBox("Cannot open required file: \art\critter\critter.lst", MsgBoxStyle.Critical, "File Missing")
            Return True
        End If
        Critters_FRM = Misc.ClearEmptyLines(File.ReadAllLines(critterLstFile))

        For i As Integer = 0 To Critters_FRM.Count - 1
            Dim frm As String = Critters_FRM(i)
            Dim z As Integer = frm.IndexOf(","c)
            If z > 0 Then frm = frm.Remove(z)
            Critters_FRM(i) = frm.ToUpper
        Next

        Return False
    End Function

    Friend Sub GetItemsLstFRM()
        If Items_FRM Is Nothing Then
            Items_FRM = Misc.ClearEmptyLines(File.ReadAllLines(DatFiles.CheckFile(artItemsLstPath)))
        End If

        If Iven_FRM Is Nothing Then
            Iven_FRM = Misc.ClearEmptyLines(File.ReadAllLines(DatFiles.CheckFile(artInvenLstPath)))
        End If
    End Sub

    Friend Sub GetItemsLst()
        Dim list = File.ReadAllLines(DatFiles.CheckFile(itemsLstPath))
        Dim listCount = UBound(list)
        ReDim Items_LST(listCount)

        Dim count = 0
        For n = 0 To listCount
            Dim proFile = list(n).Trim
            If (proFile.Length > 0) Then
                Items_LST(count).proFile = proFile
                count += 1
            End If
        Next
        count -= 1
        If (count <> listCount) Then Array.Resize(Items_LST, count)
    End Sub

    Friend Sub GetCrittersLst()
        Dim lst = File.ReadAllLines(DatFiles.CheckFile(crittersLstPath))
        Dim listCount = UBound(lst)
        ReDim Critter_LST(listCount)

        Dim count = 0
        For n = 0 To UBound(lst)
            Dim proFile = lst(n).Trim
            If (proFile.Length > 0) Then
                Critter_LST(count).proFile = proFile
                count += 1
            End If
        Next
        count -= 1
        If (count <> listCount) Then Array.Resize(Critter_LST, count)
    End Sub

    Friend Sub CreateCritterList()
        Dim cCount As Integer = UBound(Critter_LST)

        Progress_Form.ShowProgressBar(CInt(cCount / 2))

        With Main_Form
            .ListView1.BeginUpdate()
            .ListView1.Items.Clear()

            Dim showFID As Boolean = .ShowFIDToolStripMenuItem.Checked
            If showFID Then
                If .ListView1.Columns.Count < 5 Then .ListView1.Columns.Add("FID", 65, HorizontalAlignment.Center)
            Else
                If .ListView1.Columns.Count > 4 Then .ListView1.Columns.RemoveAt(4)
            End If

            Messages.GetMsgData("pro_crit.msg")

            For n = 0 To cCount
                Critter_LST(n).crtName = Messages.GetNameObject(ProFiles.GetProCritterDataIDs(Critter_LST(n)))
                If Critter_LST(n).crtName = String.Empty Then Critter_LST(n).crtName = "<NoName>"

                Dim attrLabel As String = String.Empty
                Dim proAttr As Status = ProtoCheckFile(Critter_LST(n).proFile, 416, attrLabel)

                If (Critter_LST(n).formatF1) Then
                    attrLabel += If(attrLabel = "", "[F1]", " [F1]")
                End If

                If showFID Then
                    .ListView1.Items.Add(New ListViewItem({Critter_LST(n).crtName, Critter_LST(n).proFile, attrLabel, Critter_LST(n).PID.ToString, Critter_LST(n).FID.ToString}))
                Else
                    .ListView1.Items.Add(New ListViewItem({Critter_LST(n).crtName, Critter_LST(n).proFile, attrLabel, Critter_LST(n).PID.ToString}))
                End If
                .ListView1.Items(n).Tag = n 'запись индекса(pid) криттера в critters.lst

                If proAttr = Status.IsModFolder Then
                    .ListView1.Items(n).ForeColor = Color.DarkBlue
                ElseIf proAttr = Status.IsBadFile Then
                    .ListView1.Items(n).ForeColor = Color.Red
                ElseIf proAttr = Status.NotExist Then
                    .ListView1.Items(n).ForeColor = Color.DarkGray
                End If

                If ((n Mod 2) <> 0) Then Progress_Form.ProgressIncrement()
            Next
            .ListView1.EndUpdate()
        End With

        Progress_Form.Close()
    End Sub

    Friend Sub CreateItemsList(ByVal filter As Integer)
        Dim nameList As List(Of String) = New List(Of String)
        Dim pidList As List(Of Integer) = New List(Of Integer)
        Dim n As Integer
        Dim x As Integer

        Dim itemProCount = UBound(Items_LST)

        Progress_Form.ShowProgressBar(CInt(itemProCount / 2))

        Messages.GetMsgData("pro_item.msg")

        With Main_Form
            .ListView2.BeginUpdate()
            .ListView2.Items.Clear()

            Dim showPID As Boolean = IsShowPID()
            For n = 0 To itemProCount
                Items_LST(n).itemName = Messages.GetNameObject(ProFiles.GetProItemsDataIDs(Items_LST(n).proFile, n))
                If Items_LST(n).itemName = String.Empty Then Items_LST(n).itemName = "<NoName>"

                Dim attrLabel As String = String.Empty
                Dim proAttr As Status = ProtoCheckFile(Items_LST(n).proFile, Prototypes.GetSizeProByType(Items_LST(n).itemType), attrLabel)

                If filter <> ItemType.Unknown Then
                    If Items_LST(n).itemType = filter OrElse (filter = ItemType.Misc And Items_LST(n).itemType = ItemType.Key) Then
                        CreateListItem(n, attrLabel, showPID)
                        .ListView2.Items(x).Tag = n 'указатель индекса(pid) итема в item.lst
                        If proAttr = Status.IsModFolder Then
                            .ListView2.Items(x).ForeColor = Color.DarkBlue
                        ElseIf proAttr = Status.IsBadFile Then
                            .ListView2.Items(x).ForeColor = Color.Red
                        End If
                        x += 1
                    End If
                Else
                    CreateListItem(n, attrLabel, showPID)
                    .ListView2.Items(n).Tag = n 'запись индекса(pid) итема из item.lst

                    If proAttr = Status.IsModFolder Then
                        .ListView2.Items(n).ForeColor = Color.DarkBlue
                    ElseIf proAttr = Status.IsBadFile Then
                        .ListView2.Items(n).ForeColor = Color.Red
                    ElseIf proAttr = Status.NotExist Then
                        .ListView2.Items(n).ForeColor = Color.DarkGray
                    End If
                End If

                If Items_LST(n).itemType = ItemType.Ammo Then
                    nameList.Add(Items_LST(n).itemName)
                    pidList.Add(n + 1)
                End If
                If ((n Mod 2) <> 0) Then Progress_Form.ProgressIncrement()
            Next
            AmmoNAME = nameList.ToArray
            AmmoPID = pidList.ToArray

            .ListView2.Visible = True
            .ListView2.EndUpdate()
        End With

        Progress_Form.Close()
    End Sub

    Friend Sub GetItemsData()
        If Misc_NAME IsNot Nothing Then Return

        Misc_LST = Misc.ClearEmptyLines(File.ReadAllLines(DatFiles.CheckFile(miscLstPath)))
        Messages.GetMsgData("pro_misc.msg")
        ReDim Misc_NAME(UBound(Misc_LST))
        For n As Integer = 0 To UBound(Misc_LST)
            Misc_NAME(n) = Messages.GetNameObject((n + 1) * 100)
        Next

        Dim cList As SortedList(Of Integer, String) = New SortedList(Of Integer, String)

        Messages.GetMsgData("proto.msg")
        Dim i As Integer = Messages.GetMSGLine(300)
        If i = -1 Then i = Integer.MaxValue
        For n As Integer = i To UBound(MSG_DATATEXT)
            If MSG_DATATEXT(n).StartsWith("{") Then
                Dim msgLine As Integer = Convert.ToInt32(Val(Messages.GetParamMsg(MSG_DATATEXT(n))))
                If msgLine >= 350 Then Exit For
                cList.Add(msgLine, Messages.GetParamMsg(MSG_DATATEXT(n), True))
            End If
        Next
        CaliberNAME = cList.Values.ToArray()
        cList.Clear()

        Messages.GetMsgData("perk.msg")
        For Each line In MSG_DATATEXT
            If line.StartsWith("{") Then
                Dim msgLine As Integer = Convert.ToInt32(Val(GetParamMsg(line)))
                If msgLine > 100 Then
                    If msgLine = 1101 Then Exit For
                    cList.Add(msgLine, Messages.GetParamMsg(line, True))
                End If
            End If
        Next
        Perk_NAME = cList.Values.ToArray
    End Sub

    Friend Sub FilterCreateItemsList(ByVal filter As Integer)
        Dim x As Integer

        With Main_Form
            .ListView2.BeginUpdate()
            .ListView2.Items.Clear()

            Dim showPID As Boolean = IsShowPID()
            For n As Integer = 0 To UBound(Items_LST)

                Dim attrLabel As String = String.Empty
                Dim proAttr As Status = ProtoCheckFile(Items_LST(n).proFile, Prototypes.GetSizeProByType(Items_LST(n).itemType), attrLabel)
                If (attrLabel = String.Empty AndAlso proAttr = Status.IsModFolder) Then attrLabel = "*" '???

                If filter <> ItemType.Unknown Then
                    If Items_LST(n).itemType = filter OrElse (filter = ItemType.Misc And Items_LST(n).itemType = ItemType.Key) Then
                        CreateListItem(n, attrLabel, showPID)
                        .ListView2.Items(x).Tag = n 'указатель индекса(pid) итема в item.lst
                        If proAttr = Status.IsModFolder Then
                            .ListView2.Items(x).ForeColor = Color.DarkBlue
                        ElseIf proAttr = Status.IsBadFile Then
                            .ListView2.Items(x).ForeColor = Color.Red
                        End If
                        x += 1
                    End If
                Else
                    CreateListItem(n, attrLabel, showPID)
                    .ListView2.Items(n).Tag = n 'указатель индекса(pid) итема в item.lst
                    If proAttr = Status.IsModFolder Then
                        .ListView2.Items(n).ForeColor = Color.DarkBlue
                    ElseIf proAttr = Status.IsBadFile Then
                        .ListView2.Items(n).ForeColor = Color.Red
                    End If
                End If
            Next
            .ListView2.EndUpdate()
        End With
    End Sub

    Friend Function IsShowPID() As Boolean
        Dim result As Boolean = Main_Form.ShowPIDToolStripMenuItem.Checked
        If result Then
            If Main_Form.ListView2.Columns.Count < 5 Then Main_Form.ListView2.Columns.Add("PID", 65, HorizontalAlignment.Center)
        Else
            If Main_Form.ListView2.Columns.Count > 4 Then Main_Form.ListView2.Columns.RemoveAt(4)
        End If
        Return result
    End Function

    Friend Sub CreateListItem(ByVal n As Integer, ByVal rOnly As String, ByVal showPID As Boolean)
        If showPID Then
            Dim pid As String = Items_LST(n).PID.ToString.PadLeft(8, "0"c)
            Main_Form.ListView2.Items.Add(New ListViewItem({Items_LST(n).itemName, Items_LST(n).proFile, ItemTypesName(Items_LST(n).itemType), rOnly, pid}))
        Else
            Main_Form.ListView2.Items.Add(New ListViewItem({Items_LST(n).itemName, Items_LST(n).proFile, ItemTypesName(Items_LST(n).itemType), rOnly}))
        End If
    End Sub

    'Поиск индекса предмета в списке ListView
    Friend Function LW_SearhItemIndex(ByVal indx As Integer, ByVal LW As ListView) As Integer
        For Each Item As ListViewItem In LW.Items
            If CInt(Item.Tag) = indx Then
                Return Item.Index
            End If
        Next

        Return Nothing
    End Function

    'Cоздает и открывает новую форму для редактирования криттера
    Friend Sub Create_CritterForm(ByVal cLST_Index As Integer)
        'Check...
        If GetCrittersLstFRM() Then Return
        GetScriptLst()
        GetTeams()
        If PacketAI Is Nothing OrElse PacketAI.Count = 0 Then PacketAI = AI.GetAllAIPacketNumber(DatFiles.CheckFile(AI.AIFILE))

        Dim CrttFrm As New Critter_Form(cLST_Index)
        With CrttFrm
            .ComboBox1.Items.AddRange(Critters_FRM)
            .ComboBox2.Items.AddRange(PacketAI.Keys.ToArray)
            .ComboBox3.Items.AddRange(Teams.ToArray)
            .ComboBox9.Items.AddRange(Scripts_Lst)
            If .LoadProData() Then
                .Dispose()
            Else
                SetParent(.Handle.ToInt32, Main_Form.SplitContainer1.Handle.ToInt32)
                .Show()
            End If
        End With
    End Sub

    'Cоздает и открывает новую форму для редактирования предметов
    Friend Sub Create_ItemsForm(ByVal iLST_Index As Integer)
        If Items_LST(iLST_Index).itemType >= ItemType.Unknown Then
            MsgBox("This object has an unknown type." & vbLf & "The prototype has not the correct format or the file is corrupted.", MsgBoxStyle.Critical, "Error Item Type")
            Exit Sub
        End If

        If (Items_LST(iLST_Index).itemType = ItemType.Armor) AndAlso GetCrittersLstFRM() Then Return
        GetItemsData()
        GetItemsLstFRM()
        GetScriptLst()

        Dim ItmsFrm As New Items_Form(iLST_Index)
        SetParent(ItmsFrm.Handle.ToInt32, Main_Form.SplitContainer1.Handle.ToInt32)
        ItmsFrm.IniItemsForm()
    End Sub

    Friend Sub Create_TxtEditForm(ByVal lwIndex As Integer, ByVal type As ProType)
        Dim TxtFrm As New TxtEdit_Form(lwIndex, type)

        SetParent(TxtFrm.Handle.ToInt32, Main_Form.SplitContainer1.Handle.ToInt32)
        If type = ProType.Critter Then
            TxtFrm.Text &= Critter_LST(lwIndex).proFile
        Else
            TxtFrm.Text &= Items_LST(lwIndex).proFile
        End If

        TxtFrm.Text &= "]"
        TxtFrm.Init_Data()
    End Sub

    Friend Sub CreateAIEditForm(Optional ByRef aiPacket As Integer = -1)
        Dim aiEditForm As New AI_Form(aiPacket)
    End Sub

    Private Sub GetTeams()
        If Teams.Count > 0 Then Return

        Dim tData As String() = File.ReadAllLines(WorkAppDIR & "\teams.h")
        For Each t In tData
            Dim line As String = t.Trim("/"c, " "c)
            If line.ToLower.StartsWith("#define ") Then
                Dim fSpace As Integer = line.IndexOf(" ", 9)
                If fSpace <= 0 Then Continue For
                Dim tName As String = line.Remove(fSpace).Remove(0, 8)
                fSpace = line.IndexOf("(", fSpace) + 1
                If fSpace <= 0 Then Continue For
                Dim tNum As String = line.Substring(fSpace, line.LastIndexOf(")") - fSpace).Trim
                Teams.Add(String.Format("{0} ({1})", Strings.RSet(tNum, 3), tName))
            End If
        Next
        Teams.Sort()
    End Sub

    Friend Sub PrintLog(ByVal textLog As String, Optional ByVal newLine As Boolean = True)
        If newLine Then
            Main_Form.TextBox1.AppendText(Environment.NewLine & textLog)
        Else
            Main_Form.TextBox1.AppendText(textLog)
        End If
        On Error Resume Next ' fix for wine (Lunix)
        Main_Form.TextBox1.ScrollToCaret()
        'On Error GoTo -1
    End Sub

End Module
