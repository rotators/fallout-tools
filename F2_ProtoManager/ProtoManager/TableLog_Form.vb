Public Class TableLog_Form

    Private Sub TableLog_Form_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Main.SetParent(Handle.ToInt32, Main_Form.SplitContainer1.Panel1.Handle.ToInt32)
    End Sub

End Class