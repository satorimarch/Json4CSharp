using System.Text;

namespace Json4CSharp
{
    public enum JsonType
    {
        NUMBER,
        STRING,
        BOOL,
        NULL,
        OBJECT,
        ARRAY,
    }

    public class JsonData
    {
        public JsonType Type { get; set; }
        public object ObjectValue { get; set; }
        public double NumberValue { get; set; }

        public JsonData(JsonType type, object objectValue)
        {
            this.Type = type;
            this.ObjectValue = objectValue;
        }

        public JsonData(JsonType type, double numberValue)
        {
            this.Type = type;
            this.NumberValue = numberValue;
        }

        private void AddTab(StringBuilder sb, int num)
        {
            for (int i = 0; i < num; i++)
                sb.Append('\t');
        }

        private StringBuilder ToStringBuilder(int tabNum, bool printTab = true)
        {
            StringBuilder sb = new StringBuilder();
            if(printTab) AddTab(sb, tabNum);
            switch (Type) {
                case JsonType.NUMBER:
                    sb.Append(NumberValue); break;

                case JsonType.NULL:
                    sb.Append("null"); break;

                case JsonType.STRING:
                    sb.Append('"');
                    sb.Append(ObjectValue as string);
                    sb.Append('"');
                    break;

                case JsonType.BOOL:
                    sb.Append(ObjectValue as string); break;

                case JsonType.ARRAY:
                    sb.Append("[\n");

                    var list = (List<JsonData>)ObjectValue;
                    int listCnt = list.Count;
                    foreach (JsonData item in list) {
                        sb.Append(item.ToStringBuilder(tabNum + 1));
                        if (--listCnt > 0) sb.Append(", \n");
                    }
                    sb.Append('\n');
                    AddTab(sb, tabNum);
                    sb.Append(']');
                    break;

                case JsonType.OBJECT:
                    sb.Append("{\n");
                    var dict = (Dictionary<string, JsonData>)ObjectValue;
                    int dictCnt = dict.Count;
                    foreach (var item in dict) {
                        AddTab(sb, tabNum + 1);
                        sb.Append('"');
                        sb.Append(item.Key);
                        sb.Append("\": ");
                        if (item.Value.Type == JsonType.OBJECT || item.Value.Type == JsonType.ARRAY) {
                            sb.Append(item.Value.ToStringBuilder(tabNum + 1, false));
                        }
                        else sb.Append(item.Value.ToStringBuilder(0));
                        if (--dictCnt > 0) sb.Append(",\n");
                    }
                    sb.Append("\n");
                    AddTab(sb, tabNum);
                    sb.Append('}');
                    break;
            }
            return sb;
        }


        public override string ToString()
        {  
            return ToStringBuilder(0).ToString();
        }


        public JsonData this[int index]
        {
            get
            {
                if (this.Type != JsonType.ARRAY) throw new InvalidOperationException("Error");
                return ((List<JsonData>)ObjectValue)[index];
            }
            set
            {
                if (this.Type != JsonType.ARRAY) throw new InvalidOperationException();
                ((List<JsonData>)ObjectValue)[index] = value;
            }
        }

        public JsonData this[string key]
        {
            get
            {
                if (Type != JsonType.OBJECT) throw new InvalidOperationException();
                return ((Dictionary<string, JsonData>)ObjectValue)[key];
            }
            set
            {
                if (Type != JsonType.OBJECT) throw new InvalidOperationException();
                ((Dictionary<string, JsonData>)ObjectValue)[key] = value;
            }
        }
    }
}
