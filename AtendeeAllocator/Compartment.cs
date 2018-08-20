using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendeeAllocator
{
    [Serializable]
    public class Compartment
    {
        const int LAYOUT_X = 0;
        const int LAYOUT_Y = 1;

        enum LAYOUTPOS
        {
            X = 0, Y = 1
        };

        private string _name;
        private int _max;
        private bool _smoke;
        private string _priorityGroup;
        private int[] _layoutpos = new int[2];

        private List<Member> _listMember;


        public Compartment()
        {
            _name = "";
            _max = 0;
            _smoke = false;
            _priorityGroup = "";
            _layoutpos[LAYOUT_X] = 0;
            _layoutpos[LAYOUT_Y] = 0;

            _listMember = new List<Member>();
           
        }

        public Compartment( string[] info)
        {
            _name = info[(int)CompartmentInfo.COLUM_POS.Name];
            if(int.TryParse(info[(int)CompartmentInfo.COLUM_POS.Max],out _max) == false){
                _max = 0;
            }
            _priorityGroup = info[(int)CompartmentInfo.COLUM_POS.PriorityGroup];

            if (int.TryParse(info[(int)CompartmentInfo.COLUM_POS.Layout_X], out _layoutpos[LAYOUT_X]) == false)
            {
                _layoutpos[LAYOUT_X] = 0;
            }
            if (int.TryParse(info[(int)CompartmentInfo.COLUM_POS.Layout_Y], out _layoutpos[LAYOUT_Y]) == false)
            {
                _layoutpos[LAYOUT_Y] = 0;
            }

            _listMember = new List<Member>();
        }

        /// <summary>
        /// グループの属性と割り当てるメンバーのキーコードかこの区画に割り当て可能か調べる
        /// </summary>
        /// <param name="group"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool CanAllocate(Group group, string key) 
        {
            //この区画の収容人数に空きがあるか調べる

            //排他属性の他のグループがすでに割り当てられているか調べる
            
            //割り当てようとしているグループの属性に「分散」があり、尚且つすでにこの部屋に他のメンバーがいるか調べる


            return true;
        }

        /// <summary>
        /// 排他グループのメンバー割り当て
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public bool AllocateExclusiveMember(Member member,Group g)
        {
            bool result = false;

            //区画に空きがあるか
            if (_listMember.Count < _max)
            {

                if (IsAlreadyAllocatedExclusiveGroupMenber(g) == false) {
                    _listMember.Add(member);
                    result = true;
                }
            }

            return result;

        }

        /// <summary>
        /// メンバーが区画の構成員か調べる
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public bool IsInMember(Member member)
        {
            bool result = false;
            foreach (Member mem in _listMember)
            {
                if (mem.Data[0] == member.Data[0])
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// この区画にgroupと異なる他の排他属性グループのメンバーが割り当てられているか調べる
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool IsAlreadyAllocatedExclusiveGroupMenber(Group group)
        {
            foreach (Member member in _listMember)
            {
                //メンバーの所属するグループを調べる
                foreach (Group g in member.ListGroup)
                {
                    if (g.Name != group.Name)
                    {
                        //違う排他属性グループがすでにいる
                        if ((g.Exclusive == true) || (g.Exclude == true))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //------------------------
        //Properties
        //------------------------
        public string Name
        {
            get { return _name; }
        }
        public string PriorityGroup
        {
            get { return _priorityGroup; }
        }
        public int Max
        {
            get { return _max; }
        }

        public int RemainSpace
        {
            get { return (_max - _listMember.Count); }
        }
        public int MemberCount
        {
            get { return _listMember.Count; }
        }
        public List<Member> ListMember
        {
            get { return _listMember; }
        }
        public int X
        {
            get { return _layoutpos[LAYOUT_X];}
        }
        public int Y
        {
            get { return _layoutpos[LAYOUT_Y]; }
        }

    }
}
