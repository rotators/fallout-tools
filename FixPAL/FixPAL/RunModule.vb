Imports System.IO

Structure PalRGB
    Dim index As Byte ' индекс до сортировки
    Dim colorR As Byte
    Dim colorG As Byte
    Dim colorB As Byte

    Function getSumColor() As Integer
        Return colorR + colorG + colorB
    End Function

End Structure

Module RunModule

    Public Class ColorComparer
        Implements IComparer(Of PalRGB)

        Public Function Compare(a As PalRGB, b As PalRGB) As Integer Implements IComparer(Of PalRGB).Compare
            Return a.getSumColor() <= b.getSumColor()
        End Function
    End Class

    Friend Sub Main()
        Dim arg = My.Application.CommandLineArgs()
        If arg.Count > 0 Then
            FixedPAL(arg(0))
            Application.Exit()
        Else
            MainForm.ShowDialog()
        End If
    End Sub

    Friend Function FixedPAL(ByVal pFile As String) As Boolean
        Dim isFrm = pFile.EndsWith(".frm", StringComparison.OrdinalIgnoreCase)
        If (Not isFrm andAlso Not pFile.EndsWith(".pal", StringComparison.OrdinalIgnoreCase)) then
            Return False
        End If
        If (isFrm) then pFile = pFile.Remove(pFile.LastIndexOf(".")) & ".pal"
        If (Not File.Exists(pFile)) Then Return False

        Dim streamFile = File.Open(pFile, FileMode.Open, FileAccess.ReadWrite, FileShare.Read)
        If (streamFile.Length > 768) Then Return False

        Dim palData(767) As Byte
        streamFile.Read(palData, 0, streamFile.Length)

        Dim palette(255) As PalRGB

        Dim n = 0
        For i As Integer = 0 To palData.Length - 1 Step 3
            palette(n).colorR = palData(i)
            palette(n).colorG = palData(i + 1)
            palette(n).colorB = palData(i + 2)
            palette(n).index = CByte(n)
            n += 1
        Next

        Array.Sort(palette, New ColorComparer)

        ' обмен, 1 индекс чаще всего используется для черного заднего фона текста субтитров
        Dim black = palette(0)
        palette(0) = palette(1)
        palette(1) = black

        n = 0
        For i = 0 To 255
            palData(n) = palette(i).colorR
            palData(n + 1) = palette(i).colorG
            palData(n + 2) = palette(i).colorB
            n += 3
        Next

        streamFile.Seek(0, SeekOrigin.Begin)
        streamFile.Write(palData, 0, 768)
        streamFile.Close()

        FixedFRM(pFile, palette)

        Return True
    End Function

    Friend Sub FixedFRM(ByVal pFile As String, palette() As PalRGB)
        pFile = pFile.Remove(pFile.LastIndexOf(".")) & ".frm"
        If (Not File.Exists(pFile)) Then Return

        Dim streamFile = File.Open(pFile, FileMode.Open, FileAccess.ReadWrite, FileShare.Read)

        const offset = 74 ' начало массива пикселей
        Dim len = streamFile.Length - offset

        streamFile.Seek(offset, SeekOrigin.Begin)

        Dim pixelData(len - 1) As Byte
        streamFile.Read(pixelData, 0, len)

        For n = 0 To len - 1
            For i = 0 To 255
                If (palette(i).index = pixelData(n)) Then
                    pixelData(n) = i
                    Exit For
                End If
            Next
        Next

        streamFile.Seek(offset, SeekOrigin.Begin)
        streamFile.Write(pixelData, 0, len)
        streamFile.Close()
    End Sub

End Module
