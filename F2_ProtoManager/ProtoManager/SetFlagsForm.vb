
Public Class SetFlagsForm

    Structure SetFlags
        Dim flags As Integer
        Dim flagsExt As Integer
        Dim flagsCrt As Integer
    End Structure

    Friend sets As SetFlags

    Sub New(obj As CritterObj)
        InitializeComponent()

        sets.flags = obj.Flags
        sets.flagsExt = obj.FlagsExt
        sets.flagsCrt = obj.CritterFlags
    End Sub

    Sub New(obj As ItemPrototype)
        InitializeComponent()

        sets.flags = obj.Flags
        sets.flagsExt = obj.FlagsExt

        GroupBox3.Enabled = False
        cbExtFlags1.Enabled = False
        cbExtFlags2.Enabled = False
        cbExtFlags3.Enabled = False
        cbExtFlags4.Enabled = False
        cbExtFlags5.Enabled = False
        cbExtFlags6.Enabled = False
        cbExtFlags7.Enabled = False
        cbExtFlags8.Enabled = False
    End Sub

    Private Function IsSetFlag(value As Integer, bit As Byte) As Boolean
        Return (value And 1 << bit) <> 0
    End Function

    Private Function SetFlag(ByVal value As Integer, bit As Byte, state As Boolean) As Integer
        Dim flag = 1 << bit
        Return If(state, value Or flag, value And Not (flag))
    End Function

    Private Sub SetFlagsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        For Each cnt As CheckBox In GroupBox1.Controls
            cnt.Checked = IsSetFlag(sets.flags, CByte(cnt.Tag))
        Next

        For Each cnt As CheckBox In GroupBox2.Controls
            cnt.Checked = IsSetFlag(sets.flagsExt, CByte(cnt.Tag))
        Next

        If GroupBox3.Enabled Then
            For Each cnt As CheckBox In GroupBox3.Controls
                cnt.Checked = IsSetFlag(sets.flagsCrt, CByte(cnt.Tag))
            Next
        End If

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        For Each cnt As CheckBox In GroupBox1.Controls
            sets.flags = SetFlag(sets.flags, CByte(cnt.Tag), cnt.Checked)
        Next

        For Each cnt As CheckBox In GroupBox2.Controls
            sets.flagsExt = SetFlag(sets.flagsExt, CByte(cnt.Tag), cnt.Checked)
        Next

        If GroupBox3.Enabled Then
            For Each cnt As CheckBox In GroupBox3.Controls
                sets.flagsCrt = SetFlag(sets.flagsCrt, CByte(cnt.Tag), cnt.Checked)
            Next
        End If
        Me.Close()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

End Class