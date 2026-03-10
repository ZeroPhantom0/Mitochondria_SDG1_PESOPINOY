using System;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace PesoPinoy.UI.Helpers
{
    public static class FormAnimator
    {
        public static void FadeIn(Form form, int interval = 10, int steps = 20)
        {
            form.Opacity = 0;
            Timer timer = new Timer { Interval = interval };
            int currentStep = 0;

            timer.Tick += (sender, e) =>
            {
                if (currentStep < steps)
                {
                    currentStep++;
                    form.Opacity = (double)currentStep / steps;
                }
                else
                {
                    timer.Stop();
                    form.Opacity = 1;
                }
            };

            timer.Start();
        }

        public static void FadeOut(Form form, int interval = 10, int steps = 20)
        {
            Timer timer = new Timer { Interval = interval };
            int currentStep = steps;

            timer.Tick += (sender, e) =>
            {
                if (currentStep > 0)
                {
                    currentStep--;
                    form.Opacity = (double)currentStep / steps;
                }
                else
                {
                    timer.Stop();
                    form.Close();
                }
            };

            timer.Start();
        }

        public static void SlideIn(Form form, Direction direction = Direction.Left, int duration = 300)
        {
            int targetX = form.Location.X;
            int targetY = form.Location.Y;

            switch (direction)
            {
                case Direction.Left:
                    form.Location = new Point(-form.Width, targetY);
                    break;
                case Direction.Right:
                    form.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width, targetY);
                    break;
                case Direction.Top:
                    form.Location = new Point(targetX, -form.Height);
                    break;
                case Direction.Bottom:
                    form.Location = new Point(targetX, Screen.PrimaryScreen.WorkingArea.Height);
                    break;
            }

            Timer timer = new Timer { Interval = 10 };
            int steps = duration / 10;
            int currentStep = 0;
            int startX = form.Location.X;
            int startY = form.Location.Y;

            timer.Tick += (sender, e) =>
            {
                if (currentStep < steps)
                {
                    currentStep++;
                    double progress = (double)currentStep / steps;

                    int newX = (int)(startX + (targetX - startX) * progress);
                    int newY = (int)(startY + (targetY - startY) * progress);

                    form.Location = new Point(newX, newY);
                }
                else
                {
                    timer.Stop();
                    form.Location = new Point(targetX, targetY);
                }
            };

            timer.Start();
        }

        public static void Shake(Form form)
        {
            Point originalLocation = form.Location;
            Random random = new Random();
            Timer timer = new Timer { Interval = 20 };
            int shakes = 0;

            timer.Tick += (sender, e) =>
            {
                if (shakes < 10)
                {
                    form.Location = new Point(
                        originalLocation.X + random.Next(-5, 6),
                        originalLocation.Y + random.Next(-5, 6)
                    );
                    shakes++;
                }
                else
                {
                    timer.Stop();
                    form.Location = originalLocation;
                }
            };

            timer.Start();
        }
    }

    public enum Direction
    {
        Left,
        Right,
        Top,
        Bottom
    }
}