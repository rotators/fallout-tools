namespace ScriptEditor.CodeTranslation
{
    public enum OpcodeType
    {
        None,

        Option,
        giq_option,
        gsay_option,

        Reply,
        gsay_reply,

        Message,
        gsay_message,

        call, // standart call

        Call  // используется для состовных макросов в которых присходит переход к ноде без сообщения
    }
}
