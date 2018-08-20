///////////////////////////////////////////////////////////
//  Group.cs
//  Implementation of the Class Group
//  Generated by Enterprise Architect
//  Created on:      2014/08/05 9:05:17
//  Original author: shiba
///////////////////////////////////////////////////////////



using System;
using System.Collections.Generic;
namespace AttendeeAllocator
{
    [Serializable]
    public class GroupInfo : CsvData
    {
        /// <summary>
        /// �O���[�v���ڒ�`
        /// </summary>
        string[] GROUP_DATA_COLUMN = new string[]{"����","�r��","�W��","���U","���O","�D��x"};
        public enum COLUM_POS
        {
            Name = 0,
            Exclusive = 1,
            Gather = 2,
            Scatter = 3,
            Exclude = 4,
            Priority = 5
        };



		public GroupInfo(){

		}

		~GroupInfo(){

		}

		public virtual void Dispose(){

		}

        //------------------------
        //Method
        //------------------------

        /// <summary>
        /// �K��̌`������������
        /// </summary>
        /// <returns>����(OK : true,NG : false)</returns>
        public bool CheckFormat()
        {
            //���o���s�擾
            string[] title = Rows[0];
            
            if( title.Length != GROUP_DATA_COLUMN.Length){
                return false;
            }
            int i=0;
            foreach (string word in title)
            {
                if (word != GROUP_DATA_COLUMN[i++])
                {
                    return false;
                }
            }
            return true;

        }

        //------------------------
        //Properties
        //------------------------



        /// <summary>
        /// �O���[�v�𖼑O�Ō���
        /// </summary>
        /// <param name="name">�O���[�v��</param>
        /// <returns>���݂���ꍇ�F�O���[�v��`��s���A���݂��Ȃ��ꍇ null</returns>
        public string[] FindGroupByName(string name)
        {
            string[] result = null;

            for (int i = 1; i < _rows.Count; i++ )
            {
                string[] row = _rows[i];
                if( row[(int)COLUM_POS.Name] == name){
                    result = row;
                    break;
                }
            }
            return result;
        }
    }//end Group

}//end namespace SWESTAttendeeAllocator