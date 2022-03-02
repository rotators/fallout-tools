Imports System.IO
Imports System.Text

Public Class AI_TextForm

    Private pathAI As String
    Private aiCustom As Boolean

    Private buffer As List(Of String)
    Private sPacketID As Integer
    Private ePacketID As Integer
    Private change As Boolean
    Private ownerSaveButton As Boolean

    Friend Sub New(ByVal sPacketID As Integer, ByVal ePacketID As Integer, ByRef path As String, ByVal ownerSaveButton As Boolean, ByVal aiCustom As Boolean)
        InitializeComponent()

        Me.sPacketID = sPacketID
        Me.ePacketID = ePacketID
        Me.ownerSaveButton = ownerSaveButton
        Me.aiCustom = aiCustom
        Me.pathAI = path

        buffer = File.ReadAllLines(path).ToList
        Dim str As StringBuilder = New StringBuilder
        For n = sPacketID To ePacketID - 1
            If (buffer(n).Length = 0) Then Continue For
            str.Append(buffer(n))
            str.AppendLine()
        Next
        TextBox1.Text = str.ToString
    End Sub

    Private Sub AI_TextForm_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        FormControl(Me.Owner)
    End Sub

    Private Sub AI_TextForm_FormClosed(ByVal sender As Object, ByVal e As FormClosedEventArgs) Handles MyBase.FormClosed
        FormControl(Me.Owner, True)
    End Sub

    Private Sub Close_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        If change Then
            AI_Form.ReloadFile(CType(Me.Owner, AI_Form))
            ownerSaveButton = False
        End If
        Me.Close()
    End Sub

    Private Sub Save_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        buffer.RemoveRange(sPacketID, (ePacketID - sPacketID))
        buffer.InsertRange(sPacketID, Split(TextBox1.Text.Trim + vbCrLf, vbCrLf).ToList)

        ' save to file
        Dim sFile As String = If(aiCustom, pathAI, SaveMOD_Path & AI.AIFILE)

        Dim dir = Path.GetDirectoryName(sFile)
        If (Directory.Exists(dir) = False) Then Directory.CreateDirectory(dir)

        File.WriteAllLines(sFile, buffer)

        change = True

        If (Main.PacketAI IsNot Nothing) Then Main.PacketAI.Clear()
        Main.PrintLog("Update AI: " & sFile)
    End Sub

    ' Рекурсивный перебор контролов класса формы
    Private Sub FormControl(сontrol As Control, Optional state As Boolean = False)
        For Each ctrl As Control In сontrol.Controls
            If (TypeOf ctrl Is GroupBox) Then
                FormControl(ctrl, state)
            ElseIf Not (TypeOf ctrl Is Label) Then
                If ctrl.Name = "SaveButton" Then
                    ctrl.Enabled = ownerSaveButton
                Else
                    ctrl.Enabled = state
                End If
            End If
        Next
    End Sub

End Class