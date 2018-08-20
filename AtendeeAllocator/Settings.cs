using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendeeAllocator
{
    [Serializable]
    public class Settings
    {
        //レイアウト時に区画の未割り当て領域（５人部屋に３人で２人分空きがあるなど）を表示させるかどうか
        private bool _AllocateBlankSpaceToLayout;


        public Settings()
        {
            _AllocateBlankSpaceToLayout = true;


        }
    }
}
