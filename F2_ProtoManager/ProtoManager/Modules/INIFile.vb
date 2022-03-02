Module INIFile

    'lpApplicationName - Раздел-имя,заключенное в квадратные скобки [] и группирующее ключи и значения. 
    'lpKeyName - Значение ключа.Ключ должен быть уникальным только внутри своего раздела. 
    'lpDefault -Возвращаемое значение, если правильное(допустимое) значение не может читаться. 
    'lpReturnedString - Строка фиксированной длины, получаемая при чтении любой строки файла или lpDefault. 
    'nSize - Длина в символах переменной lpReturnedString. 
    'lpFileName - Имя INI-файла для чтения.

    Private Declare Function GetPrivateProfileString Lib "kernel32.dll" Alias "GetPrivateProfileStringA" _
                        (ByVal lpApplicationName As String, ByVal lpKeyName As String,
                         ByVal lpDefault As String, ByVal lpReturnedString As String,
                         ByVal nSize As UInt32, ByVal lpFileName As String) As UInt32

    'lpApplicationName - Значение раздела INI-файла. 
    'lpKeyName - Значение ключа. 
    'lpString - устанавлимое строковое значение. 
    'lpFileName - Имя INI-файла.

    Private Declare Function WritePrivateProfileString Lib "kernel32.dll" Alias "WritePrivateProfileStringA" _
                    (ByVal lpApplicationName As String, ByVal lpKeyName As String,
                     ByVal lpString As String, ByVal lpFileName As String) As UInt32


    Private Declare Function GetPrivateProfileInt Lib "kernel32.dll" Alias "GetPrivateProfileIntA" _
                    (ByVal lpApplicationName As String, ByVal lpKeyName As String,
                     ByVal nDefault As Int32, ByVal lpFileName As String) As Int32


    ' Получение строкового параметра из секции
    Function GetString(ByVal section As String, ByVal key As String, ByVal fPath As String, ByVal def As String) As String
        Dim maxLen As UInt32 = 255
        Dim sValue = Space(maxLen)  ' обеспечиваем достаточно места для функции, чтобы поместить значение в буфер
        ' читаем файл, slength длина получаемой строки
        Dim sLength As UInteger = GetPrivateProfileString(section, key, def, sValue, maxLen, fPath)
        Return Left(sValue, sLength) ' извлекаем нужную строчку из буфера
    End Function

    ' Получение числового параметра из секции
    Function GetInt(ByVal section As String, ByRef key As String, ByRef fPath As String, Optional ByRef def As Integer = -1) As Integer
        ' читаем в файл
        Return GetPrivateProfileInt(section, key, def, fPath)
    End Function

    ' Запись параметра или секции в AI (возвращает 0 при ошибке в выполнении и 1 при успешном выполнении)
    Function SetValue(ByVal section As String, ByRef key As String, ByVal value As String, ByVal fPath As String) As UInt32
        ' запись в файл
        Return WritePrivateProfileString(section, key, value, fPath)
    End Function

End Module
