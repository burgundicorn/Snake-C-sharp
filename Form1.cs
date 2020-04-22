using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Snake
{
    public partial class Form1 : Form
    {
        //Snake este o lista de obiecte Circle (cu coordonate)
        private List<Circle> Snake = new List<Circle>();
        //food este un singur obiect Circle care isi schimba mereu pozitia
        private Circle food = new Circle();
        private string name;

        public Form1()
        {
            //Pregateste partial componentele din Form si controlul
            InitializeComponent();

            //Initializez setarile standard
            new Settings();

            //Setez intervalul la care vreau sa se miste elementele
            gameTimer.Interval = 1000 / Settings.Speed;  //Este dependent de viteza
            gameTimer.Tick += UpdateScreen;
            gameTimer.Start(); //Let the game begin!
            
        }

        private void StartGame()
        {
            lblGameOver.Visible = false;

            //Initializez setarile standard
            new Settings();

            //Creez un obiect (Player)
            Snake.Clear();
            
            //Adaug prima componenta a sarpelui
            Circle head = new Circle();
            head.X = 10;
            head.Y = 5;
            Snake.Add(head);

            //Setez scorul initial
            lblScore.Text = Settings.Score.ToString();

            //Creez primul target
            GenerateFood();

        }

        //Plaseaza pe harta in pozitii random target-uri
        private void GenerateFood()
        {
            //Cea mai in dreapta pozitie posibila pentru un nou target (Latimea canvas-ului / Latimea unui target)
            int maxXPos = pbCanvas.Size.Width / Settings.Width;
            //Cea mai de jos pozitie posibila pentru un canvas (Inaltimea canvas-ului / Inaltimea unui target)

            int maxYPos = pbCanvas.Size.Height / Settings.Height;

            //Creez un obiect de tip Random
            Random random = new Random();
            //Creez un nou Circle (Obiect care contine coordonate)
            food = new Circle();
            //random.Next(a,b) va returna o valoare x cu prop. a <= x < b
            food.X = random.Next(0, maxXPos);
            food.Y = random.Next(0, maxYPos);
            //Si am setat coordonatele pentru noul target
        }


        private void UpdateScreen(object sender, EventArgs e)
        {
            //Verific continuitatea jocului
            if (Settings.GameOver)
            {
                //Daca se apasa Enter, jocul reincepe
                if (Input.KeyPressed(Keys.Enter))
                {
                    StartGame();
                }
            }
            else
            {
                //In functie de tasta apasata, schimb directia de deplasare
                if (Input.KeyPressed(Keys.Right) && Settings.direction != Direction.Left)
                    Settings.direction = Direction.Right;
                else if (Input.KeyPressed(Keys.Left) && Settings.direction != Direction.Right)
                    Settings.direction = Direction.Left;
                else if (Input.KeyPressed(Keys.Up) && Settings.direction != Direction.Down)
                    Settings.direction = Direction.Up;
                else if (Input.KeyPressed(Keys.Down) && Settings.direction != Direction.Up)
                    Settings.direction = Direction.Down;
                //Altfel, directia ramane cea anterioara
                //Si apelez functia care "pune lucrurile in miscare"
                MovePlayer();
            }

            //Invalidează întreaga suprafață a canvas-ului și determină redesenarea lui
            pbCanvas.Invalidate();

        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            //Aici creez canvas-ul
            Graphics canvas = e.Graphics;

            if(!Settings.GameOver)
            {
                //Setez culoarea sarpelui
                Brush snakeColour;

                //Desenez sarpele
                for(int i = 0; i< Snake.Count; i++ )
                {

                    if (i == 0)
                        snakeColour = Brushes.Purple;     //Desenez capul
                    else
                        snakeColour = Brushes.Purple;    //Restul corpului

                    //Sa coloram sarpele!
                    canvas.FillEllipse(snakeColour,
                        new Rectangle(Snake[i].X * Settings.Width,
                                      Snake[i].Y * Settings.Height,
                                      Settings.Width, Settings.Height));


                    //Si target-ul
                    canvas.FillEllipse(Brushes.Red,
                        new Rectangle(food.X * Settings.Width,
                             food.Y * Settings.Height, Settings.Width, Settings.Height));

                }
            }
            else
            {
               
                //Scriu textul de final
                string gameOver = name + ", jocul s-a terminat. Poti mai mult de atat!\nScorul tau final este: " + Settings.Score + ".\nApasa Enter pentru a incepe un joc nou.";
                //Ii atribui etichetei specifice
                lblGameOver.Text = gameOver;
                //Pe care o fac sa fie vizibila
                lblGameOver.Visible = true;
            }
        }

        
        private void MovePlayer()
        {
            for(int i = Snake.Count -1; i >= 0; i--)
            {
                //Misc capul sarpelui
                if(i == 0)
                {
                    //In functie de directie
                    switch (Settings.direction)
                    {
                        case Direction.Right:
                            Snake[i].X++;
                            break;
                        case Direction.Left:
                            Snake[i].X--;
                            break;
                        case Direction.Up:
                            Snake[i].Y--;
                            break;
                        case Direction.Down:
                            Snake[i].Y++;
                            break;
                    }


                    //Setez pozitiile maxime pe care le poate atinge in canvas
                    int maxXPos = pbCanvas.Size.Width/Settings.Width;
                    int maxYPos = pbCanvas.Size.Height/Settings.Height;

                    //Detectez coliziunile cu marginile canvas-ului
                    if (Snake[i].X < 0 || Snake[i].Y < 0 || Snake[i].X >= maxXPos || Snake[i].Y >= maxYPos)
                    {
                        //Omor sarpele!
                        Die();
                    }


                    //Detectez coliziunile cu propriul corp
                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if(Snake[i].X == Snake[j].X && Snake[i].Y == Snake[j].Y )
                        {
                            //Si, de asemenea, omor sarpele
                            Die();
                        }
                    }

                    //Detectez coliziunile cu mancarea
                    if(Snake[0].X == food.X && Snake[0].Y == food.Y)
                    {
                        //Apelez functia de "crestere"
                        Eat();
                    }

                }
                else
                {
                    //Misc corpul sarpelui
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //Apelez functia cand se apasa tasta
            Input.ChangeState(e.KeyCode, true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //Apelez functia cand se ia mana de pe tasta
            Input.ChangeState(e.KeyCode, false);
        }

        private void Eat()
        {
            //Adaug o componenta sarpelui
            //Creez componenta cu datele ultimei componenta deja existente
            Circle circle = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };
            //Dupa care o adaug sarpelui
            Snake.Add(circle);

            //Actualizez scorul
            Settings.Score += Settings.Points;
            //Afisez scorul actualizat
            lblScore.Text = Settings.Score.ToString();
            //Generez un nou target
            GenerateFood();
        }

        private void Die()
        {
            //Marchez sfarsitul jocului
            Settings.GameOver = true;
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Reincep jocul
            StartGame();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Inchid aplicatia
            Application.Exit();
        }

        private void aboutMeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Numele meu este Gheorghita Ana, am 18 ani si sunt eleva in clasa a 12-a.", "Despre developer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pbCanvas_Click(object sender, EventArgs e)
        {

        }

        private void lblGameOver_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            name = textBox1.Text;
            if (name == "" || name == null)
            {
                MessageBox.Show("Mai intai introdu numele, apoi poti incepe.", "Ups!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                MessageBox.Show("Mult succes, " + name + "!","Perfect!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                //Setez fortat focus-ul pe Form1. Altfel comenzile mergeau catre TextBox sau Button1
                this.Activate();
                this.button1.Enabled = false;
                this.textBox1.Enabled = false;
                //Sa inceapa jocul!
                StartGame();
            }
        }
    }
}
