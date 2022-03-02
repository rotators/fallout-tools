Public Class Comparer

    ' Implements the manual sorting of items by columns.
    Class ListViewItemComparer
        Implements IComparer

        Private col As Integer

        Public Sub New()
            col = 0
        End Sub

        Public Sub New(ByVal column As Integer)
            col = column
        End Sub

        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
            On Error Resume Next
            Return String.Compare(CType(x, ListViewItem).SubItems(col).Text, CType(y, ListViewItem).SubItems(col).Text)
        End Function

    End Class

    Public Class ExtraModComparer
        Implements IComparer(Of ExtraModData)

        Public Function Compare(a As ExtraModData, b As ExtraModData) As Integer Implements IComparer(Of ExtraModData).Compare
            Return String.Compare(a.filePath, b.filePath)
        End Function
    End Class

End Class
