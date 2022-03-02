using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using ScriptEditor.TextEditorUI.Function;

namespace ScriptEditor.CodeTranslation
{
    public class DialogueParser
    {
        private static readonly char[] trimming = new char[] { ' ', '(', ')' };
        private static readonly char[] digit    = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        private static int nLine;
        private static ProgramInfo pi;

        #region Dialogue Parser class

        public int numberMsgLine;       // msg number line
        public int numberMsgFile = -1;  // msg number file, -1 - если используется msg файл по умолчанию
        public string toNode;           // name goto node
        public string iq;               // player IQ
        public string shortcode;        // short code for test dialog
        public string code;             // code for diagram dialog
        public int codeNumLine;         // номер строки в коде ноды
        public OpcodeType opcode;

        private static int nextPosition = -1;

        public DialogueParser(OpcodeType opcode, string name, string code, int offset = -1)
        {
            this.opcode = opcode;
            this.code = code;
            nextPosition = -1;

            switch (opcode) {
                case OpcodeType.Reply:
                case OpcodeType.Message:
                case OpcodeType.gsay_reply:
                case OpcodeType.gsay_message:
                    this.opcode = ReplyOpcode(name, code, offset);
                    break;
                case OpcodeType.Option:
                case OpcodeType.gsay_option:
                case OpcodeType.giq_option:
                    OptionOpcode(offset, name, code);
                    break;
                case OpcodeType.Call:
                    this.opcode = MiscCode(name, code, offset);
                    break;
                case OpcodeType.call:
                    CallOpcode(code, offset);
                    break;
                default:
                    //this.opcode = OpcodeType.None;
                    break;
            }
        }

        /*
         * gsay_message({int msg_list}, {int msg_num}, {int reaction});
         * gsay_reply({int msg_list}, {int msg_num});
         * Reply/Message (macros)
         */
        private OpcodeType ReplyOpcode(string name, string code, int offset)
        {
            this.codeNumLine = nLine;

            int x = GetOpcodeEndPosition(code, offset);
            nextPosition = (x != -1) ? x--: offset + 1;

            string lineMsg;
            if (DialogFunctionsRules.opcodeTemplates.ContainsKey(name)) {
                OpcodeTemplate template = DialogFunctionsRules.opcodeTemplates[name];
                if (template.totalArgs > 1) {

                    int z = -1, argsCount = 0;
                    do {
                        argsCount++;
                        z = code.IndexOf(',', ++z);
                    } while (z > -1);
                    if (argsCount != template.totalArgs) {
                        lineMsg = ReplyParseCode(offset, x, name, ref code);
                    } else {
                        code = code.Replace(';', ' ');
                        string[] args = code.Substring(offset + 1).Split(new char[] { ',' }, template.totalArgs + 1);
                        lineMsg = args[template.msgArg];
                        if (template.msgFileArg != -1) {
                            string msgFile = args[template.msgFileArg];
                            if (!int.TryParse(msgFile.Trim(trimming), out this.numberMsgFile)) this.numberMsgFile = messageSubCode(msgFile); // извлекаем номер файла msg
                        }
                    }
                } else {
                    int z = code.IndexOf(')', offset);
                    lineMsg = code.Substring(offset + 1, z - offset);
                }
                // получить короткий код
                if (this.shortcode == null) {
                    offset -= name.Length;
                    this.shortcode = this.code.Substring(offset, ((x > -1) ? x : this.code.Length - 1) - offset + 1);
                }
            } else {
                lineMsg = ReplyParseCode(offset, x, name, ref code);
            }
            // msg line number parse
            if (lineMsg != string.Empty && !int.TryParse(lineMsg.Trim(trimming), out this.numberMsgLine)) this.numberMsgLine = messageSubCode(lineMsg); // неудалось получить номер строки, распарсить доп код.

            if (this.opcode == OpcodeType.Message || this.opcode == OpcodeType.gsay_message) {
                this.toNode = "[Message]";
                return OpcodeType.Message;
            }
            this.toNode = "[Reply]";
            return OpcodeType.Reply;
        }

        private string ReplyParseCode(int offset, int x, string name, ref string code)
        {
            // пробуем парсить код
            if (x < 0) {
                this.opcode = OpcodeType.None;
                return string.Empty;
            }
            int sOffset = offset - name.Length;
            this.shortcode = code.Substring(sOffset, x + 1 - sOffset);
            if (code[x] == ')') x--;
            try {
                code = code.Substring(offset + 1, x - offset);
            } catch {
                return string.Empty;
            }

            int z;
            if (this.opcode == OpcodeType.gsay_message) {
                z = code.LastIndexOf(',');
                code = code.Remove(z);
            }
            if (this.opcode == OpcodeType.gsay_reply || this.opcode == OpcodeType.gsay_message) {
                z = code.IndexOf(',');
                string fileNum = code.Remove(z++);
                if (!int.TryParse(fileNum.Trim(trimming), out this.numberMsgFile)) { // тут номер файла msg
                    this.numberMsgFile = messageSubCode(fileNum);
                }
                code = code.Substring(z, code.Length - z); // тут номер строки
            }
            return code;
        }

        /*
         * gsay_option(msg_list, msg_num,  procedure, reaction);
         * giq_option (iq_test,  msg_list, msg_num,   procedure, reaction);
         * XOption    (msg_num, procedure, iq_test);
         * XLowOption (msg_num, procedure);
         */
        private void OptionOpcode(int offset, string name, string code)
        {
            this.codeNumLine = nLine;

            int x = GetOpcodeEndPosition(code, offset);
            nextPosition = (x != -1) ? x--: offset + 1;

            string lineMsg;
            if (DialogFunctionsRules.opcodeTemplates.ContainsKey(name)) {
                OpcodeTemplate template = DialogFunctionsRules.opcodeTemplates[name];

                int z = -1, argsCount = 0;
                do {
                    argsCount++;
                    z = code.IndexOf(',', ++z);
                } while (z > -1);
                if (argsCount != template.totalArgs) {
                    lineMsg = ParseOptionCode(offset, x, name, ref code);
                } else {
                    code = code.Replace(';', ' ');
                    string[] args = code.Substring(offset + 1).Split(new char[] {','}, template.totalArgs + 1);

                    lineMsg = args[template.msgArg];
                    this.toNode = args[template.nodeArg].Trim(trimming).TrimStart('@');

                    if (template.msgFileArg != -1) {
                        string msgFile = args[template.msgFileArg];
                        if (!int.TryParse(msgFile.Trim(trimming), out this.numberMsgFile)) this.numberMsgFile = messageSubCode(msgFile); // извлекаем номер файла msg
                    }
                    if (template.iqArg != -1) {
                        int iq;
                        if (int.TryParse(args[template.iqArg].Trim(trimming), out iq)) {
                            this.iq = iq.ToString();
                        }
                    } else {
                        this.iq = "-";
                    }
                    // получить короткий код
                    offset -= name.Length;
                    this.shortcode = code.Substring(offset, ((x > -1) ? x : code.Length - 1) - offset + 1);
                    code = args[template.msgArg];
                }
            } else {
                lineMsg = ParseOptionCode(offset, x, name, ref code);
            }
            // msg line number parse
            if (lineMsg != string.Empty && !int.TryParse(lineMsg.Trim(trimming), out this.numberMsgLine)) this.numberMsgLine = messageSubCode(code); // распарсить доп код
        }

        private string ParseOptionCode(int offset, int x, string name, ref string code)
        {
            // prepare code
            //int z = code.IndexOf("(", offset) + 1;
            if (x < offset) {
                this.opcode = OpcodeType.None;
                return string.Empty;
            }
            int sOffset = offset - name.Length;
            this.shortcode = code.Substring(sOffset, x + 1 - sOffset);
            try {
                code = code.Substring(++offset, x - offset);
            } catch {
                return string.Empty;
            }
            int z = code.LastIndexOf(',') + 1;

            string msgNum = string.Empty;

            switch (this.opcode) {
            case OpcodeType.gsay_option:
            case OpcodeType.giq_option:
                // iq param parse
                if (this.opcode == OpcodeType.giq_option) {
                    x = code.IndexOf(',');
                    this.iq = code.Remove(x).Trim(trimming);
                    code = code.Substring(++x); // удаляем IQ параметр
                    z -= x;
                } else {
                    this.iq = "-";
                }

                // node param parse
                code = code.Remove(z - 1); // удаляем Reaction параметр
                z = code.LastIndexOf(',') + 1;
                this.toNode = code.Substring(z, code.Length - z).Trim(trimming).TrimStart('@');
                code = code.Remove(z - 1); // удаляем Node параметр

                // msg file number parse
                z = code.IndexOf(',');
                string fileNum = code.Remove(z);
                x = fileNum.IndexOf(',') + 1;
                if (!int.TryParse(fileNum.Substring(x).Trim(trimming), out this.numberMsgFile)) {
                    this.numberMsgFile = messageSubCode(fileNum);
                }
                code = code.Substring(z + 1); // удаляем

                // msg line number parse
                msgNum = code;
                z = msgNum.IndexOf(',');
                if (z > 0) msgNum = msgNum.Remove(z); //.Trim(trimming);
                break;

            // for all macros
            case OpcodeType.Option:
                // iq/node param parse
                int result;
                string str = code.Substring(z).Trim(trimming);
                if (int.TryParse(str, out result)) {
                    this.iq = result.ToString();
                    code = code.Remove(z - 1);
                } else {
                    this.toNode = str.TrimStart('@');
                }
                // node param parse
                if (this.toNode == null) {
                    int y = code.LastIndexOf(',') + 1;
                    this.toNode = code.Substring(y, --z - y).Trim(trimming).TrimStart('@');
                    z = y;
                }
                msgNum = code = code.Remove(z - 1); // удаляем Node параметр
                break;
            }
            return msgNum;
        }

        private OpcodeType MiscCode(string name, string code, int offset)
        {
            int x = GetOpcodeEndPosition(code, offset);
            nextPosition = (x != -1) ? x--: offset + 1;

            OpcodeTemplate template = DialogFunctionsRules.opcodeTemplates[name];
            if (template.totalArgs > 1) {

                int z = -1, argsCount = 0;
                do {
                    argsCount++;
                    z = code.IndexOf(',', ++z);
                } while (z > -1);
                if (argsCount != template.totalArgs) {
                    // получить ноду путем поиска по ключевому слову
                    string node = null;
                    z = offset;
                    do {
                        z = code.IndexOf("node", z, StringComparison.OrdinalIgnoreCase);
                        if (z != -1) {
                            node = GetWordAt(code, z);
                            int i = pi.GetProcedureIndex(node);
                            if (i != 1) {
                                node = pi.GetProcedureByIndex(i).name;
                                break;
                            }
                            z++;
                        }
                    } while (z != -1 && z < code.Length);
                    this.toNode = node ?? "<Failed get node name>";
                } else {
                    code = code.Replace(';', ' ');
                    string[] args = code.Substring(offset + 1).Split(new char[] { ',' }, template.totalArgs + 1);
                    this.toNode = args[template.nodeArg];
                }
            } else {
                int z = code.IndexOf(')', offset);
                this.toNode = code.Substring(offset + 1, z - offset);
            }
            // получить короткий код
            offset -= name.Length;
            this.shortcode = this.code.Substring(offset, ((x > -1) ? x : this.code.Length - 1) - offset + 1);
            this.numberMsgLine = -1;

            return OpcodeType.call;
        }

        //for standart call
        private void CallOpcode(string code, int offset)
        {
            int m = code.IndexOf(";", offset);
            if (m == -1) m = code.Length;
            int sOffset = offset - 4;
            this.shortcode = code.Substring(sOffset, m - sOffset);

            int x = code.IndexOf('(', offset);
            if (x > 0 && x < m) m = x;

            try {
                this.toNode = code.Substring(offset, m - offset).Trim();
            } catch {
                this.toNode = "<Failed get call node name>";
            }
            this.numberMsgLine = -1;
        }

        private int messageSubCode(string scode)
        {
            int result = -1;
            int macroValue = -1;
            string token = null;
            bool isMsgStr = false;

            int j = scode.IndexOf('(');
            if (j != -1) {
                int i = scode.IndexOf("message_str");
                if (i == -1) i = scode.IndexOf("random");

                if (i != -1) token = GetOpcodeName(scode, ref i);
                if (token != null) {
                    switch (token.ToLowerInvariant()) {
                    case "message_str":
                        isMsgStr = true;
                        break;
                    case "random":
                        j = scode.IndexOf(',') - 1;
                        i = scode.IndexOf('(', i);
                        token = scode.Substring(i + 1, j - i).Trim(trimming);
                        return CheckMacrosValue(token);
                    }
                } else {
                    token = scode.Remove(j).Trim(trimming); //.ToLowerInvariant();
                }
            }

            if (token != null && (isMsgStr || pi.macros.ContainsKey(token))) {
                string def = isMsgStr ? scode : pi.macros[token].code; //message_str(NAME,x)
                int x = def.IndexOf("message_str", StringComparison.OrdinalIgnoreCase);
                int z = def.IndexOf('(');
                int y = def.IndexOf(',', z);

                if (x != -1) { //Берем макрос номера файла сообщения
                    string argMsgNum = def.Substring(z + 1, y - z - 1).Trim(trimming); //.ToLowerInvariant(); //macros NAME
                    macroValue = CheckMacrosValue(argMsgNum);
                    if (macroValue == -1)
                        return macroValue;
                }
                // возвращаем номер msg строки
                if (isMsgStr)
                    z = scode.IndexOf(',');
                else
                    z = scode.IndexOf('(');
                if (z == -1) return result;

                numberMsgFile = macroValue; // number msg file

                /* do // ишем первое вхожднение числа
                {
                    z++;
                    if (z == scode.Length)
                        return result;
                } while (!Char.IsDigit(scode[z])); */

                z++;
                if (!isMsgStr) y = scode.IndexOf(',', z);
                if (y == -1 || isMsgStr) y = scode.IndexOf(')', z);

                string argMsgLine = scode.Substring(z, y - z);
                if (!int.TryParse(argMsgLine.Trim(trimming), out result)) {
                    //result = scode.LastIndexOf(')');
                    //if (result > -1)
                    //    result = messageSubCode(scode.Substring(z, result - z));
                    result = messageSubCode(argMsgLine);
                }
            } else if (pi.macros.ContainsKey(scode)) {
                result = CheckMacrosValue(scode);
            } else {
                // получить первое найденное число
                int d = 0, number = 0;
                do {
                    d = scode.IndexOfAny(digit, d);
                    if (d != -1) {
                        int n = CheckDigitValue(ref d, scode);
                        if (number == 0)
                            number = n;
                        else
                            number += n;
                    }
                } while (d != -1 && d < scode.Length - 1 && scode.IndexOf('+', d) != -1); // операция суммирования
                result = number;
            }
            return result; // return number msg line
        }

        private int CheckDigitValue(ref int d, string code)
        {
            int i;
            for (i = d; i < code.Length; i++)
            {
                if (!Char.IsDigit(code[i])) break;
            }
            code = code.Substring(d, i - d);
            d = i;
            return Convert.ToInt32(code);
        }

        private int CheckMacrosValue(string argToken)
        {
            int result = -1;
        loop:
            if (pi.macros.ContainsKey(argToken)) {
                pi.MacrosGetValue(ref argToken);
                if (!int.TryParse(argToken, out result)) // проверяем значение макроса
                    goto loop;
            } else { // такого макроса нет, проверяем на явно указаный номер
                if (!int.TryParse(argToken, out result))
                    result = -1; // выход, не удалось получить номер
            }
            return result;
        }

        #endregion

        public static void PrepareNodeCode(string nodeProcedureText, List<DialogueParser> args, ProgramInfo pi, bool excludeComment)
        {
            string[] preNodeBody = nodeProcedureText.Split(new char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < preNodeBody.Length; i++)
            {
                preNodeBody[i] = preNodeBody[i].TrimEnd().Replace("\t", new string(' ', 3));

                int n = 0;
                do {
                    n = preNodeBody[i].IndexOf(";", n);
                    if (n > 0) {
                        if (++n >= preNodeBody[i].Length)
                            break; // выходим достигнут конец строки

                        int z = preNodeBody[i].IndexOf("else", n);
                        if (excludeComment && z != -1)
                            z = (preNodeBody[i].IndexOf("//") != -1) ? -1 : z; //исключаем перенос для закоментированных строк кода
                        if (z < 0)
                            break; // в строке нет ключевого слова 'else'

                        int x;
                        for (x = 0; x < preNodeBody[i].Length; x++)
                        {
                            if (!Char.IsWhiteSpace(preNodeBody[i][x]))
                                break;
                        }

                        preNodeBody[i] = preNodeBody[i].Insert(z, Environment.NewLine + new string(' ', x));
                        n = z + 6;
                    }
                } while (n > -1);
            }

            ParseNodeCode(String.Join("\n", preNodeBody), args, pi, true, excludeComment);
        }

        public static void ParseNodeCode(string text, List<DialogueParser> args, ProgramInfo pi, bool diagram = false,
                                         bool excludeComment = true, StringSplitOptions splitOption = StringSplitOptions.RemoveEmptyEntries)
        {
            DialogueParser.pi = pi;
            int _comm = 0, _count = 0;

            Regex regex = new Regex(@"\b" + OpcodeType.call.ToString() + @"\b", RegexOptions.IgnoreCase);
            string[] bodyNode = text.Split(new char[] {'\n'}, splitOption);

            for (int i = 0; i < bodyNode.Length; i++)
            {
                nLine = i;
                string str = bodyNode[i].TrimEnd();

                if (excludeComment || !diagram) {
                    string _str = str.TrimStart();
                    if (ParserInternal.CommentBlockParse(ref _str, ref _comm))
                        continue;
                }
                if (excludeComment || diagram)
                    _count = args.Count;

                ReplySubParse(args, str, OpcodeType.Message);
                ReplySubParse(args, str, OpcodeType.Reply);
                OptionSubParse(args, str);
                MiscSubParse(args, str); // для макросов использующихся как переход к ноде

                // for call opcode
                MatchCollection matches = regex.Matches(str);
                foreach (Match m in matches)
                    args.Add(new DialogueParser(OpcodeType.call, null, str, m.Index + 4));

                // для диаграмм добавляем строку кода из ноды
                if (diagram && _count == args.Count)
                    args.Add(new DialogueParser(OpcodeType.None, null, str));
            }
        }

        private static void ReplySubParse(List<DialogueParser> Args, string incode, OpcodeType _opcode)
        {
            int m, n = 0;
            do {
                OpcodeType opcode = _opcode;
                m = n;
                n = incode.IndexOf(opcode.ToString(), m, StringComparison.OrdinalIgnoreCase);
                if (n == -1) {
                    foreach (var op in DialogFunctionsRules.opcodeTemplates)
                    {
                        if (op.Value.isDefault || op.Value.opcode != OpcodeType.Reply) continue;

                        n = incode.IndexOf(op.Value.opcodeName, m, StringComparison.Ordinal);
                        if (n != -1) break;
                    }
                }
                if (n > -1) {
                    string name = GetOpcodeName(incode, ref n);
                    if (name == null) break; // bad code

                    string nameLower = name.ToLowerInvariant();
                    if (opcode == OpcodeType.Message) {
                        if (nameLower == "message_str") break;
                    }
                    if (nameLower == "gsay_reply" || nameLower == "gsay_message") {
                        opcode = (opcode == OpcodeType.Reply) ? OpcodeType.gsay_reply : OpcodeType.gsay_message;
                        //name = nameLower;
                    }
                    if ((n + 2) < incode.Length) {
                        Args.Add(new DialogueParser(opcode, name, incode, n));
                        n = nextPosition;
                    }
                }
            } while (n > -1 && n < incode.Length);
        }

        private static void OptionSubParse(List<DialogueParser> Args, string incode)
        {
            int m, n = 0;
            do {
                OpcodeType opcode = OpcodeType.Option;
                m = n;
                n = incode.IndexOf(opcode.ToString(), m, StringComparison.OrdinalIgnoreCase);
                if (n == -1) {
                    foreach (var op in DialogFunctionsRules.opcodeTemplates)
                    {
                        if (op.Value.isDefault || op.Value.opcode != OpcodeType.Option) continue;

                        n = incode.IndexOf(op.Value.opcodeName, m, StringComparison.Ordinal);
                        if (n != -1) break;
                    }
                }
                if (n > -1) {
                    string name = GetOpcodeName(incode, ref n);
                    if (name == null) break; // bad code

                    string nameLower = name.ToLowerInvariant();
                    if (nameLower == "gsay_option") {
                        opcode = OpcodeType.gsay_option;
                        //name = nameLower;
                    } else if (nameLower == "giq_option") {
                        opcode = OpcodeType.giq_option;
                        //name = nameLower;
                    }
                    Args.Add(new DialogueParser(opcode, name, incode, n)); // n+6
                    n = nextPosition;
                }
            } while (n > -1 && n < incode.Length);
        }

        private static void MiscSubParse(List<DialogueParser> Args, string incode)
        {
            int m, n = 0;
            do {
                m = n;
                n = -1;
                foreach (var op in DialogFunctionsRules.opcodeTemplates)
                {
                    if (op.Value.opcode != OpcodeType.Call) continue;

                    n = incode.IndexOf(op.Value.opcodeName, m, StringComparison.Ordinal);
                    if (n != -1) break;
                }
                if (n > -1) {
                    string name = GetOpcodeName(incode, ref n);
                    if (name == null) break; // bad code

                    Args.Add(new DialogueParser(OpcodeType.Call, name, incode, n));
                    n = nextPosition;
                }
            } while (n > -1 && n < incode.Length);
        }

        public static OpcodeType GetOpcodeType(string name)
        {
            switch (name) {
                case "Option" :
                    return OpcodeType.Option;
                case "Reply" :
                    return OpcodeType.Reply;
                case "Message" :
                    return OpcodeType.Message;
                case "Call" :
                    return OpcodeType.Call;
            }
            return OpcodeType.None;
        }

        public static List<string> GetAllNodesName(Procedure[] procedures)
        {
            List<string> nodesName = new List<string>();
            foreach (var p in procedures)
            {
                if ((p.Name.IndexOf("node") > -1) || p.Name == "talk_p_proc") {
                    nodesName.Add(p.name);
                }
            }
            return nodesName;
        }

        #region Помощники для парсинга кода

        /// <summary>
        /// Возвращает имя opcode в строке кода расположенный в указазной позиции
        /// </summary>
        /// <param name="index"> Возвращает позицию указываемую на открываемум скобку опкода</param>
        private static string GetOpcodeName(string incode, ref int index)
        {
            string name = null;
            // forward
            int end = -1;
            for (int i = index + 1; i < incode.Length; i++)
            {
                char c = incode[i];
                if (c != '_' && !char.IsLetterOrDigit(c)) {
                    while (char.IsWhiteSpace(c) && ++i < incode.Length) { c = incode[i]; }
                    if (c == '(') end = i;
                    break;
                }
            }
            if (end == -1) return null;

            // backward
            int start = (index > 0) ? -1 : 0;
            if (start != 0) {
                for (int i = index - 1; i >= 0; i--)
                {
                    char c = incode[i];
                    if (c != '_' && !char.IsLetterOrDigit(c)) {
                        if (char.IsWhiteSpace(c))
                            start = i + 1;
                        break;
                    }
                    if (i == 0) start = 0;
                }
            }
            if (end != -1) {
                index = end;
                if (start != -1) name = incode.Substring(start, end - start).TrimEnd(); //.ToLowerInvariant();
            }
            return name;
        }

        /// <summary>
        /// Возвращает позицию указываемую на знак ';' или за последним знаком ')' в переданном строки кода
        /// </summary>
        /// <param name="offset"> Указввает на позицию после ключевого слова опкода</param>
        private static int GetOpcodeEndPosition(string code, int offset)
        {
            int x = code.IndexOf(";", offset);
            if (x != -1) return x;

            for (; offset < code.Length; ++offset)
            {
                if (code[offset] != ' ') { // // ожидается '(' и пропускаем первые пробелы если они имеются
                    offset++;
                    break;
                }
            }

            int brackets = 1;
            for (; offset < code.Length; ++offset)
            {
                char ch = code[offset];
                if (ch == '(') {
                    ++brackets;
                } else if (ch == ')') {
                    --brackets;
                    if (brackets == 0)
                        return offset + 1;
                } else if (ch == '"') {
                    while (++offset < code.Length && code[offset] != '"') { }
                } else if (ch == '/' && offset > 0) {
                    if (code[offset - 1] == '/') break;
                } else if (ch == '*' && offset > 0) {
                    if (code[offset - 1] == '/') break;
                }
            }
            return -1; // потенциальная ошибка
        }

        /// <summary>
        /// Возвращает слово в строке кода расположенное в указазной позиции
        /// </summary>
        private static string GetWordAt(string code, int position)
        {
            // forward
            int endPos = -1;
            for (int i = position + 1; i < code.Length; i++)
            {
                char c = code[i];
                if (c != '_' && !char.IsLetterOrDigit(c)) {
                    endPos = i;
                    break;
                }
            }
            if (endPos == -1) return null;

            // backward
            int beginPos = -1;
            for (int i = position - 1; i >= 0; i--)
            {
                char c = code[i];
                if (c != '_' && !char.IsLetterOrDigit(c)) {
                    beginPos = i + 1;
                    break;
                }
                if (i == 0) beginPos = 0;
            }
            return (endPos != -1 && beginPos != -1) ? code.Substring(beginPos, endPos - beginPos) : null;
        }
        #endregion
    }
}
