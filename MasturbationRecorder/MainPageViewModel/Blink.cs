using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace MasturbationRecorder {
    static class Blink {
        private static Storyboard _storyboard = new Storyboard();

        public static void PlayBlink(Rectangle target) {
            KeyTime redTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 300));
            KeyTime orginalTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 600));

            DiscreteColorKeyFrame OrginalColorToRed = new DiscreteColorKeyFrame() {
                Value = Windows.UI.Colors.Red,
                KeyTime = redTime
            };
            ColorAnimationUsingKeyFrames colorAnimationUsingKeyFrames = new ColorAnimationUsingKeyFrames() {
                EnableDependentAnimation = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            DiscreteColorKeyFrame RedToOrginalColor = new DiscreteColorKeyFrame() {
                Value = (target.Fill as SolidColorBrush).Color,
                KeyTime = orginalTime
            };

            colorAnimationUsingKeyFrames.KeyFrames.Add(OrginalColorToRed);
            colorAnimationUsingKeyFrames.KeyFrames.Add(RedToOrginalColor);
            _storyboard.Children.Add(colorAnimationUsingKeyFrames);
            Storyboard.SetTarget(colorAnimationUsingKeyFrames, target);
            Storyboard.SetTargetName(colorAnimationUsingKeyFrames, target.Name);
            Storyboard.SetTargetProperty(colorAnimationUsingKeyFrames, "(Rectangle.Fill).(SolidColorBrush.Color)");

            _storyboard.Begin();
        }

        public static void StopBlink(Rectangle target) {
            _storyboard.Stop();
        }
    }
}
