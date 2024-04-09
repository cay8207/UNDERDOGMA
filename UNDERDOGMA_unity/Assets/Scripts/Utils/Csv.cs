using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

/* Example Code:
 * 
 * StageDataTable sdt = new StageDataTable("ppap");
 * sdt.LoadCsv("stage.csv");
 * sdt.print();
 * sdt["StageID", 0] = "W1-1";
 * Console.WriteLine(sdt["StageID", 0]);  // output: W1-1
 * sdt["Difficulty", 2] = 99;
 * sdt.SaveCsv("stage1.csv");
 */
public abstract class DataTableBase : DataTable
{
    // NOTE: static abstract는 C# 10부터 지원.
    public abstract IReadOnlyDictionary<string, string> ColumnTypeMapping { get; }

    public DataTableBase(string name) : base(name) { }

    /// <summary>
    /// Get needed CSV files from Resources folder
    /// </summary>
    /// <param name="filename">ex) "table.csv"</param>
    /// <returns></returns>
    public bool LoadCsv(string filename)
    {
        filename = Application.streamingAssetsPath + "/Data/" + filename;
        string[] buffer = System.IO.File.ReadAllLines(filename, System.Text.Encoding.UTF8);

        string[] columns = buffer[0].Split(',');
        foreach (string col in columns)
        {
            DataColumn column = new DataColumn(col);
            column.DataType = System.Type.GetType(ColumnTypeMapping[col]);
            this.Columns.Add(column);
        }

        for (int i = 1; i < buffer.Length; i++)
        {
            if (buffer[i].Trim() == "")  // 빈 줄
                continue;

            // 각각의 칸을 담아줄 리스트를 만든다.
            List<string> items = new List<string>();
            bool insideQuotes = false;
            string currentItem = "";

            // i번째 줄을 한 문자씩 읽어들인다.
            foreach (char c in buffer[i])
            {
                // 만약 글자가 ". 즉 첫 따옴표라면 insideQuotes를 true로 만들어준다. 그 다음 따옴표는 false로 만든다.
                // 즉, 여기부터의 문자들은 특정 셀의 내용이라는 뜻이다.
                // 그런데 원래는 따옴표가 문장의 양 끝에 하나씩 있는걸 생각했는데, 스프레드시트에서 바로 csv 파일로 내려받으니
                // 따옴표가 세개씩 들어가게 된다. 근데 세개 있는거랑 하나 있는거랑 아래의 코드 작동은 같아서 일단 냅둠.
                // 나중에 만약 문제가 생기면 수정해야 한다. 
                if (c == '"')
                {
                    insideQuotes = !insideQuotes;
                }
                // 그렇지 않고 만약 쉼표이며, 따옴표 안에 있지 않다면 현재까지 저장된 string을 items에 넣어주고 currentItem을 초기화한다.
                else if (c == ',' && !insideQuotes)
                {
                    items.Add(currentItem);
                    currentItem = "";
                }
                // 모두 아니라면 현재 아이템에 문자를 추가해준다. 
                else
                {
                    currentItem += c;
                }
            }

            // 마지막 아이템 추가. 이 때 따옴표가 있을 수 있으므로 제거해준다. 따옴표는 들어갈 필요가 없음!
            items.Add(currentItem.Trim('"'));

            if (items.Count > 0)
                this.Rows.Add(items.ToArray());  // Automatically does type conversion
        }

        return true;
    }

    /// <summary>
    /// Resources 폴더에 csv 형식으로 파일을 내보낸다.
    /// </summary>
    /// <param name="filename">ex) "table.csv"</param>
    /// <returns></returns>
    public bool SaveCsv(string filename)
    {
        filename = "Assets/Resources/Data/" + filename;

        List<string> buffer = new List<string>();
        string line;

        string[] columnNames = this.Columns
            .Cast<DataColumn>()
            .Select(column => column.ColumnName)
            .ToArray();
        line = string.Join(",", columnNames);
        buffer.Add(line);

        foreach (DataRow row in this.Rows)
        {
            string[] fields = row.ItemArray
                .Select(field => field.ToString())
                .ToArray();
            line = string.Join(",", fields);
            buffer.Add(line);
        }

        System.IO.File.WriteAllLines(filename, buffer.ToArray(), System.Text.Encoding.UTF8);
        return true;
    }

    /// <summary>
    /// 콘솔에 table을 출력한다.
    /// </summary>
    public void print()
    {
        foreach (DataColumn column in this.Columns)
        {
            Debug.Log(column.ColumnName + ":" + column.DataType + " ");
        }

        foreach (DataRow row in this.Rows)
        {
            foreach (var item in row.ItemArray)
            {
                Debug.Log(item + ":" + item.GetType() + " ");
            }
        }
    }

    /// <summary>
    /// 주어진 열과 행의 원소를 pandas style로 get 또는 set한다.
    /// </summary>
    /// <param name="col">ex) "Type"</param>
    /// <param name="row">ex) "Elite"</param>
    /// <returns></returns>
    public object this[string col, int row]
    {
        get
        {
            return this.Rows[row][col];
        }
        set
        {
            this.Rows[row][col] = value;
        }
    }
}

public class DialogueDataTable : DataTableBase
{
    override public IReadOnlyDictionary<string, string> ColumnTypeMapping => _columnTypeMapping;
    private static IReadOnlyDictionary<string, string> _columnTypeMapping = new Dictionary<string, string>()
    {
        { "DialogueID", "System.Int32" },
        { "When", "System.String"},
        { "Order", "System.Int32" },
        { "Effect", "System.String"},
        { "Speaker1", "System.String" },
        { "Speaker1Highlight", "System.Boolean"},
        { "Speaker2", "System.String" },
        { "Speaker2Highlight", "System.Boolean"},
        { "Speaker3", "System.String" },
        { "Speaker3Highlight", "System.Boolean"},
        { "Key", "System.String" },
    };

    public DialogueDataTable(string name) : base(name) { }

    // 현재 world, stage를 변수로 넘겨받아서 ID의 값이 world*100+stage와 일치하는 row,
    // GameManager의 Language와 일치하고 When이 dialogueEvent와 일치하는 col들을 찾아서 List의 형태로 반환한다.
    public List<string> GetDialogueData(DialogueEvent dialogueEvent, Language language, int world, int stage)
    {
        List<string> pool = this.Rows
            .Cast<DataRow>()
            .Where(x => (int)x["DialogueID"] == world * 100 + stage)
            .Where(x => x["When"] as string == dialogueEvent.ToString())
            .Select(x => x[language.ToString()] as string)
            .ToList();

        return pool;
    }
}

public class CutSceneDataTable : DataTableBase
{
    override public IReadOnlyDictionary<string, string> ColumnTypeMapping => _columnTypeMapping;
    private static IReadOnlyDictionary<string, string> _columnTypeMapping = new Dictionary<string, string>()
    {
        { "DialogueID", "System.Int32" },
        { "When", "System.String"},
        { "Order", "System.Int32" },
        { "Effect", "System.String"},
        { "Speaker1", "System.String" },
        { "Speaker1Highlight", "System.Boolean"},
        { "Speaker2", "System.String" },
        { "Speaker2Highlight", "System.Boolean"},
        { "Speaker3", "System.String" },
        { "Speaker3Highlight", "System.Boolean"},
        { "Key", "System.String" },
    };

    public CutSceneDataTable(string name) : base(name) { }

    // 현재 world, stage를 변수로 넘겨받아서 ID의 값이 world*100+stage와 일치하는 row,
    // GameManager의 Language와 일치하고 When이 dialogueEvent와 일치하는 col들을 찾아서 List의 형태로 반환한다.
    public List<string> GetCutSceneData(DialogueEvent dialogueEvent, Language language, int world, int stage)
    {
        List<string> pool = this.Rows
            .Cast<DataRow>()
            .Where(x => (int)x["DialogueID"] == world * 100 + stage)
            .Where(x => x["When"] as string == dialogueEvent.ToString())
            .Select(x => x[language.ToString()] as string)
            .ToList();

        return pool;
    }
}

public class WordKeyDataTable : DataTableBase
{
    override public IReadOnlyDictionary<string, string> ColumnTypeMapping => _columnTypeMapping;
    private static IReadOnlyDictionary<string, string> _columnTypeMapping = new Dictionary<string, string>()
    {
        { "DialogueID", "System.Int32" },
        { "When", "System.String"},
        { "Order", "System.Int32" },
        { "Effect", "System.String"},
        { "Speaker1", "System.String" },
        { "Speaker1Highlight", "System.Boolean"},
        { "Speaker2", "System.String" },
        { "Speaker2Highlight", "System.Boolean"},
        { "Speaker3", "System.String" },
        { "Speaker3Highlight", "System.Boolean"},
        { "Key", "System.String" },
    };

    public WordKeyDataTable(string name) : base(name) { }

    // 현재 world, stage를 변수로 넘겨받아서 ID의 값이 world*100+stage와 일치하는 row,
    // GameManager의 Language와 일치하고 When이 dialogueEvent와 일치하는 col들을 찾아서 List의 형태로 반환한다.
    public List<string> GetCutSceneData(DialogueEvent dialogueEvent, Language language, int world, int stage)
    {
        List<string> pool = this.Rows
            .Cast<DataRow>()
            .Where(x => (int)x["DialogueID"] == world * 100 + stage)
            .Where(x => x["When"] as string == dialogueEvent.ToString())
            .Select(x => x[language.ToString()] as string)
            .ToList();

        return pool;
    }
}

public class KeyTranslateDataTable : DataTableBase
{
    override public IReadOnlyDictionary<string, string> ColumnTypeMapping => _columnTypeMapping;
    private static IReadOnlyDictionary<string, string> _columnTypeMapping = new Dictionary<string, string>()
    {
        { "Key", "System.String" },
        { "English", "System.String"},
        { "Korean", "System.String" }
    };

    public KeyTranslateDataTable(string name) : base(name) { }

    // 현재 world, stage를 변수로 넘겨받아서 ID의 값이 world*100+stage와 일치하는 row,
    // GameManager의 Language와 일치하고 When이 dialogueEvent와 일치하는 col들을 찾아서 List의 형태로 반환한다.
    public List<string> GetTranslatedData(string key, Language language)
    {
        List<string> pool = this.Rows
            .Cast<DataRow>()
            .Where(x => x["Key"] as string == key)
            .Select(x => x[language.ToString()] as string)
            .ToList();

        return pool;
    }
}


