Public Class ExtraModData

    Public filePath As String
    Public fileName As String
    Public isDat As Boolean
    Public isEnabled As Boolean = False

    Sub New(ByRef modPath As String, isDat As Boolean)
        Me.filePath = modPath
        Me.isDat = isDat

        If (isDat) Then SetDatName()
    End Sub

    Sub New(ByRef modPath As String, isDat As Boolean, isEnabled As Boolean)
        Me.filePath = modPath
        Me.isDat = isDat
        Me.isEnabled = isEnabled

        If (isDat) Then SetDatName()
    End Sub

    Sub SetDatName()
        fileName = IO.Path.GetFileName(filePath)
    End Sub

End Class

