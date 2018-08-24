using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AttendeeAllocator
{
    public class Layouter : CsvData
    {
        List<int> _listItem;


        /// <summary>
        /// 割り当て後の区画データからCSVデータ作成
        /// </summary>
        /// <param name="listCompartment"></param>
        public Layouter(List<Compartment> listCompartment, AttendeeInfo attInfo)
        {
            _listItem = new List<int>();

            //名簿見出し行でレイアウトに使用する項目数
            int layoutItems = FindItemFromAttendeeInfo(attInfo);

            //列数の算出
            //最大の列数 = LayoutXの最大値 + 名簿見出しアイテム数 + 1
            //行数の算出
            //最大行数 = LayoutYの最大値 + LayaoutY最大値となる区画の最大収容人数
            int max_x = 0;
            int max_y = 0;
            Compartment maxyCompartment = listCompartment[0];
            foreach (Compartment c in listCompartment)
            {
                if (c.X > max_x)
                {
                    max_x = c.X;
                }
                if ((c.Y + c.Max) > max_y)
                {
                    max_y = c.Y + c.Max;
                    maxyCompartment = c;
                }
            }
            max_x += layoutItems + 1;//+1は区画名が入るため

            //m_dgv = new DataGridView();
            //m_dgv.RowCount = max_x;
            //m_dgv.ColumnCount = max_y;

            //CSVデータ用の文字列を空白で埋める
            _rows = new List<string[]>();

            //見出し行の作成
            string[] tmpRow = new string[max_x];
            string[] attInfoTitle = attInfo.Rows[0];

            //int topItems = FindTopItems(listCompartment);
            List<Compartment> listTopCompartment = FindTopCompartment(listCompartment);
            if(listTopCompartment.Count < 1)
            {
                //ERROR
                return;
            }
            int topItems = listTopCompartment.Count;
            foreach( Compartment c in listTopCompartment)
//          for (int i = 0; i < topItems; i++)
            {
                //int curBaseX = (_listItem.Count + 1) * i;
                int curBaseX = c.X;
                tmpRow[curBaseX] = "区画名";
                for (int j = 0; j < _listItem.Count; j++)
                {
                    string title;
                    string tmpTitle = attInfoTitle[_listItem[j]];
                    if (tmpTitle[0] == '*')
                    {
                        title = tmpTitle.Remove(0, 1);
                    }
                    else
                    {
                        title = tmpTitle;
                    }
                    tmpRow[j + curBaseX + 1] = title;
                }

            }
            _rows.Add(tmpRow);

            //とりあえず空の行を作って埋める
            for (int i = 0; i < max_y; i++)
            {
                tmpRow = new string[max_x];
                _rows.Add(tmpRow);
            }

            //区画毎に内容を埋めていく
            foreach( Compartment c in listCompartment)
            {
                for (int i = 0; i < c.MemberCount; i++)
                {
                    tmpRow = _rows[c.Y + i + 1]; //+1は見出し行補正
                    tmpRow[0 + c.X] = c.Name;
                    Member mem = c.ListMember[i];

                    for(int j  = 0; j < _listItem.Count; j ++)
                    {
                        tmpRow[0 + c.X + j + 1] = mem.Data[_listItem[j]]; //+1は区画名補正
                    }

                    //内容反映
                    _rows[c.Y + i + 1] = tmpRow;//+1は見出し行補正
                }
            }

        }
            

        private int FindTopItems(List<Compartment> listCompartment)
        {
            int result = 0;
            foreach (Compartment c in listCompartment)
            {
                if (c.Y == 0)
                {
                    result++;
                }
            }
            return result;
        }
        private List<Compartment> FindTopCompartment(List<Compartment> listCompartment)
        {
            List<Compartment> result = new List<Compartment>();
            foreach (Compartment c in listCompartment)
            {
                if (c.Y == 0)
                {
                    result.Add(c);
                }
            }
            return result;

        }

        private int FindItemFromAttendeeInfo(AttendeeInfo attInfo)
        {
            string[] title = attInfo.Rows[0];
            for(int i = 0; i < title.Length; i++)
            {
                string s = title[i];
                if (s != "")
                {
                    if (s[0] == '*')
                    {
                        _listItem.Add(i);
                    }
                }
            }
            return _listItem.Count;

        }
        
    }
}
