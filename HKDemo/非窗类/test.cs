using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HKDemo
{
    abstract class test
    {
        public void fun1()
        {
         //   Console.WriteLine("fun1 for test");
            pria = 1;
            MessageBox.Show("fun1"+pria);
        }
        public abstract void fun2();
        private int pria;
        protected int prib;
    }
    class subtest : test
    {

        public void subfun()
        {
          //  Console.WriteLine("sub fun1 ");
            prib = 5;
            MessageBox.Show("subfun"+prib);
            base.fun1();
        }
        public override void fun2()
        {
            MessageBox.Show("fun2");
            Console.WriteLine("sub fun2");
        }
    }
}
