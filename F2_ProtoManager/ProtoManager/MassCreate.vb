Imports System.IO
Imports Prototypes

Public Class MassCreate

    Private common As CommonData
    Private wall As WallsProto
    Private tile As TilesProto

    Private savePath As String

    Friend Sub New()
        InitializeComponent()

        FBDialog.SelectedPath = Settings.SaveMOD_Path
        ComboBox1.SelectedIndex = 0
        ComboBox2.SelectedIndex = 0

        Me.ShowDialog()
    End Sub

    Private Sub StartCreate(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        If FBDialog.ShowDialog = DialogResult.OK Then
            savePath = FBDialog.SelectedPath + "\"
        Else
            Exit Sub
        End If

        Dim PID As Integer, FID As Integer = CInt(NumericUpDown3.Value)
        Dim MaxCount As Integer = CInt(NumericUpDown1.Value + NumericUpDown2.Value)

        Dim data As Integer()
        Dim proLst = New List(Of String)

        If ComboBox1.SelectedIndex = 0 Then
            proLst.Add("TILES")
            tile.Unknown = -1
            tile.MaterialID = ComboBox2.SelectedIndex
            data = ProFiles.ReverseSaveData(tile, 4)
            PID = &H4000000
        Else
            proLst.Add("WALLS")
            wall.FalgsExt = &H2000
            wall.ScriptID = -1
            wall.MaterialID = ComboBox2.SelectedIndex
            data = ProFiles.ReverseSaveData(wall, 6)
            PID = &H3000000
        End If

        Progress_Form.ShowProgressBar(CInt(NumericUpDown2.Value))

        For ProNum As Integer = CInt(NumericUpDown1.Value) To MaxCount
            ProDataSave(ProNum, PID, FID, data, proLst)
            FID += 1
            Progress_Form.ProgressIncrement()
        Next
        Progress_Form.Close()

        If PID = &H4000000 AndAlso MessageBox.Show("Create an additional prototype of the 'End of Group' tile?", "Mass Create", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            ProDataSave(MaxCount + 1, PID, 21, data, proLst)
        End If

        Dim lstFile As String = If(PID = &H4000000, "Tiles.lst", "Walls.lst")
        If (File.Exists(savePath & lstFile) AndAlso MessageBox.Show("Add a list of created pro files to " & lstFile & " file?", "Mass Create", MessageBoxButtons.YesNo) = DialogResult.Yes) Then
            Dim lst = File.ReadAllLines(savePath & lstFile).ToList
            Dim index = lst.Count - 1
            While (index >= 0 AndAlso lst(index).TrimStart = String.Empty)
                lst.RemoveAt(index)
                index -= 1
            End While

            proLst.RemoveAt(0)
            lst.AddRange(proLst)

            File.WriteAllLines(savePath & lstFile, lst)
        Else
            File.WriteAllLines(savePath & "Create.lst", proLst)
            Process.Start("explorer", savePath)
        End If
    End Sub

    Private Sub ProDataSave(ByVal proNum As Integer, ByVal pid As Integer, ByVal fid As Integer, ByRef data As Integer(), proLst As List(Of String))
        common.ProtoID = (pid + proNum)
        common.DescID = (proNum * 100)
        common.FrmID = ((pid - 1) + fid)

        Dim file As String = Format(proNum, "00000000"".pro""")

        Dim fFile As Integer = FreeFile()
        FileOpen(fFile, savePath & file, OpenMode.Binary, OpenAccess.Write, OpenShare.Shared)
        FilePut(fFile, ProFiles.ReverseSaveData(common, 3))
        FilePut(fFile, data)
        FileClose(fFile)

        proLst.Add(file)
    End Sub

End Class