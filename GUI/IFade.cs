using System.Windows.Media.Animation;

namespace TerminologyLauncher.GUI
{
    public interface IFade
    {
        Storyboard FadeInStoryboard { get; }
        Storyboard FadeOutStoryboard { get; }
        void FadeInShow();
        void FadeOutHide();
    }
}