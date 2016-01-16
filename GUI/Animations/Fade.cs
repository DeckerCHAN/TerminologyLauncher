using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace TerminologyLauncher.GUI.Animations
{
    public static class Fade
    {
        public static Storyboard CreateFadeInStoryboard(TimeSpan duration)
        {
            var anime = new DoubleAnimation()
            {
                Duration = duration,
                From = 0D,
                To = 1D
            };
            var storyBoard = new Storyboard();
            storyBoard.Children.Add(anime);
            Storyboard.SetTargetProperty(anime, new PropertyPath(UIElement.OpacityProperty));

            return storyBoard;
        }

        public static Storyboard CreateFadeOutStoryboard(TimeSpan duration)
        {
            var anime = new DoubleAnimation()
            {
                Duration = duration,
                From = 1D,
                To = 0D
            };
            var storyBoard = new Storyboard();
            storyBoard.Children.Add(anime);
            Storyboard.SetTargetProperty(anime, new PropertyPath(UIElement.OpacityProperty));

            return storyBoard;
        }

        public static void FadeInShowWindow(Window window, TimeSpan duration)
        {
            window.Opacity = 0D;
            window.Show();
            CreateFadeInStoryboard(duration).Begin(window);
        }


        public static void FadeOutHideWindow(Window window, TimeSpan duration)
        {
            window.Opacity = 1D;
            var board = CreateFadeOutStoryboard(duration);
            board.Completed += (s, e) =>
            {
                window.Hide();
            };
            board.Begin(window);


        }
    }
}
