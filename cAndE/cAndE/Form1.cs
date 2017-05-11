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
    //enums to define the different states
    enum GameState { menu, gameOn, highScore };
    enum Direction {Right,Down,Left,Up }

    /// <summary>
    /// Chicken and Egg Video Game
    /// By: Ian McTavish
    /// Version 1.0
    /// April, 2014
    /// </summary>
    public partial class Form1 : Form
    {
        //Global variables
        List<Car> Cars = new List<Car>();
        List<string> highScores = new List<string>();
        int lives = 3;
        int score = 100;
        int eggHealth = 100;
        int gameFrames = 0;
        int chickenFrame;

        Point chickenLocation;
        Point nestLocation;
        Point cursorLocation;
        Direction chickenDirection;

        //set state of game
        GameState gameState = GameState.menu;
        string[] menuItems = new string[] {"Play Game","High Scores","Quit" };
        int currentItem = 0;
        Key previousKey = Key.A;

        Random r = new Random();
        
        /// <summary>
        /// Visual studio creates this - don't mess with it
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        ///Paint method 
        ///runs every frame and draws everything on the screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //draw background
            Brush grassColour = new SolidBrush(Color.FromArgb(0, 225, 0));
            e.Graphics.FillRectangle(grassColour, 0, 0, 800, 600);
            //draw road
            Brush roadColour = new SolidBrush(Color.FromArgb(153, 153, 153));
            e.Graphics.FillRectangle(roadColour, 4 * 80, 0, 2 * 80, 600);

            //draw white lines on road
            for (int i = 0; i < 4; i++) 
            {
                e.Graphics.FillRectangle(Brushes.White, 5 * 80 - 5, i * 160, 10, 80);
            }

            //Draws all the cars
            //You need to have added a car.png file to the images
            //folder in the solution and set the copy property
            //to copy always
            Bitmap carPic = new Bitmap(@"images/car.png");
            foreach (Car c in Cars)
            {
                e.Graphics.DrawImage((Image)carPic, c.carLocation);
            }//end of foreach car loop

            //draws the nest
            //You need to have added a nest.png file to the images
            //folder in the solution and set the copy property
            //to copy always
            Bitmap nestPic = new Bitmap(@"images/nest.png");
            Rectangle nestRec = new Rectangle(nestLocation, new System.Drawing.Size(72, 72));
            e.Graphics.DrawImage((Image)nestPic, nestLocation);

            //draws the chicken
            //You need to have added a chicken.png file to the images
            //folder in the solution and set the copy property
            //to copy always
            Bitmap chickenPic = new Bitmap(@"images/chicken.png");
            Rectangle chickenRec = new Rectangle(chickenLocation,new Size(80,80));
            
            //draws a chicken if menu or high score screen
            if ((gameState == GameState.menu) || 
                (gameState==GameState.highScore))
            {
                e.Graphics.DrawImage((Image)chickenPic, chickenRec, new Rectangle(0, 0, 80, 80), GraphicsUnit.Pixel);
            }
            else if (chickenFrame == 0) //draws first frame of chicken
            {
                //The following will rotate and draw the chicken
                //based on the direction
                if (chickenDirection == Direction.Right)
                {
                    e.Graphics.DrawImage((Image)chickenPic, chickenRec, new Rectangle(0, 0, 80, 80), GraphicsUnit.Pixel);
                }
                else if (chickenDirection == Direction.Down) 
                {
                    chickenPic.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    e.Graphics.DrawImage((Image)chickenPic, chickenRec, new Rectangle(0, 0, 80, 80), GraphicsUnit.Pixel);
                }
                else if (chickenDirection == Direction.Left)
                {
                    chickenPic.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    e.Graphics.DrawImage((Image)chickenPic, chickenRec, new Rectangle(0, 0, 80, 80), GraphicsUnit.Pixel);
                }
                else if (chickenDirection == Direction.Up)
                {
                    chickenPic.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    e.Graphics.DrawImage((Image)chickenPic, chickenRec, new Rectangle(0, 0, 80, 80), GraphicsUnit.Pixel);
                }//end of if to rotate and draw chicken
                //change to frame 1 for the next time this runs
                chickenFrame = 1;
            } else 
            {
                //rotates and flips frame 1
                if (chickenDirection == Direction.Right)
                {
                    e.Graphics.DrawImage((Image)chickenPic, chickenRec, new Rectangle(80, 0, 80, 80), GraphicsUnit.Pixel);
                }
                else if (chickenDirection == Direction.Down) 
                {
                    chickenPic.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    e.Graphics.DrawImage((Image)chickenPic, chickenRec, new Rectangle(0, 80, 80, 80), GraphicsUnit.Pixel);
                }
                else if (chickenDirection == Direction.Left)
                {
                    chickenPic.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    e.Graphics.DrawImage((Image)chickenPic, chickenRec, new Rectangle(80, 0, 80, 80), GraphicsUnit.Pixel);
                }
                else if (chickenDirection == Direction.Up)
                {
                    chickenPic.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    e.Graphics.DrawImage((Image)chickenPic, chickenRec, new Rectangle(0, 80, 80, 80), GraphicsUnit.Pixel);
                }
                //change to frame 0 for the next time this runs
                chickenFrame = 0;
            }//end of drawing the chicken
            //draw the score information
            Color textColor = Color.Yellow;
            Brush textBrush = new SolidBrush(textColor);
            e.Graphics.DrawString("Lives: " + lives.ToString(), new Font(new FontFamily("Arial"), 18.0f), textBrush, new PointF(500, 50));
            e.Graphics.DrawString("Score: " + score.ToString() + " Egg Health: " + eggHealth.ToString() , new Font(new FontFamily("Arial"), 18.0f), textBrush, new PointF(500, 100));
            

            //Draw the menu
            if (gameState == GameState.menu) 
            {
                //variables
                Color backGround = Color.FromArgb(150,0,0,0);
                Color highlightTextColor = Color.GreenYellow;
                Brush highlighTextBrush = new SolidBrush(highlightTextColor);
                SolidBrush bgrndBrush = new SolidBrush(backGround);
                //draw a semi-transparent background
                e.Graphics.FillRectangle(bgrndBrush, 0, 0, this.Width, this.Height);
                //Loop through the menu items and draw them
                for (int i = 0; i < menuItems.Length; i++) 
                {
                    //if item is selected use the highlight colour
                    if (currentItem == i)
                    {
                        e.Graphics.DrawString(menuItems[i], new Font(new FontFamily("Arial"), 18.0f), highlighTextBrush, new PointF(50, i * 50 + 50));
                    }
                    else 
                    {
                        e.Graphics.DrawString(menuItems[i], new Font(new FontFamily("Arial"), 18.0f), textBrush, new PointF(50, i * 50 + 50));
                    }
                }

            }

            //Draw the High Scores
            if (gameState == GameState.highScore) 
            {
                Color backGround = Color.FromArgb(150, 0, 0, 0);
                SolidBrush bgrndBrush = new SolidBrush(backGround);
                e.Graphics.FillRectangle(bgrndBrush, 0, 0, this.Width, this.Height);
                if (highScores.Count > 0) { } else 
                {
                    e.Graphics.DrawString("There are no scores saved yet", new Font(new FontFamily("Arial"), 18.0f), textBrush, new PointF(50, 1 * 50 + 50));
                    e.Graphics.DrawString("Press the left arrow to return to the menu", new Font(new FontFamily("Arial"), 18.0f), textBrush, new PointF(50, 2 * 50 + 50));
                }
            }

        }

        /// <summary>
        /// Form load
        /// Runs when the program starts
        /// Sets the global variables
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            cursorLocation = new Point();
            //Add a car in lane 1 and lane 2
            Cars.Add(new Car(1));
            Cars.Add(new Car(2));
            //Set the starting chicken location
            chickenLocation = new Point(0, 600 / 2 - 20);
            //set the frame for the chicken graphic
            chickenFrame = 0;
            //set the direction
            chickenDirection = Direction.Right;
            //set a random location for the nest
            nestLocation = new Point((700), r.Next(25, 500));
        }

        /// <summary>
        /// Game timer tick
        /// The game timer is set to run every 33 miliseconds (around 30fps)
        /// updates everything then forces the screen to redraw
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            cursorLocation = Control.MousePosition;
            this.Text = cursorLocation.ToString();
           // cursorLocation = this.mou
            //if the game is being played
            if (gameState == GameState.gameOn)
            {
                //create a random number to determine
                //whether a new car is created
                int spawnCar = r.Next(1, 60);
                //odds of new car being created
                if (spawnCar <= 3)
                {
                    //is there space for the car?
                    bool thereIsSpace = true;
                    foreach (Car c in Cars) 
                    {
                        if ((spawnCar % 2) == 0)
                        {
                            if (c.carLocation.Y < 80)
                            {
                                thereIsSpace = false;
                            }
                        }
                        else 
                        {
                            if (c.carLocation.Y > (600 - 160)) 
                            {
                                thereIsSpace = false;
                           
                            }
                        }
                    }
                    //Add a new car if their is space
                    if (thereIsSpace)
                    {
                        Cars.Add(new Car(spawnCar % 2 + 1));//randomly picks a lane
                    }
                }
                //updates the cars using the car class
                foreach (Car c in Cars) 
                {
                    c.updateCar();
                    
                }
                //loops through all the cars
                //if they are off the screen - remove them
                for (int i = Cars.Count - 1; i >= 0; i--) 
                {
                    if (Cars[i].carLane % 2 == 1 && Cars[i].carLocation.Y > 600) 
                    {
                        Cars.RemoveAt(i);
                    }
                    else if (Cars[i].carLane % 2 == 0 && Cars[i].carLocation.Y < -80) 
                    {
                        Cars.RemoveAt(i);
                    }
                }
                
                //Check is key was pressed - move chicken based on key
                if (Keyboard.IsKeyDown(Key.Right))
                {
                    chickenDirection = Direction.Right;
                    chickenLocation.X += 20;
                }
                else if (Keyboard.IsKeyDown(Key.Down)) 
                {
                    chickenDirection = Direction.Down;
                    chickenLocation.Y += 20;
                }
                else if (Keyboard.IsKeyDown(Key.Left)) 
                {
                    chickenDirection = Direction.Left;
                    chickenLocation.X -= 20;
                }
                else if (Keyboard.IsKeyDown(Key.Up)) 
                {
                    chickenDirection = Direction.Up;
                    chickenLocation.Y -= 20;
                }
                //create a rectangle for the chicken to check for collisions
                Rectangle chickenRec = new Rectangle(chickenLocation, new Size(80, 80));
              
                //loop through each car, see if it hit the chicken
                foreach (Car c in Cars)
                {
                    Rectangle carRect = new Rectangle(c.carLocation, new Size(80, 80));
                    if (chickenRec.IntersectsWith(carRect))
                    {
                        //chicken was hit - reset variables
                        chickenLocation = new Point(0, 600 / 2 - 20);
                        chickenFrame = 0;
                        chickenDirection = Direction.Right;
                        lives -= 1;
                        //Has the player run out of lives
                        if (lives == 0) 
                        {
                            //go back to the menu
                            gameState = GameState.menu;
                        }
                    }
                }//end of foreach car
                
                //check if the chicken reached the nest
                Rectangle nestRec = new Rectangle(nestLocation, new Size(72, 72));
                if (chickenRec.IntersectsWith(nestRec)) 
                {
                    //chicken reached nest, start level again and update
                    //the scores
                    chickenLocation = new Point(0, 600 / 2 - 20);
                    chickenFrame = 0;
                    chickenDirection = Direction.Right;
                    nestLocation = new Point((700), r.Next(25, 500));
                    score += 50;
                    eggHealth = 100;
                }
                //update how many frames have run
                gameFrames++;
                if (gameFrames % 10==0) 
                {
                    //update the health
                    eggHealth -= 10;
                    //if the eggs died - done
                    if (eggHealth <= 0) 
                    {
                        gameState = GameState.menu;
                    }
                }

            }//end of if gameon logic
                //is the game running the menu
            else if (gameState == GameState.menu) 
            {
                //Since the game updates every 33 milliseconds
                //pressing the key down would actually fire
                //the if statement around 20 times
                //keeping track of the previous key prevents
                //this from being a problem
                if (Keyboard.IsKeyDown(Key.Down) && previousKey != Key.Down) 
                {
                    //set the current item to the next menu item
                    //if you are not at the bottom
                    if (currentItem < menuItems.Length-1)
                    {
                        currentItem++;
                    }
                    previousKey = Key.Down;
                }
                else if (Keyboard.IsKeyDown(Key.Up) && previousKey != Key.Up) 
                {
                    //set the current item to the previous menu item
                    //if you are not at the top
                    if (currentItem > 0) 
                    {
                        currentItem--;
                    }
                    previousKey = Key.Up;
                }
                else if (Keyboard.IsKeyDown(Key.Enter)
                    && menuItems[currentItem].Equals("Play Game"))
                {
                    //Person selected Play Game - set up the game
                    lives = 3;
                    score = 100;
                    eggHealth = 100;
                    chickenLocation = new Point(0, 600 / 2 - 20);
                    chickenFrame = 0;
                    chickenDirection = Direction.Right;
                    nestLocation = new Point((700), r.Next(25, 500));
                    gameState = GameState.gameOn;
                    previousKey = Key.Enter;
                }
                else if (Keyboard.IsKeyDown(Key.Enter)
   && menuItems[currentItem].Equals("High Scores"))
                {
                    //Person selected high scores - show the high scores
                    gameState = GameState.highScore;
                    previousKey = Key.Enter;
                }
                else if (Keyboard.IsKeyDown(Key.Enter)
                    && menuItems[currentItem].Equals("Quit"))
                {
                    //Person quit - close the game
                    this.Close();
                }
                else if ((Keyboard.IsKeyUp(Key.Down)) && (Keyboard.IsKeyUp(Key.Up)))
                {
                    //clear the previous key to something we don't track
                    previousKey = Key.A;
                }

            }
            else if (Keyboard.IsKeyDown(Key.Left)
               && (gameState == GameState.highScore))
            {
                //person is in high score
                //if they click the left button return to the menu
                gameState = GameState.menu;
                previousKey = Key.Left;
            }//end of if 
            this.Refresh();//force the screen to redraw
        }//end of gameTimer_Tick method
    }
}
