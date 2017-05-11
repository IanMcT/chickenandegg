using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace cAndE
{
    //Car class
    class Car
    {
        //variables for the car
        public Point carLocation;
        public int carSpeed;
        public int carLane;

        //runs when the car is created
        public Car(int L)
        {
            //set speed based on lane
            carLane = L;
            if (carLane % 2 == 0)
            {
                carSpeed = -5;
            }
            else
            {
                carSpeed = 5;
            }

            //Set location based on lane
            if (carLane == 1)
            {
                carLocation = new Point(4 * 80, -80);
            }
            else if (carLane == 2)
            {
                carLocation = new Point(5 * 80, 600);
            }
        }

        //updates the car
        public void updateCar() 
        {
            carLocation.Y += carSpeed;
        }
    }
}

