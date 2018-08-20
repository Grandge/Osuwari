using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AttendeeAllocator
{
    [Serializable]
    public class CsvData
    {
        /// <summary>
        /// 全行
        /// </summary>
        protected  List<string[]> _rows;
        /// <summary>
        /// 編集中フラグ
        /// </summary>
        protected bool _edited;
        /// <summary>
        /// 未セーブフラグ
        /// </summary>
        protected bool _notSaved;

        protected string _filePath;

        public CsvData()
        {
            _rows = null;
            _edited = false;
            _notSaved = false;
        }

        
        
        //------------------------
        // Methods
        //------------------------
        /// <summary>
        /// CSVファイル読み込み（一時保管）
        /// </summary>
        /// <param name="csvFilePath">CSVファイルのフルパス</param>
        /// <returns></returns>
        public List<string[]> ReadCsvFile(string csvFilePath)
        {
            List<string[]> tmpList = new List<string[]>();
            TextFieldParser parser = new TextFieldParser(csvFilePath, System.Text.Encoding.GetEncoding("Shift_JIS"));
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            while (!parser.EndOfData)
            {
                //Attendeeの生成
                string[] row = parser.ReadFields();
                tmpList.Add(row);

            }
            return tmpList;

        }

        /// <summary>
        /// ファイル読み込みと内部保持
        /// </summary>
        /// <param name="csvFilePath"></param>
        /// <returns></returns>
        public bool ReadFileAuto( string csvFilePath)
        {
            List<string[]>tmp = ReadCsvFile(csvFilePath);
            if (tmp.Count > 0)
            {
                FilePath = csvFilePath;
                _rows = tmp;
                return true;
            }
            return false;

        }

        /// <summary>
        /// データの有効性確認
        /// </summary>
        /// <returns></returns>
        virtual public bool CheckComformance()
        {
            if (_rows == null)
            {
                return false;
            }
            bool result = true;
            //データが空ならNG
            if (_rows.Count == 0)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 見出しと内容の長さが一致しているかチェックする
        /// </summary>
        /// <returns>一致していなかった行のリスト</returns>
        public List<int> CheckLength()
        {
            List<int> listError = new List<int>();
            int headLength=0;
            int count = 0;
            foreach( string[] row in _rows){
                if( count == 0)
                {
                    headLength = row.Length;
                }else{
                    if( row.Length != headLength)
                    {
                        int error = count;
                        listError.Add(error);
                    }
                }

            }

            return listError;
        }

        /// <summary>
        /// 最左に列を追加
        /// </summary>
        /// <param name="title">見出し</param>
        /// <returns></returns>
        public bool AddLeftRow(string title)
        {
            List<string[]> newList = new List<string[]>();

            if (_rows.Count == 0)
            {
                return false;
            }
            //見出し行の列数取得
            string [] titleRow = _rows[0];
            int rowlength = titleRow.Length + 1;
            string[] tmpRow = new string[rowlength + 1];

            tmpRow[0] = title;
            for(int i = 0; i < rowlength; i++){
                tmpRow[i + 1] = titleRow[i];
            }
            newList.Add(tmpRow);

            //データ行の入れなおし
            for (int i = 0; i < _rows.Count; i++)
            {
                string[] line = _rows[i];
                tmpRow = new string[rowlength + 1];
                tmpRow[0] = "";
                for (int j = 0; j < rowlength; j++)
                {
                    tmpRow[i + 1] = line[i];
                }
            }

            return true;

        }

        /// <summary>
        /// データグリッドビューの内容でデータを置き換える
        /// </summary>
        /// <param name="dgv"></param>
        /// <returns></returns>
        public List<string[]> GridToCsvData(DataGridView dgv)
        {

            List<string[]> newRows = new List<string[]>();

            //見出し行の反映
            int columnnum = dgv.Columns.Count;
            string[] newTitle = new string[dgv.Rows[0].Cells.Count];
            for (int i = 0; i < columnnum; i++)
            {
                if (dgv.Columns[i].HeaderCell.Value != null)
                {
                    newTitle[i] = dgv.Columns[i].HeaderCell.Value.ToString();
                }
                else
                {
                    newTitle[i] = "";
                }
            }
            newRows.Add(newTitle);

            //データ行の反映
            foreach (DataGridViewRow row in dgv.Rows)
            {
                int rowcount = row.Cells.Count;
                string[] newline = new string[rowcount];
                for (int i = 0; i < rowcount; i++)
                {
                    if (row.Cells[i].Value != null)
                    {
                        newline[i] = row.Cells[i].Value.ToString();
                    }
                    else
                    {
                        newline[i] = "";
                    }
                }
                newRows.Add(newline);

            }

            return newRows;

        }

        /// <summary>
        /// CsvDataの更新
        /// </summary>
        /// <param name="dgv"></param>
        public void UpdateCsvData( DataGridView dgv)
        {
            List<string[]> newRows = GridToCsvData(dgv);
            if (newRows.Count > 0)
            {
                _rows = newRows;
                Edited = true;
                NotSaved = true;
            }
        }


        /// <summary>
        /// 見出し行から name を見つけてindexを返す
        /// </summary>
        /// <param name="name">見つけたい名前</param>
        /// <returns> 0以上:index   -1:エラー</returns>
        public int FindNameFromTitle(string name)
        {
            if( Rows.Count < 2){
                return -1;
            }
            string[] title = Rows[0];

            for( int i = 0; i < title.Length; i++)
            {
                if (title[i] == name)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// CSV形式でファイル保存
        /// </summary>
        /// <param name="path"></param>
        public void SaveCSVFile(string path)
        {
            if (_rows == null)
            {
                return;
            }
            if (_rows.Count == 0)
            {
                return;
            }

            //ファイルを上書きし、Shift JISで書き込む
            System.IO.StreamWriter sw = new System.IO.StreamWriter(
                path,
                false,
                System.Text.Encoding.GetEncoding("shift_jis"));

            //TextBox1.Textの内容を1行ずつ書き込む
            foreach (string[] row in _rows)
            {
                string line = "";
                string word = "";
                for (int i = 0; i < row.Length;i++)
                {
                    word = row[i];
                    line += "\"" + word + "\"";
                    if( i < (row.Length -1)){
                        line += ",";
                    }
                }
                sw.WriteLine(line);
            }
            //閉じる
            sw.Close();
            _filePath = path;
        }

        //------------------------
        // Properties
        //------------------------
        public List<string[]> Rows{
            get{ return _rows;}
            set{ _rows = value;}
        }
        public bool Edited {
            get{ return _edited;}
            set{ _edited = value;}
        }
        public bool NotSaved {
            get{ return _notSaved;}
            set{ _notSaved = value;}
        }
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }


    }
}
