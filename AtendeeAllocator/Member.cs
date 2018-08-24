using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendeeAllocator
{
    public class Member
    {
        string[] _data;
        Compartment _allocatedCompartment;
        List<Group> _ListGroup;
        private bool _exclude;


        public Member()
        {
            _data = null;
            _allocatedCompartment = null;
            _ListGroup = new List<Group>();
            _exclude = false;
        }

        public Member(string[] data)
        {
            _data = data;
            _allocatedCompartment = null;
            _ListGroup = new List<Group>();
            _exclude = false;
        }



        public bool IsAllocated
        {
            get { 
                if (_allocatedCompartment == null)
                { 
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public Compartment AllocatedCompartment
        {
            get { return _allocatedCompartment; }
            set { _allocatedCompartment = value; }
        }

        public string[] Data
        {
            get { return _data; }
        }
        public string Key
        {
            get { return _data[0]; }
        }
        public List<Group> ListGroup
        {
            get { return _ListGroup; }
        }
        public bool Exclude
        {
            get { return _exclude; }
            set { _exclude = value; }
        }

        internal bool IsMember(Group g)
        {
            Group have = null;
            try
            {
                have = _ListGroup.Where(c => c.Name == g.Name).First();

            }catch(InvalidOperationException e)
            {
                ;
            }
            finally
            {
                ;
            }
            if (have != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
