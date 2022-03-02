Friend Class Progress_Form

    Friend Sub ShowProgressBar(ByVal maxValue As Integer)
        ProgressBar1.Value = 0
        ProgressBar1.Maximum = maxValue + 1

        If (My.Forms.Main_Form.Visible) Then
            Dim x = My.Forms.Main_Form.Bounds.X + CInt(My.Forms.Main_Form.Width / 2) - 180
            Dim y = My.Forms.Main_Form.Bounds.Y + CInt(My.Forms.Main_Form.Height / 2) - 15
            Me.SetDesktopLocation(x, y)

            Me.Show(Main_Form)
            Application.DoEvents()
        End If
    End Sub

    Friend Sub ProgressIncrement()
        ProgressBar1.Value += 1
    End Sub

End Class