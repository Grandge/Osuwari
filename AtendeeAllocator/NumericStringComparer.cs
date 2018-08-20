using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AttendeeAllocator
{
    public class NumericStringComparer : IComparer
    {
        private int sortOrder;
        private Comparer comparer;
        public NumericStringComparer(SortOrder order)
        {
            this.sortOrder = (order == SortOrder.Descending ? -1 : 1);
            this.comparer = new Comparer(
                System.Globalization.CultureInfo.CurrentCulture);
        }

        //並び替え方を定義する
        public int Compare(object x, object y)
        {
            int result = 0;

            DataGridViewRow rowx = (DataGridViewRow)x;
            DataGridViewRow rowy = (DataGridViewRow)y;

            //はじめの列のセルの値を比較し、同じならば次の列を比較する
            for (int i = 0; i < rowx.Cells.Count; i++)
            {
                //result = this.comparer.Compare(
                //    rowx.Cells[i].Value, rowy.Cells[i].Value);

                int _x,_y;
                bool resultx,resulty;
                resultx = int.TryParse(rowx.Cells[i].Value.ToString(), out _x);
                resulty = int.TryParse(rowy.Cells[i].Value.ToString(), out _y);
                if (!resultx && !resulty)
                {
                    break;
                }
                result = _x - _y;

                if (result != 0)
                    break;
            }

            //結果を返す
            return result * this.sortOrder;
        }
    }
}
