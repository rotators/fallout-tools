Imports System.IO
Imports Microsoft.VisualBasic.FileIO
Imports System.Text
Imports System.Windows
Imports System.Drawing

Imports Prototypes
Imports Enums

Friend Class Main_Form

    'Private ClickXClose As Boolean = False
    'Private Const SC_CLOSE As Int32 = &HF060
    '
    'Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
    ' Нажата кнопка Close.
    '    If m.WParam.ToInt32() = SC_CLOSE Then ClickXClose = True
    '    MyBase.WndProc(m)
    'End Sub

    Friend Sub SetFormSettings()
        DontHoverSelectToolStripMenuItem.Checked = Not (Settings.HoverSelect)
        If Not (Settings.HoverSelect) Then SetListViewHoverSelect()
        Cp866ToolStripMenuItem.Checked = Not (Settings.txtWin) And Not (Settings.txtLvCp)
        ClearToolStripMenuItem2.Checked = Settings.cArtCache
        ShowFIDToolStripMenuItem.Checked = Settings.ShowCritterFid
        ShowPIDToolStripMenuItem.Checked = Settings.ShowItemPid
        AttrReadOnlyToolStripMenuItem.Checked = Settings.proRO
    End Sub

    Private Sub Main_Form_Shown(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Shown
        Me.Text &= String.Format("{0}.{1}.{2} - by {3}", My.Application.Info.Version.Major, My.Application.Info.Version.Minor, My.Application.Info.Version.Build, My.Application.Info.Copyright)
        LinkLabel1.Text = Settings.GameDATA_Path
        LinkLabel2.Text = Settings.SaveMOD_Path
        LinkLabel3.Text = Path.GetFullPath(Settings.Game_Path + DatFiles.MasterDAT)
        SetFormSettings()
    End Sub

    Private Sub Timer1_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles Timer1.Tick
        Timer1.Stop()
        SplashScreen.Close()
        Me.Focus()
    End Sub

    Private Sub Main_Form_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Settings.Clear_Sound_Cache()

        TabControl1.Visible = True
        SplitContainer1.SplitterDistance = If(SplitSize = -1, Me.Width - 350, SplitSize)
        Settings.SetDoubleBuffered(ListView1)
        Settings.SetDoubleBuffered(ListView2)
        '
        For i As Integer = 0 To ListView2.Columns.Count - 1
            If (ColumnItemSize(i) > 15) Then ListView2.Columns(i).Width = ColumnItemSize(i)
        Next
        For i As Integer = 0 To ListView1.Columns.Count - 1
            If ColumnCritterSize(0) > 15 Then ListView1.Columns(i).Width = ColumnCritterSize(i)
        Next

        If CalcStats.GetFormula() = CalcStats.FormulaType.Fallout1 Then
            Fallout2ToolStripMenuItem.Checked = False
            Fallout1ToolStripMenuItem.Checked = True
        End If

        Timer1.Start()
    End Sub

    Private Sub Main_Form_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
        If e.CloseReason = CloseReason.UserClosing Then
            Settings.SaveConfigFile()
            Application.Exit()
            ThumbnailImage.Dispose()
            If cCache Then
                Settings.Clear_Cache()
            ElseIf cArtCache Then
                Settings.Clear_Art_Cache()
            End If
            Settings.Clear_Sound_Cache()
            If Directory.Exists("art\") Then Directory.Delete("art\", True)
        End If
    End Sub

    Private Sub AddCritterPro()
        Dim pCont As Integer = ListView1.Items.Count
        Dim pName As String = StrDup(8 - (pCont + 1).ToString.Length, "0") & (pCont + 1).ToString & ".pro"
        Dim ffile As Integer = FreeFile()

        FileOpen(ffile, "template", OpenMode.Binary, OpenAccess.Write, OpenShare.LockWrite)
        FilePut(ffile, ReverseBytes(&H1000001I + pCont))
        FilePut(ffile, ReverseBytes((pCont + 1) * 100))
        FileClose(ffile)

        ProFiles.CreateProFile(PROTO_CRITTERS, pName)

        Array.Resize(Critter_LST, pCont + 1)
        Critter_LST(pCont).proFile = pName
        Dim lst(pCont) As String
        For n = 0 To pCont
            lst(n) = Critter_LST(n).proFile
        Next
        File.WriteAllLines(SaveMOD_Path & crittersLstPath, lst)

        'Log
        Main.PrintLog("Update: " & SaveMOD_Path & crittersLstPath)

        ListView1.BeginUpdate()
        ListView1.ListViewItemSorter = Nothing
        ListView1.Items.Add("<NoName>")
        ListView1.Items(pCont).SubItems.Add(pName)
        ListView1.Items(pCont).SubItems.Add("N/A")
        ListView1.Items(pCont).SubItems.Add((&H1000001 + pCont).ToString)
        ListView1.Items(pCont).Tag = pCont
        ListView1.Items(pCont).Selected = True
        ListView1.EnsureVisible(pCont)
        ListView1.EndUpdate()

        Main.Create_CritterForm(pCont)
    End Sub

    Private Sub AddItemPro(ByVal iType As ItemType)
        Dim pCont As Integer = Items_LST.Length
        Dim pName As String = StrDup(8 - (pCont + 1).ToString.Length, "0") & (pCont + 1).ToString & ".pro"
        Dim ffile As Integer = FreeFile()

        FileOpen(ffile, "template", OpenMode.Binary, OpenAccess.Write, OpenShare.LockWrite)
        FilePut(ffile, ReverseBytes(&H1I + pCont))
        FilePut(ffile, ReverseBytes((pCont + 1) * 100))
        FileClose(ffile)

        ProFiles.CreateProFile(PROTO_ITEMS, pName)

        Array.Resize(Items_LST, pCont + 1)
        Items_LST(pCont).proFile = pName
        Items_LST(pCont).itemType = iType

        'save to lst file
        Dim lst(pCont) As String
        For n = 0 To pCont
            lst(n) = Items_LST(n).proFile
        Next
        File.WriteAllLines(SaveMOD_Path & itemsLstPath, lst)

        'Log
        Main.PrintLog("Update: " & SaveMOD_Path & itemsLstPath)

        ListView2.BeginUpdate()
        ListView2.ListViewItemSorter = Nothing
        ListView2.Items.Add("<No Name>")
        ListView2.Items(ListView2.Items.Count - 1).SubItems.Add(pName)
        ListView2.Items(ListView2.Items.Count - 1).SubItems.Add(ItemTypesName(Items_LST(pCont).itemType))
        ListView2.Items(ListView2.Items.Count - 1).SubItems.Add("N/A")
        ListView2.Items(ListView2.Items.Count - 1).Selected = True
        ListView2.Items(ListView2.Items.Count - 1).Tag = pCont
        ListView2.Items(ListView2.Items.Count - 1).ForeColor = Color.Green
        ListView2.EnsureVisible(ListView2.Items.Count - 1)
        ListView2.EndUpdate()

        Main.Create_ItemsForm(pCont)
    End Sub

    Private Sub DelToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles DeleteToolStripMenuItem.Click
        Dim canDelete As Boolean
        If (TabControl1.SelectedIndex = 0) Then ' items
            If (CInt(ListView2.FocusedItem.Tag) = UBound(Items_LST) OrElse File.Exists(SaveMOD_Path & PROTO_ITEMS & ListView2.FocusedItem.SubItems(1).Text)) Then
                canDelete = True
            End If
        Else
            If (CInt(ListView1.FocusedItem.Tag) = ListView1.Items.Count - 1 OrElse File.Exists(SaveMOD_Path & PROTO_CRITTERS & ListView1.FocusedItem.SubItems(1).Text)) Then
                canDelete = True
            End If
        End If
        If (canDelete = False) Then
            MessageBox.Show("You can delete only the last prototype in the list, or the prototype of the file located in the save mod folder.", "Can't delete prototype file.")
            Exit Sub
        End If

        If MsgBox("Delete Pro File?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
            If TabControl1.SelectedIndex = 0 Then
                'del file
                Dim focusedItem As ListViewItem = ListView2.FocusedItem
                Dim dProFile As String = SaveMOD_Path & PROTO_ITEMS & focusedItem.SubItems(1).Text
                If File.Exists(dProFile) Then
                    File.SetAttributes(dProFile, FileAttributes.Normal)
                    File.Delete(dProFile)

                    'Log
                    Main.PrintLog("Delete Pro: " & dProFile)
                End If

                Dim pCont As Integer = UBound(Items_LST)
                If CInt(focusedItem.Tag) = pCont Then
                    ListView2.Items.RemoveAt(focusedItem.Index)
                    Array.Resize(Items_LST, pCont)

                    'save to lst file
                    pCont -= 1
                    Dim lst(pCont) As String
                    For n As Integer = 0 To pCont
                        lst(n) = Items_LST(n).proFile
                    Next
                    File.WriteAllLines(SaveMOD_Path & itemsLstPath, lst)

                    'Log
                    Main.PrintLog("Update: " & SaveMOD_Path & itemsLstPath)
                Else
                    ListView2.Items.Item(focusedItem.Index).SubItems(3).Text = "?"
                    ListView2.Items.Item(focusedItem.Index).ForeColor = Color.DarkGray
                End If
            Else
                'del file
                Dim focusedItem As ListViewItem = ListView1.FocusedItem
                Dim dProFile As String = SaveMOD_Path & PROTO_CRITTERS & focusedItem.SubItems(1).Text
                If File.Exists(dProFile) Then
                    File.SetAttributes(dProFile, FileAttributes.Normal)
                    File.Delete(dProFile)

                    'Log
                    Main.PrintLog("Delete Pro: " & dProFile)
                End If

                Dim pCont As Integer = UBound(Critter_LST)
                If CInt(focusedItem.Tag) = pCont Then
                    ListView1.Items.RemoveAt(focusedItem.Index)
                    Array.Resize(Critter_LST, pCont)
                    pCont -= 1

                    'save to lst file
                    Dim lst(pCont) As String
                    For n = 0 To pCont
                        lst(n) = Critter_LST(n).proFile
                    Next
                    File.WriteAllLines(SaveMOD_Path & crittersLstPath, lst)

                    'Log
                    Main.PrintLog("Update: " & SaveMOD_Path & crittersLstPath)
                Else
                    ListView1.Items.Item(focusedItem.Index).SubItems(2).Text = "?"
                    ListView1.Items.Item(focusedItem.Index).ForeColor = Color.DarkGray
                End If
            End If
        End If
    End Sub

    Private Sub CreateToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CreateToolStripMenuItem.Click
        If TabControl1.SelectedIndex = 1 Then
            Dim pIndx As Integer = CInt(ListView1.FocusedItem.Tag)

            FileSystem.CopyFile(DatFiles.CheckFile(PROTO_CRITTERS & Critter_LST(pIndx).proFile), "template", True)
            File.SetAttributes("template", FileAttributes.Normal Or FileAttributes.Archive)

            AddCritterPro()
        Else
            Dim pIndx As Integer = CInt(ListView2.FocusedItem.Tag)

            FileSystem.CopyFile(DatFiles.CheckFile(PROTO_ITEMS & Items_LST(pIndx).proFile), "template", True)
            File.SetAttributes("template", FileAttributes.Normal Or FileAttributes.Archive)

            AddItemPro(Items_LST(pIndx).itemType)
        End If
    End Sub

    Private Sub Find_Click(sender As Object, e As EventArgs) Handles FindToolStripMenuItem.Click
        Find(sender, True)
    End Sub

    Private Sub BackwardFind_Click(sender As Object, e As EventArgs) Handles BackwardFindToolStripMenuItem.Click
        Find(sender, False)
    End Sub

    'поиск ключевого слова
    Private Sub Find(sender As Object, isForward As Boolean)
        If tstbSearchText.Text <> Nothing Then
            Dim lstView As ListView
            If TabControl1.SelectedIndex = 0 Then
                lstView = ListView2
            Else
                lstView = ListView1
            End If

            Dim n As Integer
            If (isForward) Then
                n = If(lstView.FocusedItem IsNot Nothing, lstView.FocusedItem.Index + 1, 0)
                If n >= lstView.Items.Count Then n = 0

                If Misc.SearchLW(n, lstView, tstbSearchText.Text) = False Then
                    If n = 0 OrElse Misc.SearchLW(0, lstView, tstbSearchText.Text) = False Then
                        tstbSearchText.BackColor = Color.MistyRose
                        Exit Sub
                    End If
                End If
            Else
                n = If(lstView.FocusedItem IsNot Nothing, lstView.FocusedItem.Index, lstView.Items.Count)
                If n = 0 Then n = lstView.Items.Count

                If Misc.SearchLWBack(n, lstView, tstbSearchText.Text) = False Then
                    If n = lstView.Items.Count OrElse Misc.SearchLWBack(lstView.Items.Count, lstView, tstbSearchText.Text) = False Then
                        tstbSearchText.BackColor = Color.MistyRose
                        Exit Sub
                    End If
                End If
            End If

            tstbSearchText.BackColor = SystemColors.Window

            If lstView.View = View.Details Then
                Dim pos As Point = lstView.FocusedItem.Position
                If pos.Y >= lstView.Bounds.Size.Height Then
                    lstView.TopItem = lstView.FocusedItem
                End If
            End If
            If Not (TypeOf sender Is ToolStripTextBox) Then
                lstView.Focus()
            End If
            lstView.FocusedItem.EnsureVisible()
        End If
    End Sub

    Private Sub ToolStripTextBox1_KeyUp(ByVal sender As Object, ByVal e As KeyEventArgs) Handles tstbSearchText.KeyDown
        If e.KeyData = Keys.Enter Then
            e.SuppressKeyPress = True
            Find(sender, True)
        End If
    End Sub

    Private Sub tstbSearchText_TextChanged(sender As Object, e As EventArgs) Handles tstbSearchText.TextChanged
        tstbSearchText.BackColor = SystemColors.Window
    End Sub

    Private Sub ListView1_MouseDoubleClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles ListView1.MouseDoubleClick
        Main.Create_CritterForm(CInt(ListView1.FocusedItem.Tag))
    End Sub

    Private Sub ListView2_MouseDoubleClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles ListView2.MouseDoubleClick
        Main.Create_ItemsForm(CInt(ListView2.FocusedItem.Tag))
    End Sub

    Private Sub HToolStripMenuItem_Click_1(ByVal sender As Object, ByVal e As EventArgs) Handles HToolStripMenuItem.Click
        If TabControl1.SelectedIndex = 1 Then
            Main.Create_CritterForm(CInt(ListView1.FocusedItem.Tag))
        Else
            Main.Create_ItemsForm(CInt(ListView2.FocusedItem.Tag))
        End If
    End Sub

    Private Sub ToolStripSplitButton1_MouseHover(ByVal sender As Object, ByVal e As EventArgs) Handles ToolStripSplitButton1.MouseHover
        ToolStripSplitButton1.ShowDropDown()
    End Sub

    Private Sub ToolStripSplitButton2_ButtonClick(ByVal sender As Object, ByVal e As EventArgs) Handles ToolStripSplitButton2.ButtonClick
        Setting_Form.ShowDialog()
        'Settings.SetEncoding()
        SetFormSettings()
    End Sub

    Private Sub UpdateCritterList()
        Main.GetCrittersLst()
        Main.CreateCritterList()
        If onlyOnceCritter AndAlso ListView1.View = View.Tile Then
            ThumbnailImage.GetCrittersImages()
            ListView1.Refresh()
        End If
    End Sub

    Private Sub UpdateItemList()
        Main.Items_FRM = Nothing
        Main.Iven_FRM = Nothing

        Main.GetItemsLst()
        Main.CreateItemsList(currentFilter)
        If onlyOnceItem AndAlso ListView2.View = View.Tile Then
            Main.GetItemsLstFRM()
            ThumbnailImage.GetItemsImages()
            ListView2.Refresh()
        End If
    End Sub

    Private Sub UpdateList(ByVal sender As Object, ByVal e As EventArgs) Handles tsbtnUpdateList.Click
        If TabControl1.SelectedIndex = 1 Then
            UpdateCritterList()
        Else
            UpdateItemList()
        End If
        Scripts_Lst = Nothing
        Main.GetScriptLst()
    End Sub

    Private Sub AttrReadOnlyToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AttrReadOnlyToolStripMenuItem.Click
        proRO = AttrReadOnlyToolStripMenuItem.Checked
    End Sub

    Private Sub ClearToolStripMenuItem2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ClearToolStripMenuItem2.Click
        cArtCache = ClearToolStripMenuItem2.Checked
    End Sub

    Private Sub Cp866ToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Cp866ToolStripMenuItem.Click
        Settings.txtWin = Not (Cp866ToolStripMenuItem.Checked)
        Settings.SetEncoding()
        txtLvCp = False
    End Sub

    Private currentFilter As ItemType = ItemType.Unknown

    Private Sub ClearFilter()
        fAllToolStripMenuItem1.Checked = False
        fWeaponToolStripMenuItem3.Checked = False
        fAmmoToolStripMenuItem2.Checked = False
        fArmorToolStripMenuItem2.Checked = False
        fDrugToolStripMenuItem3.Checked = False
        fMiscToolStripMenuItem2.Checked = False
        fContainerToolStripMenuItem.Checked = False

        ListView2.ListViewItemSorter = Nothing
    End Sub

    Private Sub fAllToolStripMenuItem1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles fAllToolStripMenuItem1.Click
        ClearFilter()
        fAllToolStripMenuItem1.Checked = True
        currentFilter = ItemType.Unknown
        Main.FilterCreateItemsList(ItemType.Unknown)
    End Sub

    Private Sub fWeaponToolStripMenuItem3_Click(ByVal sender As Object, ByVal e As EventArgs) Handles fWeaponToolStripMenuItem3.Click
        ClearFilter()
        fWeaponToolStripMenuItem3.Checked = True
        currentFilter = ItemType.Weapon
        Main.FilterCreateItemsList(ItemType.Weapon)
    End Sub

    Private Sub fAmmoToolStripMenuItem2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles fAmmoToolStripMenuItem2.Click
        ClearFilter()
        fAmmoToolStripMenuItem2.Checked = True
        currentFilter = ItemType.Ammo
        Main.FilterCreateItemsList(ItemType.Ammo)
    End Sub

    Private Sub fArmorToolStripMenuItem2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles fArmorToolStripMenuItem2.Click
        ClearFilter()
        fArmorToolStripMenuItem2.Checked = True
        currentFilter = ItemType.Armor
        Main.FilterCreateItemsList(ItemType.Armor)
    End Sub

    Private Sub fDrugToolStripMenuItem3_Click(ByVal sender As Object, ByVal e As EventArgs) Handles fDrugToolStripMenuItem3.Click
        ClearFilter()
        fDrugToolStripMenuItem3.Checked = True
        currentFilter = ItemType.Drugs
        Main.FilterCreateItemsList(ItemType.Drugs)
    End Sub

    Private Sub fMiscToolStripMenuItem2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles fMiscToolStripMenuItem2.Click
        ClearFilter()
        fMiscToolStripMenuItem2.Checked = True
        currentFilter = ItemType.Misc
        Main.FilterCreateItemsList(ItemType.Misc)
    End Sub

    Private Sub fContainerToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles fContainerToolStripMenuItem.Click
        ClearFilter()
        fContainerToolStripMenuItem.Checked = True
        currentFilter = ItemType.Container
        Main.FilterCreateItemsList(ItemType.Container)
    End Sub

    Private Sub TabControl1_Selecting(ByVal sender As Object, ByVal e As TabControlCancelEventArgs) Handles TabControl1.Selecting
        If TabControl1.SelectedIndex = 0 Then
            ToolStripSplitButton1.Enabled = True
            ImageListingToolStripButton.Checked = (ListView2.View = View.Tile)
        Else
            ToolStripSplitButton1.Enabled = False
            ImageListingToolStripButton.Checked = (ListView1.View = View.Tile)
        End If
        If ListView1.Items.Count = 0 Then CreateCritterList() ': TypeCrittersToolStripMenuItem.Enabled = True
    End Sub

    Private Sub ListView_Sorting(ByVal sender As Object, ByVal e As ColumnClickEventArgs) Handles ListView2.ColumnClick, ListView1.ColumnClick
        CType(sender, ListView).ListViewItemSorter = New Comparer.ListViewItemComparer(e.Column)
    End Sub

    Private Sub MainGotFogus(ByVal sender As Object, ByVal e As EventArgs) Handles ListView2.MouseEnter, ListView1.MouseEnter
        If Not (DontHoverSelectToolStripMenuItem.Checked) AndAlso Not (Me.Focused) Then Me.Focus()
    End Sub

    Private Sub ListView1_ItemSelectionChanged(ByVal sender As Object, ByVal e As ListViewItemSelectionChangedEventArgs) Handles ListView1.ItemSelectionChanged
        ToolStripStatusLabel1.Text = DatFiles.CheckFile(PROTO_CRITTERS & Critter_LST(CInt(e.Item.Tag)).proFile, , , False)
        ToolStripStatusLabel2.Text = "Critter FID: " & Critter_LST(CInt(e.Item.Tag)).FID.ToString
    End Sub

    Private Sub ListView2_ItemSelectionChanged(ByVal sender As Object, ByVal e As ListViewItemSelectionChangedEventArgs) Handles ListView2.ItemSelectionChanged
        ToolStripStatusLabel1.Text = DatFiles.CheckFile(PROTO_ITEMS & Items_LST(CInt(e.Item.Tag)).proFile, , , False)
        ToolStripStatusLabel2.Text = "Item PID: " & Items_LST(CInt(e.Item.Tag)).PID.ToString.PadLeft(8, "0"c)
    End Sub

    Private Sub TypeCrittersToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles TypeCrittersToolStripMenuItem.Click
        TabControl1.SelectTab(1)
        Application.DoEvents()

        If ListView1.Items.Count = 0 Then Main.CreateCritterList()
        File.WriteAllBytes("template", My.Resources.critter)

        AddCritterPro()
    End Sub

    Private Sub pWeaponToolStripMenuItem2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles pWeaponToolStripMenuItem2.Click
        TabControl1.SelectTab(0)
        Application.DoEvents()

        File.WriteAllBytes("template", My.Resources.weapon)
        AddItemPro(ItemType.Weapon)
    End Sub

    Private Sub pAmmoToolStripMenuItem1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles pAmmoToolStripMenuItem1.Click
        TabControl1.SelectTab(0)
        Application.DoEvents()

        File.WriteAllBytes("template", My.Resources.ammo)
        AddItemPro(ItemType.Ammo)
    End Sub

    Private Sub pArmorToolStripMenuItem1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles pArmorToolStripMenuItem1.Click
        TabControl1.SelectTab(0)
        Application.DoEvents()

        File.WriteAllBytes("template", My.Resources.armor)
        AddItemPro(ItemType.Armor)
    End Sub

    Private Sub pDrugToolStripMenuItem2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles pDrugToolStripMenuItem2.Click
        File.WriteAllBytes("template", My.Resources.drugs)
        AddItemPro(ItemType.Drugs)
    End Sub

    Private Sub ContainerToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ContainerToolStripMenuItem.Click
        TabControl1.SelectTab(0)
        Application.DoEvents()

        File.WriteAllBytes("template", My.Resources.container)
        AddItemPro(ItemType.Container)
    End Sub

    Private Sub pMiscToolStripMenuItem1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles pMiscToolStripMenuItem1.Click
        TabControl1.SelectTab(0)
        Application.DoEvents()

        File.WriteAllBytes("template", My.Resources.misc)
        AddItemPro(ItemType.Misc)
    End Sub

    Private Sub KeyToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles KeyToolStripMenuItem.Click
        TabControl1.SelectTab(0)
        Application.DoEvents()

        File.WriteAllBytes("template", My.Resources.key)
        AddItemPro(ItemType.Key)
    End Sub

    Private Sub AboutToolStripButton7_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AboutToolStripButton7.Click
        AboutBox.ShowDialog()
    End Sub

    Private Sub ListView2_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles ListView2.MouseDown
        If DontHoverSelectToolStripMenuItem.Checked AndAlso e.Button = Windows.Forms.MouseButtons.Middle Then
            ListView2.HoverSelection = True
        End If
    End Sub

    Private Sub ListView2_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles ListView2.MouseUp
        If e.Button = Windows.Forms.MouseButtons.Middle Then
            Create_ItemsForm(CInt(ListView2.FocusedItem.Tag))
            ListView2.HoverSelection = Not DontHoverSelectToolStripMenuItem.Checked
        End If
    End Sub

    Private Sub ListView1_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles ListView1.MouseDown
        If DontHoverSelectToolStripMenuItem.Checked AndAlso e.Button = Windows.Forms.MouseButtons.Middle Then
            ListView1.HoverSelection = True
        End If
    End Sub

    Private Sub ListView1_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles ListView1.MouseUp
        If e.Button = Windows.Forms.MouseButtons.Middle Then
            Main.Create_CritterForm(CInt(ListView1.FocusedItem.Tag))
            ListView1.HoverSelection = Not DontHoverSelectToolStripMenuItem.Checked
        End If
    End Sub

    Private Sub ImportCritterTableToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ImportCritterTableToolStripMenuItem.Click
        If OpenFileDialog1.ShowDialog() = DialogResult.Cancel Then Exit Sub
        TableLog_Form.ListBox1.Items.Clear()
        Table_Form.Critters_ImportTable(OpenFileDialog1.FileName)
    End Sub

    Private Sub ImportToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ImportToolStripMenuItem.Click
        If OpenFileDialog1.ShowDialog() = DialogResult.Cancel Then Exit Sub
        TableLog_Form.ListBox1.Items.Clear()
        Table_Form.Items_ImportTable(OpenFileDialog1.FileName)
    End Sub

    Private Sub ExportToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ExportToolStripMenuItem.Click
        SetParent(Table_Form.Handle.ToInt32, SplitContainer1.Handle.ToInt32)
        Table_Form.Show()
    End Sub

    Private Sub ToolStripMenuItem2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ToolStripMenuItem2.Click
        Dim cpath As String

        If TabControl1.SelectedIndex = 1 Then
            Dim pIndx As Integer = CInt(ListView1.FocusedItem.Tag)
            Dim pFile As String = Critter_LST(pIndx).proFile

            cpath = DatFiles.CheckFile(PROTO_CRITTERS & pFile, False)
            Hex_Form.LoadHex(cpath & PROTO_CRITTERS, pFile)
            Hex_Form.Tag = cpath & PROTO_CRITTERS & pFile
        Else
            Dim pFile As String = ListView2.FocusedItem.SubItems(1).Text

            cpath = DatFiles.CheckFile(PROTO_ITEMS & pFile, False)
            Hex_Form.LoadHex(cpath & PROTO_ITEMS, pFile)
            Hex_Form.Tag = cpath & PROTO_ITEMS & pFile
        End If
    End Sub

    Private Sub LinkLabel_LinkClicked(ByVal sender As Object, ByVal e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked, LinkLabel2.LinkClicked, LinkLabel3.LinkClicked
        Dim link = CType(sender, LinkLabel)
        If (link.Text <> "n/a") Then Process.Start("explorer", link.Text)
    End Sub

    Private Sub TextEditProFileToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles TextEditProFileToolStripMenuItem.Click
        If TabControl1.SelectedIndex = 1 Then
            Create_TxtEditForm(CInt(ListView1.FocusedItem.Tag), ProType.Critter)
        Else
            Create_TxtEditForm(CInt(ListView2.FocusedItem.Tag), ProType.Item)
        End If
    End Sub

    Private Sub ViewLogToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ViewLogToolStripMenuItem.Click
        TextBox1.Visible = ViewLogToolStripMenuItem.Checked
    End Sub

    Private Sub AIPacketToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AIPacketToolStripMenuItem.Click
        Main.CreateAIEditForm()
    End Sub

    Private Sub DontHoverSelectToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles DontHoverSelectToolStripMenuItem.Click
        Settings.HoverSelect = Not (DontHoverSelectToolStripMenuItem.Checked)
        SetListViewHoverSelect()
    End Sub

    Private Sub SetListViewHoverSelect()
        ListView1.HoverSelection = Settings.HoverSelect
        ListView2.HoverSelection = Settings.HoverSelect
        If DontHoverSelectToolStripMenuItem.Checked Then
            ListView1.Activation = ItemActivation.Standard
            ListView2.Activation = ItemActivation.Standard
        Else
            ListView1.Activation = ItemActivation.OneClick
            ListView2.Activation = ItemActivation.OneClick
        End If
    End Sub

    Private Sub MassCreateProfiles_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MassCreateProfilesToolStripMenuItem.Click
        Dim MassCreateFrm As New MassCreate()
    End Sub

    Private Sub ShowFIDToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowFIDToolStripMenuItem.Click
        UpdateCritterList()
        ShowCritterFid = ShowFIDToolStripMenuItem.Checked
    End Sub

    Private Sub ShowPIDToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowPIDToolStripMenuItem.Click
        UpdateItemList()
        ShowItemPid = ShowPIDToolStripMenuItem.Checked
    End Sub

    Private Sub Fallout2Formula_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Fallout2ToolStripMenuItem.Click
        Fallout1ToolStripMenuItem.Checked = False
        CalcStats.SetFormula(CalcStats.FormulaType.Fallout2)
    End Sub

    Private Sub Fallout1Formula_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Fallout1ToolStripMenuItem.Click
        Fallout2ToolStripMenuItem.Checked = False
        CalcStats.SetFormula(CalcStats.FormulaType.Fallout1)
    End Sub

#Region "Drawning trumb image"
    Private Sub ListView2_DrawItem(ByVal sender As Object, ByVal e As DrawListViewItemEventArgs) Handles ListView2.DrawItem
        Dim img As Image = ThumbnailImage.GetDrawItemImage(CInt(e.Item.Tag))
        Dim isEdit As Boolean = (e.Item.SubItems(3).Text.Length > 0)
        ThumbnailImage.DrawListItem(e, img, e.Item.SubItems(2).Text, ListView2.Font, isEdit)
    End Sub

    Private Sub ListView1_DrawItem(ByVal sender As Object, ByVal e As DrawListViewItemEventArgs) Handles ListView1.DrawItem
        Dim img As Image = ThumbnailImage.GetDrawCritterImage(CInt(e.Item.Tag))
        Dim isEdit As Boolean = (e.Item.SubItems(2).Text.Length > 0)
        ThumbnailImage.DrawListItem(e, img, "Hp: " & Critter_LST(CInt(e.Item.Tag)).crtHP.ToString, ListView1.Font, isEdit)
    End Sub

    Private onlyOnceItem As Boolean
    Private onlyOnceCritter As Boolean

    Private Sub ImageListingToolStripButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ImageListingToolStripButton.Click
        Dim list As ListView
        If TabControl1.SelectedIndex = 0 Then
            list = ListView2
            If onlyOnceItem = False Then
                onlyOnceItem = True
                Main.GetItemsLstFRM()
                ThumbnailImage.GetItemsImages()
                ListView2.TileSize = New Size(150, 80)
            End If
        Else
            list = ListView1
            If onlyOnceCritter = False Then
                onlyOnceCritter = True
                ListView1.TileSize = New Size(130, 95)
                ThumbnailImage.GetCrittersImages()
            End If
        End If

        list.OwnerDraw = ImageListingToolStripButton.Checked
        If ImageListingToolStripButton.Checked Then
            list.BackColor = Color.Black
            list.View = View.Tile
        Else
            list.BackColor = Color.White
            list.View = View.Details
        End If
    End Sub
#End Region

    Private Sub ShowCurrentKeybordLanguage()
        ToolStripLabel1.Text = InputLanguage.CurrentInputLanguage.Culture.TwoLetterISOLanguageName.ToUpper
    End Sub

    Private Sub Main_Form_InputLanguageChanged(sender As Object, e As InputLanguageChangedEventArgs) Handles MyBase.InputLanguageChanged
        ShowCurrentKeybordLanguage()
    End Sub

    Private Sub Main_Form_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
        ShowCurrentKeybordLanguage()
    End Sub

End Class
