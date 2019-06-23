using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace MasturbationRecorder {
    static class Blink {
        private static SortedDictionary<Rectangle, Storyboard> _rectangleRegister = new SortedDictionary<Rectangle, Storyboard>();

        public static void PlayBlink(Rectangle target) {
            if (!_rectangleRegister.ContainsKey(target)) {
                _rectangleRegister.Add(target, new Storyboard());
                KeyTime redTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 300));
                KeyTime orginalTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 600));

                ColorAnimationUsingKeyFrames colorAnimationUsingKeyFrames = new ColorAnimationUsingKeyFrames() {
                    EnableDependentAnimation = true,
                    RepeatBehavior = RepeatBehavior.Forever
                };
                DiscreteColorKeyFrame OrginalColorToRed = new DiscreteColorKeyFrame() {
                    Value = Windows.UI.Colors.Red,
                    KeyTime = redTime
                };
                DiscreteColorKeyFrame RedToOrginalColor = new DiscreteColorKeyFrame() {
                    Value = (target.Fill as SolidColorBrush).Color,
                    KeyTime = orginalTime
                };

                colorAnimationUsingKeyFrames.KeyFrames.Add(OrginalColorToRed);
                colorAnimationUsingKeyFrames.KeyFrames.Add(RedToOrginalColor);
                _rectangleRegister[target].Children.Add(colorAnimationUsingKeyFrames);
                Storyboard.SetTarget(colorAnimationUsingKeyFrames, target);
                Storyboard.SetTargetName(colorAnimationUsingKeyFrames, target.Name);
                Storyboard.SetTargetProperty(colorAnimationUsingKeyFrames, "(Rectangle.Fill).(SolidColorBrush.Color)");

                _rectangleRegister[target].Begin();
            }
        }

        public static void StopBlink(Rectangle target) {
            _rectangleRegister[target].Stop();
        }
    }
}
