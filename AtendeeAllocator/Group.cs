using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendeeAllocator
{

    /// <summary>
    /// 割り当て時に使用する作業用グループ
    /// </summary>
    public class Group
    {
        /// <summary>
        /// グループ名
        /// </summary>
        private string _groupName;
        private bool _exclusive;
        private bool _gather;
        private bool _scatter;
        private bool _exclude;
        private int _priority;
        private List<string> _errorMessages;

        /// <summary>
        /// メンバーリスト(中身はCSVの一行を一人とみなしてコピー）
        /// </summary>
        private List<Member> _members;

        public Group()
        {
            _groupName = "";
            _members = new List<Member>();
            _errorMessages = new List<string>();
        }
        public Group(string name)
        {
            _groupName = name;
            _members = new List<Member>();
            _errorMessages = new List<string>();
        }
        public Group(string name, bool exclusive, bool gather, bool scatter, bool exclude, int priority)
        {
            _groupName = name;
            _exclusive = exclusive;
            _gather = gather;
            _scatter = scatter;
            _exclude = exclude;
            _priority = priority;
            _members = new List<Member>();
            _errorMessages = new List<string>();
        }
        public Group( string[] groupinfo)
        {
            _groupName = groupinfo[(int)GroupInfo.COLUM_POS.Name];
            _members = new List<Member>();

            _exclusive = false;
            if (groupinfo[(int)GroupInfo.COLUM_POS.Exclusive] == "TRUE")
            {
                _exclusive = true;
            }
            _gather = false;
            if (groupinfo[(int)GroupInfo.COLUM_POS.Gather] == "TRUE")
            {
                _gather = true;
            }
            _scatter = false;
            if (groupinfo[(int)GroupInfo.COLUM_POS.Scatter] == "TRUE")
            {
                _scatter = true;
            }
            _exclude = false;
            if (groupinfo[(int)GroupInfo.COLUM_POS.Exclude] == "TRUE")
            {
                _exclude = true;
            }
            if (int.TryParse(groupinfo[(int)GroupInfo.COLUM_POS.Priority], out _priority) == false)
            {
                _priority = -1;
            }
            _errorMessages = new List<string>();
        }
        //------------------------
        //Method
        //------------------------

        /// <summary>
        /// グループメンバー追加
        /// </summary>
        /// <param name="memberInfo"></param>
        public void AddMember( Member memberInfo)
        {
            if (Exclude == true)
            {
                memberInfo.Exclude = true;
            }
            _members.Add(memberInfo);
        }

        /// <summary>
        /// メンバーにkeyと一致する人が居るか調べる（※keyはメンバー情報の先頭の列）
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsGroupMember( string key)
        {
            foreach (Member member in _members)
            {
                if (member.Data[0] == key)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// メンバーを区画へ割り当てる
        /// </summary>
        /// <param name="listCompartment"></param>
        /// <returns></returns>
        public bool AllocateMemberToCompartment(List<Compartment> listCompartment)
        {
            bool result = true;
            //=================================================
            //グループ属性に「除外」がある場合
            //=================================================
            //割り当てしない
            if (_exclude == true)
            {
                return true;
            }

            //=================================================
            //割り当ての準備
            //=================================================
            //優先設定されている部屋のリストを作る
            List<Compartment> listPriorityCompartment = new List<Compartment>();
            foreach (Compartment cmp in listCompartment)
            {
                if (cmp.PriorityGroup == _groupName)
                {
                    listPriorityCompartment.Add(cmp);
                }
            }

            //優先設定されていない部屋のリストを作る
            List<Compartment> listNormalCompartment = new List<Compartment>();
            foreach (Compartment cmp in listCompartment)
            {
                if (cmp.PriorityGroup == "")
                {
                    listNormalCompartment.Add(cmp);
                }
            }

            //割り当てなければいけないメンバーのリストを作成
            List<Member> listRemainMembers = new List<Member>();
            foreach (Member member in _members)
            {
                listRemainMembers.Add(member);
            }

            int remainMemberCount = listRemainMembers.Count;

            //=================================================
            //グループ属性に「集合」がある場合
            //=================================================
            if (_gather == true)
            {
                //まずは優先設定されている部屋への割り当てを試みる
                AllocatePriorityCompartmentGather(listPriorityCompartment, listRemainMembers);

                //優先設定区画割り当て後の残りの未割り当てメンバーを算出
                remainMemberCount = GetNotAllocatedRemainmemberCount(listRemainMembers);

                //通常区画の最大空き人数を算出
                if (remainMemberCount > 0)
                {
                    int normalCompartmentMaxRemainSpace = GetNumberOfMaxSpace(listNormalCompartment);
                    int moreNeed = remainMemberCount - normalCompartmentMaxRemainSpace;
                    if (moreNeed > 0)
                    {
                        _errorMessages.Add(string.Format("[エラー]グループ[{0}]用の{1}人分の区画がありませんでした。", this.Name, remainMemberCount));

                    }
                    else
                    {
                        moreNeed = 0;
                    }


                    //未割り当てメンバーがいなくなるまで（または、収容可能最大人数に達するまで）マッチする部屋へ割り当てる
                    int maxLoop = remainMemberCount;
                    for (int i = 0; i < maxLoop; i++)
                    {
                        if (remainMemberCount > 0)
                        {
                            Compartment matchCompartment = GetMatchCompartment(listNormalCompartment, remainMemberCount);
                            foreach (Member mem in listRemainMembers)
                            {
                                if (mem.IsAllocated == false)
                                {
                                    matchCompartment.AllocateExclusiveMember(mem, this);
                                    mem.AllocatedCompartment = matchCompartment;
                                }
                            }
                            remainMemberCount = GetNotAllocatedRemainmemberCount(listRemainMembers);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            //=================================================
            //グループ属性に「分散」がある場合
            //=================================================
            else if (_scatter == true)
            {
                //まずは優先設定されている部屋への割り当てを試みる
                AllocatePriorityCompartmentScatter(listPriorityCompartment, listRemainMembers);

                //普通の部屋の中から割り当られているメンバーが最小かつ、排他グループがいない部屋を探す
                AllocatePriorityCompartmentScatter(listNormalCompartment, listRemainMembers);

                //未割り当ての人が居るならエラー
                remainMemberCount = GetNotAllocatedRemainmemberCount(listRemainMembers);
                if (remainMemberCount > 0)
                {
                    foreach (Member m in listRemainMembers)
                    {
                        if (m.IsAllocated == false)
                        {
                            _errorMessages.Add(string.Format("[エラー]グループ{0}に所属するキーコード{1}を割り当てる事ができませんでした。", _groupName, m.Key));
                        }
                    }
                }

            }
            //=================================================
            //グループ属性が無い場合（いまのところ分散と同じ方法で割り当て）
            //=================================================
            else
            {
                //まずは優先設定されている部屋への割り当てを試みる
                //AllocatePriorityCompartmentScatter(listPriorityCompartment, listRemainMembers);
                AllocatePriorityCompartmentGather(listPriorityCompartment, listRemainMembers);

                //普通の部屋の中から割り当られているメンバーが最小かつ、排他グループがいない部屋を探す
                AllocatePriorityCompartmentScatter(listNormalCompartment, listRemainMembers);

                //どこでもいいから開いている（排他グループのメンバーがいない）開いている部屋をさがす
                AllocateCompartment(listNormalCompartment, listRemainMembers);

                //未割り当ての人が居るならエラー
                remainMemberCount = GetNotAllocatedRemainmemberCount(listRemainMembers);
                if (remainMemberCount > 0)
                {
                    foreach (Member m in listRemainMembers)
                    {
                        if (m.IsAllocated == false)
                        {
                            _errorMessages.Add(string.Format("[エラー]グループ{0}に所属するキーコード{1}を割り当てる事ができませんでした。", _groupName, m.Key));
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 優先設定の部屋への割り当て（集合）
        /// </summary>
        /// <param name="listPriorityCompartment"></param>
        /// <param name="listRemainMembers"></param>
        private void AllocatePriorityCompartmentGather(List<Compartment> listPriorityCompartment, List<Member> listRemainMembers)
        {
            foreach (Compartment cmp in listPriorityCompartment)
            {
                foreach (Member mem in listRemainMembers)
                {
                    //メンバーが（他のグループなどで）まだ割り当てられていないなら割り当てを試みる
                    if (mem.Exclude == false)
                    {
                        if (mem.IsAllocated == false)
                        {
                            //メンバーを区画への割り当てを試みる
                            if (cmp.AllocateExclusiveMember(mem, this) == true)
                            {
                                mem.AllocatedCompartment = cmp;
                                continue;
                            }

                        }
                    }
                    if (cmp.RemainSpace < 1)
                    {
                        break;
                    }

                }
            }
        }
        /// <summary>
        /// 優先設定の部屋への割り当て（分散）
        /// </summary>
        /// <param name="listPriorityCompartment"></param>
        /// <param name="listRemainMembers"></param>
        private void AllocatePriorityCompartmentScatter(List<Compartment> listPriorityCompartment, List<Member> listRemainMembers)
        {
            foreach (Member mem in listRemainMembers)
            {
                if (mem.Exclude == false)
                {
                    //割り当てられたメンバーが最小の区画を探す
                    Compartment targetCompartment = FindFewestMemberAllocatedCompartmentFromList(listPriorityCompartment);
                    if (targetCompartment != null)
                    {
                        //探しだした区画に排他属性のグループのメンバーがいないか調べる
                        if (targetCompartment.IsAlreadyAllocatedExclusiveGroupMenber(this) == false)
                        {
                            //メンバーが（他のグループなどで）まだ割り当てられていないなら割り当てを試みる
                            if (mem.IsAllocated == false)
                            {
                                //メンバーを区画への割り当てを試みる
                                if (targetCompartment.AllocateExclusiveMember(mem, this) == true)
                                {
                                    mem.AllocatedCompartment = targetCompartment;
                                    continue;
                                }

                            }
                        }
                    }
                }

            }
        }
        /// <summary>
        /// どこでもいいから開いている部屋へ割り当てる
        /// </summary>
        /// <param name="listPriorityCompartment"></param>
        /// <param name="listRemainMembers"></param>
        private void AllocateCompartment(List<Compartment> listPriorityCompartment, List<Member> listRemainMembers)
        {
            foreach (Member mem in listRemainMembers)
            {
                if ((mem.IsAllocated == false)&&(mem.Exclude == false))
                {
                    //部屋を一つづつ調べて回る
                    foreach (Compartment targetCompartment in listPriorityCompartment)
                    {
                        //空きがあるか？
                        if (targetCompartment.RemainSpace == 0)
                        {
                            //無いから次の部屋に行く
                            continue;
                        }
                        //探しだした区画に排他属性のグループのメンバーがいないか調べる
                        if (targetCompartment.IsAlreadyAllocatedExclusiveGroupMenber(this) == false)
                        {
                            //メンバーを区画への割り当てを試みる
                            if (targetCompartment.AllocateExclusiveMember(mem, this) == true)
                            {
                                mem.AllocatedCompartment = targetCompartment;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// リストの中で最もメンバーの少ない部屋を探す
        /// </summary>
        /// <param name="listCompartment"></param>
        /// <returns></returns>
        private Compartment FindFewestMemberAllocatedCompartmentFromList(List<Compartment> listCompartment)
        {
            Compartment result = null;
            if( listCompartment.Count > 0){
                Compartment min = listCompartment[0];
                foreach (Compartment c in listCompartment)
                {
                    if (min.MemberCount > c.MemberCount)
                    {
                        min = c;
                    }
                }
                result = min;

            }
            return result;

        }

        /// <summary>
        /// 条件に最も近い区画をリストから探す
        /// </summary>
        /// <param name="listCompartment"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        private Compartment GetMatchCompartment(List<Compartment> listCompartment, int number)
        {
            Compartment mostNearSpaceCompartmentL = null;
            Compartment mostNearSpaceCompartmentS = null;

            foreach (Compartment cmp in listCompartment)
            {
                //必要とする空きと一致する区画が見つかった場合
                if (cmp.RemainSpace == number)
                {
                    //この区画に排他属性の他のグループのメンバーが割り当てられていないか確認
                    if( cmp.IsAlreadyAllocatedExclusiveGroupMenber(this) == false)
                    {
                        //そういう人は居なかった
                        return cmp;
                    }
                }
                else
                {
                    //必要とする空きとの差が一番近い区画を覚えておく
                    //空き人数の方が必要とするメンバー数を上回っている部屋の記憶
                    int diff = cmp.RemainSpace - number;
                    if (diff > 0)
                    {
                        if (mostNearSpaceCompartmentL == null)
                        {
                            mostNearSpaceCompartmentL = cmp;
                        }
                        else
                        {
                            if ((mostNearSpaceCompartmentL.RemainSpace - number) > diff)
                            {
                                mostNearSpaceCompartmentL = cmp;
                            }
                        }
                    }
                    else
                    {
                        //とにかく一番空き人数が近い部屋の記憶
                        if (mostNearSpaceCompartmentS == null)
                        {
                            mostNearSpaceCompartmentS = cmp;
                        }
                        else
                        {
                            int diffAbs = Math.Abs(cmp.RemainSpace - number);
                            if (Math.Abs(mostNearSpaceCompartmentS.RemainSpace - number) > diffAbs)
                            {
                                mostNearSpaceCompartmentS = cmp;
                            }

                        }

                    }

                }

            }
            if (mostNearSpaceCompartmentL != null)
            {
                return mostNearSpaceCompartmentL;
            }
            else
            {
                return mostNearSpaceCompartmentS;

            }
        }

        /// <summary>
        /// 区画リストかすでに割り当てられている同じメンバーの区画を探す
        /// </summary>
        /// <param name="mem"></param>
        /// <param name="listCompartment"></param>
        /// <returns></returns>
        private Compartment GetAllocatedCompartment(AttendeeAllocator.Member mem, List<Compartment> listCompartment)
        {
            //すでに割り当てられていることがわかっている場合
            if (mem.IsAllocated == true)
            {
                return mem.AllocatedCompartment;
            }

            //不明な場合は区画リストから探し出す
            Compartment result = null;
            foreach (Compartment cmp in listCompartment)
            {
                if (cmp.IsInMember(mem) == true)
                {
                    result = cmp;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 割り当てられていないメンバー数を算出
        /// </summary>
        /// <param name="listMember"></param>
        /// <returns></returns>
        private int GetNotAllocatedRemainmemberCount(List<Member> listMember)
        {
            int result = 0;
            foreach (Member mem in listMember)
            {
                if (mem.Exclude == false) {
                    if (mem.IsAllocated == false)
                    {
                        result++;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// リスト中にある区画の収容最大人数算出
        /// </summary>
        /// <param name="listCompartment"></param>
        /// <returns></returns>
        private int GetNumberOfMaxSpace(List<Compartment> listCompartment)
        {
            int result = 0;
            foreach (Compartment comp in listCompartment)
            {
                if (comp.RemainSpace > result)
                {
                    result = comp.RemainSpace;
                }
            }
            return result;
        }





        //------------------------
        //Properties
        //------------------------
        public string Name
        {
            get { return _groupName; }
        }

        public List<Member> Member
        {
            get { return _members; }
        }

        public bool Exclusive
        {
            get { return _exclusive; }
        }
        public bool Gather
        {
            get { return _gather; }
        }
        public bool Scatter
        {
            get { return _scatter; }
        }
        public bool Exclude
        {
            get { return _exclude; }
        }
        public int Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }
        public List<string> ErrorMessages
        {
            get { return _errorMessages; }
        }

    }
}
