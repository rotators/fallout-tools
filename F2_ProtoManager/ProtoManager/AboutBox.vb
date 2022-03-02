Public NotInheritable Class AboutBox

    Private Sub AboutBox1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.LabelProductName.Text = My.Application.Info.ProductName
        Me.LabelVersion.Text &= String.Format(" {0}", My.Application.Info.Version.ToString)
        Me.LabelCopyright.Text &= My.Application.Info.Copyright
    End Sub

    Private Sub OKButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles OKButton.Click
        Me.Close()
    End Sub

    Private Sub LinkLabel1_LinkClicked(ByVal sender As Object, ByVal e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Process.Start("https://yadi.sk/d/bb1WTaXCwoJsU")
    End Sub

    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        Process.Start("https://github.com/FakelsHub/F2_ProtoManager/releases")
    End Sub

    Private Sub AboutBox_FormClosed(ByVal sender As Object, ByVal e As FormClosedEventArgs) Handles MyBase.FormClosed
        Me.Dispose()
    End Sub

End Class
