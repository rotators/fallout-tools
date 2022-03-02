Imports System.IO

Public Class Hex_Form
    Dim HexView As New System.ComponentModel.Design.ByteViewer

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        SetParent(Me.Handle.ToInt32, Main_Form.SplitContainer1.Handle.ToInt32)
        HexView.Left = 10
        HexView.Top = 22
        HexView.Height = 490
        HexView.Width = 425
        HexView.CellBorderStyle = TableLayoutPanelCellBorderStyle.None
        Controls.Add(HexView)
        GroupBox1.SendToBack()
    End Sub

    Friend Sub LoadHex(ByVal path As String, ByVal _file As String)
        Me.Text = "HEX View: " & _file
        path &= _file
        If (file.Exists(path)) Then HexView.SetFile(path) 'или .SetBytes
        If Not (Me.Visible) Then Me.Show()
    End Sub

    Private Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        Dim programPath As String = If(HEX_Path.Length > 0, HEX_Path, Settings.defaultHEX)
        Process.Start(programPath, """" & Me.Tag.ToString & """")
    End Sub
End Class