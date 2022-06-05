using System.Text;

namespace Json4CSharp
{
    internal class UnexpectedEndException : Exception
    {
        public UnexpectedEndException() : base("SyntaxError: Unexpected end of JSON input") { }

    }

    internal class UnexpectedTokenException : Exception
    {
        public UnexpectedTokenException(char token, int index) : base($"SyntaxError: Unexpected token {token} in JSON at position {index}") { }
    }

    internal class JsonReader
    {
        internal string JsonStr { get; set; }
        internal int Index { get; set; }

        internal char Peek()
        {
            if (Index < JsonStr.Length) return JsonStr[Index];
            throw new UnexpectedEndException();
        }

        internal JsonReader(string str)
        {
            JsonStr = str;
            Index = 0;
        }

        internal string SubStringByLen(int length)
        {
            Next(length);
            return JsonStr.Substring(Index - length, length);
        }

        internal char Read()
        {
            return JsonStr[Index++];
        }

        internal void Next(int num = 1)
        {
            Index += num;
        }

        internal bool OutOfIndex()
        {
            return Index >= JsonStr.Length;
        }
    }

    public static class Json4CSharp
    {
        public static JsonData Json(string str)
        {
            if (str == null) throw new ArgumentNullException();

            JsonReader json = new JsonReader(str);

            JsonData jsonData = ParseJson(json);
            if (json.OutOfIndex()) return jsonData;
            throw new UnexpectedTokenException(json.Peek(), json.Index);
        }

        private static JsonData ParseJson(JsonReader json)
        {
            JsonData data;
            EscapeWhiteSpace(json);

            switch (json.Peek()) {
                case '{':
                    data = ParseObject(json); break;
                case '[':
                    data = ParseArray(json); break;
                case '"':
                    data = ParseString(json); break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    data = ParseNumber(json); break;

                case 't':
                    ParseKeyword(json, "true");
                    data = new JsonData(JsonType.BOOL, "true");
                    break;
                case 'f':
                    ParseKeyword(json, "false");
                    data = new JsonData(JsonType.BOOL, "false");
                    break;
                case 'n':
                    ParseKeyword(json, "null");
                    data = new JsonData(JsonType.NULL, "null");
                    break;

                default:
                    throw new UnexpectedTokenException(json.Peek(), json.Index);
            }

            return data;
        }

        private static void ParseKeyword(JsonReader json, string ptn)
        {
            foreach (char c in ptn) {
                if (c != json.Peek())
                    throw new UnexpectedTokenException(json.Peek(), json.Index);
                json.Next();
            }
        }


        private static JsonData ParseString(JsonReader json)
        {
            if(json.Peek() == '"') json.Next();
            else throw new UnexpectedTokenException(json.Peek(), json.Index);
            StringBuilder sb = new StringBuilder();
            while (json.Peek() != '"') {
                sb.Append(json.Read());
                if (json.OutOfIndex()) {
                    throw new UnexpectedEndException();
                }
            }
            json.Next();
            return new JsonData(JsonType.STRING, sb.ToString());
        }


        private static void EscapeWhiteSpace(JsonReader json)
        {
            while (!json.OutOfIndex()) {
                switch (json.Peek()) {
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        json.Next(); break;

                    default:
                        return;
                }
            }
            throw new UnexpectedEndException();
        }


        private static JsonData ParseObject(JsonReader json)
        {
            json.Next();
            Dictionary<string, JsonData> dict = new Dictionary<string, JsonData>();
            EscapeWhiteSpace(json);

            while (json.Peek() != '}') {
                if(json.Peek() != '"') throw new UnexpectedTokenException(json.Peek(), json.Index);
                string key = (string)ParseString(json).ObjectValue;
                EscapeWhiteSpace(json);

                if (json.Peek() == ':') json.Next();
                else throw new UnexpectedTokenException(json.Peek(), json.Index);

                JsonData value = ParseJson(json);

                dict.Add(key, value);
                EscapeWhiteSpace(json);
                if (json.Peek() == ',') {
                    json.Next();
                    EscapeWhiteSpace(json);
                }
            }

            json.Next();
            return new JsonData(JsonType.OBJECT, dict);

        }

        private static JsonData ParseArray(JsonReader json)
        {
            json.Next();
            List<JsonData> list = new List<JsonData>();
            EscapeWhiteSpace(json);

            if (json.Peek() == ']') json.Next();
            else {
                while (json.Peek() != ']') {
                    list.Add(ParseJson(json));
                    EscapeWhiteSpace(json);
                    if (json.OutOfIndex()) {
                        throw new UnexpectedEndException();
                    }
                    else if (json.Peek() == ',') {
                        json.Next();
                        EscapeWhiteSpace(json);
                    }
                    else if (json.Peek() == ']') {
                        json.Next();
                        break;
                    }
                    else {
                        throw new UnexpectedTokenException(json.Peek(), json.Index);
                    }
                }
            }
            return new JsonData(JsonType.ARRAY, list);
        }

        internal static JsonData ParseNumber(JsonReader json)
        {
            StringBuilder sb = new StringBuilder();

            while (!json.OutOfIndex() && '0' <= json.Peek() && json.Peek() <= '9') {
                sb.Append(json.Read());
            }
            return new JsonData(JsonType.NUMBER, Convert.ToDouble(sb.ToString()));
        }

    }
}