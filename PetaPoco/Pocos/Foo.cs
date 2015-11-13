using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetaPoco.Pocos
{
    [PetaPoco.TableName("Foos")]
    [PetaPoco.PrimaryKey("FooID")]
    class Foo
    {
        public int FooID {get; set;}
        public int X {get; set;}
        public int Y {get; set;}
        public string Z{get; set;}
    }
}
