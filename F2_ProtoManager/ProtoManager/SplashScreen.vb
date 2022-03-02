Imports System.IO

Public NotInheritable Class SplashScreen

    Private Sub SplashScreen_Shown(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Shown
        Label3.Text &= My.Application.Info.Version.ToString
        Settings.ReadConfigFile()
        If Setting_Form.settingExit Then
            Application.Exit()
            Exit Sub
        End If
        Run()
    End Sub

    Private Sub Run()
        Label1.Visible = True
        ProgressBar1.Visible = True
        Application.DoEvents()
#If DEBUG Then
        Main.Main()
#Else
        Try
            Main.Main()
        Catch ex As Exception
            File.WriteAllText("error.log", ex.StackTrace)
            File.AppendAllText("error.log", ex.Message)
            MsgBox(ex.Message)
            ControlShow()
        End Try
#End If
    End Sub

    Private Sub ControlShow()
        Button1.Visible = True
        Button2.Visible = True
        Button3.Visible = True
        Label1.Visible = False
        ProgressBar1.Visible = False
    End Sub

    Private Sub Button2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button2.Click
        Button1.Visible = False
        Button2.Visible = False
        Button3.Visible = False
        Run()
    End Sub

    Private Sub Button3_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button3.Click
        If cCache Then Clear_Cache()
        Application.Exit()
    End Sub

    Private Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        Me.TopMost = False
        Setting_Form.ShowDialog()
        Me.TopMost = True
    End Sub

End Class
