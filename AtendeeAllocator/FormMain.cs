using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AttendeeAllocator
{
    public partial class FormMain : Form
    {
        private Project m_curProject;

        //操作系
        private int _GridRowIndex;
        private int _GridColumnIndex;
        private bool _Loading;
        private CsvData _LayoutCsv;

        public FormMain()
        {
            InitializeComponent();

            String ver = GetSystemVersion();
            m_curProject = new Project();
            Message(String.Format("Osuwari Ver{0} \r\n", ver.ToString()));
            this.Text = m_curProject.ProjectFilePath;
            textBoxProjectFile.Text = m_curProject.ProjectFilePath;
            textBoxAttendeeFile.Text = "N/A";
            textBoxGroupFile.Text = "N/A";
            textBoxCompartmentFile.Text = "N/A";

            //コマンドライン引数を配列で取得する
            string[] cmds = System.Environment.GetCommandLineArgs();
            //コマンドライン引数を列挙する
            //foreach (string cmd in cmds)
            //{
            //    Message(cmd+"\r\n");
            //}
            //if (cmds.Count() == 2)
            //{
            //    string FileName = cmds[1];
            //    string extension = System.IO.Path.GetExtension(FileName);
            //    if (extension == ".wri")
            //    {
            //        LoadProjectFile(FileName);

            //    }
            //}

        
        }
        /// <summary>
        /// システムバージョン取得
        /// </summary>
        /// <returns></returns>
        private string GetSystemVersion()
        {
            //自分自身のAssemblyを取得
            System.Reflection.Assembly asm =
                System.Reflection.Assembly.GetExecutingAssembly();
            //バージョンの取得
            System.Version ver = asm.GetName().Version;
            return ver.ToString();
        }

        //メニュー/ファイル/終了
        private void 終了_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //メニュー/ファイル/新規作成
        private void プロジェクト新規作成_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //編集中の要素の保存確認
            if (m_curProject.IsSaved() == false)
            {
                DialogResult result = MessageBox.Show("プロジェクトが保存されていませんが、破棄していよろしいですか？",
                    "質問",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2);

                if ((result == DialogResult.No)||(result == DialogResult.Cancel))
                {
                    return;
                }
            }
            m_curProject = new Project();
            textBoxMessages.Text += "プロジェクトを新規作成しました\r\n";

        }

        //メニュー/名簿/名簿を開く
        private void 名簿を開く_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //名簿の保存確認
            if (m_curProject.IsListAttendeeSaved() == false)
            {
                DialogResult result = MessageBox.Show("名簿が保存されていませんが、破棄していよろしいですか？",
                    "質問",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2);

                if ((result == DialogResult.No) || (result == DialogResult.Cancel))
                {
                    return;
                }
            }
            //ファイルダイアログを開いて読み込むファイルを選択
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "名簿.csv";
            //ofd.InitialDirectory = @""// TODO:最後に使ったディレクトリを覚えるようにする
            ofd.Filter = "CSVファイル(*.csv)|*.csv|すべてのファイル(*.*)|*.*";
            ofd.Title = "名簿ファイルを選択してください";
            ofd.RestoreDirectory = true; //ダイアログを閉じる前にカレントディレクトリを復元する

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //内容を破棄
                //m_curProject.AttendeeData = new Attendee();
                //dataGridView1 = new DataGridView();

                ReadCsvFile(ofd.FileName, dataGridView1, m_curProject.AttendeeData, contextMenuStrip,true);
                Message(string.Format("[案内]名簿ファイル読み込み：{0}\r\n", m_curProject.AttendeeFilePath));
                textBoxAttendeeFile.Text = m_curProject.AttendeeFilePath;
            }


        }

        /// <summary>
        /// メニュー/グループ/グループ定義を開く 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void グループ定義を開く_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //グループ定義の保存確認
            if (m_curProject.IsListGroupSaved() == false)
            {
                DialogResult result = MessageBox.Show("グループ定義が保存されていませんが、破棄していよろしいですか？",
                    "質問",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2);

                if ((result == DialogResult.No) || (result == DialogResult.Cancel))
                {
                    return;
                }
            }
            //ファイルダイアログを開いて読み込むファイルを選択
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "グループ.csv";
            //ofd.InitialDirectory = @""// TODO:最後に使ったディレクトリを覚えるようにする
            ofd.Filter = "CSVファイル(*.csv)|*.csv|すべてのファイル(*.*)|*.*";
            ofd.Title = "グループ定義ファイルを選択してください";
            ofd.RestoreDirectory = true; //ダイアログを閉じる前にカレントディレクトリを復元する

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //CSVファイル読み込み（ただし、グループは項目変更しないのでコンテキストメニューは指定しない）
                ReadCsvFile(ofd.FileName, dataGridView2, m_curProject.GroupData, null,false);

                //グループファイルは項目が規定された形式である必要があるので、内容を検査する
                if (m_curProject.GroupData.CheckFormat() == true)
                {
                    Message(string.Format("[案内]グループ定義ファイル読み込み：{0}\r\n", m_curProject.GroupData.FilePath));
                    textBoxGroupFile.Text = m_curProject.GroupData.FilePath;
                }
                else
                {
                    Message(string.Format("[エラー]グループファイル{0}の構造が正しくありません。\r\n",m_curProject.GroupData.FilePath));
                }
            }
        }
        /// <summary>
        /// メニュー/区画/区画定義を開く 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 区画定義を開く_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //区画定義の保存確認
            if (m_curProject.IsCompartmentDefinitionSaved() == false)
            {
                DialogResult result = MessageBox.Show("区画定義が保存されていませんが、破棄していよろしいですか？",
                    "質問",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2);

                if ((result == DialogResult.No) || (result == DialogResult.Cancel))
                {
                    return;
                }
            }
            //ファイルダイアログを開いて読み込むファイルを選択
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "区画.csv";
            //ofd.InitialDirectory = @""// TODO:最後に使ったディレクトリを覚えるようにする
            ofd.Filter = "CSVファイル(*.csv)|*.csv|すべてのファイル(*.*)|*.*";
            ofd.Title = "区画定義ファイルを選択してください";
            ofd.RestoreDirectory = true; //ダイアログを閉じる前にカレントディレクトリを復元する

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //CSVファイル読み込み
                ReadCsvFile(ofd.FileName, dataGridView3, m_curProject.CompartmentData, contextMenuStrip,false);

                //区画ファイルは前半６項目が規定された形式である必要があるので、内容を検査する
                if (m_curProject.CompartmentData.CheckFormat() == true)
                {
                    Message(string.Format("[案内]区画定義ファイル読み込み：{0}\r\n", m_curProject.CompartmentData.FilePath));
                    textBoxCompartmentFile.Text = m_curProject.CompartmentData.FilePath;
                }
                else
                {
                    Message(string.Format("[エラー]区画定義ファイルの最初の６項目が規定と異なっています。\r\n"));
                }
            }

        }

        /// <summary>
        /// CSVファイル読み込み＆dataGridViewへの内容反映
        /// </summary>
        /// <param name="ofd">読み込むファイルのパス</param>
        /// <param name="dgv"></param>
        /// <param name="data"></param>
        /// <param name="cms"></param>
        private void ReadCsvFile(string path, DataGridView dgv, CsvData data, ContextMenuStrip cms, bool layoutItem)
        {
            //名簿ファイル読み込み
            _Loading = true;
            if (m_curProject.OpenCsvFile(data, path) == true)
            {
                SetHeaderItem(dgv, data, cms, layoutItem);

                //データの残りを繁栄
                SetDataToSell(dgv, data);

            }
            _Loading = false;

        }

        /// <summary>
        /// グリッドにデータを反映
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="data"></param>
        public void SetDataToSell(DataGridView dgv, CsvData data)
        {
            int rowsCount = data.Rows.Count - 1;
            for (int i = 0; i < rowsCount; i++)
            {
                string[] rows = data.Rows[i + 1];
                int cel = 0;
                foreach (string value in rows)
                {
                    if (value != null)
                    {
                        dgv.Rows[i].Cells[cel++].Value = value;
                    }
                    else
                    {
                        dgv.Rows[i].Cells[cel++].Value = "";
                    }
                }
            }
        }

        /// <summary>
        /// グリッドのヘッダを設定
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="data"></param>
        /// <param name="cms"></param>
        /// <param name="layoutItem">レイアウト対象の識別（名簿のみ使用）</param>
        public void SetHeaderItem(DataGridView dgv, CsvData data, ContextMenuStrip cms, bool layoutItem)
        {
            //読み込んだ名簿を編集画面のグリッドに繁栄
            //CSVに合わせてグリッドのサイズを変更
            int rowSize = data.Rows.Count - 1;
            int colSize = data.Rows[0].Length;
            dgv.ColumnCount = colSize;
            dgv.RowCount = rowSize;

            //１行目を見出し行とみなして見出し行を作る
            string[] title = data.Rows[0];
            int col = 0;
            foreach (string text in title)
            {
                //列用のコンテキストメニューの設定
                dgv.Columns[col].HeaderCell.ContextMenuStrip = cms;
                dgv.Columns[col].HeaderText = text;

                //レイアウト用の見出し設定
                SetSpecialHeaderItem(dgv, layoutItem, col++, text);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="layoutItem"></param>
        /// <param name="col"></param>
        /// <param name="text"></param>
        public void SetSpecialHeaderItem(DataGridView dgv, bool layoutItem, int col, string text)
        {
            if (layoutItem)
            {
                if (text != "")
                {
                    if (text[0] == '*')
                    {
                        dgv.Columns[col].HeaderCell.Style.BackColor = Color.LightBlue;
                    }
                    else
                    {
                        dgv.Columns[col].HeaderCell.Style.BackColor = Color.LightGray;
                    }
                }
            }
        }

        //汎用コンテキストメニュー/行の追加
        private void 行の追加ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //クリクしたコントロールを取得
            DataGridView dgv = (DataGridView)contextMenuStrip.SourceControl;

            string tmpTitle = Interaction.InputBox("行の追加", "", "追加行", -1, -1);
            dgv.Columns.Add("", tmpTitle);

            //コンテキストメニューの設定
            dgv.Columns[dgv.Columns.Count - 1].HeaderCell.ContextMenuStrip = contextMenuStrip;
            return;

        }

        //汎用コンテキストメニュー/見出し変更
        private void 見出し変更_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //クリクしたコントロールを取得
            DataGridView dgv = (DataGridView)contextMenuStrip.SourceControl;

            //現在の設定内容を取得
            string curTitle = dgv.Columns[_GridColumnIndex].HeaderText;
            string tmpTitle = Interaction.InputBox("行見出し入力", "", curTitle, -1, -1);

            if (tmpTitle != "")
            {
                dgv.Columns[_GridColumnIndex].HeaderText = tmpTitle;
                if (tmpTitle != "")
                {
                    if (tmpTitle[0] == '*')
                    {
                        dgv.Columns[_GridColumnIndex].HeaderCell.Style.BackColor = Color.LightBlue;
                    }
                    else
                    {
                        dgv.Columns[_GridColumnIndex].HeaderCell.Style.BackColor = Color.LightGray;
                    }
                }
            }
            

        }

        //汎用コンテキストメニュー/行の削除
        private void 行の削除DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //クリクしたコントロールを取得
            DataGridView dgv = (DataGridView)contextMenuStrip.SourceControl;

            dgv.Columns.RemoveAt(_GridColumnIndex);

        }

        /// <summary>
        /// 列の内容を文字列ではなく数値とみなしてソート（昇順）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 数値としてソート昇順_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //クリクしたコントロールを取得
            DataGridView dgv = (DataGridView)contextMenuStrip.SourceControl;
            dgv.Sort(new NumericStringComparer(SortOrder.Ascending));
        }
        /// <summary>
        /// 列の内容を文字列ではなく数値とみなしてソート(降順）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 数値としてソート降順_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //クリクしたコントロールを取得
            DataGridView dgv = (DataGridView)contextMenuStrip.SourceControl;
            dgv.Sort(new NumericStringComparer(SortOrder.Descending));
        }

        //マウスがグリッド上を移動した時のインデックスを記憶
        private void dataGridView_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            _GridRowIndex = e.RowIndex;
            _GridColumnIndex = e.ColumnIndex;

        }


        //セルの内容が変更された際のイベント
        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!_Loading)
            {
                DataGridView dgv = (DataGridView)sender;

                //編集状態とセーブ状態の変更
                UpdateCsvData(dgv);

            }

        }
        /// <summary>
        /// 列が追加された時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            if (!_Loading)
            {
                DataGridView dgv = (DataGridView)sender;

                //編集状態とセーブ状態の変更
                UpdateCsvData(dgv);
            }
        }
        /// <summary>
        /// 列が削除されたとき 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_ColumnRemoved(object sender, DataGridViewColumnEventArgs e)
        {
            if (!_Loading)
            {
                DataGridView dgv = (DataGridView)sender;

                //編集状態とセーブ状態の変更
                UpdateCsvData(dgv);
            }

        }

        private void UpdateCsvData(DataGridView dgv)
        {
            switch (dgv.Name)
            {
                //名簿
                case "dataGridView1":
                    m_curProject.AttendeeData.UpdateCsvData(dgv);
                    break;
                //グループ
                case "dataGridView2":
                    m_curProject.GroupData.UpdateCsvData(dgv);
                    break;
                //区画
                case "dataGridView3":
                    m_curProject.CompartmentData.UpdateCsvData(dgv);
                    break;
                case "dataGridView4":
                    if (_LayoutCsv == null)
                    {
                        _LayoutCsv = new CsvData();
                    }
                    _LayoutCsv.UpdateCsvData(dgv);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 割り当て実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResultScreen_DoubleClick(object sender, EventArgs e)
        {
            Allocator tmpAllocator = new Allocator(m_curProject.AttendeeData, m_curProject.GroupData, m_curProject.CompartmentData);
            bool result = tmpAllocator.CheckSourceData();
            if (result == false)
            {
                foreach (string message in tmpAllocator.ErrorMessages)
                {
                    Message(message + "\r\n");
                }
                return;
            }
            ErrorMessageFromAllocator(tmpAllocator);

            //作業用グループリスト作成
            tmpAllocator.BuildWorkGroup();
            ErrorMessageFromAllocator(tmpAllocator);

            //作業用区画作成
            tmpAllocator.BuildWorkCompartment();
            ErrorMessageFromAllocator(tmpAllocator);

            //割り当てアルゴリズム１（グループ優先）
            tmpAllocator.AllocateByLogic1();

            foreach (string message in tmpAllocator.ErrorMessages)
            {
                Message((message + "\r\n"));
            }

            Message("[区画別割り当て結果]=========================\r\n");
            foreach(Compartment c in tmpAllocator.ListCompartment)
            {
                Message(string.Format("[{0}]--------------\r\n",c.Name));
                foreach (Member m in c.ListMember)
                {
                    Message(string.Format("  [{0}]{1}\t{2}\r\n", m.Key, m.Data[1], m.Data[2]));
                }

            }
            Message("[名簿順割り当て結果]=========================\r\n");
            foreach (Member m in tmpAllocator.ListMemberMaster)
            {
                if (m.AllocatedCompartment != null)
                {
                    Message(string.Format("[{0}][{1}]{2}\t{3}\r\n", m.AllocatedCompartment.Name, m.Key, m.Data[1], m.Data[2]));
                }
                else
                {
                    Message(string.Format("[{0}][{1}]{2}\t{3}\r\n", "N/A", m.Key, m.Data[1], m.Data[2]));
                }

            }

            //レイアウトの実行
            Layouter tmpLayout = new Layouter(tmpAllocator.ListCompartment, m_curProject.AttendeeData);

            //区画データをグリッドに反映
            _Loading = true;
            dataGridView4.Visible = true;
            SetHeaderItem(dataGridView4, tmpLayout, null, false);
            SetDataToSell(dataGridView4, tmpLayout);
            _Loading = false;

        }

        /// <summary>
        /// アロケータのエラーメッセージ処理
        /// </summary>
        /// <param name="tmpAllocator"></param>
        private void ErrorMessageFromAllocator(Allocator tmpAllocator)
        {
            foreach (string message in tmpAllocator.ErrorMessages)
            {
                Message((message + "\r\n"));
            }
            tmpAllocator.ClearErrorMessages();
        }

        /// <summary>
        /// メッセージ領域のクリア
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxMessages_Clear(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("メッセージ領域をクリアしてよろしいですか？",
                "質問",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            if ((result == DialogResult.Yes))
            {
                textBoxMessages.Text = "";
            }
            return;

        }

        private void プロジェクトに名前を付けて保存_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "割り当てプロジェクト.wri";
            sfd.Filter = "割り当てプロジェクト(*.wri)|*.wri|すべてのファイル(*.*)|*.*";
            sfd.Title = "保存先のファイルを選択してください";
            sfd.RestoreDirectory = true;
            sfd.SupportMultiDottedExtensions = true;

            if(sfd.ShowDialog() == DialogResult.OK)
            {
                //自身のファイル名を保存
                m_curProject.ProjectFilePath = sfd.FileName;
                m_curProject.ProjectEdited = false;
                //バイナリファイルとして保存
                SaveToBinaryFile(m_curProject, sfd.FileName);

                textBoxProjectFile.Text = m_curProject.ProjectFilePath;
                Message(string.Format("[案内]プロジェクトをファイル名：{0}として保存しました。\r\n", m_curProject.ProjectFilePath));
                this.Text = m_curProject.ProjectFilePath;


            }

        }

        private void プロジェクトを開く_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "割り当てプロジェクト.wri";
            ofd.Filter = "割り当てプロジェクト(*.wri)|*.wri|すべてのファイル(*.*)|*.*";
            ofd.Title = "開くファイルを選択してください";
            ofd.RestoreDirectory = true;
            ofd.SupportMultiDottedExtensions = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadProjectFile(ofd.FileName);
            }

        }

        private void LoadProjectFile(string fileName)
        {
            Project tmpProject = null;
            tmpProject = (Project)LoadFromBinaryFile(fileName);
            if (tmpProject != null)
            {
                m_curProject = tmpProject;
                textBoxProjectFile.Text = m_curProject.ProjectFilePath;
                textBoxAttendeeFile.Text = m_curProject.AttendeeFilePath;
                textBoxGroupFile.Text = m_curProject.GroupFilePath;
                textBoxCompartmentFile.Text = m_curProject.CompartmentFilePath;
                Message(string.Format("[案内]ファイル名：{0}を開きました。\r\n", fileName));
                this.Text = m_curProject.ProjectFilePath;
                //名簿データをグリッドに反映
                _Loading = true;
                SetHeaderItem(dataGridView1, m_curProject.AttendeeData, contextMenuStrip, true);
                SetDataToSell(dataGridView1, m_curProject.AttendeeData);

                //グループデータをグリッドに反映
                SetHeaderItem(dataGridView2, m_curProject.GroupData, null, false);
                SetDataToSell(dataGridView2, m_curProject.GroupData);

                //区画データをグリッドに反映
                SetHeaderItem(dataGridView3, m_curProject.CompartmentData, contextMenuStrip, false);
                SetDataToSell(dataGridView3, m_curProject.CompartmentData);
                _Loading = false;

            }
            else
            {
                Message(string.Format("[エラー]ファイル名：{0}を開けませんせんでした。\r\n", fileName));
            }
        }

        /// <summary>
        /// オブジェクトの内容をファイルから読み込み復元する
        /// </summary>
        /// <param name="path">読み込むファイル名</param>
        /// <returns>復元されたオブジェクト</returns>
        public static object LoadFromBinaryFile(string path)
        {
            FileStream fs = new FileStream(path,
                FileMode.Open,
                FileAccess.Read);
            BinaryFormatter f = new BinaryFormatter();
            //読み込んで逆シリアル化する
            object obj = f.Deserialize(fs);
            fs.Close();

            return obj;
        }
        /// <summary>
        /// オブジェクトの内容をファイルに保存する
        /// </summary>
        /// <param name="obj">保存するオブジェクト</param>
        /// <param name="path">保存先のファイル名</param>
        public static void SaveToBinaryFile(object obj, string path)
        {
            FileStream fs = new FileStream(path,
                 FileMode.Create,
                 FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            //シリアル化して書き込む
            bf.Serialize(fs, obj);
            fs.Close();
        }

        private void Message(string message)
        {
            textBoxMessages.AppendText(message);
            textBoxMessages.SelectionStart = textBoxMessages.Text.Length;
          
        }

        private void 割り当て実行_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResultScreen_DoubleClick(sender, e);
        }

        private void 名簿を名前をつけて保存_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialogAndFileSaveToCSV(m_curProject.AttendeeData, "名簿.csv");
            textBoxAttendeeFile.Text = m_curProject.AttendeeData.FilePath;
        }

        private void 名簿を保存_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_curProject.AttendeeData.FilePath == "")
            {
                名簿を名前をつけて保存_ToolStripMenuItem_Click(sender, e);
            }
            else
            {
                m_curProject.AttendeeData.SaveCSVFile(m_curProject.AttendeeData.FilePath);
            }
            textBoxAttendeeFile.Text = m_curProject.AttendeeData.FilePath;
        }

        private void グループ定義を名前をつけて保存_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialogAndFileSaveToCSV(m_curProject.GroupData, "グループ.csv");
            textBoxGroupFile.Text = m_curProject.GroupData.FilePath;
        }

        private void グループ定義を保存_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_curProject.GroupData.FilePath == "")
            {
                グループ定義を名前をつけて保存_ToolStripMenuItem_Click(sender, e);
            }
            else
            {
                m_curProject.GroupData.SaveCSVFile(m_curProject.GroupData.FilePath);
            }
            textBoxGroupFile.Text = m_curProject.GroupData.FilePath;

        }

        private void 区画定義を名前を付けて保存_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialogAndFileSaveToCSV(m_curProject.CompartmentData, "区画.csv");
            textBoxCompartmentFile.Text = m_curProject.CompartmentData.FilePath;
        }

        private void 区画定義を保存_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_curProject.CompartmentData.FilePath == "")
            {
                区画定義を保存_ToolStripMenuItem_Click(sender, e);
            }
            else
            {
                m_curProject.CompartmentData.SaveCSVFile(m_curProject.CompartmentData.FilePath);
            }
            textBoxCompartmentFile.Text = m_curProject.CompartmentData.FilePath;

        }
        private void レイアウトを名前をつけて保存_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _LayoutCsv = new CsvData();
            _LayoutCsv.UpdateCsvData(dataGridView4);


            SaveFileDialogAndFileSaveToCSV(_LayoutCsv, "割り当て.csv");
            textBoxCompartmentFile.Text = _LayoutCsv.FilePath;
        }


        private void SaveFileDialogAndFileSaveToCSV(CsvData data, string defaultFileName)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = defaultFileName;
            sfd.Filter = "CSVファイル(*.csv)|*.csv|すべてのファイル(*.*)|*.*";
            sfd.Title = "保存先のファイルを選択してください";
            sfd.RestoreDirectory = true;
            sfd.SupportMultiDottedExtensions = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                data.SaveCSVFile(sfd.FileName);
            }
        }

        private void バージョン情報_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String strSysVer = GetSystemVersion();

            textBoxMessages.AppendText("------------------------------------------------------------------\r\n");
            textBoxMessages.AppendText(String.Format("Osuwari Ver{0} \r\n", strSysVer));
            textBoxMessages.AppendText(" Copyright(C) 2014 SWEST executive committee　All rights reserved.\r\n");
            textBoxMessages.AppendText("------------------------------------------------------------------\r\n");

        }








    }
}
