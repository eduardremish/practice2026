using System;
using System.Collections.Generic;
using System.Text;

namespace task04
{
    public class Fighter : ISpaceship
    {
        public int Speed => 100;
        public int FirePower => 70;

        public void MoveForward()
        {
        }
        public void Rotate(int angle)
        {
        }
        public void Fire()
        {
        }
    }
}
